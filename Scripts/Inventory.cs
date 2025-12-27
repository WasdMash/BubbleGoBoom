using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [Header("Items")]
    public LootableItem[] inventoryItems = new LootableItem[4]; //This is the array holding my current items of usage
    public int[] inventoryStacks = new int[4]; //Consumables can be stacked but not weapons - I'm not storing the durability of multiple swords
    public LootableItem equippedItem; //We probably only want our user to have one item at a time to add to the chaos
    [SerialiseField] int storedItems = 0; //This will store the index of the last item we have - basically a counter of how many items we have in our inventory

    [Header("Graphics")]
    public Sprite[] inventoryGraphics = new Sprite[4];//default placeholder for now
    public Sprite nullSprite; //This is the placeholder sprite for each inventory slot, just to prevent errors
    //I want a visual Minecraft-like box to show current inventory

    //I should probably bind a button to run EquipItem() and another to use its features

    public void PickUpItem(LootableItem pickUpItem)
    {
        //First check if the item already exists - if it does, stack it
        for(int i = 0; i < storedItems; i++)
        {
            if(inventoryItems[i].CompareTo(pickUpItem) != 0)
            {
                //Should probably check if the lootableItem is a weapon
                //We should break the for loop if it is
                inventoryStacks[i]++; //Only if pickUpItem is not a weapon
                return; //Our job is done - we don't need 2 slots taken up by the same item
            }
        }
        if (storedItems < 3)
        {
            inventoryItems[storedItems] = pickUpItem;//Add this item to our inventory slot
            inventoryGraphics[storedItems++] = pickUpItem.getSprite(); //Assigning the sprite of this item to the inventory bar on UI
            //Automatically set the new item to be our equipped item
            equippedItem = inventoryItems[storedItems];
        }
        else return; //If our inventory is full, then we won't equip any more items
    }

    public void EquipItem(int itemIndex){ equippedItem = inventoryItems[itemIndex];}

    public void ReloadInventory(ref Inventory storedInventory)
    {
        equippedItem = storedInventory.equippedItem;
        for(int i = 0; i < 4; i++)
        {
            //Only replace the items which aren't the same
                //Ugh, I have to store the inventory stack also
            if(inventoryItems[i].CompareTo(storedInventory.inventoryItems[i]) != 0)
            {
                inventoryItems[i] = storedInventory.inventoryItems[i];
                //Handle that stack also
                inventoryStacks[i] = storedInventory.inventoryStacks[i];
            } 
        }
    }

    public void DiscardItem(int itemIndex)
    {
        if (storedItems > 0)
        {
            if(inventoryStacks[itemIndex] > 0) inventoryStacks[itemIndex]--; //One less in the stack to worry about
            else
            {
                inventoryItems[itemIndex] = null;//Remove this item this item to our inventory slot
            inventoryGraphics[itemIndex] = nullSprite; //Assigning the sprite of this item to the inventory bar on UI
            storedItems--;
            }           
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

// Example Data Structure (using C# syntax)
[System.Serializable] // For easy serialization to JSON/binary
public class ChestData
{
    public string ChestID;
    public bool opened = false; //We need to keep tracked of this so that we only save the inventories of opened chests
    //Each chest in theory stores a 4x4 grid of items
    public LootableItem[] inventoryItems = new LootableItem[16];

    //Might need a function in here to randomly generate chest contents upon opening
    public void generateContents()
    {
        LootableItem[] itemsToChooseFrom = GameManager.GetPossibleItems();
        for(int i = 0; i < 16; i++)
        {
            //Randomly selecting an item
                //If I want to implement rarity, I'll need to change this function by implementing a probability distribution
                    //Kinda like I did for enemy drops in EnemyHealth
            inventoryItems[i] = Random.GetItems(itemsToChooseFrom, 1).First();
        }
    }
}

