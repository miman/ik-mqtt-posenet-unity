using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public delegate void LeftAnklePoseEventAction(PosePosition pos);
    public static event LeftAnklePoseEventAction onLeftAnklePoseEventReceived;
    public delegate void RightAnklePoseEventAction(PosePosition pos);
    public static event RightAnklePoseEventAction onRightAnklePoseEventReceived;

    public delegate void PelvisPoseEventAction(PosePosition pos);
    public static event PelvisPoseEventAction onPelvisPoseEventReceived;
    public delegate void MiddleSpinePoseEventAction(PosePosition pos);
    public static event MiddleSpinePoseEventAction onMiddleSpinePoseEventReceived;

    protected PosePosition pelvisPose = new PosePosition();
    protected PosePosition middleSpinePose = new PosePosition();

    /**
    * Last received posenet event, that is unprocessed
    */
    protected BodyPositionState lastPose = new BodyPositionState();

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
        if (onLeftAnklePoseEventReceived != null) {
            onLeftAnklePoseEventReceived(pose.leftAnkle);
        }
        if (onRightAnklePoseEventReceived != null) {
            onRightAnklePoseEventReceived(pose.rightAnkle);
        }

        if (onPelvisPoseEventReceived != null) {
            onPelvisPoseEventReceived(this.pelvisPose);
        }
        if (onMiddleSpinePoseEventReceived!= null) {
            onMiddleSpinePoseEventReceived(this.middleSpinePose);
        }
    }

    protected void baseHandlingOfPoseEvent(PoseEvent pose) {
        calculateCalculatedNodes(pose);
        lastPose.set(pose, pelvisPose, middleSpinePose);
    }

    /**
     * Calculates the pelvis & middleSpine positions based on the other points
     */
    protected void calculateCalculatedNodes(PoseEvent newPose) {
//        pelvisPose = new PosePosition();
        pelvisPose.x = (newPose.rightHip.x + newPose.leftHip.x) / 2;
        pelvisPose.y = (newPose.rightHip.y + newPose.leftHip.y) / 2;
//        middleSpinePose = new PosePosition();
        middleSpinePose.x = (newPose.rightHip.x + newPose.leftShoulder.x) / 2;
        middleSpinePose.y = (newPose.leftShoulder.y + newPose.leftHip.y) / 2;
    }
}
