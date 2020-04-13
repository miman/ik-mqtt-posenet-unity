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
    public PosePosition root;
    public PosePosition spine3;

    public BodyPositionState(PoseEvent poseEvent) {
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
        this.root = poseEvent.root;
        this.spine3 = poseEvent.spine3;
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
        this.root = new PosePosition();
        this.spine3 = new PosePosition();
    }

    public BodyPositionState(BodyPositionState state) {
        this.nose = new PosePosition(state.nose);
        this.leftFoot = new PosePosition(state.leftFoot);
        this.rightFoot = new PosePosition(state.rightFoot);
        this.leftEar = new PosePosition(state.leftEar);
        this.rightEar = new PosePosition(state.rightEar);
        this.leftElbow = new PosePosition(state.leftElbow);
        this.rightElbow = new PosePosition(state.rightElbow);
        this.leftEye = new PosePosition(state.leftEye);
        this.rightEye = new PosePosition(state.rightEye);
        this.leftHip = new PosePosition(state.leftHip);
        this.rightHip = new PosePosition(state.rightHip);
        this.leftKnee = new PosePosition(state.leftKnee);
        this.rightKnee = new PosePosition(state.rightKnee);
        this.leftShoulder = new PosePosition(state.leftShoulder);
        this.rightShoulder = new PosePosition(state.rightShoulder);
        this.leftWrist = new PosePosition(state.leftWrist);
        this.rightWrist = new PosePosition(state.rightWrist);
        this.root = new PosePosition(state.root);
        this.spine3 = new PosePosition(state.spine3);
    }

    public void set(PoseEvent poseEvent) {
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
        this.root = poseEvent.root;
        this.spine3 = poseEvent.spine3;
    }
}
