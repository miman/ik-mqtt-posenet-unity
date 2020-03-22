using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoseEvent {
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
    public PosePosition leftAnkle;
    public PosePosition rightAnkle;

    public static PoseEvent CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PoseEvent>(jsonString);
    }
}
