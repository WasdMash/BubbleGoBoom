using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [Header("Variables")]
    const string FILE_NAME = "savestate.json";
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

    void Start()
    {
        health = FindObjectOfType<HealthManager>();
        game = FindObjectOfType<GameManager>();
        playerLocation = FindObjectOfType<PlayerMovement>().gameObject.transform;
        playerInventory = FindObjectOfType<Inventory>();
    }

    public void SaveState()
    {
        
    }

    public void LoadState()
    {
        if (Fileasa.Exists(FILE_NAME))
        {
            //Convert this thing into a GameState and then load its variables into the game, baby
        }
    }
}