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
    public float[] currentPlayerLocation = new float[2];
    public InventoryInfo inventory = new InventoryInfo();
    //We be saving everything at once, baby!
    public List<RoomData> rooms;
    public List<EnemyInfo> enemies;
    List<ChestData> chests;
 
    //Constructor of the GameState class
        //If this object fails to serialise, it could be due to issues of having Animation and other non-pure classes
        //in LootableItem class
    public GameState(int health, int score, float[] location, InventoryInfo inventory, List<RoomData> rooms, List<EnemyInfo> enemies, List<ChestData> chests)
    {
        playerHealth = health;
        currentScore = score;
        for(int i=0;i<2;i++){ currentPlayerLocation[i] = location[i];}
        //Copying the values of the inventory over to the game state
        inventory.copyTo(ref this.inventory);
        this.rooms = rooms;
        this.enemies = enemies;
        this.chests = chests;
    }
}