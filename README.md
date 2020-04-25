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

1. Create a new Lib folder in your project for the MQTT lib files
1. Drag in the MQTT library files into the new project from folder Assets/MqttLib in this project
1. Create a new PoseCore folder in your project for the PoseCore script files
1. Drag in all files from the Assets/PosenetScripts folder into the PoseCore folder you created in your project
1. Create an empty game object (called for example "PoseCoreConnection")
1. Drag the MqttController script into this controller.
1. Configure the script to communicate with a specific MQTT server in One of these 2 ways
    - Check "Is using broadcast", then the component will try to find the server using the broadcasts mechanism
    - Uncheck "Is using broadcast", then the component will connect to the server you specify
1. Drag the PoseCoreEventManager script into this controller object as well.
1. Drag the PoseCoreEventManager object into the Pose Core Event Manager member field in the Mqtt Controller object
1. Create empty game object (called for example "Player")
1. Drag the script "PoseAvatarInputController" OR "PoseInputController" to this object
1. Drag the camera object into the Cam field in the Pose Input Controller
1. Create the game objects you want to control & drag these to the correct body part in the Player object
1. The body parts hould now move according to your body movements

