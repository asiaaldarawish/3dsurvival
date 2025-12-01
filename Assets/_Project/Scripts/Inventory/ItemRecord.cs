using System.Collections.Generic;

public class ItemRecord
{
    public int totalCount;

    private readonly LinkedList<int> allSlots = new();
    private readonly LinkedList<int> partialSlots = new();
    private readonly Dictionary<int, LinkedListNode<int>> slotNodes = new();
    private readonly Dictionary<int, LinkedListNode<int>> partialNodes = new();

    public void AddSlot(int index, int count, int maxStack, bool stackable)
    {
        //if slot record not exist create
        if (!slotNodes.ContainsKey(index))
        {
            var node = allSlots.AddLast(index);
            slotNodes[index] = node;
        }

        //if exist add
        UpdatePartial(index, count, maxStack, stackable);
    }

    // if maxstack remove from partial, if not add to partial
    public void UpdatePartial(int index, int count, int maxStack, bool stackable)
    {
        if (!stackable || count >= maxStack)
        {
            RemovePartial(index);
            return;
        }

        if (partialNodes.ContainsKey(index))
            return;

        var partialNode = partialSlots.AddLast(index);
        partialNodes[index] = partialNode;
    }

    // remove it from allslots , parial slots , dictionary nodes
    public void RemoveSlot(int index)
    {
        if (slotNodes.TryGetValue(index, out var node))
        {
            allSlots.Remove(node);
            slotNodes.Remove(index);
        }

        RemovePartial(index);
    }

    // if stsck is max remove it from partial
    public void RemovePartial(int index)
    {
        if (partialNodes.TryGetValue(index, out var node))
        {
            partialSlots.Remove(node);
            partialNodes.Remove(index);
        }
    }

    // gets first partial stack
    public bool TryGetPartialSlot(out int index)
    {
        var node = partialSlots.First;
        if (node == null)
        {
            index = -1;
            return false;
        }

        index = node.Value;
        return true;
    }

    // gets first slot of the same type
    public bool TryGetFilledSlot(out int index)
    {
        var node = allSlots.First;
        if (node == null)
        {
            index = -1;
            return false;
        }

        index = node.Value;
        return true;
    }
}