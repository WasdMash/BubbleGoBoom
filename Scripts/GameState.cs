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
    public Inventory inventory;
    //We be saving everything at once, baby!
    public List<RoomData> rooms;
    public List<EnemyHealth> enemies;
    List<ChestData> chests;
 
    public const string PlayerPrefsKeyName = "SavedGameState";

    //Constructor of the GameState class
        //If this object fails to serialise, it could be due to issues of having Animation and other non-pure classes
        //in LootableItem class
    public GameState(int health, int score, Vector2 location, Inventory inventory, List<RoomData> rooms, List<EnemyHealth> enemies, List<ChestData> chests)
    {
        playerHealth = health;
        currentScore = score;
        currentPlayerLocation = location;
        //Copying the values of the inventory over to the game state
        this.inventory = inventory;
        this.rooms = rooms;
        this.enemies = enemies;
        this.chests = chests;
        this.inventory = inventory;
    }
}