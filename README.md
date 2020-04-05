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

