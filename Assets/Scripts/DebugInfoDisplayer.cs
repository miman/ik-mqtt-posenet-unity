using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Globalization;
using System.Threading;

public class DebugInfoDisplayer : MonoBehaviour
{
    [Tooltip("The position of the root/pelvis")]
    public Text rootLvl = null;
    [Tooltip("The delta position from the root center")]
    public Text rootDeltaFromCenter = null;
    [Tooltip("The Average root value")]
    public Text averageRoot = null;
    [Tooltip("The Max root value")]
    public Text maxRoot = null;
    [Tooltip("The Min root value")]
    public Text minRoot = null;
    [Tooltip("The Current state string")]
    public Text currentStateText = null;

    [Tooltip("The center of the screen to set pos for text elements")]
    public Vector3 originScreenCenter = new Vector3(0,1,3);

    [Tooltip("If all received poses should be remembered & stored to file")]
    public bool storeReceivedPoses = false;


    private Vector3 rootCenter = new Vector3(0, 1, 3);

    /**
    * Last received posenet event, that is unprocessed
    */
    protected BodyPositionState lastPose = null;
    protected BodyPositionState averagePose = null;
    protected BodyPositionState maxPose = new BodyPositionState();
    protected BodyPositionState minPose = new BodyPositionState();

    /**
     * Contains all received pose evetns
     */
    List<BodyPositionState> receivedPoseEvents = new List<BodyPositionState>();
    List<BodyPositionState> diffPoses = new List<BodyPositionState>();

    private float xAdjustmentToZero = -1;

    START_END stateTransition = START_END.STARTING;
    private POSE_ACTION_STATE currentState = POSE_ACTION_STATE.NONE;

    void OnEnable() {
        PoseCoreEventManager.onPoseEventReceived += onPoseEventReceived;
        PoseCoreEventManager.onInitialPoseCalculated += onInitialPoseCalculated;
        PoseCoreEventManager.onCrouching += onCrouching;
        PoseCoreEventManager.onJumping += onJumping;
        Debug.Log("DebugInfoDisplayer::onPoseEventReceived enabled");
    }

    void OnDisable() {
        PoseCoreEventManager.onPoseEventReceived -= onPoseEventReceived;
        PoseCoreEventManager.onInitialPoseCalculated -= onInitialPoseCalculated;
        PoseCoreEventManager.onCrouching -= onCrouching;
        PoseCoreEventManager.onJumping -= onJumping;
        Debug.Log("DebugInfoDisplayer::onPoseEventReceived disabled");
        if (storeReceivedPoses) {
            storeReceivedPosesToFile(receivedPoseEvents, "/receivedPoseEvents.csv");
            storeReceivedPosesToFile(diffPoses, "/diffPoses.csv");
        }
    }

    /**
     * This event handler will be called every time a new pose is received
     */
    public void onPoseEventReceived(BodyPositionState pose) {
        // Debug.Log("PoseEvent handled by PoseEventHandler");
        lastPose = pose;
        updateMaxValues(lastPose);
        updateMinValues(lastPose);
        if (storeReceivedPoses) {
            receivedPoseEvents.Add(new BodyPositionState(pose));
            if (averagePose != null) {
                diffPoses.Add(pose - averagePose);
            }
        }
    }

    public void onInitialPoseCalculated(BodyPositionState pose, float xAdjustmentToZero) {
        averagePose = pose;
        this.xAdjustmentToZero = xAdjustmentToZero;
    }


    private void onJumping(START_END state, BodyPositionState pose) {
        if (state == START_END.STARTING) {
            currentState = POSE_ACTION_STATE.JUMPING;
        } else {
            currentState = POSE_ACTION_STATE.NONE;
        }
    }

    private void onCrouching(START_END state, BodyPositionState pose) {
        if (state == START_END.STARTING) {
            currentState = POSE_ACTION_STATE.CROUCHING;
        } else {
            currentState = POSE_ACTION_STATE.NONE;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rootLvl.transform.position.Set(originScreenCenter.x, originScreenCenter.y + 1, originScreenCenter.z);
        rootDeltaFromCenter.transform.position.Set(originScreenCenter.x + 0.5f, originScreenCenter.y + 1, originScreenCenter.z);
        minPose.root = new PosePosition(100, 100, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (rootLvl != null && lastPose != null) {
            rootLvl.text = "Root: " + lastPose.root.ToString();
            rootLvl.transform.position.Set(rootLvl.transform.position.x, lastPose.root.y + 1, rootLvl.transform.position.z);
        }

        if (rootDeltaFromCenter != null && lastPose != null && averagePose != null) {
            rootDeltaFromCenter.text = "Root Delta: " + (lastPose.root - averagePose.root).ToString();
            rootDeltaFromCenter.transform.position.Set(rootDeltaFromCenter.transform.position.x + 0.5f, lastPose.root.y + 1, rootDeltaFromCenter.transform.position.z);
        }
        if (averageRoot != null && averagePose != null) {
            averageRoot.text = "Average: " + averagePose.root.ToString();
        }
        if (maxRoot != null && maxPose != null) {
            maxRoot.text = "Max: " + maxPose.root.ToString();
        }
        if (minRoot != null && minPose != null) {
            minRoot.text = "Min: " + minPose.root.ToString();
        }
        if (currentStateText != null) {
            currentStateText.text = currentState.ToString();
        }
    }

    private void updateMaxValues(BodyPositionState pose) {
        if (maxPose.root.x < pose.root.x) {
            // Update max to new value
            maxPose.root.x = pose.root.x;
        }
        if (maxPose.root.y < pose.root.y) {
            // Update max to new value
            maxPose.root.y = pose.root.y;
        }
        if (maxPose.root.z < pose.root.z) {
            // Update max to new value
            maxPose.root.z = pose.root.z;
        }
    }

    private void updateMinValues(BodyPositionState pose) {
        if (minPose.root.x > pose.root.x) {
            // Update min to new value
            minPose.root.x = pose.root.x;
        }
        if (minPose.root.y > pose.root.y) {
            // Update min to new value
            minPose.root.y = pose.root.y;
        }
        if (minPose.root.z > pose.root.z) {
            // Update min to new value
            minPose.root.z = pose.root.z;
        }
    }

    private void storeReceivedPosesToFile(List<BodyPositionState> receivedPoseEvents, string filename) {
        string folder = "tmpfiles";
        string path = folder + "/" + filename;
        bool newFile = false;

        Debug.Log("Culture before: " + CultureInfo.CurrentCulture.DisplayName);
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Debug.Log("Culture after: " + CultureInfo.CurrentCulture.DisplayName);

        if (!Directory.Exists(folder)) {
            Directory.CreateDirectory(folder);
        }
        if (!File.Exists(path)) {
            newFile = true;
        }
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        if (newFile) {
            writer.Write("root x");
            writer.Write(",");
            writer.Write("root y");
            writer.Write(",");
            writer.Write("root z");
            writer.Write(",");
            writer.Write("Left Foot x");
            writer.Write(",");
            writer.Write("Left Foot y");
            writer.Write(",");
            writer.Write("Left Foot z");
            writer.Write(",");
            writer.Write("Right Foot x");
            writer.Write(",");
            writer.Write("Right Foot y");
            writer.Write(",");
            writer.Write("Right Foot z");
            writer.WriteLine("");
        }
        foreach (BodyPositionState poseEvent in receivedPoseEvents) {
            writer.Write(poseEvent.root.x);
            writer.Write(",");
            writer.Write(poseEvent.root.y);
            writer.Write(",");
            writer.Write(poseEvent.root.z);
            writer.Write(",");
            writer.Write(poseEvent.leftFoot.x);
            writer.Write(",");
            writer.Write(poseEvent.leftFoot.y);
            writer.Write(",");
            writer.Write(poseEvent.leftFoot.z);
            writer.Write(",");
            writer.Write(poseEvent.rightFoot.x);
            writer.Write(",");
            writer.Write(poseEvent.rightFoot.y);
            writer.Write(",");
            writer.Write(poseEvent.rightFoot.z);
            writer.WriteLine("");
        }
        writer.Close();

        //Print the text from the file
        Debug.Log("Stored receivedPoseEvents");
    }
}
