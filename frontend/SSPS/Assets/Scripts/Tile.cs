using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //public Team teamSpawnFlag;
    public Character character;
    public Sprite defaultSprite;
    public Sprite highlightSprite;
    
    public Character Character { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Highlight()
    {
        Debug.Log("I am a mighty tile. My name is " + gameObject.name + " and I am highlighted.");
        gameObject.GetComponent<SpriteRenderer>().sprite = highlightSprite;
    }

    public void UndoHighlighting()
    {
        Debug.Log("My Name is " + gameObject.name + " and I am no longer highlighted.");
        gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }
    

}

