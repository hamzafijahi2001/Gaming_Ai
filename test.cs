using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleRunGame : MonoBehaviour
{
    // Player
    public float playerSpeed = 5f;
    public float jumpForce = 5f;
    private bool isJumping = false;
    private GameObject player;

    // Road
    public GameObject roadSegment;
    public int roadSegments = 10;
    public float segmentLength = 10f;
    private List<GameObject> currentRoadSegments = new List<GameObject>();

    // Obstacles
    public GameObject obstacle;
    public float obstacleSpawnInterval = 2f;
    public float obstacleSpawnRange = 2f;
    private List<GameObject> currentObstacles = new List<GameObject>();

    void Start()
    {
        // Create player
        CreatePlayer();

        // Start generating road
        GenerateInitialRoad();

        // Start obstacle spawner
        StartCoroutine(SpawnObstacles());

        // Start moving player
        StartCoroutine(MovePlayerForward());
    }

    void Update()
    {
        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            Jump();
        }

        // Remove obstacles and road segments that are too far back
        CleanUp();
    }

    void CreatePlayer()
    {
        // Create a sphere as the player
        player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player";
        player.transform.position = Vector3.zero;

        // Add Rigidbody to the player
        if (!player.GetComponent<Rigidbody>())
        {
            player.AddComponent<Rigidbody>();
        }

        // Add collider to the player
        if (!player.GetComponent<SphereCollider>())
        {
            player.AddComponent<SphereCollider>();
        }
    }

    void GenerateInitialRoad()
    {
        for (int i = 0; i < roadSegments; i++)
        {
            GameObject segment = CreateRoadSegment();
            segment.transform.position = new Vector3(0f, 0f, i * segmentLength);
            currentRoadSegments.Add(segment);
        }
    }

    GameObject CreateRoadSegment()
    {
        // Create a cube as the road segment
        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segment.name = "RoadSegment";
        segment.transform.localScale = new Vector3(2f, 0.2f, segmentLength);
        segment.transform.rotation = Quaternion.identity;

        // Add collider to the road segment
        if (!segment.GetComponent<BoxCollider>())
        {
            segment.AddComponent<BoxCollider>();
        }

        return segment;
    }

    IEnumerator SpawnObstacles()
    {
        while (true)
        {
            // Create an obstacle
            GameObject obj = CreateObstacle();
            currentObstacles.Add(obj);

            yield return new WaitForSeconds(obstacleSpawnInterval);
        }
    }

    GameObject CreateObstacle()
    {
        // Create a cube as the obstacle
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "Obstacle";
        obj.transform.position = new Vector3(Random.Range(-obstacleSpawnRange, obstacleSpawnRange), 0.5f, currentRoadSegments[currentRoadSegments.Count - 1].transform.position.z + segmentLength);
        obj.transform.localScale = new Vector3(1f, 1f, 1f);

        // Add collider to the obstacle
        if (!obj.GetComponent<BoxCollider>())
        {
            obj.AddComponent<BoxCollider>();
        }

        return obj;
    }

    void Jump()
    {
        isJumping = true;
        player.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        player.GetComponent<Rigidbody>().AddForce(Vector3.forward * playerSpeed, ForceMode.Impulse);
        Invoke("ResetJump", 1f);
    }

    void ResetJump()
    {
        isJumping = false;
    }

    void CleanUp()
    {
        // Remove road segments that are too far back
        if (currentRoadSegments.Count > 0 && currentRoadSegments[0].transform.position.z < player.transform.position.z - segmentLength)
        {
            Destroy(currentRoadSegments[0]);
            currentRoadSegments.RemoveAt(0);

            // Create a new road segment
            GameObject segment = CreateRoadSegment();
            segment.transform.position = new Vector3(0f, 0f, currentRoadSegments[currentRoadSegments.Count - 1].transform.position.z + segmentLength);
            currentRoadSegments.Add(segment);
        }

        // Remove obstacles that are too far back
        if (currentObstacles.Count > 0 && currentObstacles[0].transform.position.z < player.transform.position.z - segmentLength)
        {
            Destroy(currentObstacles[0]);
            currentObstacles.RemoveAt(0);
        }
    }

    IEnumerator MovePlayerForward()
    {
        while (true)
        {
            player.transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime);
            yield return null;
        }
    }
}