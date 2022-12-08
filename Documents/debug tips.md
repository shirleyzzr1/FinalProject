Debug Tips for Nreal glass Development

#### Debug Tools

Use [Android Debug Bridge(adb)](https://developer.android.com/studio/command-line/adb) to install the apk, pull files from the mobile device. Wifi connection is super useful since Nreal glass also needs to be connected with phone through type-C cable.

Use [Android Logcat](https://docs.unity3d.com/Packages/com.unity.mobile.android-logcat@0.1/manual/index.html) to view the output from Unity while debugging your app. I usually set tags to Unity and Nreal to filter the messages.

Use [Debugger for Unity](https://marketplace.visualstudio.com/items?itemName=Unity.unity-debug) to debug your code in VSCode.

#### Unity Development

1.How to do collision detection?

Add Collider to both the cube representing shoes and game object. Select "Is Trigger" in game Object so we can use OnTriggerEnter() function to respond to the collision event, like displaying some special effect or adding current score points. According to the Collision action matrix in Unity Documentation, two Static Collider cannot create collision detection, so an Rigidbody Collider is needed. I add Rigidbody Component to shoe object, and uncheck "Use Gravity"to make sure the transform of the shoe object is not affected.

2.Add user permission in Assets/Plugins/Android/AndroidManifest.xml

```xml
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
```
3.To avoid some RGB Camera issue, in player settings, uncheck multithreaded rendering and set Write Permission to external

#### Nreal Development

##### RGB Camera

To close Power Saving mode for RGB Camera on Nebula APP

- Go to Mine-About, and click on logo for App Version for multiple times until you see "You are now a developer".


- Go to Mine-Settings, developer options, turn on develop options and turn off Power Saving Mode


- Restart the Nebula to update the configuration

If it is still not working, try replug the cable or restart the mobile phone. Sometimes, after error with RGB Camera, restarting Nebula will force shutdown the phone.

##### NRSDK Prefab

1.NRCameraRig

An new camera called RGBCamera is added, and this camera is used for more accurate object detection. Here is the [link](https://community.nreal.ai/t/is-there-an-alternative-to-screentoworldpoint-specific-for-rgb-camera/1977/7) for how to do that in Unity.

2.Open Plane Finding Mode

Also, click on the NRKernalSessionConfig in NR Session Behaviour, and set Plane Finding Mode to be **HORIZONTAL** and Image Tracking Mode to be **DISABLE**.

3.CustomNRVirtualDisplayer

This prefab is for modifying the default UI on the controller. I added an extra button for recording purpose.

4.Create intereactive canvas

When creating a new canvas, to enable laser ray interaction with canvas, add CanvasRaycastTarget Script Component.



