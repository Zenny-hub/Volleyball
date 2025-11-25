using System.Xml.Serialization;
using UnityEngine;

public class volleyballplayer : MonoBehaviour
{
    public Transform volleyball;
    public Transform player;

    public float idealDistance;
    public float setPower;
    public float hitPower;
    public float bumpPower;
    public enemy enemy;

    // Update is called once per frame
    void Update()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            TryHitBall();
        }
        if (Input.GetKeyDown(KeyCode.F ))
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
            rb.linearVelocity = GetReflected() * -power;


            if (enemy != null)
                
                enemy.JumpTrigger();
        }
    }


    private void bump()
    {
        float power = GetBumpPower();
        if (power > 0)
        {
            Rigidbody rb = volleyball.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * power, ForceMode.Impulse);

            if (enemy != null)
                enemy.JumpTrigger();
        }
    }

    private void set()
    {
        float power = GetSetPower();
        if (power > 0)
        {
            Rigidbody rb = volleyball.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * power, ForceMode.Impulse);
            rb.AddForce(player.forward * power / 5, ForceMode.Impulse);

            if (enemy != null)
                enemy.JumpTrigger();
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
        float y = -Mathf.Abs(x - idealDistance) / 3 + 1;

        float power = y * hitPower;
        power = Mathf.Clamp(power, 0, hitPower);
        return power;

    }

    private float GetBumpPower()
    {


        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3 + 1;

        float power = y * bumpPower;
        power = Mathf.Clamp(power, 0, bumpPower);
        return power;

    }

    private float GetSetPower()
    {


        float x = Vector3.Distance(volleyball.transform.position, transform.position);
        float y = -Mathf.Abs(x - idealDistance) / 3 + 1;

        float power = y * setPower;
        power = Mathf.Clamp(power, 0, setPower);
        return power;

    }

}
