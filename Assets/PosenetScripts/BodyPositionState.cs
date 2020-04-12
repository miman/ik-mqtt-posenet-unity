using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BodyPositionState {
    public PosePosition nose;
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
    public PosePosition leftHip;
    public PosePosition rightHip;
    public PosePosition leftKnee;
    public PosePosition rightKnee;
    public PosePosition leftFoot;
    public PosePosition rightFoot;
    public PosePosition pelvisPose;
    public PosePosition middleSpinePose;

    public BodyPositionState(PoseEvent poseEvent, PosePosition pelvisPose, PosePosition middleSpinePose) {
        this.nose = poseEvent.nose;
        this.leftFoot = poseEvent.leftFoot;
        this.rightFoot = poseEvent.rightFoot;
        this.leftEar = poseEvent.leftEar;
        this.rightEar = poseEvent.rightEar;
        this.leftElbow = poseEvent.leftElbow;
        this.rightElbow = poseEvent.rightElbow;
        this.leftEye = poseEvent.leftEye;
        this.rightEye = poseEvent.rightEye;
        this.leftHip = poseEvent.leftHip;
        this.rightHip = poseEvent.rightHip;
        this.leftKnee = poseEvent.leftKnee;
        this.rightKnee = poseEvent.rightKnee;
        this.leftShoulder = poseEvent.leftShoulder;
        this.rightShoulder = poseEvent.rightShoulder;
        this.leftWrist = poseEvent.leftWrist;
        this.rightWrist = poseEvent.rightWrist;
        this.pelvisPose = pelvisPose;
        this.middleSpinePose = middleSpinePose;
    }

    public BodyPositionState() {
        this.nose = new PosePosition();
        this.leftFoot = new PosePosition();
        this.rightFoot = new PosePosition();
        this.leftEar = new PosePosition();
        this.rightEar = new PosePosition();
        this.leftElbow = new PosePosition();
        this.rightElbow = new PosePosition();
        this.leftEye = new PosePosition();
        this.rightEye = new PosePosition();
        this.leftHip = new PosePosition();
        this.rightHip = new PosePosition();
        this.leftKnee = new PosePosition();
        this.rightKnee = new PosePosition();
        this.leftShoulder = new PosePosition();
        this.rightShoulder = new PosePosition();
        this.leftWrist = new PosePosition();
        this.rightWrist = new PosePosition();
        this.pelvisPose = new PosePosition();
        this.middleSpinePose = new PosePosition();
    }

    public void set(PoseEvent poseEvent, PosePosition pelvisPose, PosePosition middleSpinePose) {
        this.nose = poseEvent.nose;
        this.leftFoot = poseEvent.leftFoot;
        this.rightFoot = poseEvent.rightFoot;
        this.leftEar = poseEvent.leftEar;
        this.rightEar = poseEvent.rightEar;
        this.leftElbow = poseEvent.leftElbow;
        this.rightElbow = poseEvent.rightElbow;
        this.leftEye = poseEvent.leftEye;
        this.rightEye = poseEvent.rightEye;
        this.leftHip = poseEvent.leftHip;
        this.rightHip = poseEvent.rightHip;
        this.leftKnee = poseEvent.leftKnee;
        this.rightKnee = poseEvent.rightKnee;
        this.leftShoulder = poseEvent.leftShoulder;
        this.rightShoulder = poseEvent.rightShoulder;
        this.leftWrist = poseEvent.leftWrist;
        this.rightWrist = poseEvent.rightWrist;
        this.pelvisPose = pelvisPose;
        this.middleSpinePose = middleSpinePose;
    }
}
