using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Pawn : Character
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    /// <summary>
    /// role of the pawn
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// rock sprite(hidden)
    /// </summary>
    [SerializeField]
    private Sprite hiddenRockSprite;
    
    /// <summary>
    /// paper sprite(hidden)
    /// </summary>
    [SerializeField]
    private Sprite hiddenPaperSprite;
    
    /// <summary>
    /// scissor sprite(hidden)
    /// </summary>
    [SerializeField]
    private Sprite hiddenScissorSprite;
    
    /// <summary>
    /// scissor sprite(visible)
    /// </summary>
    [SerializeField]
    private Sprite visibleRockSprite;
    
    /// <summary>
    /// paper sprite(visible)
    /// </summary>
    [SerializeField]
    private Sprite visiblePaperSprite;
    
    /// <summary>
    /// scissor sprite(visible)
    /// </summary>
    [SerializeField]
    private Sprite visibleScissorSprite;

    /// <summary>
    /// boolean determining if the weapon is visible to the opponent
    /// </summary>
    public bool WeaponIsVisible { get; set; } = false;


    /// <summary>
    /// reference to the Animator component
    /// </summary>
    [SerializeField]
    private Animator animator;

    /// <summary>
    /// reference to the SpriteRenderer component
    /// </summary>
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        characterSprite.UpdateTeam(teamflag);
    }
    
    //currently used for dirty testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveAnimation(MoveDirection.Up);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveAnimation(MoveDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveAnimation(MoveDirection.Down);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveAnimation(MoveDirection.Right);
        }
        else if (Input.GetKeyDown((KeyCode.I)))
        {
            AttachWeapon(Role.Rock);
        }
        else if (Input.GetKeyDown((KeyCode.O)))
        {
            AttachWeapon(Role.Paper);
        }
        else if (Input.GetKeyDown((KeyCode.P)))
        {
            AttachWeapon(Role.Scissor);
        }
        else if (Input.GetKeyDown((KeyCode.V)))
        {
            ShowWeapon();
        }
        else if (Input.GetKeyDown((KeyCode.B)))
        {
            WeaponIsVisible = false;
        }
    }

    /// <summary>
    /// changes role/weapon of pawn and updates the Sprite
    /// </summary>
    /// <param name="newWeapon">a string declaring the weapon type</param>
    public void AttachWeapon(Role newRole)
    {
        if (newRole == Role.Rock)
        {
            Role = Role.Rock;
            if (WeaponIsVisible)
            {
                spriteRenderer.sprite = visibleRockSprite;
            }
            else if(teamflag == Team.Ally)
            {
                spriteRenderer.sprite = hiddenRockSprite;
            }
            else
            {
                spriteRenderer.sprite = null;
            }


        }
        else if (newRole == Role.Paper)
        {
            Role = Role.Paper;
            if (WeaponIsVisible)
            {
                spriteRenderer.sprite = visiblePaperSprite;
            }
            else if(teamflag == Team.Ally)
            {
                spriteRenderer.sprite = hiddenPaperSprite;
            }
            else
            {
                spriteRenderer.sprite = null;
            }
            
        }
        else if (newRole == Role.Scissor)
        {
            Role = Role.Scissor;
            if (WeaponIsVisible)
            {
                spriteRenderer.sprite = visibleScissorSprite;
            }
            else if(teamflag == Team.Ally)
            {
                spriteRenderer.sprite = hiddenScissorSprite;
            }
            else
            {
                spriteRenderer.sprite = null;
            }
            
        }
        else
        {
            Debug.Log("Not a valid weapon type");
        }
    }

    /// <summary>
    /// updates sprite to visible weapon, called when weapon is revealed(after fight)
    /// </summary>
    public void ShowWeapon()
    {
        WeaponIsVisible = true;

        if (Role == Role.Rock)
        {
            spriteRenderer.sprite = visibleRockSprite;
        }
        else if (Role == Role.Paper)
        {
            spriteRenderer.sprite = visiblePaperSprite;
        }
        else if (Role == Role.Scissor)
        {
            spriteRenderer.sprite = visibleScissorSprite;
        }
    }

   

    /// <summary>
    /// clears current occupied tile, sets tile to destination and assigns character to destination tile
    /// </summary>
    /// <param name="destination">the tile the character moves to</param>
    public void Move(Tile destination)
    {
        Tile.Character = null;
        Tile = destination;
        destination.Character = this;
    }

    /// <summary>
    /// triggers rootmotion to move character
    /// </summary>
    /// <param name="direction"></param>
    /// <exception cref="Exception"></exception>
    public void MoveAnimation(MoveDirection direction)
    {
        if (direction == MoveDirection.Up)
        {
            animator.SetTrigger("UpTrigger");
        }
        else if (direction == MoveDirection.Down)
        {
            animator.SetTrigger(("DownTrigger"));
        }
        else if (direction == MoveDirection.Left) 
        { 
            animator.SetTrigger(("LeftTrigger"));
        }
        else if (direction == MoveDirection.Right) 
        { 
            animator.SetTrigger(("RightTrigger"));
        }
        else 
        { 
            Exception invalidInputMoveAnimation = new Exception("not a valid input, only accepts char 'u', 'd', 'l', 'r'"); 
            Debug.LogException(invalidInputMoveAnimation); 
            throw new Exception(invalidInputMoveAnimation.Message);
        }
    }
}
