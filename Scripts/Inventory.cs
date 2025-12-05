using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [Header("Items")]
    public LootableItem[] inventoryItems = new LootableItem[4]; //This is the array holding my current items of usage
    public LootableItem equippedItem; //We probably only want our user to have one item at a time to add to the chaos
    int storedItems = 0; //This will store the index of the last item we have - basically a counter of how many items we have in our inventory

    [Header("Graphics")]
    public Sprite[] inventoryGraphics = new Sprite[4];//default placeholder for now
    public Sprite nullSprite; //This is the placeholder sprite for each inventory slot, just to prevent errors
    //I want a visual Minecraft-like box to show current inventory

    public void Start()
    {
        //Setting the default sprite across the inventory slot
        for (int i = 0; i < inventoryGraphics.Length; i++) inventoryGraphics[i] = nullSprite;
        equippedItem = inventoryItems[0]; //By default, our equipped item should be the first item in our inventory
    }

    public void EquipItem(LootableItem pickUpItem)
    {
        if (storedItems < 3)
        {
            inventoryItems[storedItems] = pickUpItem;//Add this item to our inventory slot
            inventoryGraphics[storedItems++] = pickUpItem.itemSprite; //Assigning the sprite of this item to the inventory bar on UI
        }
        else return; //If our inventory is full, then we won't equip any more items
    }

    public void DiscardItem(int itemIndex)
    {
        if (storedItems > 0)
        {
            inventoryItems[itemIndex] = null;//Remove this item this item to our inventory slot
            inventoryGraphics[itemIndex] = nullSprite; //Assigning the sprite of this item to the inventory bar on UI
            storedItems--;
        }
        else return; //If our inventory is empty, then we won't discard any more items
    }

    public void Update()
    {
        //We probably want to assign a button to equip a main item to be used
        //This item's effect will be tied to a button on the keyboard
        if (Input.GetKeyDown(KeyCode.Q)) Debug.Log("This is where we would use the effect of this item - (e.g damage, healing etc)");
    }
}
