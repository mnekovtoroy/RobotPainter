namespace RobotPainter.Application
{
    partial class ParametersPanel
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
            label_numOfLayers = new Label();
            label_canvasWidth = new Label();
            label_canvasHeight = new Label();
            textBox_cavasWidth = new TextBox();
            textBox_canvasHeight = new TextBox();
            textBox_numOfLayers = new TextBox();
            tabControl_layerTabs = new TabControl();
            button_propagateParams = new Button();
            button_calculatePrediction = new Button();
            SuspendLayout();
            // 
            // label_numOfLayers
            // 
            label_numOfLayers.AutoSize = true;
            label_numOfLayers.Font = new Font("Segoe UI", 9F);
            label_numOfLayers.Location = new Point(0, 61);
            label_numOfLayers.Name = "label_numOfLayers";
            label_numOfLayers.Size = new Size(101, 15);
            label_numOfLayers.TabIndex = 0;
            label_numOfLayers.Text = "Number of layers:";
            // 
            // label_canvasWidth
            // 
            label_canvasWidth.AutoSize = true;
            label_canvasWidth.Location = new Point(0, 3);
            label_canvasWidth.Name = "label_canvasWidth";
            label_canvasWidth.Size = new Size(114, 15);
            label_canvasWidth.TabIndex = 1;
            label_canvasWidth.Text = "Canvas width (mm):";
            // 
            // label_canvasHeight
            // 
            label_canvasHeight.AutoSize = true;
            label_canvasHeight.Location = new Point(0, 32);
            label_canvasHeight.Name = "label_canvasHeight";
            label_canvasHeight.Size = new Size(118, 15);
            label_canvasHeight.TabIndex = 2;
            label_canvasHeight.Text = "Canvas height (mm):";
            // 
            // textBox_cavasWidth
            // 
            textBox_cavasWidth.Font = new Font("Segoe UI", 9F);
            textBox_cavasWidth.Location = new Point(124, 0);
            textBox_cavasWidth.Name = "textBox_cavasWidth";
            textBox_cavasWidth.Size = new Size(50, 23);
            textBox_cavasWidth.TabIndex = 3;
            textBox_cavasWidth.Text = "400";
            textBox_cavasWidth.TextAlign = HorizontalAlignment.Center;
            textBox_cavasWidth.Validating += textBox_IntValidating;
            // 
            // textBox_canvasHeight
            // 
            textBox_canvasHeight.Font = new Font("Segoe UI", 9F);
            textBox_canvasHeight.Location = new Point(124, 29);
            textBox_canvasHeight.Name = "textBox_canvasHeight";
            textBox_canvasHeight.Size = new Size(50, 23);
            textBox_canvasHeight.TabIndex = 4;
            textBox_canvasHeight.Text = "300";
            textBox_canvasHeight.TextAlign = HorizontalAlignment.Center;
            textBox_canvasHeight.Validating += textBox_IntValidating;
            // 
            // textBox_numOfLayers
            // 
            textBox_numOfLayers.Font = new Font("Segoe UI", 9F);
            textBox_numOfLayers.Location = new Point(124, 58);
            textBox_numOfLayers.Name = "textBox_numOfLayers";
            textBox_numOfLayers.Size = new Size(50, 23);
            textBox_numOfLayers.TabIndex = 5;
            textBox_numOfLayers.Text = "3";
            textBox_numOfLayers.TextAlign = HorizontalAlignment.Center;
            textBox_numOfLayers.Validating += textBox_numOfLayers_Validating;
            // 
            // tabControl_layerTabs
            // 
            tabControl_layerTabs.Location = new Point(0, 87);
            tabControl_layerTabs.Name = "tabControl_layerTabs";
            tabControl_layerTabs.SelectedIndex = 0;
            tabControl_layerTabs.Size = new Size(320, 284);
            tabControl_layerTabs.TabIndex = 6;
            // 
            // button_propagateParams
            // 
            button_propagateParams.Location = new Point(-1, 377);
            button_propagateParams.Name = "button_propagateParams";
            button_propagateParams.Size = new Size(175, 23);
            button_propagateParams.TabIndex = 7;
            button_propagateParams.Text = "Propagate Layer 1 parameters";
            button_propagateParams.UseVisualStyleBackColor = true;
            button_propagateParams.Click += button_propagateParams_Click;
            // 
            // button_calculatePrediction
            // 
            button_calculatePrediction.Location = new Point(180, 377);
            button_calculatePrediction.Name = "button_calculatePrediction";
            button_calculatePrediction.Size = new Size(140, 23);
            button_calculatePrediction.TabIndex = 8;
            button_calculatePrediction.Text = "Calculate prediction";
            button_calculatePrediction.UseVisualStyleBackColor = true;
            button_calculatePrediction.Click += button_calculatePrediction_Click;
            // 
            // ParametersPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(button_calculatePrediction);
            Controls.Add(button_propagateParams);
            Controls.Add(tabControl_layerTabs);
            Controls.Add(textBox_numOfLayers);
            Controls.Add(textBox_canvasHeight);
            Controls.Add(textBox_cavasWidth);
            Controls.Add(label_canvasHeight);
            Controls.Add(label_canvasWidth);
            Controls.Add(label_numOfLayers);
            Name = "ParametersPanel";
            Size = new Size(320, 400);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_numOfLayers;
        private Label label_canvasWidth;
        private Label label_canvasHeight;
        private TextBox textBox_cavasWidth;
        private TextBox textBox_canvasHeight;
        private TextBox textBox_numOfLayers;
        private TabControl tabControl_layerTabs;
        private Button button_propagateParams;
        private Button button_calculatePrediction;
    }
}
