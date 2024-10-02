using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// King Role of the Character Class
/// </summary>
public class King : Character
{
    /// <summary>
    /// char to determine role instead of checking for type
    /// </summary>
    public Role Role { get; } = Role.King;
    
    private void Start()
    {
        characterSprite.UpdateTeam(teamflag);
    }
}
