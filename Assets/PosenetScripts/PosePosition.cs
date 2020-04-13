using UnityEngine;

[System.Serializable]
public class PosePosition {
    public float x = -1;
    public float y = -1;
    public float z = -1;

    public PosePosition() {
    }

    public PosePosition(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public PosePosition(Vector3 v) {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public PosePosition(PosePosition v) {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public override string ToString() {
        return "(" + x + ", " + y + ", " + z + ")";
    }

    /**
     * If this has been set to a real value or not
     */
    public bool isSet() {
        return x >= 0 || y >= 0  || z >= 0 ;
    }

    public static PosePosition operator -(PosePosition a, PosePosition b) {
        PosePosition p = new PosePosition();
        p.x = a.x - b.x;
        p.y = a.y - b.y;
        p.z = a.z - b.z;
        return p;
    }
}
