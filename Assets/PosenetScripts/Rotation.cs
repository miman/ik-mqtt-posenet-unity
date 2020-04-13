using UnityEngine;

[System.Serializable]
public class Rotation {
    public float x = -1;
    public float y = -1;
    public float z = -1;
    public float w = -1;

    public override string ToString() {
        return "(" + x + ", " + y + ", " + z + ", " + w  + ")";
    }

    /**
     * If this has been set to a real value or not
     */
    public bool isSet() {
        return x >= 0 || y >= 0 || z >= 0 || w >= 0;
    }
}
