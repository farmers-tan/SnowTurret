using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.Drawing;
using System.Windows.Forms;
using System;
using SnowTurret.Detection;
using Emgu.CV.CvEnum;

namespace SnowTurret
{
    public partial class CameraFeed
    {
        private System.ComponentModel.IContainer components = null;

        // Determines boundary of brightness while turning grayscale image to binary (black-white) image
        private const int Threshold = 5;

        // Erosion to remove noise (reduce white pixel zones)
        private const int ErodeIterations = 3;

        // Dilation to enhance erosion survivors (enlarge white pixel zones)
        private const int DilateIterations = 3;

        private VideoCapture _capture;
        private Mat _frame;
        private long _processingTime;

        private static Mat rawFrame = new Mat(); // Frame as obtained from video
        private static Mat backgroundFrame = new Mat(); // Frame used as base for change detection
        private static Mat diffFrame = new Mat(); // Image showing differences between background and raw frame
        private static Mat grayscaleDiffFrame = new Mat(); // Image showing differences in 8-bit color depth
        private static Mat binaryDiffFrame = new Mat(); // Image showing changed areas in white and unchanged in black
        private static Mat denoisedDiffFrame = new Mat(); // Image with irrelevant changes removed with opening operation
        private static Mat finalFrame = new Mat(); // Video frame with detected object marked

        public CameraFeed()
        {
            InitializeComponent();

            _capture = new VideoCapture(0);
            _frame = new Mat();

            _capture.ImageGrabbed += Capture_ImageGrabbed;
            button1.Click += FireButton_Click;

            if (_capture != null)
            {
                backgroundFrame = _capture.QueryFrame();
                _capture.Start();
            }
        }

        private void FireButton_Click(object sender, EventArgs e)
        {
            //code to fire the cannon once I get around to that and such
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                //_capture.Retrieve(_frame, 0);
                //pictureBox1.Image = FindPerson.Find(_frame.ToImage<Bgr, Byte>(), out _processingTime).ToBitmap();

                _capture.Retrieve(rawFrame, 0);
                if (rawFrame != null)
                {
                    ProcessFrame(backgroundFrame, Threshold, ErodeIterations, DilateIterations);
                }
            }
        }

        private static void ProcessFrame(Mat backgroundFrame, int threshold, int erodeIterations, int dilateIterations)
        {
            // Find difference between background (first) frame and current frame
            CvInvoke.AbsDiff(backgroundFrame, rawFrame, diffFrame);

            // Apply binary threshold to grayscale image (white pixel will mark difference)
            CvInvoke.CvtColor(diffFrame, grayscaleDiffFrame, ColorConversion.Bgr2Gray);
            CvInvoke.Threshold(grayscaleDiffFrame, binaryDiffFrame, threshold, 255, ThresholdType.Binary);

            // Remove noise with opening operation (erosion followed by dilation)
            CvInvoke.Erode(binaryDiffFrame, denoisedDiffFrame, null, new Point(-1, -1), erodeIterations, BorderType.Default, new MCvScalar(1));
            CvInvoke.Dilate(denoisedDiffFrame, denoisedDiffFrame, null, new Point(-1, -1), dilateIterations, BorderType.Default, new MCvScalar(1));

            rawFrame.CopyTo(finalFrame);
            FindPerson.DetectObject(denoisedDiffFrame, finalFrame);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1245, 603);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 621);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(225, 90);
            this.button1.TabIndex = 1;
            this.button1.Text = "Fire!";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // CameraFeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 717);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "CameraFeed";
            this.Text = "CameraFeed";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox pictureBox1;
        private Button button1;
    }
}