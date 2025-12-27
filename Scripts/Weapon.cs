[System.Serializable]
//Yay - using interfaces to check if two items are the same
    //Finna make my life so much easier in the long run

//If I remember correctly, as long as each interface is implemented, I can have as many as I want - yippee!
public class Weapon : LootableItem
{
    [Header("Item properties")]
    [SerializeField] protected int itemDamage;
    [SerializeField] protected int maxDurability;
    [SerializeField] protected int durability; //This durability can be altered on instances when it is used over time
    [SerializeField] protected Collider2D attackHitBox;
    
    public virtual void Attack(int damageDone)
    {
        //Make an attacking motion and show necessary particles
            //Give it a collider or raycast within attacking range
            //Do set damage if we hit anything
    }

    public virtual void WearDown(int damageTaken)
    {
        durability -= damageTaken;
        if(durability <= 0) Destroy(this.gameObject);
    }
}
