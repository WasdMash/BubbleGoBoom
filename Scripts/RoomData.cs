using UnityEngine;

[System.Serializable]
public class RoomData: MonoBehaviour
{
    [Header("Door trigger booleans")]
    public bool hasNorthDoor;
    public bool hasEastDoor;
    public bool hasSouthDoor;
    public bool hasWestDoor;

    [Header("Values to be stored")]
    [SerializeField] protected int roomID; //I can lazily use this to find its index in array and instantly instantiate it - O(1) instead of O(n^2)
    [SerializeField] protected string name;
    [SerializeField] protected Vector3 position; //Might need a third dimension so that head always renders on top
    public bool bossInsideDefeated = true; //Only set to false if there actually is a boss inside
        //I'll use this boolean to determine when to lock the player inside of a room

    //Yay! - we're using copy constructors!
    public void copyTo(ref RoomData oldRoom)
    {
        oldRoom.hasNorthDoor = hasNorthDoor;
        oldRoom.hasEastDoor = hasEastDoor;
        oldRoom.hasSouthDoor = hasSouthDoor;
        oldRoom.hasWestDoor = hasWestDoor;
        oldRoom.name = name;
        oldRoom.position = position;
        oldRoom.bossInsideDefeated = bossInsideDefeated;
    }

    public int GetID() {return roomID;}

    public Vector3 GetPosition(){ return position;}
    public void SetPosition(Vector3 newPosition) => position = newPosition;
}
