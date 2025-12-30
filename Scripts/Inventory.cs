using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : MonoBehaviour, IItemContainer
{
    [Header("Items")]
    public LootableItem[] inventoryItems = new LootableItem[4]; //This is the array holding my current items of usage
    public int[] inventoryStacks = new int[4]; //Consumables can be stacked but not weapons - I'm not storing the durability of multiple swords
    public LootableItem equippedItem; //We probably only want our user to have one item at a time to add to the chaos
    [SerializeField] int storedItems; //This will store the index of the last item we have - basically a counter of how many items we have in our inventory

    [Header("Graphics")]
    public InventorySlot[] inventoryGraphics = new InventorySlot[4];//default placeholder for now
    public Sprite nullSprite; //This is the placeholder sprite for each inventory slot, just to prevent errors
    //I want a visual Minecraft-like box to show current inventory

    public void Awake()
    {
        inventoryGraphics = GetComponentsInChildren<InventorySlot>();
        RefreshUI();
    }

    //A beautiful copy constructor yet again
    public Inventory(ref Inventory other){
        storedItems = other.storedItems;
        //Will need to double check if this data is copied correctly
        equippedItem = other.equippedItem;
        Debug.Log(equippedItem.getName() + " and " + other.equippedItem.getName() + " should be the same");
        for(int i=0;i<4;i++){
            inventoryItems[i] = other.inventoryItems[i];
            inventoryStacks[i] = other.inventoryStacks[i];
            inventoryGraphics[i] = other.inventoryGraphics[i];     
        }
    }
    public LootableItem GetItem(int index) => inventoryItems[index];

    public void PickUpItem(LootableItem item)
    {
        //First check if the item already exists - if it does, stack it
        for(int i = 0; i < storedItems; i++)
        {
            if(inventoryItems[i].CompareTo(item) != 0)
            {
                //Should probably check if the lootableItem is a weapon
                //We should break the for loop if it is
                if(inventoryItems[i] is not Weapon) inventoryStacks[i]++; //Only if pickUpItem is not a weapon
                return; //Our job is done - we don't need 2 slots taken up by the same item
            }
        }
        if (storedItems < 3)
        {
            SetItem(storedItems, item); //Assign the item to the last available spot
            //Automatically set the new item to be our equipped item
            if(storedItems == 0) equippedItem = inventoryItems[storedItems++];
        }
        else return; //If our inventory is full, then we won't equip any more items
    }
    public void SetItem(int index, LootableItem item)
    {
        inventoryItems[index] = item;//Add this item to our inventory slot
        RefreshUI();
    }

    //Should perhaps trigger the selected function of UI to visually show equipped item
    public void EquipItem(int itemIndex){ equippedItem = inventoryItems[itemIndex];}

    //I don't really know what use cases they may be for this function as opposed to the copy constructor, but I'll keep it just in case
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
        RefreshUI();
    }

    public void DiscardItem(int itemIndex)
    {
        if (storedItems > 0)
        {
            if(inventoryStacks[itemIndex] > 0) inventoryStacks[itemIndex]--; //One less in the stack to worry about
            else
            {
                inventoryItems[itemIndex] = null;//Remove this item this item to our inventory slot
            storedItems--;
            }           
        }
        else return; //If our inventory is empty, then we won't discard any more items
    }

    public void Update()
    {
        //We probably want to assign a button to equip a main item to be used
        //This item's effect will be tied to a button on the keyboard
        if (Input.GetKeyDown(KeyCode.Q)) equippedItem.useItem();
    }

    public void RefreshUI()
    {
        //In theory, if inventoryGraphics are my UI slots holding my items, this should be enough
        for(int i = 0; i < 4; i++)
        {
            inventoryGraphics[i].Setup(i, inventoryItems[i], this);
        }
    }
}

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

