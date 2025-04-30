namespace RobotPainter.Application
{
    partial class ControlPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button_start = new Button();
            button_stop = new Button();
            label_progress = new Label();
            label_progressDisplay = new Label();
            label_currentLayer = new Label();
            label_currentLayerDisplay = new Label();
            label_timePassed = new Label();
            label_timePassedDisplay = new Label();
            label_timeEstCurrLayer = new Label();
            label_timeEstCurrLayerDisplay = new Label();
            label_timeEstAllLayers = new Label();
            label_timeEstAllLayersDisplay = new Label();
            SuspendLayout();
            // 
            // button_start
            // 
            button_start.Font = new Font("Segoe UI", 12F);
            button_start.Location = new Point(0, 270);
            button_start.Name = "button_start";
            button_start.Size = new Size(190, 30);
            button_start.TabIndex = 0;
            button_start.Text = "Start";
            button_start.UseVisualStyleBackColor = true;
            button_start.Click += button_start_Click;
            // 
            // button_stop
            // 
            button_stop.Font = new Font("Segoe UI", 12F);
            button_stop.Location = new Point(210, 270);
            button_stop.Name = "button_stop";
            button_stop.Size = new Size(190, 30);
            button_stop.TabIndex = 1;
            button_stop.Text = "Stop";
            button_stop.UseVisualStyleBackColor = true;
            // 
            // label_progress
            // 
            label_progress.AutoSize = true;
            label_progress.Font = new Font("Segoe UI", 12F);
            label_progress.Location = new Point(0, 0);
            label_progress.Name = "label_progress";
            label_progress.Size = new Size(74, 21);
            label_progress.TabIndex = 2;
            label_progress.Text = "Progress:";
            // 
            // label_progressDisplay
            // 
            label_progressDisplay.AutoSize = true;
            label_progressDisplay.Font = new Font("Segoe UI", 12F);
            label_progressDisplay.Location = new Point(0, 21);
            label_progressDisplay.Name = "label_progressDisplay";
            label_progressDisplay.Size = new Size(73, 21);
            label_progressDisplay.TabIndex = 3;
            label_progressDisplay.Text = "-/- layers";
            // 
            // label_currentLayer
            // 
            label_currentLayer.AutoSize = true;
            label_currentLayer.Font = new Font("Segoe UI", 12F);
            label_currentLayer.Location = new Point(0, 47);
            label_currentLayer.Name = "label_currentLayer";
            label_currentLayer.Size = new Size(104, 21);
            label_currentLayer.TabIndex = 4;
            label_currentLayer.Text = "Current layer:";
            // 
            // label_currentLayerDisplay
            // 
            label_currentLayerDisplay.AutoSize = true;
            label_currentLayerDisplay.Font = new Font("Segoe UI", 12F);
            label_currentLayerDisplay.Location = new Point(0, 68);
            label_currentLayerDisplay.Name = "label_currentLayerDisplay";
            label_currentLayerDisplay.Size = new Size(82, 21);
            label_currentLayerDisplay.TabIndex = 5;
            label_currentLayerDisplay.Text = "-/- strokes";
            // 
            // label_timePassed
            // 
            label_timePassed.AutoSize = true;
            label_timePassed.Font = new Font("Segoe UI", 12F);
            label_timePassed.Location = new Point(0, 94);
            label_timePassed.Name = "label_timePassed";
            label_timePassed.Size = new Size(99, 21);
            label_timePassed.TabIndex = 6;
            label_timePassed.Text = "Time passed:";
            // 
            // label_timePassedDisplay
            // 
            label_timePassedDisplay.AutoSize = true;
            label_timePassedDisplay.Font = new Font("Segoe UI", 12F);
            label_timePassedDisplay.Location = new Point(0, 115);
            label_timePassedDisplay.Name = "label_timePassedDisplay";
            label_timePassedDisplay.Size = new Size(82, 21);
            label_timePassedDisplay.TabIndex = 7;
            label_timePassedDisplay.Text = "--h:--m:--s";
            // 
            // label_timeEstCurrLayer
            // 
            label_timeEstCurrLayer.AutoSize = true;
            label_timeEstCurrLayer.Font = new Font("Segoe UI", 12F);
            label_timeEstCurrLayer.Location = new Point(0, 141);
            label_timeEstCurrLayer.Name = "label_timeEstCurrLayer";
            label_timeEstCurrLayer.Size = new Size(238, 21);
            label_timeEstCurrLayer.TabIndex = 8;
            label_timeEstCurrLayer.Text = "Time estimation for current level:";
            // 
            // label_timeEstCurrLayerDisplay
            // 
            label_timeEstCurrLayerDisplay.AutoSize = true;
            label_timeEstCurrLayerDisplay.Font = new Font("Segoe UI", 12F);
            label_timeEstCurrLayerDisplay.Location = new Point(0, 162);
            label_timeEstCurrLayerDisplay.Name = "label_timeEstCurrLayerDisplay";
            label_timeEstCurrLayerDisplay.Size = new Size(60, 21);
            label_timeEstCurrLayerDisplay.TabIndex = 9;
            label_timeEstCurrLayerDisplay.Text = "--h:--m";
            // 
            // label_timeEstAllLayers
            // 
            label_timeEstAllLayers.AutoSize = true;
            label_timeEstAllLayers.Font = new Font("Segoe UI", 12F);
            label_timeEstAllLayers.Location = new Point(0, 188);
            label_timeEstAllLayers.Name = "label_timeEstAllLayers";
            label_timeEstAllLayers.Size = new Size(213, 21);
            label_timeEstAllLayers.TabIndex = 10;
            label_timeEstAllLayers.Text = "Time estimatoin for all layers:";
            // 
            // label_timeEstAllLayersDisplay
            // 
            label_timeEstAllLayersDisplay.AutoSize = true;
            label_timeEstAllLayersDisplay.Font = new Font("Segoe UI", 12F);
            label_timeEstAllLayersDisplay.Location = new Point(0, 209);
            label_timeEstAllLayersDisplay.Name = "label_timeEstAllLayersDisplay";
            label_timeEstAllLayersDisplay.Size = new Size(60, 21);
            label_timeEstAllLayersDisplay.TabIndex = 11;
            label_timeEstAllLayersDisplay.Text = "--h:--m";
            // 
            // ControlPanel
            // 
            AutoScaleMode = AutoScaleMode.None;
            Controls.Add(label_timeEstAllLayersDisplay);
            Controls.Add(label_timeEstAllLayers);
            Controls.Add(label_timeEstCurrLayerDisplay);
            Controls.Add(label_timeEstCurrLayer);
            Controls.Add(label_timePassedDisplay);
            Controls.Add(label_timePassed);
            Controls.Add(label_currentLayerDisplay);
            Controls.Add(label_currentLayer);
            Controls.Add(label_progressDisplay);
            Controls.Add(label_progress);
            Controls.Add(button_stop);
            Controls.Add(button_start);
            MaximumSize = new Size(400, 300);
            MinimumSize = new Size(400, 300);
            Name = "ControlPanel";
            Size = new Size(400, 300);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_start;
        private Button button_stop;
        private Label label_progress;
        private Label label_progressDisplay;
        private Label label_currentLayer;
        private Label label_currentLayerDisplay;
        private Label label_timePassed;
        private Label label_timePassedDisplay;
        private Label label_timeEstCurrLayer;
        private Label label_timeEstCurrLayerDisplay;
        private Label label_timeEstAllLayers;
        private Label label_timeEstAllLayersDisplay;
    }
}
