using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPrefab : MonoBehaviour
{
   
    
    [System.Serializable]
    public struct ItemPrefabStruct
    {
        public ItemEnums.Item _itemType;
        public Item itemPrefab;

    }

    public ItemPrefabStruct[] ItemPrefabArray;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
