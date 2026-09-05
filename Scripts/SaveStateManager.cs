using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

public class SaveStateManager : MonoBehaviour
{
    [Header("Variables")]
    //Should be able to alter the name slightly to store data for multiple savestates at some point
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
    [SerializeField] GameObject[] roomPrefabs;
    [SerializeField] GameObject[] enemyTypes;
    [SerializeField] GameObject[] chestTypes;
    HealthManager health;
    GameManager game;
    private JsonSerializerSettings settings;
    float[] playerLocation = new float[2];
    InventoryInfo playerInventory;
    int[] playerInventoryStack = new int[4];
    //I should probably save the layout of the dungeon as well
        //To keep the fear factor up, storing the position of each enemy will be necessary
            //To punish pauses, I will reset enemy health upon reloading (less stuff for me to save so yay!)
    
    List<RoomHandler> rooms;
    List<EnemyHealth> enemies;
    List<ChestData> chests;

    void Start()
    {
        health = FindObjectOfType<HealthManager>();
        game = FindObjectOfType<GameManager>();       
    }

    public void SaveState()
    {
        Debug.Log("It's time to save data, baby!");
        //We only care about tracking these values when it is actually time to save
            //Anything else will just itnroduce meaningless lag and bottleneck performance
        Vector3 playerPos = FindObjectOfType<PlayerMovement>().gameObject.transform.position;
        playerInventory = FindObjectOfType<Inventory>().getInfo();

        // Using FindObjectsSortMode.None is significantly faster in 2022.3.62f1+
            //I don't really need the found objects to be sorted - I'm looping over them all anyways
        rooms = new List<RoomHandler>(FindObjectsByType<RoomHandler>(FindObjectsSortMode.None));
        enemies = new List<EnemyHealth>(FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None)); //Assumes that all enemies have the same script attached to them
        chests = new List<ChestData>(FindObjectsByType<ChestData>(FindObjectsSortMode.None));
        //Now that I have these items in a room, I need to serialise information about them

        playerLocation[0] = playerPos.x;
        playerLocation[1] = playerPos.y;

        List<EnemyInfo> enemyInfos = new List<EnemyInfo>();
        foreach(EnemyHealth e in enemies) { enemyInfos.Add(e.GetEnemyInfo()); }
        List<RoomData> roomInfos = new List<RoomData>();
        foreach(RoomHandler r in rooms) {roomInfos.Add(r.GetRoomData());}
        List<ChestSaveData> chestInfos = new List<ChestSaveData>();
        foreach(ChestData c in chests) { chestInfos.Add(c.GetSaveData()); }

        currentGameState = new GameState(health.GetPlayerHealth(), game.GetGameScore(), game.GetWavesSurvived(), playerLocation, playerInventory, roomInfos, enemyInfos, chestInfos);

        //string json = JsonUtility.ToJson(currentGameState); //This is the JSONUtility version - good for non-nested objects and non-gameObjects
        settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, ContractResolver = new NonPublicResolver()}; //Avoids normlisation reference issues - using nonPublicResolver to store protected variables too
        string json = JsonConvert.SerializeObject(currentGameState, Formatting.Indented, settings);
        //Will need to adjust file nme in case I want a save file for each user of the game/ each save state
        string path = Path.Combine(Application.persistentDataPath, FILE_NAME);
        // Optional but recommended: ensure the folder exists
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(path, json);

    }
    public void SaveState(InputAction.CallbackContext context) => SaveState();
    public void LoadState()
    {
        Debug.Log("It's time to load data, baby!");
        //Loading our player's vital info from PlayerPrefs into our current game state
        string path = Path.Combine(Application.persistentDataPath, FILE_NAME);

        if (File.Exists(path))
        {
            //Convert this thing into a GameState and then load its variables into the game, baby
                //Don't forget to decrypt it, so we can actually understand it
            string jsonString = File.ReadAllText(path);
            //This can cause issues if there are no chests yet loaded into the scene
            GameState loadedData = JsonConvert.DeserializeObject<GameState>(jsonString, settings);

            foreach (var room in FindObjectsByType<RoomHandler>(FindObjectsSortMode.None)) Destroy(room.gameObject);
            foreach (var enemy in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None)) Destroy(enemy.gameObject);
            foreach (var chest in FindObjectsByType<ChestData>(FindObjectsSortMode.None)) Destroy(chest.gameObject);

            //Loading in the appropiate data values
            health.SetPlayerHealth(loadedData.playerHealth);
            game.SetGameScore(loadedData.currentScore);
            game.SetWavesSurvived(loadedData.wavesSurived);
            for(int i=0;i<2;i++) {playerLocation[i] = loadedData.currentPlayerLocation[i];}

            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            player.gameObject.transform.position = new Vector3(playerLocation[0], playerLocation[1], player.gameObject.transform.position.z);
            
            //Must work on syncing info between the active Inventory object and its InventoryInfo object
            playerInventory = FindObjectOfType<Inventory>().getInfo();
            loadedData.inventory.copyTo(ref playerInventory);

            Vector3 pos = new Vector3();
            //Will need to instantiate the rooms and enemies in their correct positions
            foreach (RoomData room in loadedData.rooms)
            {
                //Cool, we've found the room type that we want to instantiate
                GameObject currentRoom = roomPrefabs[room.GetID()];
                float[] comPos = room.GetPosition();
                for (int i = 0; i < 3; i++) { pos[i] = comPos[i]; }
                //Manually assign its RoomData values from room to restore the room fully to its proper form
                GameObject instantiated = Instantiate(currentRoom, pos, currentRoom.transform.rotation);
                RoomHandler handler = instantiated.GetComponent<RoomHandler>();
                handler.LoadFromData(room);//Copying the values over so nothing is lost
            }

            //Ignore the health - I want to punish users for logging out by resetting health
                    //Can be set here though in case they really complain about this feature
            foreach(EnemyInfo enemy in loadedData.enemies)
            {
                float[] comPos = enemy.GetPosition();      
                for(int i=0;i<3;i++) {pos[i] = comPos[i];}
                Instantiate(enemyTypes[enemy.GetID()], pos, Quaternion.identity);
            } 
            
            foreach(ChestSaveData chest in loadedData.chests)
            {
                //Instantiate a chest object in its correct position
                GameObject currentChest = chestTypes[chest.chestID];
                GameObject instantiatedChest = Instantiate(currentChest, chest.position, Quaternion.identity); // this captures the instance instead of the immutable prefab
                instantiatedChest.GetComponent<ChestData>().LoadFromData(chest); // bruh, I was using currentChest (the prefab) before
            }

        }
    }
    public void LoadState(InputAction.CallbackContext context) => LoadState();
}

public class NonPublicResolver : DefaultContractResolver {
    protected override List<MemberInfo> GetSerializableMembers(Type objectType) {
        // Includes public, private, and protected instance fields
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        return objectType.GetFields(flags).Cast<MemberInfo>().ToList();
    }
}