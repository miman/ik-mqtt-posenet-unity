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
    public Vector2 legScaleAdjustment = new Vector2(2, 2);
    [Tooltip("The adjustment for going from % to game coordinates")]
    public Vector2 leftShoulderScaleAdjustment = new Vector2(2, 2);
    [Tooltip("The adjustment for going from % to game coordinates")]
    public Vector2 rightShoulderScaleAdjustment = new Vector2(2, 2);
    
    [Tooltip("The adjustment for the left hand related to the left shoulder")]
    public Vector2 leftHandScaleAdjustment = new Vector2(2, 2);
    [Tooltip("The adjustment for the right hand related to the right shoulder")]
    public Vector2 rightHandScaleAdjustment = new Vector2(2, 2);
    [Tooltip("The adjustment for the head related to the left shoulder")]
    public Vector2 headScaleAdjustment = new Vector2(2, 2);

    [Tooltip("The floor level in posenet Y-%")]
    public float floorLevel = 0;

    [Header("% to coordinate factors")]
    [Tooltip("The leg length in game coordinates")]
    public float legLength = 1.155f;
    [Tooltip("The arm length in game coordinates")]
    public float armLength = 0.76f;
    [Tooltip("The body length in game coordinates (between pelvis & right/left shoulder)")]
    public float bodyLength = 0.626f;
    [Tooltip("The neck length in game coordinates (between nose & right/left shoulder)")]
    public float neckLength = 0.277f;

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
        Vector2 sum = new Vector2(0, 0);
        foreach (PosePosition pp in adjustmentMap[pelvisStr]) {
            sum.y += pp.y;
            sum.x += pp.x;
        }
        Vector2 hipVector = new Vector2();
        hipVector.x = (sum.x / adjustmentMap[pelvisStr].Count);
        hipVector.y = (sum.y / adjustmentMap[pelvisStr].Count);
        zeroPointAdjustment.x = hipVector.x;

        // Handle left foot
        Vector2 leftFootvector = getAverageAdjustmentValue(leftFootStr);
        zeroPointAdjustment.y = leftFootvector.y;

        // Handle right foot
        Vector2 rightFootvector = getAverageAdjustmentValue(rightFootStr);
        floorLevel = (floorLevel + rightFootvector.y) / 2;  // Take average from left & right foot
        zeroPointAdjustment.y = (zeroPointAdjustment.y + rightFootvector.y) / 2; // Take average from left & right foot

        legScaleAdjustment.y = legLength / (hipVector.y - zeroPointAdjustment.y);

        // Handle shoulders
        Vector2 leftShoulderVector = getAverageAdjustmentValue(leftShoulderStr);
        Vector2 rightShoulderVector = getAverageAdjustmentValue(rightShoulderStr);
        float bodyScale = bodyLength / (leftShoulderVector - hipVector).magnitude;
        leftShoulderScaleAdjustment = bodyScale * (leftShoulderVector - hipVector);
        rightShoulderScaleAdjustment = bodyScale * (rightShoulderVector - hipVector);

        // Handle hands
        Vector2 leftWristVector = getAverageAdjustmentValue(leftHandStr);
        Vector2 rightWristVector = getAverageAdjustmentValue(rightHandStr);
        float armScale = armLength / (leftWristVector - leftShoulderVector).magnitude;
        leftHandScaleAdjustment = armScale * (leftShoulderVector - leftWristVector);
        rightHandScaleAdjustment = armScale * (rightShoulderVector - rightWristVector);

        // Handle head
        Vector2 headVector = getAverageAdjustmentValue(headStr);
        float headScale = neckLength / (headVector - leftShoulderVector).magnitude;
        headScaleAdjustment = headScale * (headVector - leftShoulderVector);

        Debug.Log("Adjustment ended");
        Debug.Log("Adjustment base data: hipVector: " + hipVector + ", leftFootvector = " + leftFootvector + ", rightFootvector: " + rightFootvector + ", # of adjustment entries: " + adjustmentMap[pelvisStr].Count);
        Debug.Log("Adjustment base data: leftShoulderVector: " + leftShoulderVector + ", rightShoulderVector = " + rightShoulderVector + ", headVector: " + headVector);
        Debug.Log("Adjustment base data: leftWristVector: " + leftWristVector + ", rightWristVector = " + rightWristVector);
        Debug.Log("Adjustment adaptions: zeroPointAdjustment: " + zeroPointAdjustment);
        Debug.Log("Adjustment adaptions: legScaleAdjustment = (" + legScaleAdjustment.x + ", " + legScaleAdjustment.y + ")");
        Debug.Log("Adjustment adaptions: leftShoulderScaleAdjustment = (" + leftShoulderScaleAdjustment.x + ", " + leftShoulderScaleAdjustment.y + ")");
        Debug.Log("Adjustment adaptions: rightShoulderScaleAdjustment = (" + rightShoulderScaleAdjustment.x + ", " + rightShoulderScaleAdjustment.y + ")");
        Debug.Log("Adjustment adaptions: leftHandScaleAdjustment = (" + leftHandScaleAdjustment.x + ", " + leftHandScaleAdjustment.y + ")");
        Debug.Log("Adjustment adaptions: rightHandScaleAdjustment = (" + rightHandScaleAdjustment.x + ", " + rightHandScaleAdjustment.y + ")");
        Debug.Log("Adjustment adaptions: headScaleAdjustment = (" + headScaleAdjustment.x + ", " + headScaleAdjustment.y + ")");
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

    private void handleNewPoseEvent(PoseEvent pose) {
        calculateFloorLevel(pose);

        handleNodeMovement(pelvisPose, pelvis, ref prevPelvisCoord, "Pelvis", ref currentPelvisPos);
//        handleNodeMovement(middleSpinePose, middleSpine, ref prevMiddleSpineCoord, "MiddleSpine");

        handleLeftShoulderNodeMovement(lastPose.leftShoulder, leftShoulder, ref prevLeftShoulderCoord, "leftShoulder");
        handleLeftShoulderNodeMovement(lastPose.rightShoulder, rightShoulder, ref prevRightShoulderCoord, "rightShoulder");
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
        Vector2 currentCoord = (previousCoord - prevPelvisCoord) * leftShoulderScaleAdjustment + currentPelvisPos;
        currentLeftShoulderPos.x = currentCoord.x;
        currentLeftShoulderPos.y = currentCoord.y;

        Debug.Log("New pos for '" + nodeName + "', " + " Pose: " + posePos + " -> After: " + currentCoord);
        Debug.Log("previousCoord: " + previousCoord + ", currentLeftShoulderPos: " + currentLeftShoulderPos);

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
    private void handleRightShoulderNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string nodeName) {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        // Try to remove gittering in positions due to invalid points by smoothening
        previousCoord = smoothenMovement(posePos, previousCoord);
        // Convert from percentage value to game coordinates & adjust for screen center not being zero in input
        Vector2 currentCoord = (previousCoord - prevPelvisCoord) * rightShoulderScaleAdjustment + currentPelvisPos;
        currentRightShoulderPos.x = currentCoord.x;
        currentRightShoulderPos.y = currentCoord.y;

        Debug.Log("New pos for '" + nodeName + "', " + " Pose: " + posePos + " -> After: " + currentCoord);
        //        Debug.Log("floorPercentageLevel: " + floorPercentageLevel + ", xAvgFactor: " + xAvgFactor);

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
