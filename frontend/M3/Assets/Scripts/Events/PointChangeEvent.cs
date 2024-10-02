using UnityEngine;
using UnityEngine.Events;

public class PointChangeEvent : UnityEvent<PointChangeEventData> { }

public class PointChangeEventData
{
    public int points;
    public Player player;

    public PointChangeEventData(int points, Player player)
    {
        this.points = points;
        this.player = player;
    }
}
