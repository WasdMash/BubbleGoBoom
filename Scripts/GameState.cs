using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public int playerHealth;
    public int wavesSurived;
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
    [JsonConstructor]
    public GameState(int playerHealth, int currentScore, int wavesSurived, float[] currentPlayerLocation, InventoryInfo inventory, List<RoomData> rooms, List<EnemyInfo> enemies, List<ChestSaveData> chests)
    {
        this.playerHealth = playerHealth;
        this.currentScore = currentScore;
        this.wavesSurived = wavesSurived;

        if (currentPlayerLocation != null)
            for (int i = 0; i < 2 && i < currentPlayerLocation.Length; i++)
                this.currentPlayerLocation[i] = currentPlayerLocation[i];

        if (inventory != null)
            inventory.copyTo(ref this.inventory);

        this.rooms = rooms ?? new List<RoomData>();
        this.enemies = enemies ?? new List<EnemyInfo>();
        this.chests = chests ?? new List<ChestSaveData>();
    }
}