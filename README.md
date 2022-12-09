## README

Here's the current demo for the game
https://user-images.githubusercontent.com/70987668/206808148-c406e83a-e180-4cb1-8edf-2120db8fe049.mp4

### Hardware Requirements

1. Nreal light(with firmware version)
2. Samsung S21 5G

### Software Requirements

1. Unity 2021.3.0f1
2. Nebular V3.1.1(Works better for RGBCamera than V3.2)
3. NRSDK 1.9.5(Already included in the repo because I made some changes)
4. OpenCV for Unity 2.4.9
4. Barracuda 3.0.0(Only for shoe detection not for marker)

### Installation

If you just want to try out the demo, install the FinalProject.apk file to your mobile device, and attach the *ArUco* tag board to your feet.

To build your own apk file

1.git clone the repository

2.Open the Project in Unity(There will be some errors because OpenCVForUnity is not imported yet, continue open the project and exit safe mode once opened)

3.Import OpenCVForUnity Package

4.Switch platform to be Android to build apk file , and add the Menu scene under Assets/Scenes and for Rehabilitation game demo or Add shoeDetection scene to  for shoe detection demo.

### Modification

For shoeDetection Scene, check CameraStreaming to test the scene with RGBCamera or uncheck it to test the result for single picture. 

### Results

Saved pacedata can be viewed under Documents and saved videos can be viewed under Videos on your mobile phone

