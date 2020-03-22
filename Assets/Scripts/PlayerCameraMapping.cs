using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * This class is used to get the viewport height & width at the Z-level where the player object is located in reference to the camera.
 */
public class PlayerCameraMapping : MonoBehaviour
{
    [Header("Camera")]
    public Camera cam;

    public List<GameObject> changeListeners = new List<GameObject>();

    [Tooltip("Offset the player is in front of the camera")]
    public float playerZOffset = 20;

    /**
     * The last known screen wifth & height, used for knowing when the screen dimensions have changed
     */
    private float viewWidth = 0.0f;
    private float viewHeight = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        handleCameraDimensions();
    }

    // Update is called once per frame
    void Update()
    {
        handleCameraDimensions();
    }

    /**
     * Retrieves the camera size and changes the player offset & pose-scaling based on this.
     * The player should be located in the middle of the camera
     */
    private void handleCameraDimensions()
    {
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, playerZOffset));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, playerZOffset));
        float newViewWidth = topRight.x - bottomLeft.x;
        float newViewHeight = topRight.y - bottomLeft.y;
        if ((Math.Abs(viewWidth - newViewWidth) > 0.01) || (Math.Abs(viewHeight - newViewHeight) > 0.01))
        {
            Debug.Log("Screen bottomLeft: " + bottomLeft + ", topRight: " + topRight);
            viewWidth = newViewWidth;
            viewHeight = newViewHeight;
            Debug.Log("Camera [width: " + viewWidth + ", height: " + viewHeight + "]");

            // Now set the player location to be located in the middle of the camera, but a little bit in front of it
            float x = bottomLeft.x; // + (viewWidth / 2);
            float y = bottomLeft.y; // + (viewHeight / 2);
            float z = cam.transform.position.z + playerZOffset;
            Debug.Log("Player location is set to [x: " + x + ", y: " + y + ", z: " + z + "]");
//            Transform transform = player.transform;
            transform.localPosition = new Vector3(x, y, z);

            notifyObservers(bottomLeft, topRight);
        }
    }

    /**
     * Notifies all objects in the observer list about changes in the camera size & location
     */
    private void notifyObservers(Vector3 bottomLeft, Vector3 topRight) {
        foreach (GameObject obj in changeListeners) {
            CameraDimensionObserver observer = obj.GetComponent<CameraDimensionObserver>();
            if (observer != null)
            {
                observer.cameraDimensionsChanged(bottomLeft, topRight, viewWidth, viewHeight);
            }
            
        }
    }
}
