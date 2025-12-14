using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class volleyballplayer : MonoBehaviour
{
    public Transform volleyball;
    public Transform player;
    Rigidbody rigid;
    

    public float diveForce;
    public float idealDistance;
    public float setPower;
    public float hitPower;
    public float bumpPower;

    [Header("Audio")]
    private AudioSource audio;
    public AudioClip hitSFX;
    public float hitSfxLevel;

    public enemy enemy;

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

        if (Input.GetKeyDown(KeyCode.F))
        {
            bump();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            set();
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
            
            if (enemy != null)
            {
                enemy.JumpTrigger();
                enemy.resetToBump();
            }
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
            rb.AddForce(player.forward * power/3f , ForceMode.Impulse);

            if (enemy != null)
            {
                enemy.JumpTrigger();
                enemy.resetToBump();
            }
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

}
