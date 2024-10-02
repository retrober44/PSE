using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Joker Role of the Character Class
/// </summary>
public class Joker : Character
{
    /// <summary>
    /// char to determine role instead of checking for type
    /// </summary>
    public Role Role { get; } = Role.Joker;
    
    private void Start()
    {
        characterSprite.UpdateTeam(teamflag);
    }
}
