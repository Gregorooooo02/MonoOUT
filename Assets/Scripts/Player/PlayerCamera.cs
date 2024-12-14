using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] public float sensitivity = 100f;
    [SerializeField] public Transform orientation;

    private Vector2 rotation = Vector2.zero;

    private InputAction lookAction;


    private void Awake() {
        lookAction = InputSystem.actions.FindAction("Look");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        Vector2 lookDelta = lookAction.ReadValue<Vector2>() * sensitivity * Time.deltaTime;

        rotation.y += lookDelta.x;
        rotation.x -= lookDelta.y;
        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
        orientation.localRotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
}
