using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSprite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// white team character sprite
    /// </summary>
    [SerializeField] 
    private Sprite whiteCharacterSprite;

    /// <summary>
    /// black team character sprite
    /// </summary>
    [SerializeField] 
    private Sprite blackCharacterSprite;
    
    /// <summary>
    /// reference to the SpriteRenderer component
    /// </summary>
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void UpdateTeam(Team teamFlag)
    {
        if (teamFlag == Team.Enemy)
        {
            spriteRenderer.sprite = blackCharacterSprite;
        }
        else if (teamFlag == Team.Ally)
        {
            spriteRenderer.sprite = whiteCharacterSprite;
        }
        
    }
}
