using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Inventory : MonoBehaviour, IItemContainer
{
    [Header("Items")]
    [SerializeField]
    protected InventoryInfo info;
    public LootableItem equippedItem; //We probably only want our user to have one item at a time to add to the chaos
    //It should probably be made a child of the player
    private int lastUsedItem = 0; //Kinda need this for equipItem in InputManager to have no arguments

    [Header("Graphics")]
    [SerializeField] InventorySlot equippedItemSprite; //So we can visually show what the equipped item is at any given time
    [SerializeField] InventorySlot[] inventoryGraphics = new InventorySlot[4];//default placeholder for now
    [SerializeField] Sprite nullSprite; //This is the placeholder sprite for each inventory slot, just to prevent errors
    //I want a visual Minecraft-like box to show current inventory
    Vector3 originalPos;
    [Range(0f, 200f)]
    [SerializeField] float downShift; //value which detemrines how far out of view the inventory slides out of the 
    [Range(0f, 1f)]
    [SerializeField] float downShiftTime; //how long it takes for the UI to move out of the way
    [Range(50f,500f)]
    [SerializeField] float mouseThreshold = 10f;
    private bool isHidden;
    private float cameraCentre;

    public void Awake()
    {
        originalPos = transform.position;
        inventoryGraphics = GetComponentsInChildren<InventorySlot>();
        RefreshUI();
        cameraCentre = Screen.height / 2f;
    }

    //Quick fix to get the UI out ot the way when aiming downwards, especially on controller
    void Update()
    {
        //Can be annoying, especially when we want to be dragging something down into inventory
            //Perhaps add a key to hold while looking down to activate this drag
        if(Input.GetKey(KeyCode.LeftShift)) return; //LeftShift for mouse+keyboard should do the trick
        float look = InputManager.Instance.LookInput.y;
        bool likelyMouse = Mathf.Abs(look) > 1.5f;
        float magnitude = likelyMouse ? Mathf.Abs(look - cameraCentre) : Mathf.Abs(look);
        float threshold = likelyMouse ? mouseThreshold : 0.9f;
        bool shouldHide = magnitude > threshold;

        if (shouldHide == isHidden) return;
        //Should do a check for mouse to return if difference from centre also meets a threshold

        isHidden = shouldHide;
        transform.DOKill();

        Vector3 target = isHidden ? originalPos - new Vector3(0f, downShift, 0f) : originalPos;
        transform.DOMove(target, downShiftTime);
        
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
            lastUsedItem = index; //Cool, the latest thing to be set to be equipped
        } 
        else info.inventoryItems[index] = null;
        RefreshUI();
    }

    //Should perhaps trigger the selected function of UI to visually show equipped item
    public void EquipItem(InputAction.CallbackContext context) => EquipItem();
    public void EquipItem(){ 
        equippedItem = info.inventoryItems[lastUsedItem]; //By default, just assumes that you want to equip the last moved item.
        equippedItemSprite.Setup(0, equippedItem, this); //This could be more cumbersome, but hey, let's find out,
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

