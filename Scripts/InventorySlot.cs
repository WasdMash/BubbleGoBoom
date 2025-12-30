using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex;
    public Image itemIcon;
    public IItemContainer container; // The Chest or Player inventory this slot belongs to

    // References to the "Ghost" image that follows the mouse
    public static GameObject dragIcon; 
    //Bruh, I need to access the UI canva which is displaying this image

    //By default, this changes the background image instead of the foreground one
        //This is because of how Unity layers UI with a bottom-up approach - can work around this I think
    public void Awake() => itemIcon = transform.GetChild(0).GetComponent<Image>() as Image;

    //Lowkey got no idea what this function here is doing
    public void Setup(int index, LootableItem item, IItemContainer invent)
    {
        //I should probably also check for text on this slot and assign the stack number to this slot
        slotIndex = index;
        container = invent;
        if (item != null)
        {
            itemIcon.sprite = item.getSprite(); // Assumes LootableItem has a sprite field
            itemIcon.enabled = true;
        }
        else itemIcon.enabled = false;
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (container.GetItem(slotIndex) == null) return;

        // Create a temporary icon to follow the mouse
        dragIcon = new GameObject("Icon");
        dragIcon.transform.SetParent(GetComponentInParent<Canvas>().transform);
        dragIcon.AddComponent<Image>().sprite = itemIcon.sprite;
        dragIcon.GetComponent<Image>().raycastTarget = false; // Important!
        dragIcon.transform.SetParent(GameObject.Find("MainCanvas").transform);
        dragIcon.transform.SetAsLastSibling(); // Puts it on top of everything
        
        itemIcon.color = new Color(1, 1, 1, 0.5f); // Fade the original
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null) dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(dragIcon);
        itemIcon.color = Color.white;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Get the slot we started dragging from
        InventorySlot sourceSlot = eventData.pointerDrag.GetComponent<InventorySlot>();

        if (sourceSlot != null)
        {
            // 1. Get the items involved
            LootableItem itemMoving = sourceSlot.container.GetItem(sourceSlot.slotIndex);
            LootableItem itemAlreadyHere = this.container.GetItem(this.slotIndex);

            // 2. Swap them across containers
            this.container.SetItem(this.slotIndex, itemMoving);
            sourceSlot.container.SetItem(sourceSlot.slotIndex, itemAlreadyHere);

            // 3. Refresh both UI panels
        }
    }
}

//Interface that we can use for player inventory, chests and even shops - just anything which stores items really
public interface IItemContainer
{
    LootableItem GetItem(int index);
    void SetItem(int index, LootableItem item);
    void DiscardItem(int index);
    void RefreshUI(); //Should basically loop through and Setup() on each inventory slot
}