using UnityEngine;

public class RoomHandler: MonoBehaviour
{
    public RoomData info;
    void Start()
    {
        info = new RoomData();
        Vector3 pos = transform.position;
        float[] posArray = new float[3];
        posArray[0] = pos.x; posArray[1] = pos.y; posArray[2] = pos.z;
        info.SetPosition(posArray);
    }
    public RoomData GetRoomData(){return info;}
}