using UnityEngine;

public class Volleyball : MonoBehaviour
{
    public Transform player;
    
    public GameObject inPrefab;   // The prefab to spawn
    public GameObject outPrefab;
    private bool hasSpawned = false;

    private GameObject prefab;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            transform.position = player.transform.position + new Vector3(0, 10, 0);
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            hasSpawned = false;
            Destroy(prefab);
        }
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Only spawn once
        if (hasSpawned) return;

        // Make sure the collision is with the ground
        if (collision.gameObject.CompareTag("In"))
        {
            hasSpawned = true;

            // Get exact contact point
            Vector3 spawnPos = collision.contacts[0].point;

            // Lift slightly to avoid clippingsd
            spawnPos.y += 0.115f; 

            // Flat rotations
            Quaternion rotation = Quaternion.identity;

            // Spawn
            prefab = Instantiate(inPrefab, spawnPos, rotation);
        }
        if (collision.gameObject.CompareTag("Out"))
        {
            hasSpawned = true;

            // Get exact contact point
            Vector3 spawnPos = collision.contacts[0].point;

            // Lift slightly to avoid clipping
            spawnPos.y += 0.115f;

            // Flat rotation
            Quaternion rotation = Quaternion.identity;

            // Spawn
            prefab = Instantiate(outPrefab, spawnPos, rotation);
        }
    }
}



