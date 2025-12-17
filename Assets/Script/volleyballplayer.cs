using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class volleyballplayer : MonoBehaviour
{
    [Header("Refrences")]
    public Transform volleyball;
    public Transform player;
    public enemy enemy;
    public Volleyball points;

    [Header("Power")]
    public float idealDistance;
    public float setPower;
    public float hitPower;
    public float bumpPower;
    public float servePower;

    [Header("Audio")]
    private AudioSource audio;
    public AudioClip hitSFX;
    public float hitSfxLevel;

    Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }


    void Update()
    {
        buttonInputs();
    }

    private void buttonInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryHitBall();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            bump();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            set();
        }
        if (Input.GetKeyDown(KeyCode.F) && points.playerServe)
        {
            serve();
            points.playerServe = false;
        }
    }
    private void TryHitBall()
    {
        // Gets hit power
        float power = GetHitPower();

        // Only hits if ball is in tange
        if (power > 0)
        {
            Rigidbody rb = volleyball.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.linearVelocity = GetReflected() * -power;
            audio.PlayOneShot(hitSFX, hitSfxLevel);
            points.playerTouch();
            
            
        }
    }

    private void bump()
    {
        float power = GetBumpPower();

        if (power > 0)
        {
            Rigidbody rb = volleyball.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(Vector3.up * power, ForceMode.Impulse);
            points.playerTouch();
        }
    }

    private void set()
    {
        float power = GetSetPower();

        if (power > 0)
        {
            Rigidbody rb = volleyball.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(Vector3.up * power, ForceMode.Impulse);
            rb.AddForce(player.forward * power / 3f, ForceMode.Impulse);
            points.playerTouch();

        }
    }
        private void serve()
    {
        float power = GetServePower();

        if (power > 0)
        {
            Rigidbody rb = volleyball.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(Vector3.up * power, ForceMode.Impulse);
            rb.AddForce(player.forward * power / 3f, ForceMode.Impulse);
            points.playerTouch();
            
        }
    }

    private Vector3 GetReflected()
    {
        // distance of player relative to object
        Vector3 volleyballVector = transform.position - volleyball.transform.position;
        // first cross product
        Vector3 planeTangent = Vector3.Cross(volleyballVector, player.transform.forward);
        // second cross product
        Vector3 planeNormal = Vector3.Cross(planeTangent, volleyballVector);
        
        Vector3 reflected = Vector3.Reflect(player.transform.forward, planeNormal);
        return reflected.normalized;
    }

    

    
    private float GetHitPower()
    {
        //distance from ball
        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3f + 1f;

        //calculating power
        float power = y * hitPower;
        power = Mathf.Clamp(power, 0f, hitPower);
        return power;
    }

    private float GetBumpPower()
    {
        //distance from ball
        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3f + 1f;

        //calculating power
        float power = y * bumpPower;
        power = Mathf.Clamp(power, 0f, bumpPower);
        return power;
    }

    private float GetSetPower()
    {
        //distance from ball
        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3f + 1f;

        //calculating power
        float power = y * setPower;
        power = Mathf.Clamp(power, 0f, setPower);
        return power;
    }
    private float GetServePower()
    {
        //distance from ball
        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3f + 1f;

        //calculating power
        float power = y * servePower;
        power = Mathf.Clamp(power, 0f, servePower);
        return power;
    }

}
