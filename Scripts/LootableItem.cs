using UnityEngine;
using System;

[System.Serializable]
//Yay - using interfaces to check if two items are the same
    //Finna make my life so much easier in the long run

//If I remember correctly, as long as each interface is implemented, I can have as many as I want - yippee!
[CreateAssetMenu(fileName = "LootableItem", menuName = "Lootables/LootableItem")]
public class LootableItem : ScriptableObject, IComparable<LootableItem>
{
    [Header("Item properties")]
    [SerializeField] protected string itemName;

    [Header("Visible things")]
    [SerializeField] protected Animation anim;
    [SerializeField] protected Sprite itemSprite;
    [SerializeField] protected ParticleSystem particles; //I assume that this is a particle effect to be displayed when this item's trigger has been entered
    //Maybe store the name of the attribute which this item affects
        //Also have an integer value of how much it changes the attribute by?
    
    // Implement the IComparable<T> interface
    public int CompareTo(LootableItem other)
    {
        if (other == null) return 1;

        // Sort primarily by name (ascending)
        // CompareTo returns <0 if the current instance is less than the other object
        return this.itemName.CompareTo(other.getName());
    }

    public Sprite getSprite() {return itemSprite;}
    public string getName() {return itemName;}

    public virtual void useItem()
    {
        Debug.Log(itemName + " has just been used");
    }

    //The stuff which should happen when this particle gets picked up
    public void pickedUpParticles(){ particles.Play();}

    //Probably should try to add an OnTriggerEnter2D() function to trigger the pickup
        //This would also be common between all items who inherit this interface
}
