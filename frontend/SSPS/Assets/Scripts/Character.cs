using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Role
{
    King,
    Joker,
    Rock,
    Paper,
    Scissor,
    None
}

public class Character: MonoBehaviour
{

    /// <summary>
    /// enum flag to see which team the character belongs to
    /// </summary>
    [SerializeField] 
    protected Team teamflag;
    
    /// <summary>
    /// getter for teamFlag
    /// </summary>
    public Team TeamFlag => teamflag;

    /// <summary>
    /// the current tile the character occupies
    /// </summary>
    public Tile Tile { get; set; }

    [SerializeField] 
    protected CharacterSprite characterSprite;

    private void Start()
    {
        characterSprite.UpdateTeam(teamflag);
    }

    /// <summary>
    /// clears the occupied tile and sets the gameObject as inactive
    /// </summary>
    public void Die()
    {
     Tile.Character = null;
     
     gameObject.SetActive(false);
    }

    /// <summary>
    /// OnClick method of character
    /// </summary>
    public void OnClick()
    {
     
     //TO DO
     
     //hightlight all movable tiles
     //BoardManager.instance.Highlight(Tile);

        
     //make this the selected character in TurnManager
     //TurnManager.instance.OnCharacterSelected(this);
     
     //if joker-picking-phase, then replace this character with joker
     
     //if king-picking-phase, then replace this character with king
    }
}
