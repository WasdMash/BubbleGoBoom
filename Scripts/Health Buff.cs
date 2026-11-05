using UnityEngine;

public class HealthBuff : LootableItem
{
    [SerializeField] HealthManager healthManager;
    [SerializeField] int healAmount = 10;

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")){
            Debug.Log("Should be trying to heal now");
            healthManager = FindObjectOfType<HealthManager>().GetComponent<HealthManager>();
            healthManager.HealPlayer(healAmount);
            Destroy(gameObject);
        }
    }

    //Also forgot to override here as well
    public override void useItem()
    {
        base.useItem();
        healthManager = FindObjectOfType<HealthManager>().GetComponent<HealthManager>();
        healthManager.HealPlayer(healAmount);
        Destroy(gameObject);
    }
}
