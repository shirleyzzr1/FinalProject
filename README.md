## README

Here's the current demo for the game
https://www.youtube.com/watch?v=hwb22EpPZPA

### Hardware Requirements

1. Nreal light(with firmware version)
2. Samsung S21 5G

### Software Requirements

1. Unity 2021.3.0f1
2. Nebular V3.1.1(Works better for RGBCamera than V3.2)
3. NRSDK 1.9.5
4. OpenCV for Unity 2.4.9
4. Barracuda 3.0.0(Only for shoe detection not for marker)

### Installation

If you just want to try out the demo, install the FinalProject.apk file to your mobile device, and attach the *ArUco* tag board to your feet.

To build your own apk file

1.Clone the repository

2.Open the Project in Unity

3.In Build Settings-Player Setting,uncheck multi-threaded rendering and set Write Permission to external

4.Select platform to be Android to build apk file , and add the Menu scene under Assets/Scenes and for Rehabilitation game demo or Add shoeDetection scene to  for shoe detection demo.

### Modification

For shoeDetection Scene, check CameraStreaming to test the scene with RGBCamera or uncheck it to test the result for single picture. 

