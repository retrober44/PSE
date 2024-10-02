using UnityEngine;
using UnityEngine.Events;

public class MoveEvent : UnityEvent <MoveEventData>
{ 
    public const int OP_CODE = 30;
}

public class MoveEventData
{
    public GameObject item;
    public BoardEnums.Row row;
    public BoardEnums.Slot slot;

    public MoveEventData(BoardEnums.Row row, BoardEnums.Slot slot, GameObject item = null)
    {
        this.item = item;
        this.slot = slot;
        this.row = row;
    }
}

