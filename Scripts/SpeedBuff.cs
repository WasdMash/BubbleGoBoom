using Unity.VisualScripting;
using UnityEngine;

public class SpeedBuff : LootableItem
{
    [SerializeField] float speedDuration = 10f;
    [SerializeField] float timer =0f;
    [Range(1f, 2f)]
    [SerializeField] float speedIncrease;
    [Range(10f, 50f)]
    [SerializeField] float maxSpeed;
    bool picked_up = false;
    [SerializeField] GameObject player;
    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")){
            picked_up = true;
            print("in");
            var player = FindObjectOfType<PlayerMovement>();
            if(player.playerSpeed < maxSpeed) player.playerSpeed *= speedIncrease;
            GetComponent<Transform>().position = new Vector3(0,1000,0);
            }
    }

    void Update(){
        if (picked_up){
            timer += Time.deltaTime;
            if (timer> speedDuration){
                player.GetComponent<PlayerMovement>().playerSpeed /= 2;
                Destroy(this.GameObject());}
        }
    }
}
