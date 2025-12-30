using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

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
    HealthManager health;
    GameManager game;
    Transform playerLocation;
    Inventory playerInventory;
    int[] playerInventoryStack = new int[4];
    //I should probably save the layout of the dungeon as well
        //To keep the fear factor up, storing the position of each enemy will be necessary
            //To punish pauses, I will reset enemy health upon reloading (less stuff for me to save so yay!)
    
    List<RoomData> rooms;
    List<EnemyHealth> enemies;
    List<ChestData> chests;

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

        // Using FindObjectsSortMode.None is significantly faster in 2022.3.62f1+
            //I don't really need the found objects to be sorted - I'm looping over them all anyways
        rooms = new List<RoomData>(FindObjectsByType<RoomData>(FindObjectsSortMode.None));
        enemies = new List<EnemyHealth>(FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None)); //Assumes that all enemies have the same script attached to them
        chests = new List<ChestData>(FindObjectsOfType<ChestData>());
        //Now that I have these items in a room, I need to serialise information about them
        /*
        Namely,
            Their type (room type, enemy type)
            Location
            Referenece to their sprite (if I have an array of sprites to chooose from, then just get its index)
                Makes it much easier than trying to save multiple copies of the same image
        */

        currentGameState = new GameState(health.GetPlayerHealth(), game.GetGameScore(), playerLocation.position, playerInventory, rooms, enemies, chests);

        string json = JsonUtility.ToJson(currentGameState);
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

    public void LoadState()
    {
        //Loading our player's vital info from PlayerPrefs into our current game state
        string path = Path.Combine(Application.persistentDataPath, FILE_NAME);

        if (File.Exists(path))
        {
            //Convert this thing into a GameState and then load its variables into the game, baby
                //Don't forget to decrypt it, so we can actually understand it
            string jsonString = File.ReadAllText(path);
            GameState loadedData = JsonUtility.FromJson<GameState>(jsonString);

            //Loading in the appropiate data values
            health.SetPlayerHealth(loadedData.playerHealth);
            game.SetGameScore(loadedData.currentScore);
            playerLocation = FindObjectOfType<PlayerMovement>().gameObject.transform;
            playerInventory = FindObjectOfType<Inventory>();

            playerLocation.position = loadedData.currentPlayerLocation;
            playerInventory = new Inventory(ref loadedData.inventory);

            //Will need to instantiate the rooms and enemies in their correct positions
            foreach(RoomData room in loadedData.rooms)
            {
                //Search through possible room types to find the same room type as stored
                foreach(GameObject possibleRoom in roomPrefabs)
                {
                    if(string.Equals(room.name, possibleRoom.name))
                    {
                        //Cool, we've found the room type that we want to instantiate
                        Instantiate(possibleRoom, room.position, possibleRoom.transform.rotation);
                        //Manually assign its RoomData values from room to restore the room fully to its proper form
                        RoomData newRoom = possibleRoom.GetComponent<RoomData>();
                        newRoom = new RoomData(room); //Copying the values over so nothing is lost
                    }
                }
            }

            foreach(EnemyHealth enemy in loadedData.enemies)
            {
                //Ignore the health - I want to punish users for logging out by resetting health
                    //Can be set here though in case they really complain about this feature
                //Set the enemy type and position
                    //Might have to search through possible enemy types and instantiate an enemy with a matching name
                //Do that and we should be good to go
            }
            foreach(ChestData chest in chests)
            {
                //The usual
                    //Instantiate a chest object in its correct position
                //Check if it has been opened
                    //If it has, fetch its previous contents and re-generate those
                    //If not, ignore this step
            }

        }
    }
}