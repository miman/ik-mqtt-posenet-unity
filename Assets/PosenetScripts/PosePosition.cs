using UnityEngine;

[System.Serializable]
public class PosePosition {
    public float x;
    public float y;
    public float z;

    public override string ToString() {
        return "(" + x + ", " + y + ", " + z + ")";
    }
}
