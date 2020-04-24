using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class BroadcastProxy {

    [Tooltip("Port where the posenet server info is broadcasted")]
    public int srvInfoPort = 45459;
    [Tooltip("Port where the the posenet server info request is broadcasted")]
    public int srvInfoRequestPort = 45458;

    private bool started = false;
    public PoseClient.MqttPoseProxy mqttPoseProxy;

    private UdpClient udpSrvinfoClient = null;

    private List<string> localIpList;

    // Use this for initialization
    public void Start () {
        Debug.Log("Initializing Brodcasting");
        if (started)
        {
            Debug.Log("Brodcasting already started once, ignored.");
            return;
        }
        localIpList = new List<string>();
        getLocalIpAddresses();
        udpSrvinfoClient = new UdpClient();
        // EnableBroadcast=true MUST be added for Android to allow sending broadcast messages
        udpSrvinfoClient.EnableBroadcast = true;
        udpSrvinfoClient.Client.Bind(new IPEndPoint(IPAddress.Any, srvInfoPort));

        started = true;
        var from = new IPEndPoint(0, 0);

        sendSrvInfoRequest();

        Task.Run(() =>
        {
            Debug.Log("Initializing Brodcasting listening thread started ");
            while (true)
            {
                var recvBuffer = udpSrvinfoClient.Receive(ref from);
                string receivedMsg = Encoding.UTF8.GetString(recvBuffer);
                Debug.Log("Broadcast msg received :" + receivedMsg);
                MqttSrvInfoMsg mqttSrvInfo = MqttSrvInfoMsg.parseJson(receivedMsg);
                mqttPoseProxy.setMqttSrvAddress(mqttSrvInfo.payload.ip, mqttSrvInfo.payload.port);
            }
        });
    }

    public void disconnect()
    {
        Debug.Log("Closing Brodcasting port listening");
        if (udpSrvinfoClient.Client != null) {
            udpSrvinfoClient.Client.Close();
        }
    }
    /**
     * Send Broadcast request for server location info
     */
    void sendSrvInfoRequest()
    {
        foreach (string ip in localIpList) {
            var data = Encoding.UTF8.GetBytes("{\"header\":{\"type\":\"MqttSrvInfoRequest\",\"version\":1,\"sendTime\":\"" + 
                System.DateTime.UtcNow.ToString("o")  + "\",\"correlationI\":\"1234\"},\"payload\":{\"ip\":\"" + ip + "\"}}");
            string[] ipParts = ip.Split('.');
            string broadcastIp = ipParts[0] + "." + ipParts[1] + "." + ipParts[2] + ".255";
//            Debug.Log("Sending MQTT srv info request on IP: " + broadcastIp);
            udpSrvinfoClient.Send(data, data.Length, broadcastIp, srvInfoRequestPort);
        }
    }

    private void getLocalIpAddresses()
    {
        string strHostName = string.Empty;
        // Getting Ip address of local machine...
        // First get the host name of local machine.
        strHostName = Dns.GetHostName();
        Debug.Log("Local Machine's Host Name: " + strHostName);
        // Then using host name, get the IP address list..
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] localIPs = ipEntry.AddressList;
        foreach (IPAddress addr in localIPs)
        {
            string ipStr = addr.ToString();
            if (ipStr.IndexOf('.') > 0)
            {   // This is an IPv4 address
                Debug.Log("Local IPv4: " + ipStr);
                localIpList.Add(ipStr);
            } else
            {   // This is an IPv6 address
                Debug.Log("Local IPv6: " + ipStr);
            }
            
        }
    }
}
