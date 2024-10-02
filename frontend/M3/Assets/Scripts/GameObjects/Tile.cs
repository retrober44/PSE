using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{

    //public TileEnums.TileType _tileType;
    public Item tileItem { get; set; }
    //private int _tileValue;
    //public int position;

    //private int tileRow, tileColumn;
    public int tileRow, tileColumn;

    //Position TilePosition;


    // Start is called before the first frame update
    void Start()
    {
       // GameObject tile = Instantiate(itemPrefab, transform.position, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
   
    
}
