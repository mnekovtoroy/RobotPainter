namespace RobotPainter.Application
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox_sourceImage = new PictureBox();
            pictureBox_prediction = new PictureBox();
            pictureBox_lastPhoto = new PictureBox();
            label_sourceImage = new Label();
            label_lastPhoto = new Label();
            label_Prediction = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox_sourceImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox_prediction).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox_lastPhoto).BeginInit();
            SuspendLayout();
            // 
            // pictureBox_sourceImage
            // 
            pictureBox_sourceImage.BackColor = SystemColors.ControlLightLight;
            pictureBox_sourceImage.Location = new Point(40, 40);
            pictureBox_sourceImage.Name = "pictureBox_sourceImage";
            pictureBox_sourceImage.Size = new Size(400, 300);
            pictureBox_sourceImage.TabIndex = 0;
            pictureBox_sourceImage.TabStop = false;
            // 
            // pictureBox_prediction
            // 
            pictureBox_prediction.BackColor = SystemColors.ControlLightLight;
            pictureBox_prediction.Location = new Point(480, 40);
            pictureBox_prediction.Name = "pictureBox_prediction";
            pictureBox_prediction.Size = new Size(400, 300);
            pictureBox_prediction.TabIndex = 1;
            pictureBox_prediction.TabStop = false;
            // 
            // pictureBox_lastPhoto
            // 
            pictureBox_lastPhoto.BackColor = SystemColors.ControlLightLight;
            pictureBox_lastPhoto.Location = new Point(40, 380);
            pictureBox_lastPhoto.Name = "pictureBox_lastPhoto";
            pictureBox_lastPhoto.Size = new Size(400, 300);
            pictureBox_lastPhoto.TabIndex = 2;
            pictureBox_lastPhoto.TabStop = false;
            // 
            // label_sourceImage
            // 
            label_sourceImage.AutoSize = true;
            label_sourceImage.Font = new Font("Segoe UI", 12F);
            label_sourceImage.Location = new Point(40, 16);
            label_sourceImage.Name = "label_sourceImage";
            label_sourceImage.Size = new Size(108, 21);
            label_sourceImage.TabIndex = 3;
            label_sourceImage.Text = "Source image:";
            // 
            // label_lastPhoto
            // 
            label_lastPhoto.AutoSize = true;
            label_lastPhoto.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label_lastPhoto.Location = new Point(40, 356);
            label_lastPhoto.Name = "label_lastPhoto";
            label_lastPhoto.Size = new Size(86, 21);
            label_lastPhoto.TabIndex = 4;
            label_lastPhoto.Text = "Last photo:";
            // 
            // label_Prediction
            // 
            label_Prediction.AutoSize = true;
            label_Prediction.Font = new Font("Segoe UI", 12F);
            label_Prediction.Location = new Point(480, 16);
            label_Prediction.Name = "label_Prediction";
            label_Prediction.Size = new Size(83, 21);
            label_Prediction.TabIndex = 5;
            label_Prediction.Text = "Prediction:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1064, 721);
            Controls.Add(label_Prediction);
            Controls.Add(label_lastPhoto);
            Controls.Add(label_sourceImage);
            Controls.Add(pictureBox_lastPhoto);
            Controls.Add(pictureBox_prediction);
            Controls.Add(pictureBox_sourceImage);
            Name = "MainForm";
            Text = "RobotPainter";
            ((System.ComponentModel.ISupportInitialize)pictureBox_sourceImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox_prediction).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox_lastPhoto).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox_sourceImage;
        private PictureBox pictureBox_prediction;
        private PictureBox pictureBox_lastPhoto;
        private Label label_sourceImage;
        private Label label_lastPhoto;
        private Label label_Prediction;
    }
}
