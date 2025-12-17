using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [Header("Variables")]
    const string FILE_NAME = "savestate.json";
    GameState currentGameState;
        //Depending on how I implement things, this JSON file may need sections
        /*
        1) PLayer values
            E.g health, score, position and inventory
        2) Enemy positions
        3) Dungeon layout
            Position of each room
            Which rooms have been cleared by the player
        4) Chests
            Locations
            Contents - only store the contents of opened chests
                We can generate the contents of unopened chests at the time of opening
        */

    [Header("Data sources")]
    HealthManager health;
    GameManager game;
    Transform2D playerLocation;
    Inventory playerInventory;
    //I should probably save the layout of the dungeon as well
        //To keep the fear factor up, storing the position of each enemy will be necessary
            //To punish pauses, I will reset enemy health upon reloading (less stuff for me to save so yay!)
    
    List<GameObject> rooms;
    List<EnemyMovement> enemies;

    void Start()
    {
        health = FindObjectOfType<HealthManager>();
        game = FindObjectOfType<GameManager>();
        
    }

    public void SaveState()
    {
        //We only care about tracking these values when it is actually time to save
            //Anything else will just itnroduce meaningless lag and bottleneck performance
        playerLocation = FindObjectOfType<PlayerMovement>().gameObject.transform;
        playerInventory = FindObjectOfType<Inventory>();

        rooms = FindObjectsOfTag("Room"); //Double-check that I've correctly spelt the tags for these
        enemies = FindObjectsOfType<EnemyMovement>(); //Assmes that all enemies have the same script attached to them
        //Now that I have these items in a room, I need to serialise information about them
        /*
        Namely,
            Their type (room type, enemy type)
            Location
            Referenece to their sprite (if I have an array of sprites to chooose from, then just get its index)
                Makes it much easier than trying to save multiple copies of the same image
        */

        currentGameState = new GameState(health.GetPlayerHealth(), game.GetGameScore(), playerLocation.position, playerInventory);
        //Now, time to save this in PlayerPrefs, because, why not? Life can be easy sometimes
        currentGameState.SaveToPlayerPrefs(); //This should save our player info in PlayerPrefs - most important to hide

        //Now, it's time to save everything else
    }

    public void LoadState()
    {
        //Loading our player's vital info from PlayerPrefs into our current game state
        currentGameState = GameState.CreateFromPlayerPrefs();
        //Appropiate assigning these values to the player
        health.SetPlayerHealth(currentGameState.playerHealth);
        game.SetGameScore(currentGameState.currentScore);
        playerLocation = FindObjectOfType<PlayerMovement>().gameObject.transform;
        playerInventory = FindObjectOfType<Inventory>();

        playerLocation.position = currentGameState.currentPlayerLocation;
        playerInventory.ReloadInventory(currentGameState.inventoryItems);

        if (File.Exists(FILE_NAME))
        {
            //Convert this thing into a GameState and then load its variables into the game, baby
        }
    }
}