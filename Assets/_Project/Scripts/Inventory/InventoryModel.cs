using System.Collections.Generic;
using System;
using UnityEngine;

public class InventoryModel
{
    public InventorySlot[] Slots { get; private set; }

    private readonly Dictionary<ItemData, HashSet<int>> itemSlotLookup;
    private readonly Queue<int> emptySlotQueue;
    private readonly HashSet<int> emptySlotSet;

    public event Action<int> OnSlotChanged;
    public event Action OnInventoryChanged;

    public InventoryModel(int size)
    {
        Slots = new InventorySlot[size];

        itemSlotLookup = new Dictionary<ItemData, HashSet<int>>();
        emptySlotQueue = new Queue<int>();
        emptySlotSet = new HashSet<int>();

        //all slots empty at start
        for (int i = 0; i < size; i++)
        {
            Slots[i] = new InventorySlot();
            MarkSlotEmpty(i);
        }
    }

    public int AddItem(ItemData data, int amount)
    {
        if (data == null || amount <= 0)
            return amount;

        int remaining = amount;

        if (data.stackable && itemSlotLookup.TryGetValue(data, out var stackSlots))
        {
            foreach (var stackIndex in stackSlots)
            {
                if (remaining <= 0) break;

                var slot = Slots[stackIndex];
                if (slot.IsEmpty || slot.item.data != data) continue;

                int spaceLeft = data.maxStack - slot.item.count;
                if (spaceLeft <= 0) continue;

                int toAdd = Mathf.Min(spaceLeft, remaining);
                slot.item.count += toAdd;
                remaining -= toAdd;

                OnSlotChanged?.Invoke(stackIndex);
            }
        }

        while (remaining > 0 && TryGetEmptySlot(out int freeIndex))
        {
            int toPut = data.stackable
                ? Mathf.Min(remaining, data.maxStack)
                : 1;

            var toolData = data as ToolItemData;
            var previousItem = Slots[freeIndex].item;

            Slots[freeIndex].item = new InventoryItem
            {
                data = data,
                count = data.stackable ? toPut : 1,
                durability = toolData != null ? toolData.maxDurability : 0
            };

            remaining -= toPut;

            UpdateSlotMappings(freeIndex, previousItem);

            OnSlotChanged?.Invoke(freeIndex);
        }

        OnInventoryChanged?.Invoke();
        return remaining;
    }

    public bool RemoveItem(ItemData data, int amount)
    {
        if (data == null || amount <= 0)
            return false;

        if (GetItemCount(data) < amount)
            return false;

        int remaining = amount;

        if (!itemSlotLookup.TryGetValue(data, out var slotSet))
            return false;

        var slotsToCheck = new List<int>(slotSet);

        for (int i = slotsToCheck.Count - 1; i >= 0 && remaining > 0; i--)
        {
            int index = slotsToCheck[i];
            var slot = Slots[index];
            if (slot.IsEmpty || slot.item.data != data) continue;

            int toTake = Mathf.Min(remaining, Mathf.Max(1, slot.item.count));
            slot.item.count -= toTake;
            remaining -= toTake;

            if (slot.item.count <= 0)
            {
                var previous = slot.item;
                slot.Clear();
                UpdateSlotMappings(index, previous);
            }

            OnSlotChanged?.Invoke(index);
        }

        OnInventoryChanged?.Invoke();
        return remaining == 0;
    }


    public int GetItemCount(ItemData data)
    {
        if (data == null) return 0;

        
        if (!itemSlotLookup.TryGetValue(data, out var slotsForItem))
            return 0;

        int total = 0;
        foreach (var index in slotsForItem)
        {
            var slot = Slots[index];
            if (slot.IsEmpty || slot.item.data != data) continue;

            total += Mathf.Max(1, slot.item.count);
        }

        return total;
    }

    public bool HasItems(IEnumerable<CraftingIngredient> ingredients)
    {
        if (ingredients == null) return false;

        foreach (var ingredient in ingredients)
        {
            if (ingredient == null || ingredient.item == null) return false;
            if (ingredient.amount <= 0) return false;

            if (GetItemCount(ingredient.item) < ingredient.amount)
                return false;
        }

        return true;
    }

    public bool ConsumeItems(IEnumerable<CraftingIngredient> ingredients)
    {
        if (!HasItems(ingredients))
            return false;

        foreach (var ingredient in ingredients)
        {
            RemoveItem(ingredient.item, ingredient.amount);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public void SwapSlots(int from, int to)
    {
        if (from == to) return;
        if (!IsValidIndex(from) || !IsValidIndex(to)) return;

        var fromItem = Slots[from].item;
        var toItem = Slots[to].item;

        Slots[from].item = toItem;
        Slots[to].item = fromItem;

        UpdateSlotMappings(from, fromItem);
        UpdateSlotMappings(to, toItem);

        OnSlotChanged?.Invoke(from);
        OnSlotChanged?.Invoke(to);
        OnInventoryChanged?.Invoke();
    }

    public bool SplitStack(int index)
    {
        if (!IsValidIndex(index)) return false;

        var slot = Slots[index];
        if (slot.IsEmpty) return false;

        var item = slot.item;
        if (!item.data.stackable || item.count < 2) return false;

        if (!TryGetEmptySlot(out int freeIndex)) return false;

        int half = item.count / 2;
        item.count -= half;

        Slots[freeIndex].item = new InventoryItem
        {
            data = item.data,
            count = half,
            durability = item.durability
        };

        UpdateSlotMappings(freeIndex, null);

        OnSlotChanged?.Invoke(index);
        OnSlotChanged?.Invoke(freeIndex);
        OnInventoryChanged?.Invoke();
        return true;
    }

    private void UpdateSlotMappings(int index, InventoryItem previousItem)
    {
        if (previousItem != null && previousItem.data != null && itemSlotLookup.TryGetValue(previousItem.data, out var previousSet))
        {
            previousSet.Remove(index);
            if (previousSet.Count == 0)
                itemSlotLookup.Remove(previousItem.data);
        }

        var current = Slots[index].item;
        if (current != null && current.data != null)
        {
            if (!itemSlotLookup.TryGetValue(current.data, out var set))
            {
                set = new HashSet<int>();
                itemSlotLookup[current.data] = set;
            }

            set.Add(index);
            RemoveSlotFromEmpty(index);
        }
        else
        {
            Slots[index].Clear();
            MarkSlotEmpty(index);
        }
    }

    private void MarkSlotEmpty(int index)
    {
        if (emptySlotSet.Add(index))
            emptySlotQueue.Enqueue(index);
    }

    private void RemoveSlotFromEmpty(int index)
    {
        emptySlotSet.Remove(index);
    }

    private bool TryGetEmptySlot(out int index)
    {
        while (emptySlotQueue.Count > 0)
        {
            int candidate = emptySlotQueue.Dequeue();
            if (!emptySlotSet.Contains(candidate))
                continue;

            if (Slots[candidate].IsEmpty)
            {
                emptySlotSet.Remove(candidate);
                index = candidate;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Slots.Length;
    }
}