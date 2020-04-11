# ik-mqtt-posenet-unity
Test Unity app for controlling avatar with Posenet client.

Contains scripts that:
- Communicates with a Posenet server (over MQTT)
    - MqttController
- Converts the input to control an avatar
    - PoseAvatarInputController
- Converts the input to control gameobjects in a view
    - PoseInputController

## Scenes
There are 2 scenes in this project

### Upper body pose scene
The "Upper body pose scene" scene is used to visualize how to use the library that controls an upperbody only avatar.

It uses the "PoseInputController" script which will give each body part a 0-100% value for the X&Y values based on the camera screen.


### VR-pose
The "VR-pose" scene is used to visualize how to use the library that controls a full Avatar body using Inverse Kinematics.

It uses the "PoseAvatarInputController" script which will try to control reference points for each body part.
It will try to calculate the pose-value vs body size in the app ratio & location and then use this to place the reference points in the corrrect place.

# Howto Use the scripts

## Steps to do
To enable you to control game objects in a scene you need to do the following steps.

1. Create an empty game object (called for example "MqttConnection")
2. Drag the MqttController script into this controller.
3. Configure the script to communicate with a specific MQTT server in One of these 2 ways
    3a. Check "Is using broadcast", then the component will try to find the server using the broadcasts mechanism
    3b. Uncheck "Is using broadcast", then the component will connect to the server you specify
4. Create empty game object (called for example "Player")
5. Drag the script "PoseAvatarInputController" OR "PoseInputController" to this object
6. increase the size of event handlers to 1 in the MqttConnection
7. Create an Empty GameObject called for example PoseEventMgr & drag the script PoseCoreEventManager into this object
8. Create the game objects you want to control & drag these to the correct body part in the Player object
9. The body parts hould now mova according to your body movements

