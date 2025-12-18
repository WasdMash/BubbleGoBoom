using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Variables to manage")]
    [SerializeField] static LootableItem[] possibleItemsToDrop;
    [SerializeField] int gameScore = 1;
    [SerializeField] int waves = 0;

    [Header("Managed scripts")]
    [SerializeField] AudioManager audioManager;
    [SerializeField] AudioClip[] songs;
    [SerializeField] HealthManager health;

    [Header("Dynamic wave spawning around the player")]
    [SerializeField]    Transform player;

    [SerializeField] int minDist;
    [SerializeField] int maxDist;
    [SerializeField] BoxCollider2D spawnBoundBox;

    [Header("Enemy prefabs")]
    [SerializeField] GameObject Suicider;
    [SerializeField] GameObject RegularGoon;
    [SerializeField] GameObject Shooter;
    [SerializeField] GameObject Mitosis;
    [SerializeField] GameObject KingOlay;

    [Header("General UI")]
    [SerializeField] TMP_Text scoreText, wavesSurived;


    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        health = GetComponent<HealthManager>();
        waves = 0;
        StartCoroutine(SpawnWave());
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.T)) SceneManager.LoadScene(0);
    }

    public IEnumerator SpawnWave(){
        waves++;
        if(waves % 24 == 0) {
            BossWave();
        }else{
        int noOfEnemies = Mathf.RoundToInt(4 * Mathf.Log10(waves + 8));
        SpawnEmenies(noOfEnemies, waves);
        yield return new WaitForSeconds(10f);
        StartCoroutine(SpawnWave());
        }
    }

    void BossWave(){SpawnEmenies(1, waves, true);}

    void SpawnEmenies(int number, int waves, bool isBoss = false){
        if(isBoss == false){
        int spawned = 0;
        int attempts = 0;

        while (spawned < number){
            attempts++;

            GameObject enemy;
            int minChance = 0;
            int maxChance = 100;

            int randomNum = UnityEngine.Random.Range(minChance, maxChance);

            if(randomNum < 50){
                enemy = RegularGoon;
            }else if(50<= randomNum && randomNum < 75){
                enemy = Shooter;
            } else {
                enemy = Mitosis;
            }


            float angle = UnityEngine.Random.Range(0f, 2f * (float)Math.PI );
            float distance = UnityEngine.Random.Range(minDist, maxDist);

            Vector2 randomPosition = new Vector2(player.position.x + Mathf.Cos(angle) * distance, player.position.y + Mathf.Sin(angle) * distance);

            if (spawnBoundBox.OverlapPoint(randomPosition)){

                Instantiate(enemy, randomPosition, Quaternion.identity);
                spawned++;
            }
            if(attempts > 1000) break;

            }
        } else if(isBoss == true){
            Debug.Log("Spawning boss");
            int attempts = 0;
            Vector2 randomPosition = Vector2.zero;
            bool validSpawn = false;
            while(!validSpawn && attempts < 1000){
                attempts++;
                randomPosition = new Vector2(player.position.x + UnityEngine.Random.Range(minDist, maxDist), player.position.y + UnityEngine.Random.Range(minDist, maxDist));
                if(spawnBoundBox.OverlapPoint(randomPosition)){
                    validSpawn = true;
                }
            }
            if(!validSpawn){
                Debug.LogError("Couldn't find a valid position to spawn the boss in the allotted number of attempts");
            } else {
                //When we have more than one boss, insert boss prefab here
                Instantiate(KingOlay, randomPosition, Quaternion.identity);
            }
        }
    }

    void LateUpdate(){
        scoreText.text = "Score: " + gameScore.ToString();
        wavesSurived.text = "Waves survived: " + waves.ToString();
    }

    public void StartGoCoroutine(){ StartCoroutine(SpawnWave()); }

    public void EndGame(){
        // Debug.Log("You lost the game bro, fr");
        scoreText.text = "Final score: " + gameScore.ToString();
        //I want the user to watch as I completely empty their inventory upon death
            //MAKE THEM SUFFER
        Invoke("ResetGame", 5f); //reset the game after 5 second if there isn't any user input, i guess
    }

    public void ResetGame(){
        gameScore = 0;
        health.ResetGame(); //Resetting the health variables
        //Destroying all livin enemies
        var enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        foreach(var enemy in enemies){
            Destroy(enemy.gameObject);
        }
        StopAllCoroutines();
        waves = 0;
        StartGoCoroutine();
        
    }

    public void ChangeScore(int amount){ gameScore += amount; }

    public int GetGameScore() {return gameScore;}
    public void SetGameScore(int newScore) {gameScore = newScore;}

    public LootableItem[] GetPossibleItems() {return possibleItemsToDrop;}
}