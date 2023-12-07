using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstPersonController : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    // Basic movement
    [Header("Movement")]
    private float moveSpeed = 7f;
    public float walkSpeed;
    public float sprintSpeed;

    public float speedMult = 10f;
    public float groundDrag = 5f;


    [Header("Jumping")]
    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 1f;
    bool readyToJump = true;


    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;


    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    bool isGrounded;


    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    [Header("Movement State")]
    public MovementState state; // always storing the current state the player is in

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }


    [Header("Other")]
    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    // Crouching
    // Sliding
    // Grappling

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;

    }


    private void Update()
    {
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        PlayerInput();
        SpeedControl();
        StateHandler();

        // Handle drag
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        // stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

    }


    private void StateHandler()
    {
        // Mode - crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - sprinting
        else if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        // Mode - walking
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // Mode - air
        else
        {
            state = MovementState.air;
        }
    }


    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        else if (isGrounded)
        {
            // Check if the player is crouching and set the moveSpeed accordingly
            float currentMoveSpeed = state == MovementState.crouching ? crouchSpeed : moveSpeed;
            rb.AddForce(moveDirection.normalized * moveSpeed * speedMult, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * speedMult * airMultiplier, ForceMode.Force);
        }

        // turn of gravity while standing on the slope
        rb.useGravity = !OnSlope();
    }


    private void SpeedControl()
    {
        // limit speed on slope
        if (OnSlope())
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        // limit speed on ground or air
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity to moveSpeed if need
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }


    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity so that you always jump the same height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }


    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }


    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
