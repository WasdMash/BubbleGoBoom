using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    public static readonly GameObject[] roomPrefabs; //This is the array we want to store the rooms which can be instantiated
    //These variables dictate the minimum and maximum number of rooms generated
    [Range(1,15)] [SerializeField] int minRooms;
    [Range(15, 100)] [SerializeField] int maxRooms = 15;
    [SerializeField] int currentRoomCount = 0; //Bruh, nearly forgot to track the number of rooms currently generating

    //int roomOrder should be in range 0-3
    /*
        with 0 with up entry,
        1 - right entry
        2 - down entry
        3 - left entry
     */
    public void GenerateNextRoom(int roomOrder, Vector3 doorPosition)
    {
        if (currentRoomCount >= maxRooms) return;

        // First, find all rooms that match the door requirement
            //Time complexity = O(n) where n is the number of possible rooms
        List<GameObject> validRooms = new List<GameObject>();
        foreach (GameObject roomPrefab in roomPrefabs)
        {
            RoomData roomData = roomPrefab.GetComponent<RoomData>();
            if (roomData == null) continue;
            
            bool isValid = false;
            switch (roomOrder)
            {
                case 0: isValid = roomData.hasSouthDoor; break;
                case 1: isValid = roomData.hasWestDoor; break;
                case 2: isValid = roomData.hasNorthDoor; break;
                case 3: isValid = roomData.hasEastDoor; break;
            }
            
            if (isValid) validRooms.Add(roomPrefab);
        }
        
        if (validRooms.Count == 0)
        {
            Debug.LogError($"No valid rooms found for roomOrder {roomOrder}!");
            return;
        }
        
        // Pick a random valid room
        GameObject selectedRoom = validRooms[UnityEngine.Random.Range(0, validRooms.Count)];

        if (selectedRoom != null)
        {
            // INSTANTIATE FIRST with basic position - we'll calculate exact position after
                //I've intentionally removed the Quaternion.identity rotation here as some of my room prefabs are just rotated versions of existing sprites
            GameObject newRoomInstance = Instantiate(selectedRoom, doorPosition, selectedRoom.transform.rotation);
            
            // NOW get size and name from the instantiated copy, not the prefab
            int roomSize = (int)newRoomInstance.transform.GetComponent<Renderer>().bounds.size.x / 2;
            int roomHeight = (int)newRoomInstance.transform.GetComponent<Renderer>().bounds.size.y / 2;
            int yOffset = newRoomInstance.name.Contains("L Room Variant") ? roomSize/2 : 0;
            
            // Calculate the correct position
            Vector3 spawnPosition = doorPosition;
            switch (roomOrder)
            {
                case 0: spawnPosition += new Vector3(0, roomHeight, 0);
                    break;
                case 1: spawnPosition += new Vector3(roomSize, -yOffset, 0);
                    break;
                case 2: spawnPosition += new Vector3(0, -roomHeight, 0);
                    break;
                case 3: spawnPosition += new Vector3(-roomSize, yOffset, 0);
                    break;
            }
            
            // Move the instantiated room to the correct position
            newRoomInstance.transform.position = spawnPosition;
            newRoomInstance.GetComponent<RoomData>().position = spawnPosition;
            currentRoomCount++;

            // Disable the door trigger that connects back to the current room
            string doorToDisable = "Wall" + ((roomOrder+2) % 4).ToString();
            Transform doorTransform = newRoomInstance.transform.Find(doorToDisable);
            if (doorTransform != null)
            {
                Collider2D doorCollider = doorTransform.GetComponent<Collider2D>();
                if (doorCollider != null)
                {
                    doorCollider.enabled = false;
                    Debug.Log("Deleting that pesky double-door to prevent the backtrack");
                }
                    
            }
        }
    }
}
