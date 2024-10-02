using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// singleton, indexes and manages all the characters of both players, enables interaction with characters
/// </summary>
public class PieceManager : Singleton<PieceManager>
{
   /// <summary>
   /// temporary array to store all character as generic gameObjects
   /// </summary>
   private GameObject[] allGOs;

   /// <summary>
   /// temporary list to store all character gameObjects as characters
   /// </summary>
   private List<Character> allPieces = new List<Character>();

   /// <summary>
   /// list of all currently alive characters belonging to the black team
   /// </summary>
   public List<Character> BlackPieces { get; set; } = new List<Character>();
   

   /// <summary>
   /// list of all currently alive charactes belonging to the white team
   /// </summary>
   public List<Character> WhitePieces { get; set; } = new List<Character>();
   

   /// <summary>
   /// indexes all characters in the scene at the beginning of the game into lists "blackPieces" and "whitePieces"
   /// </summary>
   private void Start()
   {
      allGOs = GameObject.FindGameObjectsWithTag("Characters");

      foreach (GameObject go in allGOs)
      {
         allPieces.Add((go.GetComponent<Character>()));
         //AllPieces.Add(go.GetComponent<Character>());
      }

      foreach (Character c in allPieces)
      {
         if (c.TeamFlag == Team.Enemy)
         {
            BlackPieces.Add(c);
         }
         else if (c.TeamFlag == Team.Ally)
         {
            WhitePieces.Add(c);
         }
      }
   }
   
   /// <summary>
   /// calls "Die" method of passed character, also removes character from the team list
   /// </summary>
   /// <param name="toBeKilled">the character to be removed from the active game</param>
   public void KillCharacter(Character toBeKilled)
   {
      if (toBeKilled.TeamFlag == Team.Enemy)
      {
         foreach (Character c in BlackPieces.ToList())
         {
            if (c == toBeKilled)
            {
               BlackPieces.Remove(toBeKilled);
               toBeKilled.Die();
            }
         }
      }
      else if (toBeKilled.TeamFlag == Team.Ally)
      {
         foreach (Character c in WhitePieces.ToList())
         {
            if (c == toBeKilled)
            {
               WhitePieces.Remove(toBeKilled);
               toBeKilled.Die();
            }
         }
      }
   }

   /// <summary>
   /// moves a passed pawn from one tile to another, also triggers the corresponding animation
   /// </summary>
   /// <param name="toBeMoved">the pawn supposed to move</param>
   /// <param name="destination">the destination tile</param>
   /// <param name="direction">the direction in which the animation should be played</param>
   /// TODO after merge: direction (char) -> (direction enum) 
   public void MovePawn(Pawn toBeMoved, Tile destination, char direction)
   {
      //if empty just move
      if (BoardManager.Instance.AreTilesCompatible(toBeMoved.Tile, destination))
      {
         toBeMoved.Move(destination);
         // toBeMoved.MoveAnimation(direction);
      }
      //if not empty, check if friend or foe
      else if (!BoardManager.Instance.AreTilesCompatible(toBeMoved.Tile, destination))
      {
         if (destination.Character.TeamFlag == Team.Enemy)
         {
            //friendly, can't move
         }
         else if (destination.Character.TeamFlag == Team.Ally)
         {
            //enemy, start fight
         }
      }
      
   }

   /// <summary>
   /// determines winner of a fight between two characters
   /// </summary>
   /// <param name="attackingCharacter">the character initiating combat</param>
   /// <param name="defendingCharacter">the character being attacked</param>
   public void GetWinner(Character attackingCharacter, Character defendingCharacter)
   {
      //NetworkingManager.Instance.SendCommand();
      
   }

   /// <summary>
   /// initiates a fight between two characters
   /// </summary>
   /// <param name="attackingCharacter">the character initiating combat</param>
   /// <param name="defendingCharacter">the character being attacked</param>
   public void InitiateFight(Character attackingCharacter, Character defendingCharacter)
   {
      
   }
}
