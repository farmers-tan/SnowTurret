using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Diagnostics;
using Emgu.CV.Cuda;

namespace SnowTurret.Detection
{
    public static class FindPerson
    {
        /// <summary>
        /// Find the pedestrian in the image
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="processingTime">The pedestrian detection time in milliseconds</param>
        /// <returns>The image with pedestrian highlighted.</returns>
        public static Image<Bgr, Byte> Find(Image<Bgr, Byte> image, out long processingTime)
        {
            Stopwatch watch;
            MCvObjectDetection[] regions;

            //check if there is a compatible GPU to run pedestrian detection
            if (CudaInvoke.HasCuda)
            {  //this is the GPU version
                using (CudaHOG des = new CudaHOG(new System.Drawing.Size(64, 128), new Size(16, 16), new Size(8, 8), new Size(8, 8)))
                {
                    des.SetSVMDetector(des.GetDefaultPeopleDetector());

                    watch = Stopwatch.StartNew();
                    using (CudaImage<Bgr, Byte> gpuImg = new CudaImage<Bgr, byte>(image))
                    using (CudaImage<Bgra, Byte> gpuBgra = gpuImg.Convert<Bgra, Byte>())
                    {
                        regions = des.DetectMultiScale(gpuBgra);
                    }
                }
            }
            else
            {  //this is the CPU version
                using (HOGDescriptor des = new HOGDescriptor())
                {
                    des.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

                    watch = Stopwatch.StartNew();
                    regions = des.DetectMultiScale(image);
                }
            }
            watch.Stop();

            processingTime = watch.ElapsedMilliseconds;

            foreach (MCvObjectDetection pedestrain in regions)
            {
                image.Draw(pedestrain.Rect, new Bgr(Color.Red), 1);
            }
            return image;
        }
    }
}
