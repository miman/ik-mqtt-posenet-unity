using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * The PoseCoreEventManager is the event manager that will receive new pose info from the proxies and route this to event handlers.
 * This cannot be used directly in the GUI editor, for this you need to use the PoseEventHandler
 */
public class PoseCoreEventManager : MonoBehaviour {
    public delegate void PoseEventAction(BodyPositionState pose);
    public static event PoseEventAction onPoseEventReceived;

    public delegate void NosePoseEventAction(PosePosition pos);
    public static event NosePoseEventAction onNosePoseEventReceived;
    public delegate void LeftEyePoseEventAction(PosePosition pos);
    public static event LeftEyePoseEventAction onLeftEyePoseEventReceived;
    public delegate void RightEyePoseEventAction(PosePosition pos);
    public static event RightEyePoseEventAction onRightEyePoseEventReceived;
    public delegate void LeftEarPoseEventAction(PosePosition pos);
    public static event LeftEarPoseEventAction onLeftEarPoseEventReceived;
    public delegate void RightEarPoseEventAction(PosePosition pos);
    public static event RightEarPoseEventAction onRightEarPoseEventReceived;
    public delegate void LeftShoulderPoseEventAction(PosePosition pos);
    public static event LeftShoulderPoseEventAction onLeftShoulderPoseEventReceived;
    public delegate void RightShoulderPoseEventAction(PosePosition pos);
    public static event RightShoulderPoseEventAction onRightShoulderPoseEventReceived;
    public delegate void LeftElbowPoseEventAction(PosePosition pos);
    public static event LeftElbowPoseEventAction onLeftElbowPoseEventReceived;
    public delegate void RightElbowPoseEventAction(PosePosition pos);
    public static event RightElbowPoseEventAction onRightElbowPoseEventReceived;
    public delegate void LeftWristPoseEventAction(PosePosition pos);
    public static event LeftWristPoseEventAction onLeftWristPoseEventReceived;
    public delegate void RightWristPoseEventAction(PosePosition pos);
    public static event RightWristPoseEventAction onRightWristPoseEventReceived;
    public delegate void LeftHipPoseEventAction(PosePosition pos);
    public static event LeftHipPoseEventAction onLeftHipPoseEventReceived;
    public delegate void RightHipPoseEventAction(PosePosition pos);
    public static event RightHipPoseEventAction onRightHipPoseEventReceived;
    public delegate void LeftKneePoseEventAction(PosePosition pos);
    public static event LeftKneePoseEventAction onLeftKneePoseEventReceived;
    public delegate void RightKneePoseEventAction(PosePosition pos);
    public static event RightKneePoseEventAction onRightKneePoseEventReceived;
    public delegate void LeftFootPoseEventAction(PosePosition pos);
    public static event LeftFootPoseEventAction onLeftFootPoseEventReceived;
    public delegate void RightFootPoseEventAction(PosePosition pos);
    public static event RightFootPoseEventAction onRightFootPoseEventReceived;

    public delegate void RootPoseEventAction(PosePosition pos);
    public static event RootPoseEventAction onRootPoseEventReceived;
    public delegate void MiddleSpinePoseEventAction(PosePosition pos);
    public static event MiddleSpinePoseEventAction onMiddleSpinePoseEventReceived;

    public delegate void InitialPoseAction(BodyPositionState pose);
    public static event InitialPoseAction onInitialPoseCalculated;

    /**
    * Last received posenet event, that is unprocessed
    */
    protected BodyPositionState lastPose = new BodyPositionState();

    [Tooltip("# Secs for initial factor adjustment")]
    public float secsForAdjustment = 5;
    [Tooltip("# Secs before starting the adjustment")]
    public float adjustmentDelay = 3;

    /**
     * This is the percentage value received from PoseNet that we say is the floor level
     */
    private float floorPercentageLevel = 1;
    private DateTime adjustmentEndTime;
    private DateTime adjustmentStartTime;
    private bool adjusting = true;

    private Dictionary<string, List<PosePosition>> adjustmentMap = new Dictionary<string, List<PosePosition>>();
    private string rootStr = "Root";
    private string leftHandStr = "Left Hand";
    private string rightHandStr = "Right Hand";
    private string leftFootStr = "Left Foot";
    private string rightFootStr = "Right Foot";
    private string headStr = "Head";
    private string leftShoulderStr = "Left Shoulder";
    private string rightShoulderStr = "Right Shoulder";

    /**
     * A new posenet event was received
     */
    public void HandlePoseEvent(PoseEvent pose) {
//        Debug.Log("PoseEvent handled by PoseCoreEventManager: " + pose);
        baseHandlingOfPoseEvent(pose);

        if (onPoseEventReceived != null) {
            // Inform all handlers of this event
            onPoseEventReceived(lastPose);
        }

        if (onNosePoseEventReceived != null) {
            onNosePoseEventReceived(pose.nose);
        }
        if (onLeftEyePoseEventReceived != null) {
            onLeftEyePoseEventReceived(pose.leftEye);
        }
        if (onRightEyePoseEventReceived != null) {
            onRightEyePoseEventReceived(pose.rightEye);
        }

        if (onLeftEarPoseEventReceived != null) {
            onLeftEarPoseEventReceived(pose.leftEar);
        }
        if (onRightEarPoseEventReceived != null) {
            onRightEarPoseEventReceived(pose.rightEar);
        }
        if (onLeftShoulderPoseEventReceived != null) {
            onLeftShoulderPoseEventReceived(pose.leftShoulder);
        }
        if (onRightShoulderPoseEventReceived != null) {
            onRightShoulderPoseEventReceived(pose.rightShoulder);
        }
        if (onLeftElbowPoseEventReceived != null) {
            onLeftElbowPoseEventReceived(pose.leftElbow);
        }
        if (onRightElbowPoseEventReceived != null) {
            onRightElbowPoseEventReceived(pose.rightElbow);
        }
        if (onLeftWristPoseEventReceived != null) {
            onLeftWristPoseEventReceived(pose.leftWrist);
        }
        if (onRightWristPoseEventReceived != null) {
            onRightWristPoseEventReceived(pose.rightWrist);
        }
        if (onLeftHipPoseEventReceived != null) {
            onLeftHipPoseEventReceived(pose.leftHip);
        }
        if (onRightHipPoseEventReceived != null) {
            onRightHipPoseEventReceived(pose.rightHip);
        }
        if (onLeftKneePoseEventReceived != null) {
            onLeftKneePoseEventReceived(pose.leftKnee);
        }
        if (onRightKneePoseEventReceived != null) {
            onRightKneePoseEventReceived(pose.rightKnee);
        }
        if (onLeftFootPoseEventReceived != null) {
            onLeftFootPoseEventReceived(pose.leftFoot);
        }
        if (onRightFootPoseEventReceived != null) {
            onRightFootPoseEventReceived(pose.rightFoot);
        }

        if (onRootPoseEventReceived != null) {
            onRootPoseEventReceived(pose.root);
        }
        if (onMiddleSpinePoseEventReceived!= null) {
            onMiddleSpinePoseEventReceived(pose.spine3);
        }
    }

    protected void baseHandlingOfPoseEvent(PoseEvent pose) {
        calculateCalculatedNodes(ref pose);
        lastPose.set(pose);
    }

    /**
     * Calculates the pelvis & middleSpine positions based on the other points, if they aren't already present
     */
    protected void calculateCalculatedNodes(ref PoseEvent newPose) {
        if (newPose.root == null) {
            newPose.root = new PosePosition();
            newPose.root.x = (newPose.rightHip.x + newPose.leftHip.x) / 2;
            newPose.root.y = (newPose.rightHip.y + newPose.leftHip.y) / 2;
        }
        if (newPose.spine3 == null) {
            newPose.spine3 = new PosePosition();
            newPose.spine3.x = (newPose.rightHip.x + newPose.leftShoulder.x) / 2;
            newPose.spine3.y = (newPose.leftShoulder.y + newPose.leftHip.y) / 2;
        }
    }

    public void Start() {
        DontDestroyOnLoad(this);

        adjustmentMap.Add(rootStr, new List<PosePosition>());
        adjustmentMap.Add(leftHandStr, new List<PosePosition>());
        adjustmentMap.Add(rightHandStr, new List<PosePosition>());
        adjustmentMap.Add(leftFootStr, new List<PosePosition>());
        adjustmentMap.Add(rightFootStr, new List<PosePosition>());
        adjustmentMap.Add(leftShoulderStr, new List<PosePosition>());
        adjustmentMap.Add(rightShoulderStr, new List<PosePosition>());
        adjustmentMap.Add(headStr, new List<PosePosition>());

        adjustmentEndTime = System.DateTime.Now;
        adjustmentStartTime = System.DateTime.Now;
        adjustmentStartTime = adjustmentStartTime.AddSeconds(adjustmentDelay);
        adjustmentEndTime = adjustmentEndTime.AddSeconds(secsForAdjustment + adjustmentDelay);
    }

    void Update() {
        if (adjusting) {
            if (System.DateTime.Now.CompareTo(adjustmentStartTime) < 0) {
                // Don't start adjustment yet
                return;
            }
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
    }

    /**
     * Using the adjustment info to prepare the system variables needed for operation
     */
    private void handleAdjustmentInfo() {
        // Calculate factors
        Vector3 rootVector = getAveragePosition(rootStr);
        Vector3 leftFootvector = getAveragePosition(leftFootStr);
        Vector3 rightFootvector = getAveragePosition(rightFootStr);
        Vector3 leftShoulderVector = getAveragePosition(leftShoulderStr);
        Vector3 rightShoulderVector = getAveragePosition(rightShoulderStr);
        Vector3 leftWristVector = getAveragePosition(leftHandStr);
        Vector3 rightWristVector = getAveragePosition(rightHandStr);
        Vector3 headVector = getAveragePosition(headStr);

        BodyPositionState state = new BodyPositionState();
        state.root = new PosePosition(rootVector);
        state.leftFoot = new PosePosition(leftFootvector);
        state.rightFoot = new PosePosition(rightFootvector);
        if (onInitialPoseCalculated != null) {
            onInitialPoseCalculated(state);
        }
    }

    /**
     * Returns the average position for this body element based on all values the last X seconds
     */
    private Vector3 getAveragePosition(string bodyPartStr) {
        Vector3 sum = new Vector3(0, 0);
        foreach (PosePosition pp in adjustmentMap[bodyPartStr]) {
            sum.y += pp.y;
            sum.x += pp.x;
            sum.z += pp.z;
        }
        Vector3 averageVector = new Vector3(sum.x / adjustmentMap[bodyPartStr].Count, sum.y / adjustmentMap[bodyPartStr].Count, sum.z / adjustmentMap[bodyPartStr].Count);
        return averageVector;
    }

    /**
     * Reading adjustment information
     */
    private void readAdjustmentInfo() {
        // Still adjusting
        if (lastPose.root != null) {
            adjustmentMap[rootStr].Add(lastPose.root);
        }

        if (lastPose.leftFoot != null) {
            adjustmentMap[leftFootStr].Add(lastPose.leftFoot);
        } else {
            Debug.Log("lastPose.leftFoot == null !!!");
        }
        if (lastPose.rightFoot != null) {
            adjustmentMap[rightFootStr].Add(lastPose.rightFoot);
        } else {
            Debug.Log("lastPose.rightFootStr == null !!!");
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
}
