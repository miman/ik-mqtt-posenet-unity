using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectSpawner : MonoBehaviour, CameraDimensionObserver
{
    [Header("Referred entities")]
    /**
     * The camera, we will spawn the items in relation to the location of this
     */
    [Tooltip("The camera object")]
    public Camera cam;
    /**
     * The player character reference
     */
    [Tooltip("The player character")]
    public Transform player;
    /**
     * The position of the "floor" of the game
     */
    [Tooltip("The 'floor' of the game")]
    public Transform gameBoard;

    [Header("Obstacle spawn window")]
    [Tooltip("Obstacles are spawned in a box with a Y-size of this % of the total view")]
    public float yWindowPerc = 40;
    [Tooltip("Obstacles are spawned in a box with a X-size of this % of the total view")]
    public float xWindowPerc = 70;
    [Tooltip("Obstacles are spawned above this Y-value (zero is bottom of viewport)")]
    public float yWindowOffset = 10;
    [Tooltip("Obstacles are spawned to the right of this X-value (zero is left edge of viewport)")]
    public float xWindowOffset = 10;

    [Header("Obstacle spawn & Delete values")]
    [Tooltip("Number of ms between each obstacle spawn")]
    public float insertDeltaMs = 1000.0f;
    [Tooltip("Offset from the camera where the obstacles are deleted")]
    public float deleteCameraZOffset = 0;
    [Tooltip("The speed of the Obstacles")]
    public float obstacleSpeed = 10;

    // The item is added this amount ahead of the player
    [Tooltip("The item is added this amount ahead of the player")]
    public float insertAtHorizon = 10;

    [Tooltip("A template for the obstacle")]
    public GameObject obstacleTemplate = null;
    [Tooltip("Another template for the obstacle")]
    public GameObject obstacle2Template = null;

    /**
     * The list of all generated obstacles
     */
    List<ObstacleEntity> obstacleList = new List<ObstacleEntity>();
    string obstacleResourceName = "Ball";  // Obstacle
    string nonCriticalResName = "SA_TrafficBarrier_01";

    private System.DateTime lastUpdatedTime = System.DateTime.Now;

    Vector3 viewBottomLeft;
    Vector3 viewTopRight;
    float viewHeight = 0.0f;
    float viewWidth = 0.0f;

    private int objectsHit = 0;
    private int objectsMissed = 0;

    public int ObjectsHit { get => objectsHit; }

    public int ObjectsMissed { get => objectsMissed; }

    // Use this for initialization
    void Start()
    {
//        Debug.Log("ObjectSpawner Initiated");
    }

    // Update is called once per frame
    void Update()
    {
        autoGenerateObstacles();
        // Remove all old obstacles to free up memory
        removeObstaclesBeforeZ(player.position.z);
    }

    void FixedUpdate()
    {
        // Move all obstacles towards their end position
        for (int i = 0; i < obstacleList.Count; i++)
        {
            ObstacleEntity obj = obstacleList[i];
            Vector3 currentPos = obj.obstacle.transform.position;
            Vector3 velocity = (obj.endPosAtPlayerLvl - currentPos).normalized * obstacleSpeed;
            obj.obstacle.GetComponent<Rigidbody>().MovePosition(currentPos + velocity * Time.deltaTime);
        }
    }

    /**
     * Insert new object if enough time has passed since we last added one
     */
    void autoGenerateObstacles()
    {
        System.DateTime now = System.DateTime.Now;
        if ((now - lastUpdatedTime).TotalMilliseconds > insertDeltaMs)
        {   // We update every second
            lastUpdatedTime = now;

            addRandomObstacle();
        }
    }

    /**
     * Add an obstacle that will end up at at the given X/Y-ccordinates at the player Z-level
     * The Z value is on what Z-level the obstacle will be generated
     */
    void addRandomObstacle()
    {
        Vector3 startPos = getObstacleGenerationPosition();

        float rnd2 = UnityEngine.Random.value;
        if (rnd2 < 0.25f)
        {
//            addNonCritcalObstacle(pos);
        }
        else
        {
            addObstacle(startPos);
//            Debug.Log("Obstacle added @ (x: " + x + ", y: " + y + ", z: " + z + ")");
        }
    }

    /**
     * Returns the 3-dimensional postion where we should generate the obstacles
     * We base this in the viewport Z & Y values + insertAtHorizon for Z-vale
     */
    private Vector3 getObstacleGenerationPosition()
    {
        // Create X & Y values at the middle of the viewport
        float x = viewBottomLeft.x + (Math.Abs(viewBottomLeft.x - viewTopRight.x) / 2);
        float y = viewBottomLeft.y + (Math.Abs(viewBottomLeft.y - viewTopRight.y) / 2);
        // Id the obstacle a distan in front of the player object
        float z = player.position.z + insertAtHorizon;

        Vector3 pos = new Vector3(x, y, z);
        return pos;
    }

    // Add an obstacle add the given position
    void addObstacle(Vector3 generatorPos)
    {
        float rnd = UnityEngine.Random.value;
        GameObject newObj = null;
        if (rnd > 0.5f) {
            newObj = Instantiate(obstacleTemplate);
        } else {
            newObj = Instantiate(obstacle2Template);
        }

        // Created the wanted end position at the player Z-level
        float x = getRandomX();
        float y = getRandomY();
        float z = player.position.z - 0.3f;    // -0.3 to that we can remove it when it passes the player object
        Vector3 endPosAtPlayerLvl = new Vector3(x, y, z);

        newObj.transform.position = generatorPos;
        ObstacleEntity entity = new ObstacleEntity(endPosAtPlayerLvl, newObj);
        obstacleList.Add(entity);
//        Debug.Log("Obstacle endPoint: '" + endPosAtPlayerLvl + "', BottomLeft: '" + viewBottomLeft + "', TopRight: '" + viewTopRight);
    }

    private void initiateGameObjects()
    {
        obstacleTemplate = Resources.Load(obstacleResourceName, typeof(GameObject)) as GameObject;
    }

    // Remova all obstacles before the given Z value
    // TODO reuse instead
    void removeObstaclesBeforeZ(float z)
    {
        List<ObstacleEntity> objToDeleteList = new List<ObstacleEntity>();
        for (int i = 0; i < obstacleList.Count; i++)
        {
            ObstacleEntity obj = obstacleList[i];
            if (obj.obstacle.transform.position.z < z)
            {
                objToDeleteList.Add(obj);
            }
        }
        for (int i = 0; i < objToDeleteList.Count; i++)
        {
            ObstacleEntity obj = objToDeleteList[i];
//            Debug.Log("Deleting obsolete obstacle '" + obj + "' @ position: " + obj.obstacle.transform.position + ", cam Z: " + z + ", active obstacles: " + obstacleList.Count);
            obstacleList.Remove(obj);
            Destroy(obj.obstacle);
            obj.obstacle = null;
            objectsMissed = objectsMissed + 1;
        }
    }

    /**
    * Get a random y-value adapted according to yWindowPerc & yWindowOffset
    */
    private float getRandomY()
    {
        float rnd = UnityEngine.Random.value;
        // Create random Y-value within the %-filtered viewport
        // We only want the obstacle to be present at a sub-set of the full height
        float randY = (rnd * viewHeight * (yWindowPerc / 100));
        // Now offset the Y-value from the bottom side
        randY = randY + (viewHeight * (yWindowOffset / 100));
        // Add offset
        randY = randY + viewBottomLeft.y;

        return randY;
    }

    /**
    * Get a random x-value adapted according to xWindowPerc & xWindowOffset
    */
    private float getRandomX()
    {
        float rnd = UnityEngine.Random.value;
        // Create random X-value within the %-filtered viewport
        // We only want the obstacle to be present at a sub-set of the full width
        float randX = (rnd * viewWidth * (xWindowPerc / 100));
        // Now offset the X-value from the left side
        randX = randX + (viewWidth * (xWindowOffset / 100));
        // Add offset
        randX = randX + viewBottomLeft.x;

        return randX;
    }

    /**
     * ORetrieves the camera size, which is used to know within which X & Y values to generate the obstacles.
     */
    public void cameraDimensionsChanged(Vector3 viewBottomLeft, Vector3 viewTopRight, float viewWidth, float viewHeight)
    {
        Debug.Log("New camera dimensions received by ObjectSpawner [ bottomLeft: " + viewBottomLeft + ", topRight: " + viewTopRight + ")");
        this.viewBottomLeft = viewBottomLeft;
        this.viewTopRight = viewTopRight;
        this.viewWidth = viewWidth;
        this.viewHeight = viewHeight;
    }

    /**
     * The received GameObject should be deleted from the spawned object pool
     */
    public void obstacleHit(GameObject objToDelete)
    {
        for (int i = 0; i < obstacleList.Count; i++)
        {
            ObstacleEntity obj = obstacleList[i];
            if (obj.obstacle == objToDelete)
            {
                Debug.Log("Spawned object found and will be deleted");
                obstacleList.Remove(obj);
                Destroy(obj.obstacle);
                objectsHit = objectsHit + 1;
                return;
            }
        }
        Debug.Log("The received object '" + objToDelete + "' to delete wasn't found !");
    }

    /**
     * An obstacle entity including the wanted end position
     */
    private class ObstacleEntity
    {
        public Vector3 endPosAtPlayerLvl;
        public GameObject obstacle;

        public ObstacleEntity(Vector3 endPosAtPlayerLvl, GameObject obstacle)
        {
            this.endPosAtPlayerLvl = endPosAtPlayerLvl;
            this.obstacle = obstacle;
        }
    }
}
