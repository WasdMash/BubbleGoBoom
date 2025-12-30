using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] float health;
    [SerializeField] protected int enemyID;
    [SerializeField] Vector3 position;
    [SerializeField] protected int scoreValue;
    SpriteRenderer spriteRenderer;
    [SerializeField] Color damageColor = Color.red;
    [SerializeField] Color noDamageColor = Color.white;

    [Header("Fields here cos im lazy to test")]
    [SerializeField] Boolean isSuicider;
    [SerializeField] GameObject suicider;
    [SerializeField] GameObject[] itemsToDrop;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioClip deathSound;
    //Need to get the spawn rates of these items
        //Current items - soap item and baby oil

        //using cumulative frequencies here
    
    //0.2f chance we don't drop anything
    [SerializeField] float[] pickUpRates = {0.96f, 1.0f};

    GameObject dropItem(){
        float dropRate = UnityEngine.Random.Range(0.0f,1.0f);
        if(dropRate <= 0.9f) return null;

        for(int i=0; i<pickUpRates.Length; i++){
            //Keep going up until we find the first probability greater than our value
            if(pickUpRates[i] >= dropRate){
                //Then this is the item which we shall return to be dropped
                return itemsToDrop[i];
            }
        }
        //Lol, you get nothing
        return null;
    }

    Coroutine changeColorCoroutine;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null){
            Debug.Log("No sprite renderer found");
            Application.Quit();
        }
        spriteRenderer.color = noDamageColor;

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if(gameManager == null){
            Debug.Log("No game manager found");
            Application.Quit();
        }

    }

    //Updating position so we can 
    void Update(){ position = transform.position;}
    public Vector3 GetPosition(){return position;}
    public int GetID(){return enemyID;}


    public void Damage(float damage){
        health -= damage;

        if(changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);

        changeColorCoroutine = StartCoroutine(ChangeColor());

        if(health <= 0) Death();
    }

    void Death(){

        gameManager.ChangeScore(scoreValue);
        //Here, we can spawn an item and decide which one that is
        GameObject itemToSpawn = dropItem();
        //Need to instantiate this item where the enemy was
        if(itemToSpawn != null) Instantiate(itemToSpawn, transform.position, transform.rotation);

        //playing the death sound
        musicPlayer.PlayOneShot(deathSound);
        
        if(isSuicider){
            int maxAmount = 8;
            for(int i = 0; i < UnityEngine.Random.Range(1, maxAmount + 1); i++){
                Instantiate(suicider, transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f), 0), transform.rotation);
            }
        }

        if(gameObject.name.Contains("KingOlaf")){
            gameManager.StartGoCoroutine();
        }

        Destroy(gameObject);
    }

    IEnumerator ChangeColor(){
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = noDamageColor;
    }
}
