using UnityEngine;
using UnityEngine.UIElements;

public class Volleyball : MonoBehaviour
{
    [Header("Refrences")]
    public Transform player;
    public Transform enemy;
    public GameObject inPrefab; 
    public GameObject outPrefab;
    private GameObject prefab;
    public BoxCollider enemyIn;
    public BoxCollider playerIn;

    private bool hasSpawned = false;
    private bool playerLastTouch;
    public float playerPoint;
    public float enemyPoint;
    public bool scoreUpdated = false;
    public bool playerScored = false;
    public bool enemyServe = false;
    public bool playerServe = false;
    public bool gameStart;
    

    private int randomInt;
    private void Start()
    {
        // See who serves
        randomInt = Random.Range(1, 3);
        Debug.Log(randomInt);
        enemyPoint = 0;
        playerPoint = 0;
        gameStart = true;

    }





    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.G) && gameStart)
        {
  
            if (randomInt == 1)
            {
                // Player serves
                transform.position = player.transform.position + new Vector3(0, 10, 0);
                playerServe = true;
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

                Debug.Log(playerPoint);
                Debug.Log(enemyPoint);
            }
            else
            {
                // Enemy serves
                enemyServe = true;
                // TP ball for enemy serve
                transform.position = new Vector3(-32, 32, 0);


                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

                Debug.Log(playerPoint);
                Debug.Log(enemyPoint);

            }

        }
        //player serves after point
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

        // If score changed
        if (scoreUpdated)
        {
            player.transform.position = new Vector3(3, 2, 0);
            scoreUpdated = false;
            
            // Enemy serves
            if (!playerScored)
                {
                enemyServe = true;
                transform.position = new Vector3(-32, 32, 0);
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

            // Where it lands
            if (isInEnemy())
            {
                // Player gets point
                playerPoint += 1;
                scoreUpdated = true;
                playerScored = true;
            }
            if (isInPlayer())
            {
                // Enemy gets point
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
                // Enemy gets point
                enemyPoint += 1;
                scoreUpdated = true;
                playerScored = false;
                
            }
            else
            {
                // Player gets point
                playerPoint += 1;
                scoreUpdated = true;
                playerScored = true;
            }

        }

    }

    // Checks if ball enters a box collider with "isTrigger"
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



