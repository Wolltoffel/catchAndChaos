using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform gamePosition;

    public void GameCamera()
    {
        transform.position = gamePosition.position;
        transform.rotation = gamePosition.rotation;
    }
}
