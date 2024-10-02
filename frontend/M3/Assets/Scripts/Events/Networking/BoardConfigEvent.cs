using System;
using UnityEngine;
using UnityEngine.Events;

public class BoardConfigEvent : UnityEvent <BoardConfigEventData>
{
    public const int OP_CODE = 11;
}

public class BoardConfigEventData
{
    public ItemEnums.Item[,] board;
    public ItemEnums.Item[] itemspinner;

    public BoardConfigEventData(ItemEnums.Item[,] board, ItemEnums.Item[] itemspinner)
    {
        if (itemspinner.Length != 3) throw new ArgumentException("Array needs to have exact 3 Items", nameof(itemspinner));
        this.board = board;
        this.itemspinner = itemspinner;
    }
}
