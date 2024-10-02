using UnityEngine;
using UnityEngine.Events;

public class OpponentMoveEvent : UnityEvent <OpponentMoveEventData>
{
    public const int OP_CODE = 30;
}

public class OpponentMoveEventData
{
    public GameObject item;
    public BoardEnums.Row row;
    public BoardEnums.Slot slot;
    public ItemEnums.Item newItem;

    public OpponentMoveEventData(BoardEnums.Row row, BoardEnums.Slot slot, ItemEnums.Item newItem, GameObject item = null)
    {
        this.item = item;
        this.slot = slot;
        this.row = row;
        this.newItem = newItem;
    }
}

