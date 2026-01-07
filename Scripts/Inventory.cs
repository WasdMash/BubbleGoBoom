using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class Inventory : MonoBehaviour, IItemContainer
{
    [Header("Items")]
    protected InventoryInfo info;
    public LootableItem equippedItem; //We probably only want our user to have one item at a time to add to the chaos

    [Header("Graphics")]
    public InventorySlot[] inventoryGraphics = new InventorySlot[4];//default placeholder for now
    public Sprite nullSprite; //This is the placeholder sprite for each inventory slot, just to prevent errors
    //I want a visual Minecraft-like box to show current inventory

    public void Awake()
    {
        info = new InventoryInfo();
        inventoryGraphics = GetComponentsInChildren<InventorySlot>();
        RefreshUI();
    }
    public LootableItem GetItem(int index)
    {
        if(info.inventoryItems[index] != null) return info.inventoryItems[index];
        else return null;
    }

    public void PickUpItem(LootableItem item)
    {
        //First check if the item already exists - if it does, stack it
        for(int i = 0; i < info.storedItems; i++)
        {
            if(info.inventoryItems[i].CompareTo(item) != 0)
            {
                //Should probably check if the lootableItem is a weapon
                //We should break the for loop if it is
                if(info.inventoryItems[i] is not Weapon) info.inventoryStacks[i]++; //Only if pickUpItem is not a weapon
                return; //Our job is done - we don't need 2 slots taken up by the same item
            }
        }
        if (info.storedItems < 3)
        {
            SetItem(info.storedItems, item); //Assign the item to the last available spot
            //Automatically set the new item to be our equipped item
            if(info.storedItems == 0) equippedItem = info.inventoryItems[info.storedItems++];
        }
        else return; //If our inventory is full, then we won't equip any more items
    }
    public void SetItem(int index, LootableItem item)
    {
        if(item != null)
        {
            //The order of these checks is important - will get errors if we first compare a null value in .CompareTo()
            if(info.inventoryItems[index] == null) info.inventoryItems[index] = item;//Add this item to our empty inventory slot
            else if(info.inventoryItems[index].CompareTo(item) != 0)
            {
                //First check if a copy of the item exists - if so, stack it
                if(info.inventoryItems[index] is not Weapon) info.inventoryStacks[index]++; //Only if pickUpItem is not a weapon
            }
        } 
        else info.inventoryItems[index] = null;
        RefreshUI();
    }

    //Should perhaps trigger the selected function of UI to visually show equipped item
    public void EquipItem(int itemIndex){ equippedItem = info.inventoryItems[itemIndex];}

    //I don't really know what use cases they may be for this function as opposed to the copy constructor, but I'll keep it just in case
    public void ReloadInventory(ref Inventory storedInventory)
    {
        equippedItem = storedInventory.equippedItem;
        for(int i = 0; i < 4; i++)
        {
            //Only replace the items which aren't the same
                //Ugh, I have to store the inventory stack also
            if(info.inventoryItems[i].CompareTo(storedInventory.info.inventoryItems[i]) != 0)
            {
                info.inventoryItems[i] = storedInventory.info.inventoryItems[i];
                //Handle that stack also
                info.inventoryStacks[i] = storedInventory.info.inventoryStacks[i];
            } 
        }
        RefreshUI();
    }

    public void DiscardItem(int itemIndex)
    {
        if (info.storedItems > 0)
        {
            if(info.inventoryStacks[itemIndex] > 0) info.inventoryStacks[itemIndex]--; //One less in the stack to worry about
            else
            {
                info.inventoryItems[itemIndex] = null;//Remove this item this item to our inventory slot
            info.storedItems--;
            }           
        }
        else return; //If our inventory is empty, then we won't discard any more items
    }

    public InventoryInfo getInfo(){return info;}
    public void useEquippedItem(InputAction.CallbackContext context) => equippedItem.useItem();
    

    public void RefreshUI()
    {
        //In theory, if inventoryGraphics are my UI slots holding my items, this should be enough
        for(int i = 0; i < 4; i++)
        {
            inventoryGraphics[i].Setup(i, info.inventoryItems[i], this);
        }
    }
}

//Need to use this class to serialise info and avoid annoying MonoBehaviour features
[System.Serializable]
public class InventoryInfo
{
    public LootableItem[] inventoryItems = new LootableItem[4]; //This is the array holding my current items of usage
    public int[] inventoryStacks = new int[4]; //Consumables can be stacked but not weapons - I'm not storing the durability of multiple swords
    public int storedItems = 0; //This will store the index of the last item we have - basically a counter of how many items we have in our inventory

    //A beautiful copy constructor yet again
    public void copyTo(ref InventoryInfo other){
        other.storedItems = storedItems;
        //Will need to double check if this data is copied correctly
        for(int i=0;i<4;i++){
            other.inventoryItems[i] = inventoryItems[i];
            other.inventoryStacks[i] = inventoryStacks[i]; 
        }
    }
}

