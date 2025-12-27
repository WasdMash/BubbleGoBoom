[System.Serializable]
public class RoomData
{
    [Header("Door trigger booleans")]
    public bool hasNorthDoor;
    public bool hasEastDoor;
    public bool hasSouthDoor;
    public bool hasWestDoor;

    [Header("Values to be stored")]
    public string name;
    public Vector3 position; //Might need a third dimension so that head always renders on top
    public bool bossInsideDefeated = true; //Only set to false if there actually is a boss inside
        //I'll use this boolean to determine when to lock the player inside of a room

    public void CopyData(ref RoomData oldRoom)
    {
        this.hasNorthDoor = oldRoom.hasNorthDoor;
        this.hasEastDoor = oldRoom.hasEastDoor;
        this.hasSouthDoor = oldRoom.hasSouthDoor;
        this.hasWestDoor = oldRoom.hasWestDoor;
        this.name = oldRoom.name;
        this.position = oldRoom.position;
        this.bossInsideDefeated = oldRoom.bossInsideDefeated;
    }
}
