using UnityEngine;

[System.Serializable]
public class Rotation {
    public float x;
    public float y;
    public float z;
    public float w;

    public override string ToString() {
        return "(" + x + ", " + y + ", " + z + ", " + w  + ")";
    }
}
