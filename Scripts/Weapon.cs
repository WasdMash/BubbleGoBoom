using UnityEngine;
[System.Serializable]

//If I remember correctly, as long as each interface is implemented, I can have as many as I want - yippee!
public class Weapon : LootableItem
{
    [Header("Item properties")]
    [SerializeField] protected int itemDamage;
    [SerializeField] protected int maxDurability;
    [SerializeField] protected int durability; //This durability can be altered on instances when it is used over time
    protected float nextAttackTime;
    public override void useItem()
    {
        Debug.Log(itemName + " is doing damage, baby!");
    } 
    //Could be an issue if loading a half-used weapon after a save
    public void Awake() => durability = maxDurability; //Gotta set durability to max when starting the game for the first time
    //Bruh, just saw a Code Monkey article adviising me not to use Scriptable Objects to store changing variables
    public void BuffDamage(int damage) => itemDamage += damage;
    public virtual void WearDown(int damageTaken)
    {
        durability -= damageTaken;
        Debug.Log($"{name} has broken.");
        if (durability <= 0) Destroy(gameObject); //I want this object to destroy itself when durability is all gone
    }
    public void SetDurability(int value) => durability = Mathf.Clamp(value, 0, maxDurability);
}
