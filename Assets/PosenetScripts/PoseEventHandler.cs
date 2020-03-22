using UnityEngine;

public class PoseEventHandler: MonoBehaviour {
    [Header("GameObject body mapping")]
    public GameObject nose = null;
    public GameObject leftEye = null;
    public GameObject rightEye = null;
    public GameObject leftEar = null;
    public GameObject rightEar = null;
    public GameObject leftShoulder = null;
    public GameObject rightShoulder = null;
    public GameObject leftElbow = null;
    public GameObject rightElbow = null;
    public GameObject leftWrist = null;
    public GameObject rightWrist = null;
    public GameObject leftHip = null;
    public GameObject rightHip = null;
    public GameObject leftKnee = null;
    public GameObject rightKnee = null;
    public GameObject leftAnkle = null;
    public GameObject rightAnkle = null;

    [Header("Calculated locations")]
    // Calculated
    public GameObject pelvis = null;
    public GameObject middleSpine = null;

    /**
    * Last received posenet event, that is unprocessed
    */
    protected PoseEvent lastPose = null;
    // Last posenet event that has been processed
    protected PoseEvent processedPose = null;

    [Tooltip("Smoothening om movements to avoid jerkiness")]
    public float smoothening = 10;

    /**
     * The last known body part location, used to get a smoother camera tracking
     */
    protected Vector2 prevNoseCoord;
    protected Vector2 prevLeftEyeCoord;
    protected Vector2 prevRightEyeCoord;
    protected Vector2 prevLeftEarCoord;
    protected Vector2 prevRightEarCoord;
    protected Vector2 prevLeftShoulderCoord;
    protected Vector2 prevRightShoulderCoord;
    protected Vector2 prevLeftElbowCoord;
    protected Vector2 prevRightElbowCoord;
    protected Vector2 prevLeftWristCoord;
    protected Vector2 prevRightWristCoord;
    protected Vector2 prevLeftHipCoord;
    protected Vector2 prevRightHipCoord;
    protected Vector2 prevLeftKneeCoord;
    protected Vector2 prevRightKneeCoord;
    protected Vector2 prevLeftAnkleCoord;
    protected Vector2 prevRightAnkleCoord;

    protected Vector2 prevPelvisCoord;
    protected Vector2 prevMiddleSpineCoord;

    protected PosePosition pelvisPose;
    protected PosePosition middleSpinePose;

    /**
     * A new posenet event was received
     */
    public void HandlePoseEvent(PoseEvent pose) {
//        Debug.Log("PoseEvent handled by PoseEventHandler");
        lastPose = pose;
        calculateCalculatedNodes(lastPose);
    }

    /**
     * Initializes the previous coordinates
     */
    protected void initPrevCoords(float xFactor, float yFactor) {
        prevNoseCoord = new Vector2(xFactor, yFactor);
        prevLeftEyeCoord = new Vector2(xFactor, yFactor);
        prevRightEyeCoord = new Vector2(xFactor, yFactor);
        prevLeftEarCoord = new Vector2(xFactor, yFactor);
        prevRightEarCoord = new Vector2(xFactor, yFactor);
        prevLeftShoulderCoord = new Vector2(xFactor, yFactor);
        prevRightShoulderCoord = new Vector2(xFactor, yFactor);
        prevLeftElbowCoord = new Vector2(xFactor, yFactor);
        prevRightElbowCoord = new Vector2(xFactor, yFactor);
        prevLeftWristCoord = new Vector2(xFactor, yFactor);
        prevRightWristCoord = new Vector2(xFactor, yFactor);
        prevLeftHipCoord = new Vector2(xFactor, yFactor);
        prevRightHipCoord = new Vector2(xFactor, yFactor);
        prevLeftKneeCoord = new Vector2(xFactor, yFactor);
        prevRightKneeCoord = new Vector2(xFactor, yFactor);
        prevLeftAnkleCoord = new Vector2(xFactor, yFactor);
        prevRightAnkleCoord = new Vector2(xFactor, yFactor);

        prevPelvisCoord = new Vector2(xFactor, yFactor);
        prevMiddleSpineCoord = new Vector2(xFactor, yFactor);
    }

    protected void calculateCalculatedNodes(PoseEvent newPose) {
        pelvisPose = new PosePosition();
        pelvisPose.x = (newPose.rightHip.x + newPose.leftHip.x) / 2;
        pelvisPose.y = (newPose.rightHip.y + newPose.leftHip.y) / 2;
        middleSpinePose = new PosePosition();
        middleSpinePose.x = (newPose.rightHip.x + newPose.leftShoulder.x) / 2;
        middleSpinePose.y = (newPose.leftShoulder.y + newPose.leftHip.y) / 2;
    }
}
