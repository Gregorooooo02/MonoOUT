using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    private void Update() {
        transform.position = cameraTransform.position;
    }
}
