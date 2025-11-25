using UnityEngine;

public class Volleyball : MonoBehaviour
{
    public Transform player;
    
   
private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            transform.position = player.transform.position + new Vector3(0, 10, 0);
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }
}

