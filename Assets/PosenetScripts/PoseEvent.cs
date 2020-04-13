using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoseEvent {
    public PosePosition nose;
    public PosePosition head;
    public PosePosition leftEye;
    public PosePosition rightEye;
    public PosePosition leftEar;
    public PosePosition rightEar;
    public PosePosition leftShoulder;
    public PosePosition rightShoulder;
    public PosePosition leftElbow;
    public PosePosition rightElbow;
    public PosePosition leftWrist;
    public PosePosition rightWrist;
    public PosePosition root; // mid-point between hips (pelvis)
    public PosePosition leftHip;
    public PosePosition rightHip;
    public PosePosition leftKnee;
    public PosePosition rightKnee;
    public PosePosition leftFoot;
    public PosePosition rightFoot;

    // The ones below are extras for more detailed models

    public PosePosition spine1;
    public PosePosition spine2;
    public PosePosition spine3;
    public PosePosition spine4;
    public PosePosition spine5;
    public PosePosition spine6;
    public PosePosition spine7;

    public PosePosition neck1;
    public PosePosition neck2;
    public PosePosition neck3;
    public PosePosition neck4;

    public PosePosition jaw;
    public PosePosition chin;

    // Right Hand
    public PosePosition rightHandThumbStart;
    public PosePosition rightHandThumbJoint1;
    public PosePosition rightHandThumbJoint2;
    public PosePosition rightHandThumbEnd;

    public PosePosition rightHandIndexStart;
    public PosePosition rightHandIndexJoint1;
    public PosePosition rightHandTIndexJoint2;
    public PosePosition rightHandTIndexJoint3;
    public PosePosition rightHandIndexEnd;

    public PosePosition rightHandMidStart;
    public PosePosition rightHandMidJoint1;
    public PosePosition rightHandTMidJoint2;
    public PosePosition rightHandTMidJoint3;
    public PosePosition rightHandMidEnd;

    public PosePosition rightHandRingStart;
    public PosePosition rightHandRingJoint1;
    public PosePosition rightHandTRingJoint2;
    public PosePosition rightHandTRingJoint3;
    public PosePosition rightHandRingEnd;

    public PosePosition rightHandPinkyStart;
    public PosePosition rightHandPinkyJoint1;
    public PosePosition rightHandTPinkyJoint2;
    public PosePosition rightHandTPinkyJoint3;
    public PosePosition rightHandPinkyEnd;

    // Left Hand
    public PosePosition leftHandThumbStart;
    public PosePosition leftHandThumbJoint1;
    public PosePosition leftHandThumbJoint2;
    public PosePosition leftHandThumbEnd;

    public PosePosition leftHandIndexStart;
    public PosePosition leftHandIndexJoint1;
    public PosePosition leftHandTIndexJoint2;
    public PosePosition leftHandTIndexJoint3;
    public PosePosition leftHandIndexEnd;

    public PosePosition leftHandMidStart;
    public PosePosition leftHandMidJoint1;
    public PosePosition leftHandTMidJoint2;
    public PosePosition leftHandTMidJoint3;
    public PosePosition leftHandMidEnd;

    public PosePosition leftHandRingStart;
    public PosePosition leftHandRingJoint1;
    public PosePosition leftHandTRingJoint2;
    public PosePosition leftHandTRingJoint3;
    public PosePosition leftHandRingEnd;

    public PosePosition leftHandPinkyStart;
    public PosePosition leftHandPinkyJoint1;
    public PosePosition leftHandTPinkyJoint2;
    public PosePosition leftHandTPinkyJoint3;
    public PosePosition leftHandPinkyEnd;

    // Right foot
    public PosePosition rightToes;
    public PosePosition rightToesEnd;

    // Left foot
    public PosePosition leftToes;
    public PosePosition leftToesEnd;

    // ================================================
    // Describing the rotation of an object
    public Rotation noseRotation;
    public Rotation headRotation;
    public Rotation leftEyeRotation;
    public Rotation rightEyeRotation;
    public Rotation leftEarRotation;
    public Rotation rightEarRotation;
    public Rotation leftShoulderRotation;
    public Rotation rightShoulderRotation;
    public Rotation leftElbowRotation;
    public Rotation rightElbowRotation;
    public Rotation leftWristRotation;
    public Rotation rightWristRotation;
    public Rotation rootRotation;
    public Rotation leftHipRotation;
    public Rotation rightHipRotation;
    public Rotation leftKneeRotation;
    public Rotation rightKneeRotation;
    public Rotation leftFootRotation;
    public Rotation rightFootRotation;

    public static PoseEvent CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PoseEvent>(jsonString);
    }

    /**
     * This is a hack to set all values missing in the received msg to null while the Unity JSON utility always creates an object for everything even if it is missing.
     */
    public void setAllValuesWithoutValueToNull() {
        if (!root.isSet()) {
            root = null;
        }
        if (!spine1.isSet()) {
            spine1 = null;
        }
        if (!spine2.isSet()) {
            spine2 = null;
        }
        if (!spine3.isSet()) {
            spine3 = null;
        }
        if (!spine4.isSet()) {
            spine4 = null;
        }
        if (!spine5.isSet()) {
            spine5 = null;
        }
        if (!spine6.isSet()) {
            spine6 = null;
        }
        if (!spine7.isSet()) {
            spine7 = null;
        }
        if (!leftFoot.isSet()) {
            leftFoot = null;
        }
        if (!rightFoot.isSet()) {
            rightFoot = null;
        }
    }
}
