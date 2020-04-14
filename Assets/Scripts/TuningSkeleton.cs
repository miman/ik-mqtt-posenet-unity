using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuningSkeleton : MonoBehaviour
{
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
    public GameObject root = null;
    public GameObject middleSpine = null;

    [Tooltip("The width factor the 100% should be recalculated to")]
    public float avatarWidthFactor = 100;
    [Tooltip("The height factor the 100% should be recalculated to")]
    public float avatarHeightFactor = 100;

    public float zLevel = 0;

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

    protected Vector2 prevRootCoord;
    protected Vector2 prevMiddleSpineCoord;

    /**
    * Last received posenet event, that is unprocessed
    */
    protected BodyPositionState lastPose = null;
    protected BodyPositionState basePose = null;
    private Vector3 adjustmentToZero = new Vector3(0,0,0);

    void OnEnable() {
        PoseCoreEventManager.onPoseEventReceived += onPoseEventReceived;
        PoseCoreEventManager.onInitialPoseCalculated += onInitialPoseCalculated;
        Debug.Log("TuningSkeleton::onPoseEventReceived enabled");
    }


    void OnDisable() {
        PoseCoreEventManager.onPoseEventReceived -= onPoseEventReceived;
        PoseCoreEventManager.onInitialPoseCalculated -= onInitialPoseCalculated;
        Debug.Log("TuningSkeleton::onPoseEventReceived disabled");
    }

    // Start is called before the first frame update
    void Start()
    {
        initPrevCoords(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        handleNodeMovement(lastPose.nose, nose, ref prevNoseCoord, "nose");
        handleNodeMovement(lastPose.leftEye, leftEye, ref prevLeftEyeCoord, "leftEye");
        handleNodeMovement(lastPose.rightEye, rightEye, ref prevRightEyeCoord, "rightEye");
        handleNodeMovement(lastPose.leftEar, leftEar, ref prevLeftEarCoord, "leftEar");
        handleNodeMovement(lastPose.rightEar, rightEar, ref prevRightEarCoord, "rightEar");
        handleNodeMovement(lastPose.leftShoulder, leftShoulder, ref prevLeftShoulderCoord, "leftShoulder");
        handleNodeMovement(lastPose.rightShoulder, rightShoulder, ref prevRightShoulderCoord, "rightShoulder");
        handleNodeMovement(lastPose.leftElbow, leftElbow, ref prevLeftElbowCoord, "leftElbow");
        handleNodeMovement(lastPose.rightElbow, rightElbow, ref prevRightElbowCoord, "rightElbow");
        handleNodeMovement(lastPose.leftWrist, leftWrist, ref prevLeftWristCoord, "leftWrist");
        handleNodeMovement(lastPose.rightWrist, rightWrist, ref prevRightWristCoord, "rightWrist");
        handleNodeMovement(lastPose.leftHip, leftHip, ref prevLeftHipCoord, "leftHip");
        handleNodeMovement(lastPose.rightHip, rightHip, ref prevRightHipCoord, "rightHip");
        handleNodeMovement(lastPose.leftKnee, leftKnee, ref prevLeftKneeCoord, "leftKnee");
        handleNodeMovement(lastPose.rightKnee, rightKnee, ref prevRightKneeCoord, "rightKnee");
        handleNodeMovement(lastPose.leftFoot, leftFoot, ref prevLeftFootCoord, "leftFoot");
        handleNodeMovement(lastPose.rightFoot, rightFoot, ref prevRightFootCoord, "rightFoot");

        handleNodeMovement(lastPose.root, root, ref prevRootCoord, "Root");
        handleNodeMovement(lastPose.spine3, middleSpine, ref prevMiddleSpineCoord, "MiddleSpine");
    }

    /**
     * This event handler will be called every time a new pose is received
     */
    public void onPoseEventReceived(BodyPositionState pose) {
        // Debug.Log("PoseEvent handled by PoseEventHandler");
        lastPose = pose;
    }

    public void onInitialPoseCalculated(BodyPositionState pose, float xAdjustmentToZero) {
        basePose = new BodyPositionState(pose);
        adjustmentToZero.Set(pose.root.x, xAdjustmentToZero+1, zLevel);
        Debug.Log("adjustmentToZero: " + adjustmentToZero.ToString());
        Debug.Log("basePose: " + basePose.root.ToString());
    }

    /**
     * Converts screen percentage to actual Unity screen pixels.
     * param name="posePos" A position in %
     * returns  The position in game coordinates
     */
    private Vector2 convertPercentageToPixels(PosePosition posePos) {
        Vector2 coord = new Vector2(0, 0);
        coord.x = posePos.x * (avatarWidthFactor / 100.0f);
        coord.y = posePos.y * (avatarHeightFactor / 100.0f); //  (100 - adjustmentToZero.y));
        return coord;
    }

    /**
     * Act on node movement
     * param name="posePos" The new position value for this node
     * param name="node"    The node this new position is for
     * param name="desc"    The description of the node (used for logging)
     */
    private void handleNodeMovement(PosePosition posePos, GameObject node, ref Vector2 previousCoord, string desc) {
        if (node == null) {
            // GameObject not set -> ignore it
            return;
        }
        // Add offset
        posePos.x = posePos.x - adjustmentToZero.x;
        posePos.y = posePos.y - adjustmentToZero.y;
        Vector2 currentCoord = convertPercentageToPixels(posePos);

        if (node == leftFoot) {
            Debug.Log("currentCoord: " + currentCoord.ToString());
        }

        // Smoothen the change in controlled GameObject
        float delta = currentCoord.x - previousCoord.x;
        currentCoord.x = previousCoord.x + (delta / smoothening);
        delta = currentCoord.y - previousCoord.y;
        currentCoord.y = previousCoord.y + (delta / smoothening);

        previousCoord.x = currentCoord.x;
        previousCoord.y = currentCoord.y;

        if (node == leftFoot) {
            Debug.Log("posePos: " + posePos.ToString() + ", previousCoord: " + previousCoord.ToString() + ", adjustmentToZero: " + adjustmentToZero.ToString());
        }
        Transform transform = node.transform;
        transform.localPosition = new Vector3(previousCoord.x, previousCoord.y, transform.localPosition.z);
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

        prevRootCoord = new Vector2(xFactor, yFactor);
        prevMiddleSpineCoord = new Vector2(xFactor, yFactor);
    }
}
