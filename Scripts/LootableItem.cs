[System.Serializable]
//Yay - using interfaces to check if two items are the same
    //Finna make my life so much easier in the long run

//If I remember correctly, as long as each interface is implemented, I can have as many as I want - yippee!
public class LootableItem : IComparable<LootableItem>
{
    [Header("Item properties")]
    public string ItemID;
    public string itemName;
    public int itemDamage;
    public int maxDurability;
    public int durability; //This durability can be altered on instances when it is used over time

    [Header("Visible things")]
    public Animation anim;
    public Sprite itemSprite;
    public ParticleSystem particles;
    
    // Implement the IComparable<T> interface
    public int CompareTo(LootableItem other)
    {
        if (other == null) return 1;

        // Sort primarily by value (descending), then by name (ascending)
        // CompareTo returns <0 if the current instance is less than the other object
        int valueComparison = other.value.CompareTo(this.maxDurability); // Invert for descending
        if (valueComparison != 0)
        {
            return valueComparison;
        }

        //If the maxDurability is the same, then surely these should be similar objects, if not the same
        return this.itemName.CompareTo(other.itemName);
    }
}
