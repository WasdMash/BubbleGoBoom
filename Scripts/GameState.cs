using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public int playerHealth;
    public int currentScore;
    //It has been advised to save Vector2s as a float[2] for easier serialisation
        //I'll do that iff I'm having any issues
    public Vector2 currentPlayerLocation;
    public LootableItem[4] inventoryItems;
    //We be saving everything at once, baby!
    public List<RoomData> rooms;
    public List<EnemyHealth> enemies;
    List<ChestData> chests;
 
    public const string PlayerPrefsKeyName = "SavedGameState";

    //Constructor of the GameState class
        //If this object fails to serialise, it could be due to issues of having Animation and other non-pure classes
        //in LootableItem class
    public GameState(int health, int score, Vector2 location, LootableItem[] inventory, List<RoomData> rooms, List<EnemyHealth> enemies, List<ChestData> chests)
    {
        playerHealth = health;
        currentScore = score;
        currentPlayerLocation = location;
        //Copying the values of the inventory over to the game state
        for(int i = 0; i < 4; i++)
        {
            inventoryItems[i] = inventory[i];
        }
        this.rooms = rooms;
        this.enemies = enemies;
        this.chests = chests;
    }
}