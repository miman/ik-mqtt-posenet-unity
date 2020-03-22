using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * Uses Posenet input to send control commands to the game
 */
public class PoseInputController : PoseEventHandler, CameraDimensionObserver {

    [Header("Tracking boundary factor")]
    [Tooltip("The width factor the 100% should be recalculated to")]
    public float widthSizeFactor = 100;
    [Tooltip("The height factor the 100% should be recalculated to")]
    public float heightSizeFactor = 100;

    /**
     * To keep track of max min values of tracking.
     */
    private Dictionary<string, MaxMinCoord> maxMinCoordMap = new Dictionary<string, MaxMinCoord>();

    /**
     * The last known screen size, used for knowing when the screen dimensions have changed
     */
    private Vector2 cameraDimension;

    public void Awake() {
        DontDestroyOnLoad(this);

        cameraDimension = new Vector2(0,0);
        initPrevCoords(widthSizeFactor, heightSizeFactor);

        maxMinCoordMap.Add("nose", new MaxMinCoord());
        maxMinCoordMap.Add("leftWrist", new MaxMinCoord());
        maxMinCoordMap.Add("rightWrist", new MaxMinCoord());
        maxMinCoordMap.Add("leftAnkle", new MaxMinCoord());
        maxMinCoordMap.Add("rightAnkle", new MaxMinCoord());
    }

    void Update()
    {
        if (lastPose != null)
        {   // act on last pose-event
            handleNodedMovement(lastPose.nose, nose, ref prevNoseCoord, "nose");
            handleNodedMovement(lastPose.leftEye, leftEye, ref prevLeftEyeCoord, "leftEye");
            handleNodedMovement(lastPose.rightEye, rightEye, ref prevRightEyeCoord, "rightEye");
            handleNodedMovement(lastPose.leftEar, leftEar, ref prevLeftEarCoord, "leftEar");
            handleNodedMovement(lastPose.rightEar, rightEar, ref prevRightEarCoord, "rightEar");
            handleNodedMovement(lastPose.leftShoulder, leftShoulder, ref prevLeftShoulderCoord, "leftShoulder");
            handleNodedMovement(lastPose.rightShoulder, rightShoulder, ref prevRightShoulderCoord, "rightShoulder");
            handleNodedMovement(lastPose.leftElbow, leftElbow, ref prevLeftElbowCoord, "leftElbow");
            handleNodedMovement(lastPose.rightElbow, rightElbow, ref prevRightElbowCoord, "rightElbow");
            handleNodedMovement(lastPose.leftWrist, leftWrist, ref prevLeftWristCoord, "leftWrist");
            handleNodedMovement(lastPose.rightWrist, rightWrist, ref prevRightWristCoord, "rightWrist");
            handleNodedMovement(lastPose.leftHip, leftHip, ref prevLeftHipCoord, "leftHip");
            handleNodedMovement(lastPose.rightHip, rightHip, ref prevRightHipCoord, "rightHip");
            handleNodedMovement(lastPose.leftKnee, leftKnee, ref prevLeftKneeCoord, "leftKnee");
            handleNodedMovement(lastPose.rightKnee, rightKnee, ref prevRightKneeCoord, "rightKnee");
            handleNodedMovement(lastPose.leftAnkle, leftAnkle, ref prevLeftAnkleCoord, "leftAnkle");
            handleNodedMovement(lastPose.rightAnkle, rightAnkle, ref prevRightAnkleCoord, "rightAnkle");

            PosePosition pelvisPose = new PosePosition();
            pelvisPose.x = (lastPose.rightHip.x + lastPose.leftHip.x)/2;
            pelvisPose.y = (lastPose.rightHip.y + lastPose.leftHip.y) / 2;
            PosePosition middleSpinePose = new PosePosition();
            middleSpinePose.x = (lastPose.rightHip.x + lastPose.leftShoulder.x) / 2;
            middleSpinePose.y = (lastPose.leftShoulder.y + lastPose.leftHip.y) / 2;
            handleNodedMovement(pelvisPose, pelvis, ref prevPelvisCoord, "Pelvis");
            handleNodedMovement(middleSpinePose, middleSpine, ref prevMiddleSpineCoord, "MiddleSpine");

            processedPose = lastPose;
            lastPose = null;
        } else
        {
//            Debug.Log("No lastPose present");
        }
    }

    /**
     * Act on node movement
     * Value can be [0 - 600]
     */
    private void handleNodedMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string desc)
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

//        Debug.Log(desc + " [x:" + posePos.x + ", y: " + posePos.y + "] -> [x: " + currentCoord.x + ", y: " + currentCoord.y + "]. (xOffset: " + xOffset + ", yOffset: " + yOffset + ")");

        previousCoord.x = currentCoord.x;
        previousCoord.y = currentCoord.y;
        // Add offset
//        currentCoord.x = currentCoord.x + xOffset;
//        currentCoord.y = currentCoord.y + yOffset;
        Debug.Log("Offset added, " + " Before: [x:" + previousCoord.x + ", y: " + previousCoord.y + "] -> After: [x: " + currentCoord.x + ", y: " + currentCoord.y + "]");

        Transform transform = node.transform;
        transform.localPosition = new Vector3(currentCoord.x, currentCoord.y, transform.localPosition.z);
    }

    /**
     * Converts screen percentage to actual Unity screen pixels.
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

    public void cameraDimensionsChanged(Vector3 viewBottomLeft, Vector3 viewTopRight, float viewWidth, float viewHeight)
    {
        Debug.Log("New camera dimensions received by PoseInputController  [ width: " + viewWidth + ", height: " + viewHeight + ")");
        widthSizeFactor = viewWidth;
        heightSizeFactor = viewHeight;
    }
}
