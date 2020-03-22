using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MqttSrvInfoMsg
{
    public MapiMsgHeader header;
    public MqttSrvInfo payload;

    static public MqttSrvInfoMsg parseJson(string json)
    {
        MqttSrvInfoMsg obj = JsonUtility.FromJson<MqttSrvInfoMsg>(json);

        return obj;
    }
}
