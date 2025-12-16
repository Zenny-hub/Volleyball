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


    [Header("Bump Settings")]
    public float bumpRange;
    public float bumpHeightOffset;
    public float bumpPower;
    public float bumpCooldown;

    [Header("Set Settings")]
    public float setPower;
    public float servePower;
    public float setForwardDiv;         
    private float lastTouchTime = -999f;     
    private bool hasBumped = false;          


    public Volleyball points;

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


        AIInput();
        TryBumpThenSet();
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


    private void ResetJump()
    {
        readyToJump = true;

    }

  

    private void TryBumpThenSet()
    {
        if (ball == null || vballRb == null) return;
        if (!IsBallInArea()) return;
        if (Time.time < lastTouchTime + bumpCooldown) return;
        

        Vector3 touchPoint = transform.position + Vector3.up * bumpHeightOffset;
        float dist = Vector3.Distance(ball.position, touchPoint);

        if (dist > bumpRange) return;

        if (!hasBumped)
        {
     
            float power = GetBumpPower(dist);
            if (power <= 0f) return;

            vballRb.linearVelocity = Vector3.zero;
            vballRb.AddForce(Vector3.up * power, ForceMode.Impulse);

            hasBumped = true;
        }
        else
        {

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
        if (points.enemyServe)
        {
            return Mathf.Clamp(y * servePower, 0f, servePower);
            points.enemyServe = false;
        }
        else
        {
            return Mathf.Clamp(y * setPower, 0f, setPower);
        }
        
    }

    private bool IsBallInArea()
    {
        if (ball == null || enemyArea == null) return false;
        return enemyArea.bounds.Contains(ball.position);
    }
}
