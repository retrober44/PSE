using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    public Dictionary<int, List<GameObject>> myTiles = new Dictionary<int, List<GameObject>>();

    public GameObject boardContainer;

    public Sprite blackSprite, whiteSprite;

    private List<Tile> highlightedTiles = new List<Tile>();

    
    
    // Start is called before the first frame update
    void Start()
    {
        int rowCounter = 0;
        
        // name every tile on the board and add the tile and its status to the dictionary
        foreach (Transform child in boardContainer.transform)
        {
            List<GameObject> goList = new List<GameObject>();

            bool isBlackTile = true;

            int counter = 0;

            foreach (Transform tile in child)
            {
                goList.Add(tile.gameObject);

                if (isBlackTile)
                {
                    tile.gameObject.GetComponent<Tile>().defaultSprite = blackSprite;
                }
                else
                {
                    tile.gameObject.GetComponent<Tile>().defaultSprite = whiteSprite;
                }

                tile.gameObject.name = rowCounter + "-" + counter;

                isBlackTile = !isBlackTile;
                ++counter;
            }

            myTiles.Add(rowCounter, goList);

            ++rowCounter;
        }

        Highlight(myTiles[0][5]);
        // Unhighlight();
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// highlights the tile before, behind, to the left and right from the chosen tile by spliting/checking the name/position
    /// </summary>
    /// <param name="clickedTile"></param>
    void Highlight(GameObject clickedTile)
    {
        highlightedTiles.Clear();

        string[] nameParts = clickedTile.name.Split('-');

        int myY = Int32.Parse(nameParts[0]);
        int myX = Int32.Parse(nameParts[1]);

        // on the left border
        int leftX = myX - 1;
        if (leftX >= 0 && leftX <= 6)
        {
            GameObject leftTile = myTiles[myY][leftX];
            leftTile.GetComponent<Tile>().Highlight();

            highlightedTiles.Add(leftTile.GetComponent<Tile>());
        }

        // on the right border
        int rightX = myX + 1;
        if (rightX >= 0 && rightX <= 6)
        {
            GameObject rightTile = myTiles[myY][rightX];
            rightTile.GetComponent<Tile>().Highlight();

            highlightedTiles.Add(rightTile.GetComponent<Tile>());
        }

        // on the top border
        int topY = myY - 1;
        if (topY >= 0 && topY <= 5)
        {
            GameObject topTile = myTiles[topY][myX];
            topTile.GetComponent<Tile>().Highlight();

            highlightedTiles.Add(topTile.GetComponent<Tile>());
        }

        // on the bottom border
        int bottomY = myY + 1;
        if (bottomY >= 0 && bottomY <= 5)
        {
            GameObject bottomTile = myTiles[bottomY][myX];
            bottomTile.GetComponent<Tile>().Highlight();

            highlightedTiles.Add(bottomTile.GetComponent<Tile>());
        }

        /*Diagonal
         * // on the left-top corner
        int topLeY = myY - 1;
        int leftToX = myX - 1;
        if ((leftToX >= 0 && leftToX <= 6) && (topLeY >= 0 && topLeY <= 5))
        {
            GameObject leftTopTile = myTiles[topLeY][leftToX];
            leftTopTile.GetComponent<Tile>().Highlight();
            
            highlightedTiles.Add(leftTopTile.GetComponent<Tile>());
        }

        // on the right-top corner
        int rightToX = myX + 1;
        int topRiY = myY - 1;
        if ((rightToX >= 0 && rightToX <= 6) && (topRiY >= 0 && topRiY <= 5))
        {
            GameObject rightTopTile = myTiles[topRiY][rightToX];
            rightTopTile.GetComponent<Tile>().Highlight();
            
            highlightedTiles.Add(rightTopTile.GetComponent<Tile>());
        }

        // on the left-bottom corner
        int leftBoX = myX - 1;
        int bottomLeY = myY + 1;
        if ((bottomLeY >= 0 && bottomLeY <= 5) && (leftBoX >= 0 && leftBoX <= 6))
        {
            GameObject leftBottomTile = myTiles[bottomLeY][leftBoX];
            leftBottomTile.GetComponent<Tile>().Highlight();
            
            highlightedTiles.Add(leftBottomTile.GetComponent<Tile>());
        }

        // on the right-bottom corner
        int rightBoX = myX + 1;
        int bottomRiY = myY + 1;
        if ((bottomRiY >= 0 && bottomRiY <= 5) && (rightBoX >= 0 && rightBoX <= 6))
        {
            GameObject rightBottomTile = myTiles[bottomRiY][rightBoX];
            rightBottomTile.GetComponent<Tile>().Highlight();
            
            highlightedTiles.Add(rightBottomTile.GetComponent<Tile>());
        }*/
    }

    /// <summary>
    /// end the highlighting by calling .UndoHighlighting which resets the dafaultSprite
    /// and clears the list highlitedTiles
    /// </summary>
    void Unhighlight()
    {
        foreach (var highlightedTile in highlightedTiles)
        {
            highlightedTile.UndoHighlighting();
        }

        highlightedTiles.Clear();
    }

    /*
 * +areTilesCompatible(tileA, tileB) haben, die kann so implementiert werden:
 * 
    Tile hat einen member Character character, der entweder null oder der auf dem Feld stehende Character ist
    BoardManager fragt Tile getOccupyingCharacer(), liefert einen Characer zurück
    BM checkt: Wenn Rückgabe == null, move erlaubt
    Wenn Rückgabe != null: Wenn Character.getTeamFlag() == GameManager.getTeamFlag() [gibt es noch nicht],
    dann move nicht erlaubt, ansonsten move (mit Kampf) erlaubt
 */

    /// <summary>
    /// checks if next Move is allowed by checking if a tile is occupied or an enemy 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool AreTilesCompatible(Tile a, Tile b)
    {
        Character currentCharPos = GetOccupyingCharacter(a);
        Character nextCharPos = GetOccupyingCharacter(b);

        if (nextCharPos == null)
        {
            
            Debug.Log("move b' get out my way");
            //move(currentCharPos, nextCharPos);
            return true;
        }
        else if (nextCharPos != null)// && Character.getTeamFlag() != GameManager.getTeamFlag())
        {
            /*if (Character.getTeamFlag() == GameManager.getTeamFlag())
            {
                return false;
            }
            else*/
            
                Debug.Log("Fiiiight!");
                //fight();
            
        }

        return false;
    }

    Character GetOccupyingCharacter(Tile currentPosition)
    {
        return currentPosition.character;
    }
}