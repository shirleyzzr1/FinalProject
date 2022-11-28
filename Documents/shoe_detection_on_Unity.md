In this post, I'm gonna go through all the detail configuration to prepare your own dataset for shoe detection and how to use this model in the Unity.

Data collection:

The goal of this sub-module is to detect shoes from top-down view. The reason why we need top-down view is that, we will be wearing the AR glass on our head, and the only way to see our shoes from our glass is to bend our head. To train a .

Since the footwear dataset contains shoes shooting from all different directions and some of them are too small to recognize, we randomly downloaded 10,000 training images and manually selected 163 images which best suits our needs. Another part of the training images are extracted from video shooting by the RGB Camera on Nreal AR Glass directly. Here we only choose the frame where shoes are shown up in the image. So in total, we had 450 images. And then, use LabelImg to add labels to those images and save as VOC data type. You can run the script generate_vocdata.py to generate the txt file for train, validation and test sets.

Next, we train images on yolo-v2-tiny. Since we are gonna use this model on mobile device, we need to choose a light-weight model suitable for use. However, Barracuda doesn't support SSD model for now, so MobilenetSSD is not a valid option. So Let's try YOLO.

Follow the step on the [YOLO website](https://pjreddie.com/darknet/yolov2/) to build the project. To train on GPU, modify makefile to be GPU=1,CUDNN=1,OPENCV=1. Also, change the classes in cfg/tiny-yolo-voc.cfg to 1 and filters to 30. You can change the number of epochs to save weights in examples/detector.c. I didn't change any other hyper parameters in cfg file and the result is still very promising. I choose the model generated at 1900 epoch, the mAP is 84.06% for this model.

Next, we will need to transfer the YOLO model to ONXX form to be used in Barracuda. Barracuda is a lightweight cross-platform Neural Networks inference library for Unity. Another common option to integrate deep learning model into Unity is to use TensorFlowSharp package. However, that experimental package works with Unity 2019 but not compatible with Unity 2021. So, to install Barracuda 3.0.0, add "com.unity.barracuda": "3.0.0" to manifest.json file in Packages.

[This notebook](https://github.com/DaphiFluffi/Yolov3-To-Onnx-Unity/blob/main/Weights_Pb_FrozenPB_Onnx.ipynb) talks about the steps to convert yolov3 model to ONXX. Luckily, we can follow the same step to convert our yolov2-tiny model. To convert the YOLO model to ONXX model, we first need to convert them from .cfg and .weights to the format .pb and .ckpt used by tensorflow. Clone the repository [DW2TF](https://github.com/jinyu121/DW2TF), and follow REAME file to convert model. I used Tensorflow 2.0, so to use those Tensorflow 1.x function in the code, we could change the import line in cfg_layer.py and main.py.

```python
#import tensorflow
import tensorflow.compat.v1 as tf
tf.disable_v2_behavior()
```

After converting, we need to freeze the converted tensorflow graph using tensorflow freeze_graph tools. And final step is to convert the frozen tensorflow graph to ONXX format using tf2onnx. Finally, we get the model ready to be used in Unity for shoe detection.

