using UnityEngine;

/**
 * This handler will receive all pose events and will update the positions of the related GameObjects that has been assigned to it.
 */
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
    public GameObject leftFoot = null;
    public GameObject rightFoot = null;

    [Header("Calculated locations")]
    // Calculated
    public GameObject pelvis = null;
    public GameObject middleSpine = null;

    /**
    * Last received posenet event, that is unprocessed
    */
    protected BodyPositionState lastPose = null;
    // Last posenet event that has been processed
    protected BodyPositionState processedPose = null;

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
    protected Vector2 prevLeftFootCoord;
    protected Vector2 prevRightFootCoord;

    protected Vector2 prevPelvisCoord;
    protected Vector2 prevMiddleSpineCoord;

    void OnEnable() {
        PoseCoreEventManager.onPoseEventReceived += onPoseEventReceived;
        Debug.Log("PoseEventHandler::onPoseEventReceived enabled");
    }


    void OnDisable() {
        PoseCoreEventManager.onPoseEventReceived -= onPoseEventReceived;
        Debug.Log("PoseEventHandler::onPoseEventReceived disabled");
    }

    /**
     * This event handler will be called every time a new pose is received
     */
    private void onPoseEventReceived(BodyPositionState pose) {
        // Debug.Log("PoseEvent handled by PoseEventHandler");
        lastPose = pose;
    }

    /**
     * Initializes the previous coordinates to the given x/y values
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
        prevLeftFootCoord = new Vector2(xFactor, yFactor);
        prevRightFootCoord = new Vector2(xFactor, yFactor);

        prevPelvisCoord = new Vector2(xFactor, yFactor);
        prevMiddleSpineCoord = new Vector2(xFactor, yFactor);
    }
}
