using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.Drawing;
using System.Windows.Forms;
using System;
using SnowTurret.Detection;

namespace SnowTurret
{
    public partial class CameraFeed
    {
        private System.ComponentModel.IContainer components = null;

        private VideoCapture _capture;
        private Mat _frame;
        private long _processingTime;

        public CameraFeed()
        {
            InitializeComponent();

            _capture = new VideoCapture();
            _frame = new Mat();

            _capture.ImageGrabbed += Capture_ImageGrabbed;
            button1.Click += FireButton_Click;

            if (_capture != null)
            {
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
                _capture.Retrieve(_frame, 0);
                pictureBox1.Image = FindPerson.Find(_frame.ToImage<Bgr, Byte>(), out _processingTime).ToBitmap();
                //label1.Text = "Processing Time: " + _processingTime.ToString();
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
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
            this.ClientSize = new System.Drawing.Size(1269, 723);
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