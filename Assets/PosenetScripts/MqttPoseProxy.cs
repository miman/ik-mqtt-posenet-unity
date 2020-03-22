﻿/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

// NOTE: Unity currently (March 2018) cannot connect to AWS IoT endpoints due to TLS 1.2 being missing from the Unity runtime.
// This may change soon, as a new release fixing this has been promised from the Unity developers, but for now you have to set
// up an MQTT server yourself which accepts user-password-authentication instead of client certificates as AWS IoT requires.
// It is, however, easy to setup a Mosquitto server which bridges everything under a particular prefix topic (such as "workshop/")
// and connects using client certificates to AWS IoT.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace PoseClient
{
    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    public class MqttPoseProxy
    {
        // IP addres or URL of host running the broker
        public string brokerAddress = "mqtt.thorman.eu";
        // Port where the broker accepts connections
        public int brokerPort = 1883;
        // Use encrypted connection
        public bool isEncrypted = false;
        // The username to access the MQTT srv
        public String username= null;
        // The password to access the MQTT srv
        public String password = null;
        // Use Broadcast to find the MQTT srv (Compared to directly using the given address:port above)
        public bool isUsingBroadcast = true;
        // The client id for this client, if not set, this will be autogenerated
        public string clientId = "poseclient-" + Guid.NewGuid().ToString();

        // The controller doing something usefull with the input
        public PoseEventHandler poseEventHandler;

        private MqttClient client;

        private BroadcastProxy broadcastProxy = null;

        private static PoseClient.MqttPoseProxy staticMqttProxy = null;

        private bool initiated = false;
//        private int msgReceived = 0;

        public static PoseClient.MqttPoseProxy getProxyObject()
        {
            if (staticMqttProxy == null)
            {
                staticMqttProxy = new PoseClient.MqttPoseProxy();
            }
            return staticMqttProxy;
        }

        public void cleanupResources()
        {
            Debug.Log("Closing MQTT Connection");
            if (client != null)
            {
                client.Disconnect();
            }
            broadcastProxy.disconnect();
        }

        public void Initialize(PoseEventHandler inputController)
        {
            poseEventHandler = inputController;
            if (initiated)
            {
                Debug.Log("MQTT already initiated, ignoring connect call.");
                return;
            }
            initiated = true;
            Debug.Log("MqttPoseProxy::Connect");
            broadcastProxy = new BroadcastProxy();
            broadcastProxy.mqttPoseProxy = this;
            if (isUsingBroadcast) {
                Debug.Log("MqttPoseProxy is using broadcast to connect to MQTT srv");
                broadcastProxy.Start();
            } else {
                Debug.Log("MqttPoseProxy is directly connecting to MQTT srv");
                Connect();
            }
        }

        public void Connect()
        {
            try
            {
                // create client instance
                client = new MqttClient(brokerAddress, brokerPort, isEncrypted, null, null, MqttSslProtocols.None);
                // register to message received
                client.MqttMsgPublishReceived += MqttMsgReceived;

                byte connectResponse;
                if ((username != null && username.Length > 0) && (password != null && password.Length > 0))
                {
                    Debug.Log("Connecting " + brokerAddress + ":" + brokerPort + "using username/passw");
                    connectResponse = client.Connect(clientId, username, password);
                }
                else
                {
                    Debug.Log("Connecting to " + brokerAddress + ":" + brokerPort + " NOT using username/passw");
                    connectResponse = client.Connect(clientId);
                }
                if (!client.IsConnected)
                {   // Couldn't connect !
                    Debug.Log("Failed to connect to MQTT Srv @ " + brokerAddress + ":" + brokerPort);

                    return;
                }
                Debug.Log("Connected to MQTT @ " + brokerAddress + ":" + brokerPort + ", status: " + connectResponse);

                SubscribeToTopic("posetracking/+/+/+/pose-event");
                SubscribeToTopic("posetracking/+/+/+/pose-setting");

                ushort publishResp = client.Publish("posetracking/" + clientId + "/connected", System.Text.Encoding.UTF8.GetBytes(@"{ ""cloudStatus"": ""ready"" }"), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
                Debug.Log("Publish QOS0 response: : " + publishResp);
            }
            catch (MqttCommunicationException ex)
            {
                Debug.Log("MQTT Exception: " + ex.ToString());
                Debug.Log("MQTT Inner Exception: " + ex.InnerException);
            }
        }

        /**
         * Use this fucntion to subscribe to a topic.
         */
        ushort SubscribeToTopic(string topicUri)
        {
            // OBS, even if the Connect functions takes an array of string, you can ONLY use one, otherwise the MQTT connection doesn't work !
            ushort subscribeResp = client.Subscribe(new string[] { topicUri },
                new byte[] {
                MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE
                });
            Debug.Log("Subscribed to MQTT topic [" + topicUri + "], response: " + subscribeResp);
            return subscribeResp;
        }

        void MqttMsgReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string topic = e.Topic;
            string[] topicComponents = topic.Split('/');
            string msg = System.Text.Encoding.UTF8.GetString(e.Message);
//            Debug.Log("Received on topic: " + e.Topic + "[" + (msgReceived++) + "] : " + System.Text.Encoding.UTF8.GetString(e.Message));

            MsgHeader msgHeader = MsgHeader.CreateFromJSON(msg);
            if (msgHeader.type != null && msgHeader.type.Equals("POSE_UPDATE"))
            {
                long pingTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - msgHeader.sendTime;
//                Debug.Log("Pose event received (ping: " + pingTime + ")");
                int startIndex = msg.IndexOf("payload\":") + "payload\":".Length;
                PoseEvent pose = PoseEvent.CreateFromJSON(msg.Substring(startIndex, msg.Length - (startIndex + 1)));
                poseEventHandler.HandlePoseEvent(pose);
            }
            else
            {
                Debug.Log("Unknown msg received: " + msg);
            }
        }

        public void setMqttSrvAddress(string ip, int port)
        {
            Debug.Log("MQTT srv setting set: (ip: " + ip + ", port: " + port + ")");
            brokerAddress = ip;
            brokerPort = port;
            Connect();
        }
    }
}
