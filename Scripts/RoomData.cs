using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
[JsonObject(MemberSerialization.OptOut)] //Need this for Newtonsoft.JSON to acttually serialise everything

public class RoomData
{
    [Header("Door trigger booleans")]
    public bool hasNorthDoor;
    public bool hasEastDoor;
    public bool hasSouthDoor;
    public bool hasWestDoor;

    [Header("Values to be stored")]
    [JsonProperty][SerializeField] protected int roomID; //I can lazily use this to find its index in array and instantly instantiate it - O(1) instead of O(n^2)
    [JsonProperty][SerializeField] protected float[] position = new float[3]; //Might need a third dimension so that head always renders on top
    public bool bossInsideDefeated = true; //Only set to false if there actually is a boss inside
        //I'll use this boolean to determine when to lock the player inside of a room

    //Yay! - we're using copy constructors!
    public void copyTo(ref RoomData oldRoom)
    {
        oldRoom.hasNorthDoor = hasNorthDoor;
        oldRoom.hasEastDoor = hasEastDoor;
        oldRoom.hasSouthDoor = hasSouthDoor;
        oldRoom.hasWestDoor = hasWestDoor;
        oldRoom.position = position;
        oldRoom.bossInsideDefeated = bossInsideDefeated;
    }

    public int GetID() {return roomID;}

    public float[] GetPosition(){ return position;}
    public void SetPosition(float[] newPosition)
    {
        for(int i = 0; i < 3; i++) {  position[i] = newPosition[i]; }
    }
}
