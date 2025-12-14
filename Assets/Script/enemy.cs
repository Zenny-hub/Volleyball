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

    [Header("Follow Tolerance")]
    public float followToleranceZ = 0.1f;
    public float followToleranceX = 0.1f;

    float horizontalInputZ;
    float horizontalInputX;

    Rigidbody rb;
    float lockedX;

    [Header("Area")]
    public BoxCollider enemyArea;

    [Header("Block Settings")]
    public float blockRange = 1.5f;
    public float blockHeightOffset = 2f;
    public float blockIdealDistance = 1.2f;
    public float maxBlockPower = 10f;

    public bool isBlockingJump;
    public bool hasBlockedThisJump;

    [Header("Bump Settings")]
    public float bumpRange = 1.6f;
    public float bumpHeightOffset = 1.2f;
    public float bumpPower = 7f;
    public float bumpCooldown = 0.35f;

    [Header("Set Settings")]
    public float setPower = 7f;              // NEW
    public float setForwardDiv = 3f;         // NEW (matches your player: power/3f)

    private float lastTouchTime = -999f;     // renamed from lastBumpTime
    private bool hasBumped = false;          // NEW (state)

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        lockedX = transform.position.x;
    }

    private void Update()
    {
        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.3f,
            whatIsGround
        );

        if (grounded)
        {
            isBlockingJump = false;
        }

        AIInput();
        TryBlockBall();
        TryBumpThenSet();   // CHANGED
        SpeedControl();

        rb.linearDamping = grounded ? groundDrag : 0f;

        if (!IsBallInArea())
        {
            Vector3 pos = transform.position;
            pos.x = lockedX;
            transform.position = pos;
        }
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
            horizontalInputX = 0f;
            return;
        }

        if (IsBallInArea())
        {
            float dz = ball.position.z - transform.position.z;
            float dx = ball.position.x - transform.position.x;

            horizontalInputZ = Mathf.Abs(dz) > followToleranceZ ? Mathf.Sign(dz) : 0f;
            horizontalInputX = Mathf.Abs(dx) > followToleranceX ? Mathf.Sign(dx) : 0f;
        }
        else
        {
            horizontalInputX = 0f;

            float dz = ball.position.z - transform.position.z;
            horizontalInputZ = Mathf.Abs(dz) > followToleranceZ ? Mathf.Sign(dz) : 0f;
        }
    }

    private void MoveEnemy()
    {
        Vector3 moveDirection = new Vector3(horizontalInputX, 0f, horizontalInputZ);

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        float multiplier = grounded ? 1f : airMultiplier;
        rb.AddForce(moveDirection * moveSpeed * 10f * multiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 horizontalVel = new Vector3(vel.x, 0f, vel.z);

        if (horizontalVel.magnitude > moveSpeed)
        {
            Vector3 limited = horizontalVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limited.x, vel.y, limited.z);
        }
    }

    public void resetToBump()
    {
        hasBumped = false;
    }
    public void JumpTrigger()
    {
        if (!grounded || !readyToJump) return;

        readyToJump = false;
        isBlockingJump = true;
        hasBlockedThisJump = false;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        readyToJump = true;
        isBlockingJump = false;
        hasBlockedThisJump = false;
    }

    private void TryBlockBall()
    {
        if (!isBlockingJump || hasBlockedThisJump || grounded) return;
        if (ball == null || vballRb == null) return;

        Vector3 blockPoint = transform.position + Vector3.up * blockHeightOffset;
        float dist = Vector3.Distance(ball.position, blockPoint);

        if (dist > blockRange) return;

        float power = GetBlockPower(dist);
        if (power <= 0f) return;

        float toPlayerX = Mathf.Sign(ball.position.x - lockedX);
        if (toPlayerX == 0f) toPlayerX = -1f;

        Vector3 dir = new Vector3(toPlayerX, 0.8f, 0f).normalized;
        vballRb.linearVelocity = dir * power;

        hasBlockedThisJump = true;
    }

    private float GetBlockPower(float distance)
    {
        float y = -Mathf.Abs(distance - blockIdealDistance) / 3f + 1f;
        return Mathf.Clamp(y * maxBlockPower, 0f, maxBlockPower);
    }


    private void TryBumpThenSet()
    {
        if (ball == null || vballRb == null) return;
        if (!IsBallInArea()) return;
        if (Time.time < lastTouchTime + bumpCooldown) return;
        if (isBlockingJump && !grounded) return;

        Vector3 touchPoint = transform.position + Vector3.up * bumpHeightOffset;
        float dist = Vector3.Distance(ball.position, touchPoint);

        if (dist > bumpRange) return;

        if (!hasBumped)
        {
            // FIRST TOUCH = BUMP (up only)
            float power = GetBumpPower(dist);
            if (power <= 0f) return;

            vballRb.linearVelocity = Vector3.zero;
            vballRb.AddForce(Vector3.up * power, ForceMode.Impulse);

            hasBumped = true;
        }
        else
        {
            // SECOND TOUCH = SET (up + forward)
            float power = GetSetPower(dist);
            if (power <= 0f) return;

            vballRb.linearVelocity = Vector3.zero;
            vballRb.AddForce(Vector3.up * power, ForceMode.Impulse);
            vballRb.AddForce(transform.forward * (power / setForwardDiv), ForceMode.Impulse);

            hasBumped = false;
        }

        lastTouchTime = Time.time;
    }

    private float GetBumpPower(float distance)
    {
        float y = -Mathf.Abs(distance - idealDistance) / 3f + 1f;
        return Mathf.Clamp(y * bumpPower, 0f, bumpPower);
    }

    private float GetSetPower(float distance)
    {
        float y = -Mathf.Abs(distance - idealDistance) / 3f + 1f;
        return Mathf.Clamp(y * setPower, 0f, setPower);
    }

    private bool IsBallInArea()
    {
        if (ball == null || enemyArea == null) return false;
        return enemyArea.bounds.Contains(ball.position);
    }
}
