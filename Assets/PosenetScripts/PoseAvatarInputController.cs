using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * Uses Posenet input to control an avatar body parts.
 * It ajdusts the input data with the avatar body layout based on the given input
 * This script will try to control reference points for each body part. 
 * It will try to calculate the pose-value vs body size in the app ratio & location and then use this to place the reference points in the corrrect place.
 * 
 * It assumes the player will stand still for secsForAdjustment number of seconds for it to adjust the ground level & body size adjustments.
 * 
 */
public class PoseAvatarInputController : PoseEventHandler {

    [Tooltip("The adjustment vector for what (0,0) is in %")]
    private Vector2 zeroPointAdjustment;
    [Tooltip("The adjustment for going from % to game coordinates")]
    private float legScaleAdjustment = 0.02f;
    [Tooltip("The adjustment for going from % to game coordinates")]
    private float leftShoulderScaleAdjustment = 0.02f;
    [Tooltip("The adjustment for going from % to game coordinates")]
    private float rightShoulderScaleAdjustment = 0.02f;
    [Tooltip("The adjustment for going from % to game coordinates")]
    private float shoulderWidthScaleAdjustment = 0.02f;

    [Tooltip("The adjustment for the left hand related to the left shoulder")]
    private float leftHandScaleAdjustment = 0.02f;
    [Tooltip("The adjustment for the right hand related to the right shoulder")]
    private float rightHandScaleAdjustment = 0.02f;
    [Tooltip("The adjustment for the head related to the left shoulder")]
    private float headScaleAdjustment = 0.02f;

    [Header("% to coordinate factors")]
    [Tooltip("The leg length in game coordinates")]
    public float legLength = 1.155f;
    [Tooltip("The arm length in game coordinates")]
    public float armLength = 0.76f;
    [Tooltip("The body length in game coordinates (between pelvis & right/left shoulder)")]
    public float bodyLength = 0.626f;
    [Tooltip("The neck length in game coordinates (between nose & right/left shoulder)")]
    public float neckLength = 0.277f;
    [Tooltip("The shoulder width in game coordinates (between right & left shoulder)")]
    public float shoulderWidth = 0.34f;

    [Tooltip("# Secs for initial factor adjustment")]
    public float secsForAdjustment = 5;

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

    // The current position of each pose point
    private Vector2 currentPelvisPos = new Vector2();
    private Vector2 currentLeftHandPos = new Vector2();
    private Vector2 currentRightHandPos = new Vector2();
    private Vector2 currentLeftFootPos = new Vector2();
    private Vector2 currentRightFootPos = new Vector2();
    private Vector2 currentLeftShoulderPos = new Vector2();
    private Vector2 currentRightShoulderPos = new Vector2();
    private Vector2 currentHeadPos = new Vector2();

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
        Vector2 sum = new Vector2(0, 0);
        foreach (PosePosition pp in adjustmentMap[pelvisStr]) {
            sum.y += pp.y;
            sum.x += pp.x;
        }
        Vector2 hipVector = new Vector2();
        hipVector.x = (sum.x / adjustmentMap[pelvisStr].Count);
        hipVector.y = (sum.y / adjustmentMap[pelvisStr].Count);

        // Handle left foot
        Vector2 leftFootvector = getAverageAdjustmentValue(leftFootStr);

        // Handle right foot
        Vector2 rightFootvector = getAverageAdjustmentValue(rightFootStr);

        // Handle shoulders
        Vector2 leftShoulderVector = getAverageAdjustmentValue(leftShoulderStr);
        Vector2 rightShoulderVector = getAverageAdjustmentValue(rightShoulderStr);

        // Handle hands
        Vector2 leftWristVector = getAverageAdjustmentValue(leftHandStr);
        Vector2 rightWristVector = getAverageAdjustmentValue(rightHandStr);

        // Handle head
        Vector2 headVector = getAverageAdjustmentValue(headStr);

        /* Debug code
        hipVector = new Vector2(63.8f, 58.4f);
        leftFootvector = new Vector2(63.5f, 6.9f);
        rightFootvector = new Vector2(64.8f, 6.9f);
        leftShoulderVector = new Vector2(62.8f, 88.6f);
        rightShoulderVector = new Vector2(63.5f, 88.6f);
        leftWristVector = new Vector2(65.8f, 65.4f);
        rightWristVector = new Vector2(72.5f, 64.8f);
        headVector = new Vector2(60.6f, 97.7f);
        / End - Debug code */

        // Calculate adjustments
        zeroPointAdjustment.x = hipVector.x;
        zeroPointAdjustment.y = (leftFootvector.y + rightFootvector.y) / 2; // Take average from left & right foot

        legScaleAdjustment = legLength / (hipVector - zeroPointAdjustment).magnitude;

        leftShoulderScaleAdjustment = bodyLength / (leftShoulderVector - hipVector).magnitude;
        rightShoulderScaleAdjustment = bodyLength / (rightShoulderVector - hipVector).magnitude;

        leftHandScaleAdjustment = armLength / (leftWristVector - leftShoulderVector).magnitude;
        rightHandScaleAdjustment = armLength / (rightWristVector - rightShoulderVector).magnitude;

        headScaleAdjustment = neckLength / (headVector - leftShoulderVector).magnitude;

        shoulderWidthScaleAdjustment = shoulderWidth / (leftShoulderVector - rightShoulderVector).magnitude;


        Debug.Log("Adjustment ended");
        Debug.Log("Adjustment base data: hipVector: " + hipVector + ", leftFootvector = " + leftFootvector + ", rightFootvector: " + rightFootvector + ", # of adjustment entries: " + adjustmentMap[pelvisStr].Count);
        Debug.Log("Adjustment base data: leftShoulderVector: " + leftShoulderVector + ", rightShoulderVector = " + rightShoulderVector + ", headVector: " + headVector);
        Debug.Log("Adjustment base data: leftWristVector: " + leftWristVector + ", rightWristVector = " + rightWristVector);
        Debug.Log("Adjustment adaptions: zeroPointAdjustment: " + zeroPointAdjustment);
        Debug.Log("Adjustment adaptions: legScaleAdjustment = (" + legScaleAdjustment + ")");
        Debug.Log("Adjustment adaptions: leftShoulderScaleAdjustment = (" + leftShoulderScaleAdjustment + ")");
        Debug.Log("Adjustment adaptions: rightShoulderScaleAdjustment = (" + rightShoulderScaleAdjustment + ")");
        Debug.Log("Adjustment adaptions: leftHandScaleAdjustment = (" + leftHandScaleAdjustment + ")");
        Debug.Log("Adjustment adaptions: rightHandScaleAdjustment = (" + rightHandScaleAdjustment + ")");
        Debug.Log("Adjustment adaptions: headScaleAdjustment = (" + headScaleAdjustment + ")");
        Vector2 hipV = (hipVector - zeroPointAdjustment) * legScaleAdjustment;
        Debug.Log("Adjustment Test: hip = (" + hipV.x + ", " + hipV.y + ")");
    }

    private Vector2 getAverageAdjustmentValue(string bodyPartStr) {
        Vector2 sum = new Vector2(0, 0);
        foreach (PosePosition pp in adjustmentMap[bodyPartStr]) {
            sum.y += pp.y;
            sum.x += pp.x;
        }
        Vector2 averageVector = new Vector2(sum.x / adjustmentMap[bodyPartStr].Count, sum.y / adjustmentMap[bodyPartStr].Count);
        return averageVector;
    }

    /**
     * Reading adjustment information
     */
    private void readAdjustmentInfo() {
        // Still adjusting
        if (lastPose.pelvisPose != null) {
            adjustmentMap[pelvisStr].Add(lastPose.pelvisPose);
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
        if (lastPose.rightShoulder != null) {
            adjustmentMap[rightShoulderStr].Add(lastPose.rightShoulder);
        } else {
            Debug.Log("lastPose.rightShoulder == null !!!");
        }
        if (lastPose.leftShoulder != null) {
            adjustmentMap[leftShoulderStr].Add(lastPose.leftShoulder);
        } else {
            Debug.Log("lastPose.leftShoulder == null !!!");
        }
        if (lastPose.rightWrist != null) {
            adjustmentMap[rightHandStr].Add(lastPose.rightWrist);
        } else {
            Debug.Log("lastPose.rightWrist == null !!!");
        }
        if (lastPose.leftWrist != null) {
            adjustmentMap[leftHandStr].Add(lastPose.leftWrist);
        } else {
            Debug.Log("lastPose.leftWrist == null !!!");
        }
        if (lastPose.nose != null) {
            adjustmentMap[headStr].Add(lastPose.nose);
        } else {
            Debug.Log("lastPose.nose == null !!!");
        }
    }

    private void handleNewPoseEvent(BodyPositionState pose) {
//        calculateFloorLevel(pose);

        handleNodeMovement(lastPose.pelvisPose, pelvis, ref prevPelvisCoord, "Pelvis", ref currentPelvisPos);
//        handleNodeMovement(middleSpinePose, middleSpine, ref prevMiddleSpineCoord, "MiddleSpine");

        handleLeftShoulderNodeMovement(lastPose.leftShoulder, leftShoulder, ref prevLeftShoulderCoord, "leftShoulder");
        handleRightShoulderNodeMovement(lastPose.rightShoulder, rightShoulder, ref prevRightShoulderCoord, "rightShoulder");
        handleNodeMovement(lastPose.nose, nose, ref prevNoseCoord, "nose", ref currentHeadPos);
//        handleNodeMovement(lastPose.leftEye, leftEye, ref prevLeftEyeCoord, "leftEye");
//        handleNodeMovement(lastPose.rightEye, rightEye, ref prevRightEyeCoord, "rightEye");
        //handleNodeMovement(lastPose.leftEar, leftEar, ref prevLeftEarCoord, "leftEar");
//        handleNodeMovement(lastPose.rightEar, rightEar, ref prevRightEarCoord, "rightEar");
//        handleNodeMovement(lastPose.leftElbow, leftElbow, ref prevLeftElbowCoord, "leftElbow");
//        handleNodeMovement(lastPose.rightElbow, rightElbow, ref prevRightElbowCoord, "rightElbow");
        handleLeftHandNodeMovement(lastPose.leftWrist, leftWrist, ref prevLeftWristCoord, "leftWrist");
        handleRightHandNodeMovement(lastPose.rightWrist, rightWrist, ref prevRightWristCoord, "rightWrist");
//        handleNodeMovement(lastPose.leftHip, leftHip, ref prevLeftHipCoord, "leftHip");
//        handleNodeMovement(lastPose.rightHip, rightHip, ref prevRightHipCoord, "rightHip");
//        handleNodeMovement(lastPose.leftKnee, leftKnee, ref prevLeftKneeCoord, "leftKnee");
//        handleNodeMovement(lastPose.rightKnee, rightKnee, ref prevRightKneeCoord, "rightKnee");
        handleNodeMovement(lastPose.leftAnkle, leftAnkle, ref prevLeftAnkleCoord, "leftAnkle", ref currentLeftFootPos);
        handleNodeMovement(lastPose.rightAnkle, rightAnkle, ref prevRightAnkleCoord, "rightAnkle", ref currentRightFootPos);
    }

    private void calculateFloorLevel(PoseEvent pose) {
        zeroPointAdjustment.x = lastPose.pelvisPose.x;
        if (zeroPointAdjustment.y > pose.leftAnkle.y) {
            zeroPointAdjustment.y = pose.leftAnkle.y;
            Debug.Log("zeroPointAdjustment.y = " + zeroPointAdjustment.y);
        }
        if (zeroPointAdjustment.y > pose.rightAnkle.y) {
            zeroPointAdjustment.y = pose.rightAnkle.y;
            Debug.Log("zeroPointAdjustment.y = " + zeroPointAdjustment.y);
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
    private void handleNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string nodeName, ref Vector2 currentPos) {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        // Try to remove gittering in positions due to invalid points by smoothening
        previousCoord = smoothenMovement(posePos, previousCoord);
        // Convert from percentage value to game coordinates & adjust for screen center not being zero in input
        currentPos = (previousCoord - zeroPointAdjustment) * legScaleAdjustment;

        Debug.Log("New pos for '" + nodeName + "', " + " Pose: " + posePos + " -> After: " + currentPos);
        //        Debug.Log("floorPercentageLevel: " + floorPercentageLevel + ", xAvgFactor: " + xAvgFactor);

        // Set new position on node
        Transform transform = node.transform;
        transform.localPosition = new Vector3(currentPos.x, currentPos.y, transform.localPosition.z);
    }

    /**
     * Act on node movement
     * posPos values are in %
     * param name="posePos" The current position in %
     * param name="node"    The node this position is for
     * param name="previousCoord" The previous coordinate
     */
    private void handleLeftShoulderNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string nodeName) {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        // Try to remove gittering in positions due to invalid points by smoothening
        previousCoord = smoothenMovement(posePos, previousCoord);
        // Convert from percentage value to game coordinates & adjust for screen center not being zero in input
        currentLeftShoulderPos.x = (previousCoord.x - prevPelvisCoord.x) * shoulderWidthScaleAdjustment / 2 + currentPelvisPos.x;
        currentLeftShoulderPos.y = (previousCoord.y - prevPelvisCoord.y) * leftShoulderScaleAdjustment + currentPelvisPos.y;

        Debug.Log("New pos for '" + nodeName + "', " + " Pose: " + posePos + " -> After: " + currentLeftShoulderPos);
        Debug.Log("previousCoord: " + previousCoord + ", currentLeftShoulderPos: " + currentLeftShoulderPos);

        // Set new position on node
        Transform transform = node.transform;
        transform.localPosition = new Vector3(currentLeftShoulderPos.x, currentLeftShoulderPos.y, transform.localPosition.z);
    }

    /**
 * Act on node movement
 * posPos values are in %
 * param name="posePos" The current position in %
 * param name="node"    The node this position is for
 * param name="previousCoord" The previous coordinate
 */
    private void handleRightShoulderNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string nodeName) {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        // Try to remove gittering in positions due to invalid points by smoothening
        previousCoord = smoothenMovement(posePos, previousCoord);
        // Convert from percentage value to game coordinates & adjust for screen center not being zero in input
        currentRightShoulderPos.x = (previousCoord.x - prevPelvisCoord.x) * shoulderWidthScaleAdjustment/2 + currentPelvisPos.x;
        currentRightShoulderPos.y = (previousCoord.y - prevPelvisCoord.y) * rightShoulderScaleAdjustment + currentPelvisPos.y;

        Debug.Log("New pos for '" + nodeName + "', " + " Pose: " + posePos + " -> After: " + currentRightShoulderPos);
        //        Debug.Log("floorPercentageLevel: " + floorPercentageLevel + ", xAvgFactor: " + xAvgFactor);

        // Set new position on node
        Transform transform = node.transform;
        transform.localPosition = new Vector3(currentRightShoulderPos.x, currentRightShoulderPos.y, transform.localPosition.z);
    }

    /**
     * Act on node movement
     * posPos values are in %
     * param name="posePos" The current position in %
     * param name="node"    The node this position is for
     * param name="previousCoord" The previous coordinate
     */
    private void handleLeftHandNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string nodeName) {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        // Try to remove gittering in positions due to invalid points by smoothening
        previousCoord = smoothenMovement(posePos, previousCoord);
        // Convert from percentage value to game coordinates & adjust for screen center not being zero in input
        Vector2 currentCoord = (previousCoord - prevLeftShoulderCoord) * leftHandScaleAdjustment + currentLeftShoulderPos;

        Debug.Log("New pos for '" + nodeName + "', " + " Pose: " + posePos + " -> After: " + currentCoord);
        Debug.Log("previousCoord: " + previousCoord + ", prevLeftShoulderCoord: " + prevLeftShoulderCoord);
        Debug.Log("leftHandScaleAdjustment: " + leftHandScaleAdjustment + ", currentLeftShoulderPos: " + currentLeftShoulderPos);

        // Set new position on node
        Transform transform = node.transform;
        transform.localPosition = new Vector3(currentCoord.x, currentCoord.y, transform.localPosition.z);
    }

    /**
     * Act on node movement
     * posPos values are in %
     * param name="posePos" The current position in %
     * param name="node"    The node this position is for
     * param name="previousCoord" The previous coordinate
     */
    private void handleRightHandNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string nodeName) {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        // Try to remove gittering in positions due to invalid points by smoothening
        previousCoord = smoothenMovement(posePos, previousCoord);
        // Convert from percentage value to game coordinates & adjust for screen center not being zero in input
        Vector2 currentCoord = (previousCoord - prevRightShoulderCoord) * rightHandScaleAdjustment + currentRightShoulderPos;

        Debug.Log("New pos for '" + nodeName + "', " + " Pose: " + posePos + " -> After: " + currentCoord);
        Debug.Log("previousCoord: " + previousCoord + ", prevRightShoulderCoord: " + prevRightShoulderCoord);
        Debug.Log("rightHandScaleAdjustment: " + rightHandScaleAdjustment + ", currentRightShoulderPos: " + currentRightShoulderPos);

        // Set new position on node
        Transform transform = node.transform;
        transform.localPosition = new Vector3(currentCoord.x, currentCoord.y, transform.localPosition.z);
    }

    /**
     * Act on node movement
     * posPos values are in %
     * param name="posePos" The current position in %
     * param name="node"    The node this position is for
     * param name="previousCoord" The previous coordinate
     */
    private void handleHeadNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string nodeName) {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        // Try to remove gittering in positions due to invalid points by smoothening
        previousCoord = smoothenMovement(posePos, previousCoord);
        // Convert from percentage value to game coordinates & adjust for screen center not being zero in input
        Vector2 currentCoord = (previousCoord - prevLeftShoulderCoord) * headScaleAdjustment + currentLeftShoulderPos;

        Debug.Log("New pos for '" + nodeName + "', " + " Pose: " + posePos + " -> After: " + currentCoord);

        // Set new position on node
        Transform transform = node.transform;
        transform.localPosition = new Vector3(currentCoord.x, currentCoord.y, transform.localPosition.z);
    }
}
