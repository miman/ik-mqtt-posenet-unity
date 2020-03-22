using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapiMsgHeader {
    public string type;
    public int version;
    public string sendTime;
    public string correlationId;
}
