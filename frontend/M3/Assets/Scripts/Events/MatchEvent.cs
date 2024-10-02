using UnityEngine;
using UnityEngine.Events;

public class MatchEvent : UnityEvent<MatchEventData> { }

public class MatchEventData
{
    public int points;
    public int match;

    public MatchEventData(int points, int match)
    {
        this.points = points;
        this.match = match;
    }
}
