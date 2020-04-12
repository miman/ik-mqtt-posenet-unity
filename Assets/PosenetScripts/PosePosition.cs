using UnityEngine;

[System.Serializable]
public class PosePosition {
    public float x = -1;
    public float y = -1;
    public float z = -1;

    public override string ToString() {
        return "(" + x + ", " + y + ", " + z + ")";
    }

    /**
     * If this has been set to a real value or not
     */
    public bool isSet() {
        return x >= 0 || y >= 0  || z >= 0 ;
    }
}
