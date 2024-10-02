using UnityEngine;

public class Item : MonoBehaviour
{
   //Item point values: 150, 100, 70, 40, 30, 20, 10
   private int _itemPoints;
   private int _row, _column;
   //public GameObject _itemPrefab;
   public ItemEnums.Item _itemType;

   /* public struct ItemPosition
   {
      private int _row;
      private int _column;

   } 
   
   private ItemPosition _itemPosition;
   
   */
   
   public ItemEnums.Item GETItemType()
   {
      return _itemType;
   }
   
   public void SetItemType(ItemEnums.Item itemType)
   {
      this._itemType = itemType;
      this._itemPoints = (int) itemType;
   }

   public int GETItemPoints()
   {
      return _itemPoints;
   }

   public GameObject GETItemObject()
   {
     return gameObject.GetComponent<Item>().gameObject;
   }
   
   

   /*public ItemPosition getItemPosition()
   {
      return _itemPosition;
   }*/

}
