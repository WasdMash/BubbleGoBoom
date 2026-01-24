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
    public List<ChestSaveData> chests;
 
    //Constructor of the GameState class
        //If this object fails to serialise, it could be due to issues of having Animation and other non-pure classes
        //in LootableItem class
    public GameState(int health, int score, float[] location, InventoryInfo inventory, List<RoomData> rooms, List<EnemyInfo> enemies, List<ChestSaveData> chests)
    {
        playerHealth = health;
        currentScore = score;
        for(int i=0;i<2;i++){ currentPlayerLocation[i] = location[i];}
        //Copying the values of the inventory over to the game state
        inventory.copyTo(ref this.inventory);
        //Newtonsoft.JSON throws a hissy fit if any of these are empty, due to them not needing to be saved
        this.rooms = rooms ?? new List<RoomData>();
        this.enemies = enemies ?? new List<EnemyInfo>();
        this.chests = chests ?? new List<ChestSaveData>();
    }
}