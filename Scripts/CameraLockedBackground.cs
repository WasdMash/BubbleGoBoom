using UnityEngine;

public class CameraLockedBackground : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [Range(1f, 20f)]
    [SerializeField] float yOffset; 
    private float backgroundZ = 1f;

    private void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        backgroundZ = transform.position.z;
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = targetCamera.transform.position;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y - yOffset, backgroundZ);
    }
}
