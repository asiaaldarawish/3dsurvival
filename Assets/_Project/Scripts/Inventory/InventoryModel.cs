using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel
{
    public InventorySlot[] Slots { get; private set; }

    private readonly Dictionary<ItemData, List<int>> itemIndexMap;
    private readonly Queue<int> freeSlots;
    private readonly HashSet<int> freeSlotSet;

    public event Action<int> OnSlotChanged;
    public event Action OnInventoryChanged;

    public InventoryModel(int size)
    {
        Slots = new InventorySlot[size];

        itemIndexMap = new Dictionary<ItemData, List<int>>();
        freeSlots = new Queue<int>();
        freeSlotSet = new HashSet<int>();

        for (int i = 0; i < size; i++)
        {
            Slots[i] = new InventorySlot();
            MarkSlotFree(i);
        }
    }

    public int AddItem(ItemData data, int amount)
    {
        if (data == null || amount <= 0)
            return amount;

        int remaining = amount;

        // Stack into existing stacks
        if (data.stackable && itemIndexMap.TryGetValue(data, out var indices))
        {
            for (int i = 0; i < indices.Count && remaining > 0; i++)
            {
                int index = indices[i];
                var slot = Slots[index];
                if (slot.IsEmpty || slot.item.data != data) continue;

                int spaceLeft = data.maxStack - slot.item.count;
                if (spaceLeft <= 0) continue;

                int toAdd = Mathf.Min(spaceLeft, remaining);
                slot.item.count += toAdd;
                remaining -= toAdd;

                OnSlotChanged?.Invoke(index);
            }
        }

        // Fill new slots
        while (remaining > 0 && TryGetFreeSlot(out int index))
        {
            int toPut = data.stackable
                ? Mathf.Min(remaining, data.maxStack)
                : 1;

            Slots[index].item = new InventoryItem
            {
                data = data,
                count = data.stackable ? toPut : 1,
                durability = data.hasDurability ? data.maxDurability : 0
            };

            remaining -= toPut;

            RemoveFreeSlot(index);
            AddIndexMapping(data, index);

            OnSlotChanged?.Invoke(index);
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

        RemoveItemInternal(data, amount);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetItemCount(ItemData data)
    {
        if (data == null) return 0;

        int total = 0;

        if (!itemIndexMap.TryGetValue(data, out var indices))
            return 0;

        for (int i = 0; i < indices.Count; i++)
        {
            var slot = Slots[indices[i]];
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
            RemoveItemInternal(ingredient.item, ingredient.amount);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    private void RemoveItemInternal(ItemData data, int amount)
    {
        int remaining = amount;

        if (!itemIndexMap.TryGetValue(data, out var indices))
            return;

        for (int i = indices.Count - 1; i >= 0 && remaining > 0; i--)
        {
            int index = indices[i];
            var slot = Slots[index];
            if (slot.IsEmpty || slot.item.data != data) continue;

            int toTake = Mathf.Min(remaining, Mathf.Max(1, slot.item.count));
            slot.item.count -= toTake;
            remaining -= toTake;

            if (slot.item.count <= 0)
            {
                slot.Clear();
                RemoveIndexMapping(data, index);
                MarkSlotFree(index);
            }

            OnSlotChanged?.Invoke(index);
        }
    }


    public void SwapSlots(int from, int to)
    {
        if (from == to) return;
        if (from < 0 || from >= Slots.Length) return;
        if (to < 0 || to >= Slots.Length) return;

        var fromItem = Slots[from].item;
        var toItem = Slots[to].item;

        Slots[from].item = toItem;
        Slots[to].item = fromItem;

        UpdateSlotMappingAfterChange(from, fromItem, Slots[from].item);
        UpdateSlotMappingAfterChange(to, toItem, Slots[to].item);


        OnSlotChanged?.Invoke(from);
        OnSlotChanged?.Invoke(to);
        OnInventoryChanged?.Invoke();
    }

    public bool SplitStack(int index)
    {
        if (index < 0 || index >= Slots.Length) return false;

        var slot = Slots[index];
        if (slot.IsEmpty) return false;

        var item = slot.item;
        if (!item.data.stackable || item.count < 2) return false;

        if (!TryGetFreeSlot(out int freeIndex)) return false;

        int half = item.count / 2;
        item.count -= half;

        Slots[freeIndex].item = new InventoryItem
        {
            data = item.data,
            count = half,
            durability = item.durability
        };

        RemoveFreeSlot(freeIndex);
        AddIndexMapping(item.data, freeIndex);

        OnSlotChanged?.Invoke(index);
        OnSlotChanged?.Invoke(freeIndex);
        OnInventoryChanged?.Invoke();
        return true;
    }

    private void AddIndexMapping(ItemData data, int index)
    {
        if (!itemIndexMap.TryGetValue(data, out var indices))
        {
            indices = new List<int>();
            itemIndexMap[data] = indices;
        }

        if (!indices.Contains(index))
            indices.Add(index);
    }

    private void RemoveIndexMapping(ItemData data, int index)
    {
        if (!itemIndexMap.TryGetValue(data, out var indices))
            return;

        indices.Remove(index);
        if (indices.Count == 0)
            itemIndexMap.Remove(data);
    }
    private void UpdateSlotMappingAfterChange(int index, InventoryItem previousItem, InventoryItem currentItem)
    {
        if (previousItem != null && previousItem.data != null && (currentItem == null || currentItem.data != previousItem.data))
        {
            RemoveIndexMapping(previousItem.data, index);
        }

        if (currentItem != null && currentItem.data != null)
        {
            AddIndexMapping(currentItem.data, index);
            RemoveFreeSlot(index);
        }
        else
        {
            MarkSlotFree(index);
        }
    }

    private void MarkSlotFree(int index)
    {
        if (freeSlotSet.Add(index))
            freeSlots.Enqueue(index);
    }

    private void RemoveFreeSlot(int index)
    {
        freeSlotSet.Remove(index);
    }

    private bool TryGetFreeSlot(out int index)
    {
        while (freeSlots.Count > 0)
        {
            int candidate = freeSlots.Dequeue();
            if (!freeSlotSet.Contains(candidate))
                continue;

            if (Slots[candidate].IsEmpty)
            {
                freeSlotSet.Remove(candidate);
                index = candidate;
                return true;
            }
            
        }
        index = -1;
        return false;
    }
}
