using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton
{
    private List<LineBone> lineBones = new List<LineBone>();

    public Material material;
    public float skeletonWidth = 0.05f;

    public GameObject head = null;
    public GameObject leftEye = null;
    public GameObject rightEye = null;
    public GameObject leftEar = null;
    public GameObject rightEar = null;
    public GameObject leftShoulder = null;
    public GameObject rightShoulder = null;
    public GameObject leftElbow = null;
    public GameObject rightElbow = null;
    public GameObject leftHand = null;
    public GameObject rightHand = null;
    public GameObject leftHip = null;
    public GameObject rightHip = null;
    public GameObject leftKnee = null;
    public GameObject rightKnee = null;
    public GameObject leftFoot = null;
    public GameObject rightFoot = null;
    public GameObject root = null;
    public GameObject middleSpine = null;

    // Initiates the object
    public void initiate() {

        // Arms
        LineBone lb = createNewLine(leftElbow, leftHand);
        lineBones.Add(lb);
        lb = createNewLine(rightElbow, rightHand);
        lineBones.Add(lb);
        lb = createNewLine(leftShoulder, leftElbow);
        lineBones.Add(lb);
        lb = createNewLine(rightShoulder, rightElbow);
        lineBones.Add(lb);
        lb = createNewLine(middleSpine, rightShoulder);
        lineBones.Add(lb);
        lb = createNewLine(middleSpine, leftShoulder);

        // Legs
        lineBones.Add(lb);
        lb = createNewLine(leftHip, leftKnee);
        lineBones.Add(lb);
        lb = createNewLine(rightHip, rightKnee);
        lineBones.Add(lb);
        lb = createNewLine(leftKnee, leftFoot);
        lineBones.Add(lb);
        lb = createNewLine(rightKnee, rightFoot);
        lineBones.Add(lb);
        lb = createNewLine(root, leftHip);
        lineBones.Add(lb);
        lb = createNewLine(root, rightHip);
        lineBones.Add(lb);

        // Body
        lb = createNewLine(middleSpine, head);
        lineBones.Add(lb);
        lb = createNewLine(root, middleSpine);
        lineBones.Add(lb);

        // Head
        lb = createNewLine(head, leftEye);
        lineBones.Add(lb);
        lb = createNewLine(head, rightEye);
        lineBones.Add(lb);
        lb = createNewLine(leftEye, leftEar);
        lineBones.Add(lb);
        lb = createNewLine(rightEye, rightEar);
        lineBones.Add(lb);
    }

    // Update is called once per frame
    public void Update() {
        foreach(LineBone lb in lineBones) {
            lb.updatePositions();
        }
    }

    private LineBone createNewLine(GameObject fromObj, GameObject toObj) {
        Color color = Color.green;
        LineRenderer lr = toObj.AddComponent<LineRenderer>();
        lr.material = new Material(material);
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = skeletonWidth;
        lr.endWidth = skeletonWidth;
        lr.SetPosition(0, fromObj.transform.position);
        lr.SetPosition(1, toObj.transform.position);

        LineBone lb = new LineBone(lr, fromObj, toObj);
        return lb;
    }
}

class LineBone {
    public LineRenderer lineRenderer;
    public GameObject fromObj;
    public GameObject toObj;

    public LineBone(LineRenderer lineRenderer, GameObject fromObj, GameObject toObj) {
        this.lineRenderer = lineRenderer;
        this.fromObj = fromObj;
        this.toObj = toObj;
    }

    public void updatePositions() {
        lineRenderer.SetPosition(0, fromObj.transform.position);
        lineRenderer.SetPosition(1, toObj.transform.position);
    }
}
