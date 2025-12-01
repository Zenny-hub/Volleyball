using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float groundDrag = 5f;

    public float jumpForce = 8f;
    public float jumpCooldown = 0.4f;
    public float airMultiplier = 0.4f;
    bool readyToJump;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Target (Ball)")]
    public Transform ball;
    public Rigidbody vballRb;
    public float idealDistance;
    public float maxPower;
    public float followToleranceZ = 0.1f;     // How close before it stops moving
    public float jumpBallMinHeight = 2.5f;    // Ball must be this high to jump
    public float jumpHorizontalRange = 0.8f;  // How close ball must be in Z to jump

    float horizontalInputZ;  // movement input along Z-axis

    Rigidbody rb;
    float lockedX;           // fixed X position on the net

    [Header("Area")]
    public BoxCollider enemyArea;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        // Lock X position — the blocker must stay on the net
        lockedX = transform.position.x;
    }
    private bool IsBallInArea()
    {
        if (ball == null || enemyArea == null) return false;
        return enemyArea.bounds.Contains(ball.position);
    }
    private bool IsBallOnPlayerSide()
    {
        if (ball == null) return false;

        return ball.position.x < lockedX;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        AIInput();
        SpeedControl();
        IsBallOnPlayerSide();
        IsBallInArea();
        rb.linearDamping = grounded ? groundDrag : 0f;

        // Lock X so it cannot move forward/back at all
        Vector3 pos = transform.position;
        pos.x = lockedX;
        transform.position = pos;
    }

    private void FixedUpdate()
    {
        MoveEnemy();
    }

    private void AIInput()
    {
        if (ball == null)
        {
            horizontalInputZ = 0f;
            return;
        }

        
        if (IsBallOnPlayerSide())
        {
            float dz = ball.position.z - transform.position.z;

            if (Mathf.Abs(dz) > followToleranceZ)
                horizontalInputZ = Mathf.Sign(dz);   // move along Z (towards ball's Z)
            else
                horizontalInputZ = 0f;

            return;
        }

        
        if (IsBallInArea())
        {
            float dz = ball.position.z - transform.position.z;

            if (Mathf.Abs(dz) > followToleranceZ)
                horizontalInputZ = Mathf.Sign(dz);
            else
                horizontalInputZ = 0f;
        }
        else
        {
            // Ball is on enemy side but outside our zone -> stay put
            horizontalInputZ = 0f;
        }
    }
    public void JumpTrigger()
    {

        
        // Only jump if grounded + ready
        if (grounded && readyToJump)
        {
            
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
            
        }
    }

    private void MoveEnemy()
    {
        // Move along **Z only**
        Vector3 moveDirection = new Vector3(0f, 0f, horizontalInputZ);

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 vel = rb.linearVelocity;

        // Only control Z speed — X stays locked
        float zSpeed = new Vector3(0f, 0f, vel.z).magnitude;

        if (zSpeed > moveSpeed)
        {
            float limitedZ = Mathf.Sign(vel.z) * moveSpeed;
            rb.linearVelocity = new Vector3(0f, vel.y, limitedZ);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    
}