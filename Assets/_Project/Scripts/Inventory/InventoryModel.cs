using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel
{
    public InventorySlot[] Slots { get; private set; }

    private readonly Dictionary<ItemData, ItemRecord> itemMap;
    private readonly Queue<int> emptySlots;
    private readonly bool[] emptySlotFlags;

    public event Action<int> OnSlotChanged;
    public event Action OnInventoryChanged;

    //init
    public InventoryModel(int size)
    {
        Slots = new InventorySlot[size];

        itemMap = new Dictionary<ItemData, ItemRecord>();
        emptySlots = new Queue<int>(size);
        emptySlotFlags = new bool[size];
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

        var record = GetOrCreateRecord(data);
        int remaining = amount;

        //fill partial stack
        while (remaining > 0 && record.TryGetPartialSlot(out int partialIndex))
        {
            var slot = Slots[partialIndex];
            if (slot.IsEmpty || slot.item.data != data)
            {
                record.RemoveSlot(partialIndex);
                continue;
            }

            int spaceLeft = data.maxStack - slot.item.count;
            int toAdd = Mathf.Min(spaceLeft, remaining);
            slot.item.count += toAdd;
            record.totalCount += toAdd;
            remaining -= toAdd;

            record.UpdatePartial(partialIndex, slot.item.count, data.maxStack, data.stackable);
            OnSlotChanged?.Invoke(partialIndex);
        }
        //add to empty slot
        while (remaining > 0 && TryPopEmptySlot(out int freeIndex))
        {
            int toPut = data.stackable
                ? Mathf.Min(remaining, data.maxStack)
                : 1;

            var toolData = data as ToolItemData;

            Slots[freeIndex].item = new InventoryItem
            {
                data = data,
                count = data.stackable ? toPut : 1,
                durability = toolData != null ? toolData.maxDurability : 0
            };

            record.totalCount += toPut;
            record.AddSlot(freeIndex, Slots[freeIndex].item.count, data.maxStack, data.stackable);
            remaining -= toPut;

            OnSlotChanged?.Invoke(freeIndex);
        }

        OnInventoryChanged?.Invoke();
        return remaining;
    }

    public bool RemoveItem(ItemData data, int amount)
    {
        if (data == null || amount <= 0)
            return false;

        if (!itemMap.TryGetValue(data, out var record))
            return false;

        if (record.totalCount < amount)
            return false;

        int remaining = amount;

        //remove item from first maxed slot
        while (remaining > 0 && record.TryGetFilledSlot(out int slotIndex))
        {
            var slot = Slots[slotIndex];
            if (slot.IsEmpty || slot.item.data != data)
            {
                record.RemoveSlot(slotIndex);
                continue;
            }

            int toTake = Mathf.Min(remaining, slot.item.count);
            slot.item.count -= toTake;
            record.totalCount -= toTake;
            remaining -= toTake;

            // if count = 0 add slot to empty
            if (slot.item.count <= 0)
            {
                slot.Clear();
                record.RemoveSlot(slotIndex);
                MarkSlotEmpty(slotIndex);
            }
            // if not update the partial slot
            else
            {
                record.UpdatePartial(slotIndex, slot.item.count, data.maxStack, data.stackable);
            }

            OnSlotChanged?.Invoke(slotIndex);
        }
        //if all stacks of the same type is 0 remove the record from itemMap
        if (record.totalCount <= 0)
            itemMap.Remove(data);

        OnInventoryChanged?.Invoke();
        return remaining == 0;
    }

    public void SwapSlots(int from, int to)
    {
        if (from == to) return;
        if (!IsValidIndex(from) || !IsValidIndex(to)) return;

        var fromItem = Slots[from].item;
        var toItem = Slots[to].item;

        DetachSlot(from, fromItem);
        DetachSlot(to, toItem);

        Slots[from].item = toItem;
        Slots[to].item = fromItem;

        AttachSlot(from, Slots[from].item);
        AttachSlot(to, Slots[to].item);

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

        if (!TryPopEmptySlot(out int freeIndex)) return false;

        int half = item.count / 2;
        item.count -= half;

        Slots[freeIndex].item = new InventoryItem
        {
            data = item.data,
            count = half,
            durability = item.durability
        };

        if (itemMap.TryGetValue(item.data, out var record))
        {
            record.UpdatePartial(index, item.count, item.data.maxStack, item.data.stackable);
            record.AddSlot(freeIndex, Slots[freeIndex].item.count, item.data.maxStack, item.data.stackable);
        }

        OnSlotChanged?.Invoke(index);
        OnSlotChanged?.Invoke(freeIndex);
        OnInventoryChanged?.Invoke();
        return true;
    }

    //get item count of the requested data without loops
    public int GetItemCount(ItemData data)
    {
        if (data == null) return 0;

        return itemMap.TryGetValue(data, out var record) ? record.totalCount : 0;
    }

    //check if player has required item for crafting
    public bool HasItems(CraftingRequirements requirements)
    {
        if (!IsRequirementSatisfied(requirements.requirement1)) return false;
        if (!IsRequirementSatisfied(requirements.requirement2)) return false;
        if (!IsRequirementSatisfied(requirements.requirement3)) return false;
        if (!IsRequirementSatisfied(requirements.requirement4)) return false;

        return true;
    }

    public bool ConsumeItems(CraftingRequirements requirements)
    {
        if (!HasItems(requirements))
            return false;

        ConsumeRequirement(requirements.requirement1);
        ConsumeRequirement(requirements.requirement2);
        ConsumeRequirement(requirements.requirement3);
        ConsumeRequirement(requirements.requirement4);

        OnInventoryChanged?.Invoke();
        return true;
    }

    private void ConsumeRequirement(CraftingRequirement requirement)
    {
        if (requirement.item == null) return;
        if (requirement.amount <= 0) return;

        RemoveItem(requirement.item, requirement.amount);
    }

    private bool IsRequirementSatisfied(CraftingRequirement requirement)
    {
        if (requirement.item == null) return true;
        if (requirement.amount <= 0) return false;

        return GetItemCount(requirement.item) >= requirement.amount;
    }

    // if record exits add , if not create
    private ItemRecord GetOrCreateRecord(ItemData data)
    {
        if (!itemMap.TryGetValue(data, out var record))
        {
            record = new ItemRecord();
            itemMap[data] = record;
        }

        return record;
    }

    private void AttachSlot(int index, InventoryItem item)
    {
        if (item == null || item.data == null)
        {
            Slots[index].Clear();
            MarkSlotEmpty(index);
            return;
        }

        emptySlotFlags[index] = false;
        var record = GetOrCreateRecord(item.data);
        record.AddSlot(index, item.count, item.data.maxStack, item.data.stackable);
    }

    private void DetachSlot(int index, InventoryItem item)
    {
        if (item == null || item.data == null) return;

        if (!itemMap.TryGetValue(item.data, out var record)) return;

        record.RemoveSlot(index);
    }

    private void MarkSlotEmpty(int index)
    {
        if (emptySlotFlags[index])
            return;

        emptySlotFlags[index] = true;
        emptySlots.Enqueue(index);
    }

    //add slot as empty
    private bool TryPopEmptySlot(out int index)
    {
        while (emptySlots.Count > 0)
        {
            int candidate = emptySlots.Dequeue();
            if (!emptySlotFlags[candidate])
                continue;

            emptySlotFlags[candidate] = false;
            index = candidate;
            return true;
        }

        index = -1;
        return false;
    }

    //check is index in range of the slot size
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Slots.Length;
    }
}
