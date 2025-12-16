using UnityEngine;

public class Volleyball : MonoBehaviour
{
    public Transform player;
    public Transform enemy;
    public GameObject inPrefab;   // The prefab to spawn
    public GameObject outPrefab;
    private bool hasSpawned = false;
    private bool playerLastTouch;
    public float playerPoint;
    public float enemyPoint;
    private GameObject prefab;
    public bool scoreUpdated = false;
    public bool playerScored = false;
    public bool enemyServe = false;
    public bool playerServe = false;
    public BoxCollider enemyIn;
    public BoxCollider playerIn;
    private void Start()
    {
        int randomInt = Random.Range(1, 3);
        Debug.Log(randomInt);
        if (randomInt == 1)
        {
            playerScored = true;
            player.transform.position = new Vector3(3, 2, 0);
        }
        else
        {
            player.transform.position = new Vector3(3, 2, 0);
            scoreUpdated = false;
            enemyServe = true;
            transform.position = new Vector3(-32, 32, 0);
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Debug.Log(playerPoint);
            Debug.Log(enemyPoint);

            playerPoint = 0;
            enemyPoint = 0;
        }
    }

    
    private void Update()
    {
       if (Input.GetKeyDown(KeyCode.G) && playerScored)
        {
  
            transform.position = player.transform.position + new Vector3(0, 10, 0);
            playerServe = true;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            hasSpawned = false;
            Destroy(prefab);
            Debug.Log(playerPoint);
            Debug.Log(enemyPoint);
        }
        if (scoreUpdated)
        {
            player.transform.position = new Vector3(3, 2, 0);
            scoreUpdated = false;
            

            if (!playerScored)
                {
                enemyServe = true;
    transform.position = new Vector3(-32, 32, 0);
                
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                hasSpawned = false;
                Destroy(prefab);
                Debug.Log(playerPoint);
                Debug.Log(enemyPoint);
            }
        }
    }
   
    public void playerTouch()
    {
        playerLastTouch = true;
    }
    public void enemyTouch()
    {
        playerLastTouch = false;
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

            if (isInEnemy())
            {
                playerPoint += 1;
                scoreUpdated = true;
                playerScored = true;
            }
            if (isInPlayer())
            {
                enemyPoint += 1;
                scoreUpdated = true;
                playerScored = false;
            }
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

            if (playerLastTouch)
            {

                enemyPoint += 1;
                scoreUpdated = true;
                playerScored = false;
                
            }
            else
            {
                playerPoint += 1;
                scoreUpdated = true;
                playerScored = true;
            }

        }

    }
    private bool isInPlayer()
    {
        if (enemyIn == null) return false;
        return playerIn.bounds.Contains(transform.position);
    }
    private bool isInEnemy()
    {
        if (playerIn == null) return false;
        return enemyIn.bounds.Contains(transform.position);
    }
}



