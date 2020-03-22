using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * Uses Posenet input to send control commands to the game
 */
public class PoseAvatarInputController : PoseEventHandler {

    [Header("% to coordinate factors")]

    [Tooltip("The adjustment vector for what (0,0) is in %")]
    public Vector2 zeroPointAdjustment;
    [Tooltip("The adjustment for going from % to game coordinates")]
    public Vector2 scaleAdjustment = new Vector2(2, 2);

    [Tooltip("The factor for going from % to game coord for Shoulder in relation to Hip")]
    public float hSFactor = 2;
    [Tooltip("The factor for going from % to game coord for Left Hand in relation to Shoulder")]
    public float sLhFactor = 2;
    [Tooltip("The factor for going from % to game coord for Right Hand in relation to Shoulder")]
    public float sRhFactor = 2;
    [Tooltip("The factor for going from % to game coord for Head in relation to Shoulder")]
    public float sHFactor = 2;

    [Tooltip("The average value for going from % to game X coordinates")]
    public float xAvgFactor = 5;
    [Tooltip("The center value for going from % to game X coordinates")]
    public float centerFactor = 5;
    [Tooltip("The floor level in posenet Y-%")]
    public float floorLevel = 0;

    [Header("% to coordinate factors")]
    [Tooltip("The leg length in game coordinates")]
    public float legLength = 1;
    [Tooltip("The arm length in game coordinates")]
    public float armLength = 1;

    [Tooltip("# Secs for initial factor adjustment")]
    public float secsForAdjustment = 5;

    [Tooltip("Avatar")]
    public GameObject avatar;

    /**
     * This is the percentage value received from PoseNet that we say is the floor level
     */
    private float floorPercentageLevel = 1;
    private DateTime adjustmentEndTime;
    private bool adjusting = true;

    private Dictionary<string, List<PosePosition>> adjustmentMap = new Dictionary<string, List<PosePosition>>();
    private string pelvisStr = "Pelvis";
    private string leftHandStr = "Left Hand";
    private string rightHandStr = "Right Hand";
    private string leftFootStr = "Left Foot";
    private string rightFootStr = "Right Foot";
    private string headStr = "Head";
    private string leftShoulderStr = "Left Shoulder";
    private string rightShoulderStr = "Right Shoulder";

    /**
     * To keep track of max min values of tracking.
     */
    private Dictionary<string, MaxMinCoord> maxMinCoordMap = new Dictionary<string, MaxMinCoord>();

    public void Start() {
        DontDestroyOnLoad(this);

        adjustmentMap.Add(pelvisStr, new List<PosePosition>());
        adjustmentMap.Add(leftHandStr, new List<PosePosition>());
        adjustmentMap.Add(rightHandStr, new List<PosePosition>());
        adjustmentMap.Add(leftFootStr, new List<PosePosition>());
        adjustmentMap.Add(rightFootStr, new List<PosePosition>());
        adjustmentMap.Add(leftShoulderStr, new List<PosePosition>());
        adjustmentMap.Add(rightShoulderStr, new List<PosePosition>());
        adjustmentMap.Add(headStr, new List<PosePosition>());

        adjustmentEndTime = System.DateTime.Now;
        adjustmentEndTime = adjustmentEndTime.AddSeconds(secsForAdjustment);

        initPrevCoords(0, 0);
    }

    void Update() {
        if (lastPose != null) {
            calculateCalculatedNodes(lastPose);
        }
        if (adjusting) {
            if (System.DateTime.Now.CompareTo(adjustmentEndTime) > 0) {
                // Adjustment time has ended
                handleAdjustmentInfo();

                adjusting = false;
            } else {
                Debug.Log("Adjusting...");
                readAdjustmentInfo();
                return; // Adjustment handled
            }
        }
        if (lastPose != null) {   // act on last pose-event
            handleNewPoseEvent(lastPose);

            processedPose = lastPose;
            lastPose = null;
        } else {
            //            Debug.Log("No lastPose present");
        }
    }

    /**
     * Using the adjustment info to prepare the system variables needed for operation
     */
    private void handleAdjustmentInfo() {
        // Calculate factors
        Vector2 sum = new Vector2();
        sum.y = 0;
        sum.x = 0;
        foreach (PosePosition pp in adjustmentMap[pelvisStr]) {
            sum.y += pp.y;
            sum.x += pp.x;
        }
        Vector2 hipVector = new Vector2();
        hipVector.x = (sum.x / adjustmentMap[pelvisStr].Count);
        hipVector.y = (sum.y / adjustmentMap[pelvisStr].Count);
        zeroPointAdjustment.x = hipVector.x;

        // Handle left foot
        sum.y = 0;
        sum.x = 0;
        foreach (PosePosition pp in adjustmentMap[leftFootStr]) {
            sum.y += pp.y;
            sum.x += pp.x;
        }
        Vector2 leftFootvector = new Vector2(sum.x / adjustmentMap[leftFootStr].Count, sum.y / adjustmentMap[leftFootStr].Count);
        zeroPointAdjustment.y = leftFootvector.y;

        sum.y = 0;
        sum.x = 0;
        foreach (PosePosition pp in adjustmentMap[rightFootStr]) {
            sum.y += pp.y;
            sum.x += pp.x;
        }
        Vector2 rightFootvector = new Vector2(sum.x / adjustmentMap[rightFootStr].Count, sum.y / adjustmentMap[rightFootStr].Count);
        floorLevel = (floorLevel + rightFootvector.y) / 2;  // Take average from left & right foot
        zeroPointAdjustment.y = (zeroPointAdjustment.y + rightFootvector.y) / 2; // Take average from left & right foot

        scaleAdjustment.y = legLength / (hipVector.y - zeroPointAdjustment.y);

        Debug.Log("Adjustment ended");
        Debug.Log("Adjustment base data: hipVector: " + hipVector + ", leftFootvector = " + leftFootvector + ", rightFootvector: " + rightFootvector + ", # of adjustment entries: " + adjustmentMap[pelvisStr].Count);
        Debug.Log("Adjustment adaptions: zeroPointAdjustment: " + zeroPointAdjustment + ", scaleAdjustment = (" + scaleAdjustment.x + ", " + scaleAdjustment.y + ")");
        Vector2 hipV = (hipVector - zeroPointAdjustment) * scaleAdjustment;
        Debug.Log("Adjustment Test: hip = (" + hipV.x + ", " + hipV.y + ")");
    }

    /**
     * Reading adjustment information
     */
    private void readAdjustmentInfo() {
        // Still adjusting
        if (pelvisPose != null) {
            adjustmentMap[pelvisStr].Add(pelvisPose);
        }

        if (lastPose.leftAnkle != null) {
            adjustmentMap[leftFootStr].Add(lastPose.leftAnkle);
        } else {
            Debug.Log("lastPose.leftAnkle == null !!!");
        }
        if (lastPose.rightAnkle != null) {
            adjustmentMap[rightFootStr].Add(lastPose.rightAnkle);
        } else {
            Debug.Log("lastPose.rightAnkle == null !!!");
        }
    }

    private void handleNewPoseEvent(PoseEvent pose) {
        calculateFloorLevel(pose);

        handleNodeMovement(pelvisPose, pelvis, ref prevPelvisCoord, "Pelvis");
        handleNodeMovement(middleSpinePose, middleSpine, ref prevMiddleSpineCoord, "MiddleSpine");

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
    }

    private void calculateFloorLevel(PoseEvent pose) {
        if (floorPercentageLevel > pose.leftAnkle.y) {
            floorPercentageLevel = pose.leftAnkle.y;
            Debug.Log("floorPercentageLevel = " + floorPercentageLevel);
        }
        if (floorPercentageLevel > pose.rightAnkle.y) {
            floorPercentageLevel = pose.rightAnkle.y;
            Debug.Log("floorPercentageLevel = " + floorPercentageLevel);
        }
    }

    // Smoothen the change in controlled GameObject
    private Vector2 smoothenMovement(PosePosition posePos, Vector2 previousCoord) {
        Vector2 newCoord = new Vector2(0,0);
        float delta = posePos.x - previousCoord.x;
        newCoord.x = previousCoord.x + (delta / smoothening);
        delta = posePos.y - previousCoord.y;
        newCoord.y = previousCoord.y + (delta / smoothening);
        //        Debug.Log("New pos for '" + desc + "', " + " Before: [x:" + previousCoord.x + ", y: " + previousCoord.y + "] -> After: [x: " + currentCoord.x + ", y: " + currentCoord.y + "]");

        return newCoord;
    }

    /**
     * Act on node movement
     * posPos values are in %
     * param name="posePos" The current position in %
     * param name="node"    The node this position is for
     * param name="previousCoord" The previous coordinate
     */
    private void handleNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string nodeName) {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        // Try to remove gittering in positions due to invalid points by smoothening
        previousCoord = smoothenMovement(posePos, previousCoord);
        // Convert from percentage value to game coordinates & adjust for screen center not being zero in input
        Vector2 currentCoord = (previousCoord - zeroPointAdjustment) * scaleAdjustment;

        Debug.Log("New pos for '" + nodeName + "', " + " Pose: " + posePos + " -> After: " + currentCoord);
        //        Debug.Log("floorPercentageLevel: " + floorPercentageLevel + ", xAvgFactor: " + xAvgFactor);

        // Set new position on node
        Transform transform = node.transform;
        transform.localPosition = new Vector3(currentCoord.x, currentCoord.y, transform.localPosition.z);
    }
}
