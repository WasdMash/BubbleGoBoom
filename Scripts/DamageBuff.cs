using UnityEngine;

public class DamageBuff : LootableItem
{
    [SerializeField] float duration = 10f;
    [SerializeField] float timer = 0f;
    [SerializeField] Weapon equippedItem;
    [Range(10, 50)]
    [SerializeField] int damageBuff;
    private bool picked_up = false;

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")){
            //equippedItem by default will be a LootableItem, so will need to convert it to a weapon
            equippedItem = (Weapon)FindObjectOfType<Inventory>().equippedItem;
            equippedItem.BuffDamage(damageBuff);
            picked_up = true;
        }
    }

    void Update(){
        if (picked_up){
            timer += Time.deltaTime;
            if (timer> duration){
                equippedItem.BuffDamage(-damageBuff);
                Destroy(this.gameObject);}
        }
    }
}