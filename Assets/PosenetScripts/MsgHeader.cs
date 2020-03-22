using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MsgHeader
{
    public string type;
    public string version;
    public long sendTime;

    public static MsgHeader CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MsgHeader>(jsonString);
    }
}
