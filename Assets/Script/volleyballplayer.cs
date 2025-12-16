using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class volleyballplayer : MonoBehaviour
{
    public Transform volleyball;
    public Transform player;
    Rigidbody rigid;
    


    public float idealDistance;
    public float setPower;
    public float hitPower;
    public float bumpPower;
    public float servePower;

    [Header("Audio")]
    private AudioSource audio;
    public AudioClip hitSFX;
    public float hitSfxLevel;

    public enemy enemy;
    public Volleyball points;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }


    void Update()
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
        float power = GetHitPower();

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
        Vector3 volleyballVector = transform.position - volleyball.transform.position;
        Vector3 planeTangent = Vector3.Cross(volleyballVector, player.transform.forward);
        Vector3 planeNormal = Vector3.Cross(planeTangent, volleyballVector);
        Vector3 reflected = Vector3.Reflect(player.transform.forward, planeNormal);
        return reflected.normalized;
    }

    private float GetHitPower()
    {
        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3f + 1f;

        float power = y * hitPower;
        power = Mathf.Clamp(power, 0f, hitPower);
        return power;
    }

    private float GetBumpPower()
    {
        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3f + 1f;

        float power = y * bumpPower;
        power = Mathf.Clamp(power, 0f, bumpPower);
        return power;
    }

    private float GetSetPower()
    {
        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3f + 1f;

        float power = y * setPower;
        power = Mathf.Clamp(power, 0f, setPower);
        return power;
    }
    private float GetServePower()
    {
        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3f + 1f;

        float power = y * servePower;
        power = Mathf.Clamp(power, 0f, servePower);
        return power;
    }

}
