using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSessionObject
{
    //this data structure is returned by the client service when a game match is found
    public string PlayerSessionId;
    public string PlayerId;
    public string GameSessionId;
    public string FleetId;
    public string CreationTime;
    public string Status;
    public string IpAddress;
    public string Port;
}
