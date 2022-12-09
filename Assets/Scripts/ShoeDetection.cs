// Filename:    shoeDetection.cs 
// Summary:     Detector shoes using yolov2-tiny model using RGB Camera input
// Author:      Zhuoru Zhang
// Date:        2022/11/26

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

using NRKernal.Record;
using System;
using System.Linq;

using Unity.Barracuda;

namespace NRKernal.NRExamples
{
    /// <summary> A controller for handling camera captures. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/rgb-camera")]
    public class ShoeDetection : MonoBehaviour
    {
        /// <summary> The capture image. </summary>
        public RawImage CaptureImage;
        /// <summary> Number of frames. </summary>
        public Text FrameCount;

        /// <summary> Number of frames. </summary>

        Mat rgbaMat;


        int height;
        int width;
        Texture2D texture;
        /// <summary> Gets or sets the RGB camera texture. </summary>
        /// <value> The RGB camera texture. </value>'
        private NRRGBCamTexture RGBCamTexture { get; set; }

        GameObject RGBcamObject;
        Camera RGBCamera;

        private bool isWorking = false;

        public Detector detector;
        /// <summary> list of bounding boxes </summary>

        private IList<BoundingBox> boxOutlines;

        /// <summary> Asset for image </summary>

        public TextAsset imageAsset;
        
        /// <summary> the texture of input image </summary>

        Texture2D inputImage;

        public bool CameraStreaming;


        void Start()
        {
            RGBCamTexture = new NRRGBCamTexture();
            RGBCamTexture.Play();

            inputImage = new Texture2D(2, 2);
            inputImage.LoadImage(imageAsset.bytes);
            height = inputImage.height;
            width = inputImage.width;
            Debug.Log("heights: "+ height +"width: "+width);

            rgbaMat = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));

            texture = new Texture2D (width, height, TextureFormat.RGB24, false);

            RGBcamObject = GameObject.Find("RGBCamera");
            RGBCamera = RGBcamObject.GetComponent<Camera>();
        }

        /// <summary> Updates this object. </summary>
        void Update()
        {
            if (RGBCamTexture == null)
            {
                return;
            }
            TFDetect();

        }
        
        /// <summary> Start shoe detection </summary>
        private void TFDetect()
        {
            if (this.isWorking)
            {
                return;
            }

            this.isWorking = true;
            Texture2D tmpTexture;
            if (CameraStreaming){
                tmpTexture = RGBCamTexture.GetTexture();

            }
            else{
                //copy texture to cam
                tmpTexture = new Texture2D(this.width,this.height);
                tmpTexture.SetPixels(inputImage.GetPixels());
                tmpTexture.Apply();
            }

            //flip the image in x axis since the origin in texture2D is at top-left corner and origin in YOLO output
            //is at bottom-left corner
            Mat tmpMat = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
            Utils.texture2DToMat(tmpTexture,rgbaMat);
            Utils.texture2DToMat(tmpTexture,tmpMat,false);
            Utils.matToTexture2D (tmpMat, tmpTexture);

            var scaled = TextureTools.scaled(tmpTexture, Detector.IMAGE_SIZE, Detector.IMAGE_SIZE, FilterMode.Bilinear);
            var rotated = scaled.GetPixels32();
            StartCoroutine(this.detector.Detect(rotated, boxes =>
            {
                this.boxOutlines = boxes;
                Resources.UnloadUnusedAssets();
                this.isWorking = false;
            }));

            if(CameraStreaming){
                // Utils.texture2DToMat(RGBCamTexture.GetTexture(),rgbaMat);

            }
            else{
                Utils.texture2DToMat(inputImage,rgbaMat);

            }

            if (this.boxOutlines != null && this.boxOutlines.Any()){
                foreach (var outline in this.boxOutlines)
                {
                    //draw bounding box on rgbamat
                    DrawBoxOutline(outline,rgbaMat);
                }
            }
            Utils.matToTexture2D (rgbaMat, texture);
            CaptureImage.texture = texture;
        }
        
        /// <summary> Add bounding box to  </summary>

        void DrawBoxOutline(BoundingBox outline,Mat rgbaMat)
        {
            //image is resized to square size, resize result back to original pic size 
            var scaleFactorX = this.width/(float)Detector.IMAGE_SIZE;
            var x = (int)outline.Dimensions.X * scaleFactorX;
            var width = (int)outline.Dimensions.Width * scaleFactorX;

            var scaleFactorY = this.height/(float)Detector.IMAGE_SIZE;
            var y = (int)outline.Dimensions.Y * scaleFactorY;
            var height = (int)outline.Dimensions.Height * scaleFactorY;

            Imgproc.line (rgbaMat, new Point (x,y), new Point (x+width,y), new Scalar (255, 0, 0, 255), 2);
            Imgproc.line (rgbaMat, new Point (x+width,y), new Point (x+width,y+height), new Scalar (255, 0, 0, 255), 2);
            Imgproc.line (rgbaMat, new Point (x+width,y+height), new Point (x,y+height), new Scalar (255, 0, 0, 255), 2);
            Imgproc.line (rgbaMat, new Point (x,y+height), new Point (x,y), new Scalar (255, 0, 0, 255), 2);
            Imgproc.putText (rgbaMat, outline.Label+": "+outline.Confidence, new Point (x, y+height +10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar (255, 255, 255, 255), 2, Imgproc.LINE_AA, false);

        }
    }
}
