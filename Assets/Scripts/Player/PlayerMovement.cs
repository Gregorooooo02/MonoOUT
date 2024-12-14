using System.Runtime.CompilerServices;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool CanMove {get; set;} = true;

    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float groundDrag = 6f;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float airDrag = 2.5f;
    private bool isReadyToJump;

    [Header("Ground Check Parameters")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    [SerializeField] private Transform orientation;

    private InputAction movementAction;
    private InputAction jumpAction;

    private Vector2 movementInput;
    private Vector3 moveDirection = Vector3.zero;

    private Rigidbody rb;

    private void Awake() {
        movementAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        isReadyToJump = true;
    }

    private void FixedUpdate() {
        if (CanMove) {
            HandleMovement();
        }
    }

    private void Update() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.2f, groundMask);
        if (isGrounded) {
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = 0;
        }

        HandleInput();
        HandleSpeedControl();
    }

    private void HandleInput() {
        // Get the movement input from the player
        movementInput = movementAction.ReadValue<Vector2>();

        // Get the jump input from the player
        if (jumpAction.triggered && isReadyToJump && isGrounded) {
            isReadyToJump = false;
            HandleJump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void HandleMovement() {
        moveDirection = orientation.forward * movementInput.y + orientation.right * movementInput.x;

        if (isGrounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);    
        }
        else if (!isGrounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airDrag, ForceMode.Force);
        }
    }

    private void HandleSpeedControl() {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > moveSpeed) {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void HandleJump() {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() {
        isReadyToJump = true;
    }
}
