using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel
{
    public InventorySlot[] Slots { get; private set; }

    // NEW
    private Dictionary<ItemData, List<int>> itemIndexMap;
    private Queue<int> freeSlots;

    public event Action<int> OnSlotChanged;
    public event Action OnInventoryChanged;

    public InventoryModel(int size)
    {
        Slots = new InventorySlot[size];

        itemIndexMap = new Dictionary<ItemData, List<int>>();
        freeSlots = new Queue<int>();

        for (int i = 0; i < size; i++)
        {
            Slots[i] = new InventorySlot();
            freeSlots.Enqueue(i); // all empty at start
        }
    }

    // Add a certain amount of an item. Returns leftover (if inventory full)
    public int AddItem(ItemData data, int amount)
    {
        if (data == null || amount <= 0)
            return amount;

        int remaining = amount;

        // STACK INTO existing stacks 
        if (data.stackable && itemIndexMap.TryGetValue(data, out var indices))
        {
            for (int i = 0; i < indices.Count && remaining > 0; i++)
            {
                int idx = indices[i];
                var slot = Slots[idx];

                int spaceLeft = data.maxStack - slot.item.count;
                if (spaceLeft <= 0) continue;

                int toAdd = Mathf.Min(spaceLeft, remaining);
                slot.item.count += toAdd;

                remaining -= toAdd;
                OnSlotChanged?.Invoke(idx);
            }
        }

        // PLACE INTO NEW SLOTS
        while (remaining > 0 && freeSlots.Count > 0)
        {
            int idx = freeSlots.Dequeue();
            int toPut = data.stackable
                ? Mathf.Min(remaining, data.maxStack)
                : 1;

            Slots[idx].item = new InventoryItem
            {
                data = data,
                count = toPut,
                durability = data.hasDurability ? data.maxDurability : 0
            };

            remaining -= toPut;

            // track where this item lives
            if (!itemIndexMap.ContainsKey(data))
                itemIndexMap[data] = new List<int>();

            itemIndexMap[data].Add(idx);

            OnSlotChanged?.Invoke(idx);
        }

        OnInventoryChanged?.Invoke();
        return remaining;
    }


    public void SwapSlots(int from, int to)
    {
        if (from == to) return;
        if (from < 0 || from >= Slots.Length) return;
        if (to < 0 || to >= Slots.Length) return;

        var tmp = Slots[from].item;
        Slots[from].item = Slots[to].item;
        Slots[to].item = tmp;

        OnSlotChanged?.Invoke(from);
        OnSlotChanged?.Invoke(to);
        OnInventoryChanged?.Invoke();
    }

    public bool SplitStack(int index)
    {
        // validate
        if (index < 0 || index >= Slots.Length) return false;

        var slot = Slots[index];
        if (slot.IsEmpty) return false;

        var item = slot.item;
        if (!item.data.stackable || item.count < 2) return false;

        // calculate half
        int half = item.count / 2;
        item.count -= half;

        // find empty slot
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].IsEmpty)
            {
                Slots[i].item = new InventoryItem
                {
                    data = item.data,
                    count = half,
                    durability = item.durability
                };

                OnSlotChanged?.Invoke(index);
                OnSlotChanged?.Invoke(i);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // no empty slot available
        return false;
    }


}
