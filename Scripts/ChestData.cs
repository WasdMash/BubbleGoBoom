using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

[System.Serializable]
public class ChestData: MonoBehaviour, IItemContainer
{
    [SerializeField] Sprite closedSprite, openSprite; //Need a sprite to represent it both closed and open
    [SerializeField] bool opened = false; //We need to keep tracked of this so that we only save the inventories of opened chests
    //Each chest in theory stores a 4x4 grid of items
    public LootableItem[] inventoryItems = new LootableItem[16];
    public int[] inventoryStacks = new int[16]; //Handles the stacks of each item in a chest
    public InventorySlot[] inventoryGraphics = new InventorySlot[16];//default placeholder for now

    //Annoyingly, can't just drag and click this like with other natively supported things in Unity - bruh
    public void Awake()
    {
        inventoryGraphics = GetComponentsInChildren<InventorySlot>();
    }
 
    //Might need a function in here to randomly generate chest contents upon opening
    public void generateContents()
    {
        opened = true; //We've now opened the sprite
        gameObject.GetComponent<SpriteRenderer>().sprite = openSprite; //Let's now change the sprite to reflect this
        LootableItem[] itemsToChooseFrom = GameManager.GetPossibleItems();
        for(int i = 0; i < 16; i++)
        {
            //Randomly selecting an item
                //If I want to implement rarity, I'll need to change this function by implementing a probability distribution
                    //Kinda like I did for enemy drops in EnemyHealth
            int randomItemIndex = UnityEngine.Random.Range(0, itemsToChooseFrom.Length-1);
            inventoryItems[i] = itemsToChooseFrom[randomItemIndex];
        }
    }
    public LootableItem GetItem(int index) => inventoryItems[index];
    public void SetItem(int index, LootableItem item)
    {
        //First check if a copy of the item exists - if so, stack it
        if(inventoryItems[index].CompareTo(item) != 0)
        {
            if(inventoryItems[index] is not Weapon) inventoryStacks[index]++; //Only if pickUpItem is not a weapon
        }
        else if(inventoryItems[index] == null) inventoryItems[index] = item;//Add this item to our empty inventory slot
        RefreshUI();
    }
    
    /*
     Need to make a system of either selecting or dragging things in and out of this chest
    */
    // Swaps items between two indices (used for dragging within the chest)
    public void SwapItems(int indexA, int indexB)
    {
        //Swapping the actual item
        LootableItem temp = inventoryItems[indexA];
        inventoryItems[indexA] = inventoryItems[indexB];
        inventoryItems[indexB] = temp;

        //Will need a function to refresh the UI to show the new items
        RefreshUI();
    }

    public void DiscardItem(int index)
    {
        inventoryItems[index] = null;
        RefreshUI();
    } 

        public void RefreshUI()
    {
        //In theory, if inventoryGraphics are my UI slots holding my items, this should be enough
            //Should probably show the stack number of an item as well in this function
        for(int i = 0; i < 16; i++)
        {
            inventoryGraphics[i].Setup(i, inventoryItems[i], this);
        }
    }

    //Functions to open and close the chest
    public void closeChest() {opened = false; GetComponent<SpriteRenderer>().sprite = closedSprite;}
    public void openChest() {opened = true; GetComponent<SpriteRenderer>().sprite = openSprite; RefreshUI();}
}

