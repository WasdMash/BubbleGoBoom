using UnityEngine;

public class RoomHandler: MonoBehaviour
{
    public RoomData info;
    void Start()
    {
        info = new RoomData();
        Debug.Log($"{gameObject.name} Start() ran, doors reset to default");
        Vector3 pos = transform.position;
        float[] posArray = new float[3];
        posArray[0] = pos.x; posArray[1] = pos.y; posArray[2] = pos.z;
        info.SetPosition(posArray);
    }
    public RoomData GetRoomData(){return info;}

    public void LoadFromData(RoomData data)
    {
        Debug.Log($"{gameObject.name} LoadFromData() ran, north door = {data.hasNorthDoor}");
        // assign RoomHandler's fields from data's fields
        info.hasNorthDoor = data.hasNorthDoor;
        info.hasEastDoor = data.hasEastDoor;
        info.hasSouthDoor = data.hasSouthDoor;
        info.hasWestDoor = data.hasWestDoor;
        info.SetPosition(data.GetPosition());
        info.bossInsideDefeated = data.bossInsideDefeated;
    }
}