using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using System.Linq;

[JsonObject(MemberSerialization.OptOut)] //Need this for Newtonsoft.JSON to acttually serialise everything

public class ChestData: MonoBehaviour, IItemContainer
{
    [JsonProperty][SerializeField] protected int chestID; //Finding this type of object should be easier now
    [SerializeField] protected Sprite closedSprite, openSprite; //Need a sprite to represent it both closed and open
    [SerializeField] protected GameObject chestInventoryUI;
    [JsonProperty][SerializeField] protected bool opened = false; //We need to keep tracked of this so that we only save the inventories of opened chests
    //Each chest in theory stores a 4x4 grid of items
    [JsonProperty][SerializeField] protected LootableItem[] inventoryItems = new LootableItem[16];
    [JsonProperty][SerializeField] protected int[] inventoryStacks = new int[16]; //Handles the stacks of each item in a chest
    [JsonProperty][SerializeField] protected InventorySlot[] inventoryGraphics = new InventorySlot[16];//default placeholder for now
    
    [SerializeField] protected InputActionReference chestInteraction;
    [SerializeField] protected Vector3 position;
    bool isPlayerInRange = false;

    public ChestSaveData GetSaveData()
    {
        return new ChestSaveData
        {
            chestID = this.chestID,
            opened = this.opened,
            itemIds = this.inventoryItems.Select(i => i != null ? i.getId() : null).ToArray(),
            stacks = this.inventoryStacks,
            position = transform.position
        };
    }

    public void LoadFromData(ChestSaveData data)
    {
        chestID = data.chestID;
        opened = data.opened;
        inventoryItems = data.itemIds.Select(id => string.IsNullOrEmpty(id) ? null : LootableItemDatabase.Instance.GetById(id)).ToArray();
        inventoryStacks = data.stacks;
        if (opened) GetComponent<SpriteRenderer>().sprite = openSprite;
    }


    public void Awake()
    {
        //Cannot find inactive objects - just have to hope that we can use something to find it when it needs to be found
        position = gameObject.transform.position;
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
        if(item != null)
        {
            //The order of these checks is important - will get errors if we first compare a null value in .CompareTo()
            if(inventoryItems[index] == null) inventoryItems[index] = item;//Add this item to our empty inventory slot
            else if(inventoryItems[index].CompareTo(item) != 0)
            {
                //First check if a copy of the item exists - if so, stack it
                if(inventoryItems[index] is not Weapon) inventoryStacks[index]++; //Only if pickUpItem is not a weapon
            }
        } 
        else inventoryItems[index] = null;
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
    public void closeChest() {
        GetComponent<SpriteRenderer>().sprite = closedSprite;
        //Should turn off the chest UI now
        chestInventoryUI.SetActive(false);
    }
    public void openChest() {
        //Should probably check if this is the first time that the chest is opened
            //Did we just change the value of opened? If so, now generate its contents
        if(!opened) generateContents();
        //Should turn on the Chest UI plane now
        chestInventoryUI.SetActive(true);
        inventoryGraphics = chestInventoryUI.GetComponentsInChildren<InventorySlot>();
        GetComponent<SpriteRenderer>().sprite = openSprite;
        RefreshUI();
        opened = true;
    }

    void Update()
    {
        // Check if the player is nearby AND if they just pressed the button
        if (isPlayerInRange && chestInteraction.action.triggered)
        {
            Debug.Log("Interacting with chest!");
            if (chestInventoryUI.activeSelf) closeChest();
            else openChest();
        }
    }

    void OnTriggerEnter2D(Collider2D col) {if (col.CompareTag("Player")) isPlayerInRange = true;}
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // Optional: Close the chest UI if the player walks away
            if(chestInventoryUI.activeSelf) closeChest();
        }
    }

    //Set of necessary getters and setters
    public int GetID() {return chestID;}
    public bool isOpened() {return opened;}
    public Vector3 GetPosition() {return position;}
}

[System.Serializable]
[JsonObject(MemberSerialization.OptOut)] //Need this for Newtonsoft.JSON to acttually serialise everything
public class ChestSaveData
{
    public int chestID;
    public bool opened;
    public string[] itemIds;
    public int[] stacks;
    public Vector3 position;
}
