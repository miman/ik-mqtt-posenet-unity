using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This message is sent from a client as the first message after it has connected to a MQTT/Websocket connection to the server.
 */
[System.Serializable]
public class ClientConnectedMsg
{
    /**
     * The id of this client
     */
    public string clientId = "";
    /** Which of the exposed server URL's that the client was able to connect to
     * This is while a client might expose several if it has multiple network cards, 
     * by sending the URL that worked the server will know which the server was actually bound to.
     */
    public string srvUrl = "";

    public ClientConnectedMsg() {
    }

    public ClientConnectedMsg(string clientId, string srvUrl) {
        this.srvUrl = srvUrl;
        this.clientId = clientId;
    }

    public override string ToString() {
        return "(clientId: " + clientId + ", srvUrl: " + srvUrl + ")";
    }

    public string toJSON() {
        return JsonUtility.ToJson(this);
    }
}
