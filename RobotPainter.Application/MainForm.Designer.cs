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
            controlPanel = new ControlPanel();
            parametersPanel = new ParametersPanel();
            label_parameters = new Label();
            consoleControl = new ConsoleControl();
            label_consoleOutput = new Label();
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
            pictureBox_sourceImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox_sourceImage.TabIndex = 0;
            pictureBox_sourceImage.TabStop = false;
            pictureBox_sourceImage.DoubleClick += pictureBox_DoubleClick;
            // 
            // pictureBox_prediction
            // 
            pictureBox_prediction.BackColor = SystemColors.ControlLightLight;
            pictureBox_prediction.Location = new Point(480, 40);
            pictureBox_prediction.Name = "pictureBox_prediction";
            pictureBox_prediction.Size = new Size(400, 300);
            pictureBox_prediction.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox_prediction.TabIndex = 1;
            pictureBox_prediction.TabStop = false;
            pictureBox_prediction.DoubleClick += pictureBox_DoubleClick;
            // 
            // pictureBox_lastPhoto
            // 
            pictureBox_lastPhoto.BackColor = SystemColors.ControlLightLight;
            pictureBox_lastPhoto.Location = new Point(40, 380);
            pictureBox_lastPhoto.Name = "pictureBox_lastPhoto";
            pictureBox_lastPhoto.Size = new Size(400, 300);
            pictureBox_lastPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox_lastPhoto.TabIndex = 2;
            pictureBox_lastPhoto.TabStop = false;
            pictureBox_lastPhoto.DoubleClick += pictureBox_DoubleClick;
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
            // controlPanel
            // 
            controlPanel.Location = new Point(480, 380);
            controlPanel.MaximumSize = new Size(400, 300);
            controlPanel.MinimumSize = new Size(400, 300);
            controlPanel.Name = "controlPanel";
            controlPanel.Size = new Size(400, 300);
            controlPanel.TabIndex = 6;
            // 
            // parametersPanel
            // 
            parametersPanel.Location = new Point(920, 40);
            parametersPanel.Name = "parametersPanel";
            parametersPanel.NumOfLayers = 3;
            parametersPanel.Size = new Size(320, 400);
            parametersPanel.TabIndex = 7;
            // 
            // label_parameters
            // 
            label_parameters.AutoSize = true;
            label_parameters.Font = new Font("Segoe UI", 12F);
            label_parameters.Location = new Point(920, 16);
            label_parameters.Name = "label_parameters";
            label_parameters.Size = new Size(91, 21);
            label_parameters.TabIndex = 8;
            label_parameters.Text = "Parameters:";
            // 
            // consoleControl
            // 
            consoleControl.Location = new Point(920, 480);
            consoleControl.MaximumSize = new Size(320, 200);
            consoleControl.MinimumSize = new Size(320, 200);
            consoleControl.Name = "consoleControl";
            consoleControl.Size = new Size(320, 200);
            consoleControl.TabIndex = 9;
            // 
            // label_consoleOutput
            // 
            label_consoleOutput.AutoSize = true;
            label_consoleOutput.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label_consoleOutput.Location = new Point(920, 456);
            label_consoleOutput.Name = "label_consoleOutput";
            label_consoleOutput.Size = new Size(119, 21);
            label_consoleOutput.TabIndex = 10;
            label_consoleOutput.Text = "Console output:";
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1283, 720);
            Controls.Add(label_consoleOutput);
            Controls.Add(consoleControl);
            Controls.Add(label_parameters);
            Controls.Add(parametersPanel);
            Controls.Add(controlPanel);
            Controls.Add(label_Prediction);
            Controls.Add(label_lastPhoto);
            Controls.Add(label_sourceImage);
            Controls.Add(pictureBox_lastPhoto);
            Controls.Add(pictureBox_prediction);
            Controls.Add(pictureBox_sourceImage);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "RobotPainter";
            Load += MainForm_Load;
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
        private ControlPanel controlPanel;
        private ParametersPanel parametersPanel;
        private Label label_parameters;
        private ConsoleControl consoleControl;
        private Label label_consoleOutput;
    }
}
