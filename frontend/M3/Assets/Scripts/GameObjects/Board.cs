
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class Board : MonoBehaviour
{

    //rows, columns of the board (5,5)
    public int rows, columns;
    //tile array to store tiles
    private Tile[,] _boardTiles;
    //array to store all Item Objects
    public Item[] itemsPresent;
    //prefab to instantiate tiles of board
    public Tile backgroundTile;
    //temporary int array to test board initialisation
    //private readonly int[] _boardValues = { 3, 4, 2, 1, 6, 4, 0, 4, 5, 3, 1, 1, 3, 0, 6, 1, 2, 5, 1, 1, 0, 4, 3, 2, 1 };
    private readonly int[] _boardValues = { 3, 3, 3, 1, 2,
                                            1, 0, 4, 5, 2,
                                            2, 1, 3, 0, 2,
                                            2, 2, 1, 6, 3,
                                            2, 4, 3, 2, 1};
    //dictionary to store item objects (might be used instead of itemsPresent array
    Dictionary<int, Item> boardItems = new Dictionary<int, Item>();
    //ItemPrefab objects to call Prefabs for individual items
    private ItemPrefab boardPrefabs;
    //Item variable to store items temporarily 
    private Item referenceItem = null, currentItem = null;
    private Tile currentTile = null;
    private string positionID;

    private void Awake()
    {
        setUpBoard();
        //CreateBoard();

    }


    void Start()
    {
        //setUpBoard();
        CreateBoard();
    }

    private void CreateBoard()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                currentTile = Instantiate(backgroundTile, tempPosition, Quaternion.identity);
                currentTile.transform.parent = this.transform;
                positionID = "( " + i + ", " + j + " )";
                currentTile.tileRow = i;
                currentTile.tileColumn = j;
                currentTile.name = "Tile " + positionID;
                _boardTiles[i, j] = currentTile;
                referenceItem = boardPrefabs.ItemPrefabArray[_boardValues[(((i * rows) + j))]].itemPrefab;
                currentItem = Instantiate(referenceItem, tempPosition, Quaternion.identity);
                //
                currentTile.tileItem = currentItem;
                //
                currentItem.transform.parent = currentTile.transform;
                currentItem.name = "Item " + positionID;
            }

        }
    }

    private void setUpBoard()
    {
        _boardTiles = new Tile[rows, columns];
        boardPrefabs = gameObject.GetComponent<ItemPrefab>();
    }

    private void addItemToBoard(int position, Item item)
    {

        boardItems.Add(position, item);

    }

    // Update is called once per frame
    void Update()
    {

    }

  

    /*private readonly int[] _boardValues = { 3, 3, 3, 1, 2,
                                              1, 0, 4, 5, 2,
                                              2, 1, 3, 0, 2,
                                              2, 2, 1, 6, 3,
                                              2, 4, 3, 2, 1};*/
    public void GetMatches()
    {
        List<Tile> columnsList = new List<Tile>();
        List<Tile> rowsList = new List<Tile>();
        int x = 0;
        int y = 0;

        for (y = 0; y < rows; y++)
        {
            for (x = 0; x < columns; x++)
            {
                columnsList.Add(_boardTiles[x, y]);
            }
            if (findMatch(ref columnsList))
            {
                return;
            }
            columnsList.Clear();
        }

        x = 0;
        y = 0;

        for (x = 0; x < columns; x++)
        {
            for (y = 0; y < rows; y++)
            {
                rowsList.Add(_boardTiles[x, y]);
            }
            if (findMatch(ref rowsList))
            {
                return;
            }
            rowsList.Clear();
        }
    }

    private bool findMatch(ref List<Tile> passedList)
    {
        List<Tile> matchList = new List<Tile>();
        ItemEnums.Item item = ItemEnums.Item.empty;

        if (passedList[passedList.Count / 2].tileItem._itemType == ItemEnums.Item.empty) return false;

        else
        {
            for (int x = 0; x < passedList.Count; x++)
            {
                if (passedList[x].tileItem._itemType != ItemEnums.Item.empty)
                {
                    //push first element anyway
                    item = passedList[x].tileItem._itemType;
                    if (matchList.Count == 0)
                    {
                        matchList.Add(passedList[x]);
                        continue;
                    }

                    //compare current element with last element
                    if (matchList.Count != 0 && matchList[matchList.Count - 1].tileItem._itemType == item)
                    {
                        matchList.Add(passedList[x]);
                    }
                    else
                    {
                        if (matchList.Count >= 3)
                        {
                            realizeMatch(ref matchList);
                            return true;
                        }
                        matchList.Clear();
                        continue;
                    }
                }
                else
                {
                    matchList.Clear();
                    continue;
                }
            }
        }
        if (matchList.Count >= 3)
        {
            realizeMatch(ref matchList);
            return true;
        }
        else return false;
    }

    private void realizeMatch(ref List<Tile> matchList)
    {
        int points;
        switch (matchList.Count)
        {
            case 3: //so its a match
                points = GetPoints(ref matchList);
                EventManager.Match.Invoke(new MatchEventData(points, matchList.Count));
                DestroyTiles(ref matchList);
                return;            
            case 4://its a match of 4  //4fache matches = doppelte Punktzahl
                points = GetPoints(ref matchList) * 2;
                EventManager.Match.Invoke(new MatchEventData(points, matchList.Count));
                DestroyTiles(ref matchList);
                return;
            case 5:// its a match of 5 //5fache Matches = dreifache Punktzahl
                points = GetPoints(ref matchList) * 3;
                EventManager.Match.Invoke(new MatchEventData(points, matchList.Count));
                DestroyTiles(ref matchList);
                return;
            default:
                return;//its no match at all, 3 in a row are required
        }
    }

    private int GetPoints(ref List<Tile> matchList)
    {
        if (matchList != null && matchList.Count > 0) return (int)matchList[0].tileItem._itemType;
        else return 0;
    }


    private bool DestroyTiles(ref List<Tile> matchList)
    {
        for (int i = 0; i < matchList.Count; i++)
        {
            try
            {
                Destroy(_boardTiles[matchList[i].tileRow, matchList[i].tileColumn].tileItem.GETItemObject());
                _boardTiles[matchList[i].tileRow, matchList[i].tileColumn].tileItem._itemType = ItemEnums.Item.empty;
                //shiftArray();
            }
            catch (Exception e)
            {
                Debug.Log("exception thrown destroying Tile instance on board from matchList after match" + e.Message);
                return false;
            }
        }
        return true;
    }
}