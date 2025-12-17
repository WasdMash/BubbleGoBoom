using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public int playerHealth;
    public int currentScore;
    public Vector2 currentPlayerLocation;
    public LootableItem[4] inventoryItems;
 
    public const string PlayerPrefsKeyName = "SavedGameState";

    //Constructor of the GameState class
        //If this object fails to serialise, it could be due to issues of having Animation and other non-pure classes
        //in LootableItem class
    public GameState(int health, int score, Vector2 location, LootableItem[] inventory)
    {
        playerHealth = health;
        currentScore = score;
        currentPlayerLocation = location;
        //Copying the values of the inventory over to the game state
        for(int i = 0; i < 4; i++)
        {
            inventoryItems[i] = inventory[i];
        }
    }
    public void SaveToPlayerPrefs()
    {
        // Convert this GameState instance to a JSON string
        string json = JsonUtility.ToJson(this);

        // Save the converted JSON into the PlayerPrefs
        PlayerPrefs.SetString(PlayerPrefsKeyName, json);
        PlayerPrefs.Save();
    }

    public static GameState CreateFromPlayerPrefs()
    {
        // If the game was never saved before, the key will not exist; in this case return null
        if(!PlayerPrefs.HasKey(PlayerPrefsKeyName))
            return null;

        // Retrieve the saved JSON string from the player prefs
        string json = PlayerPrefs.GetString(PlayerPrefsKeyName);

        // Deserialize the JSON string into a new GameState object and return it
        return JsonUtility.FromJson<GameState>(json);           
    }
}