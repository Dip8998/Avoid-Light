using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); 

    [SerializeField] private float smoothSpeed = 0.125f; 

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }
}
