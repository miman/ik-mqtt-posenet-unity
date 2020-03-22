

# Debug on Android

1. Enable USB debugging on Android phone
2. Connect android phone to USB
3. run command "adb tcpip 5555"
4. run command "adb connect 192.168.0.7"
    Or whatever your phone IP is
5. run command "adb devices" to verify that it is connected
6. Disconnect your phone from USB cable

You can then disconnect by running the command "adb disconnect 192.168.0.7"

## ADB Monitor
Start ADB monitor app from here:
C:\Dev\IDE\Android\sdk\tools\monitor.bat

## Install Log viewer
Install Log viewer from Unity asset store to se logs on Android device (draw circle anywhere on screen to se logs)
