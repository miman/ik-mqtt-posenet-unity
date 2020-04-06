using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * Uses Posenet input to set the position of the gameobjects set in this script mapped to the correct body part.
 * This script which will give each body part a 0-100% value for the X&Y values based on the camera screen.
 * The start point is bottom-left = (0 %,0 %) & upper-right = (100 %, 100 %)
 * 
 * The height & width size factor will be used to recalculate the % values to correct game coordinates
 */
public class PoseInputController : PoseEventHandler {

    [Header("Tracking boundary factor")]
    [Tooltip("The width factor the 100% should be recalculated to")]
    public float widthSizeFactor = 100;
    [Tooltip("The height factor the 100% should be recalculated to")]
    public float heightSizeFactor = 100;

    [Header("Camera")]
    public Camera cam;

    [Tooltip("Offset the player is in front of the camera")]
    public float playerZOffset = 20;

    /**
     * The last known screen width & height, used for knowing when the screen dimensions have changed
     */
    private float viewWidth = 0.0f;
    private float viewHeight = 0.0f;

    /**
     * To keep track of max min values of tracking.
     */
    private Dictionary<string, MaxMinCoord> maxMinCoordMap = new Dictionary<string, MaxMinCoord>();

    /**
     * The last known screen size, used for knowing when the screen dimensions have changed
     */
    private Vector2 cameraDimension;

    /**
     * Called at startup.
     * Here we initiallize all values
     */
    public void Awake() {
        DontDestroyOnLoad(this);

        cameraDimension = new Vector2(0,0);
        initPrevCoords(widthSizeFactor, heightSizeFactor);

        maxMinCoordMap.Add("nose", new MaxMinCoord());
        maxMinCoordMap.Add("leftEye", new MaxMinCoord());
        maxMinCoordMap.Add("rightEye", new MaxMinCoord());
        maxMinCoordMap.Add("leftEar", new MaxMinCoord());
        maxMinCoordMap.Add("rightEar", new MaxMinCoord());
        maxMinCoordMap.Add("leftShoulder", new MaxMinCoord());
        maxMinCoordMap.Add("rightShoulder", new MaxMinCoord());
        maxMinCoordMap.Add("leftElbow", new MaxMinCoord());
        maxMinCoordMap.Add("rightElbow", new MaxMinCoord());
        maxMinCoordMap.Add("leftWrist", new MaxMinCoord());
        maxMinCoordMap.Add("rightWrist", new MaxMinCoord());
        maxMinCoordMap.Add("leftHip", new MaxMinCoord());
        maxMinCoordMap.Add("rightHip", new MaxMinCoord());
        maxMinCoordMap.Add("leftKnee", new MaxMinCoord());
        maxMinCoordMap.Add("rightKnee", new MaxMinCoord());
        maxMinCoordMap.Add("leftAnkle", new MaxMinCoord());
        maxMinCoordMap.Add("rightAnkle", new MaxMinCoord());
    }

    public void Start() {
        handleCameraDimensions();
    }

    /**
     * Called each frame to update the view
     */
    void Update()
    {
        handleCameraDimensions();

        if (lastPose != null)
        {   // act on last pose-event
            handleNodeMovement(lastPose.nose, nose, ref prevNoseCoord, "nose");
            handleNodeMovement(lastPose.leftEye, leftEye, ref prevLeftEyeCoord, "leftEye");
            handleNodeMovement(lastPose.rightEye, rightEye, ref prevRightEyeCoord, "rightEye");
            handleNodeMovement(lastPose.leftEar, leftEar, ref prevLeftEarCoord, "leftEar");
            handleNodeMovement(lastPose.rightEar, rightEar, ref prevRightEarCoord, "rightEar");
            handleNodeMovement(lastPose.leftShoulder, leftShoulder, ref prevLeftShoulderCoord, "leftShoulder");
            handleNodeMovement(lastPose.rightShoulder, rightShoulder, ref prevRightShoulderCoord, "rightShoulder");
            handleNodeMovement(lastPose.leftElbow, leftElbow, ref prevLeftElbowCoord, "leftElbow");
            handleNodeMovement(lastPose.rightElbow, rightElbow, ref prevRightElbowCoord, "rightElbow");
            handleNodeMovement(lastPose.leftWrist, leftWrist, ref prevLeftWristCoord, "leftWrist");
            handleNodeMovement(lastPose.rightWrist, rightWrist, ref prevRightWristCoord, "rightWrist");
            handleNodeMovement(lastPose.leftHip, leftHip, ref prevLeftHipCoord, "leftHip");
            handleNodeMovement(lastPose.rightHip, rightHip, ref prevRightHipCoord, "rightHip");
            handleNodeMovement(lastPose.leftKnee, leftKnee, ref prevLeftKneeCoord, "leftKnee");
            handleNodeMovement(lastPose.rightKnee, rightKnee, ref prevRightKneeCoord, "rightKnee");
            handleNodeMovement(lastPose.leftAnkle, leftAnkle, ref prevLeftAnkleCoord, "leftAnkle");
            handleNodeMovement(lastPose.rightAnkle, rightAnkle, ref prevRightAnkleCoord, "rightAnkle");

            PosePosition pelvisPose = new PosePosition();
            pelvisPose.x = (lastPose.rightHip.x + lastPose.leftHip.x)/2;
            pelvisPose.y = (lastPose.rightHip.y + lastPose.leftHip.y) / 2;
            PosePosition middleSpinePose = new PosePosition();
            middleSpinePose.x = (lastPose.rightHip.x + lastPose.leftShoulder.x) / 2;
            middleSpinePose.y = (lastPose.leftShoulder.y + lastPose.leftHip.y) / 2;
            handleNodeMovement(pelvisPose, pelvis, ref prevPelvisCoord, "Pelvis");
            handleNodeMovement(middleSpinePose, middleSpine, ref prevMiddleSpineCoord, "MiddleSpine");

            processedPose = lastPose;
            lastPose = null;
        } else
        {
//            Debug.Log("No lastPose present");
        }
    }

    /**
     * Act on node movement
     * param name="posePos" The new position value for this node
     * param name="node"    The node this new position is for
     * param name="previousCoord"   The previous coordinates for this node (this will be updated by the function to the new last values, that is the current)
     * param name="desc"    The description of the node (used for logging)
     */
    private void handleNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string desc)
    {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        Vector2 currentCoord = convertPercentageToPixels(posePos);

        MaxMinCoord mmCoord = maxMinCoordMap[desc];
        if (mmCoord != null)
        {
            mmCoord.handleCoord(currentCoord);
//            Debug.Log(desc + ": " + mmCoord.ToString());
        }

        // Smoothen the change in controlled GameObject
        float delta = currentCoord.x - previousCoord.x;
        currentCoord.x = previousCoord.x + (delta / smoothening);
        delta = currentCoord.y - previousCoord.y;
        currentCoord.y = previousCoord.y + (delta / smoothening);


        previousCoord.x = currentCoord.x;
        previousCoord.y = currentCoord.y;
        // Add offset
        //        currentCoord.x = currentCoord.x + xOffset;
        //        currentCoord.y = currentCoord.y + yOffset;
        if (desc == "leftWrist" || desc == "rightWrist") {
            Debug.Log("Pos for " + desc + ": posePos: " + posePos + " -> currentCoord: " + currentCoord);
        }

        Transform transform = node.transform;
        transform.localPosition = new Vector3(currentCoord.x, currentCoord.y, transform.localPosition.z);
//        transform.position = new Vector3(currentCoord.x, currentCoord.y, transform.position.z);
    }

    /**
     * Converts screen percentage to actual Unity screen pixels.
     * param name="posePos" A position in %
     * returns  The position in game coordinates
     */
    private Vector2 convertPercentageToPixels(PosePosition posePos)
    {
        float x = posePos.x;
        float y = posePos.y;

        Vector2 coord = new Vector2(0,0);
        coord.x = x*(widthSizeFactor/100);
        coord.y = y*(heightSizeFactor/100);
        return coord;
    }

    /**
     * Used to set the width/height size factors to calcualte the game coordinates correctly.
     * Set this based on the value at where tha avatar is (usually not the same as the camera)
    */
    private void cameraDimensionsChanged(Vector3 viewBottomLeft, Vector3 viewTopRight, float viewWidth, float viewHeight)
    {
        Debug.Log("New camera dimensions set in PoseInputController  [ width: " + viewWidth + ", height: " + viewHeight + ")");
        widthSizeFactor = viewWidth;
        heightSizeFactor = viewHeight;
    }

    /**
     * Retrieves the camera size and changes the player offset & pose-scaling based on this.
     * The player should be located in the middle of the camera
     */
    private void handleCameraDimensions() {
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, playerZOffset));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, playerZOffset));
        float newViewWidth = topRight.x - bottomLeft.x;
        float newViewHeight = topRight.y - bottomLeft.y;
        if ((Math.Abs(viewWidth - newViewWidth) > 0.01) || (Math.Abs(viewHeight - newViewHeight) > 0.01)) {
            // The camera dimensions changed -> handle change
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
            // transform.localPosition = new Vector3(x, y, z);
            transform.position = new Vector3(x, y, z);

            cameraDimensionsChanged(bottomLeft, topRight, viewWidth, viewHeight);
        }
    }

}
