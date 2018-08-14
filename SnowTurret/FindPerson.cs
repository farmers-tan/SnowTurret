using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Diagnostics;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace SnowTurret.Detection
{
    public static class FindPerson
    {
        private static int frameNumber = 0;
        private static Stopwatch watch = new Stopwatch();
        private static MCvScalar drawingColor = new Bgr(Color.Red).MCvScalar;

        public static Image<Bgr, Byte> Find(Image<Bgr, Byte> image, out long processingTime)
        {
            Stopwatch watch;
            Rectangle[] regions;

            /*if (CudaInvoke.HasCuda)
            {
                using (CudaCascadeClassifier des = new CudaCascadeClassifier(@"C:\Users\abied\Downloads\opencv-master\opencv-master\data\haarcascades_cuda\haarcascade_frontalface_default.xml"))
                {
                    watch = Stopwatch.StartNew();
                    using (CudaImage<Bgr, Byte> gpuImg = new CudaImage<Bgr, byte>(image))
                    using (CudaImage<Bgra, Byte> gpuBgra = gpuImg.Convert<Bgra, Byte>())
                    {
                        MCvObjectDetection outArray = new MCvObjectDetection();
                        des.DetectMultiScale(gpuBgra, outArray);
                    }
                }
            }
            else
            {*/
                using (CascadeClassifier des = new CascadeClassifier(@"C:\Users\abied\Downloads\opencv-master\opencv-master\data\haarcascades\haarcascade_frontalface_default.xml"))
                {
                    watch = Stopwatch.StartNew();
                    regions = des.DetectMultiScale(image);
                }
            //}
            watch.Stop();

            processingTime = watch.ElapsedMilliseconds;

            foreach (Rectangle pedestrain in regions)
            {
                image.Draw(pedestrain, new Bgr(Color.Red), 1);
            }
            return image;
        }

        public static void DetectObject(Mat detectionFrame, Mat displayFrame)
        {
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                // Build list of contours
                CvInvoke.FindContours(detectionFrame, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                // Selecting largest contour
                if (contours.Size > 0)
                {
                    double maxArea = 0;
                    int chosen = 0;
                    for (int i = 0; i < contours.Size; i++)
                    {
                        VectorOfPoint contour = contours[i];

                        double area = CvInvoke.ContourArea(contour);
                        if (area > maxArea)
                        {
                            maxArea = area;
                            chosen = i;
                        }
                    }

                    // Draw on a frame
                    MarkDetectedObject(displayFrame, contours[chosen], maxArea);
                }
            }
        }

        private static void MarkDetectedObject(Mat frame, VectorOfPoint contour, double area)
        {
            // Getting minimal rectangle which contains the contour
            Rectangle box = CvInvoke.BoundingRectangle(contour);

            // Drawing contour and box around it
            CvInvoke.Polylines(frame, contour, true, drawingColor);
            CvInvoke.Rectangle(frame, box, drawingColor);

            // Write information next to marked object
            Point center = new Point(box.X + box.Width / 2, box.Y + box.Height / 2);

            var info = new string[] {
                $"Area: {area}",
                $"Position: {center.X}, {center.Y}"
            };

            //WriteMultilineText(frame, info, new Point(box.Right + 5, center.Y));
        }
    }
}
