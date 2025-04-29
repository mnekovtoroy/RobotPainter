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
            tabPage_layer1 = new TabPage();
            layerParametersPanel1 = new LayerParametersPanel();
            tabPage_layer2 = new TabPage();
            button_propagateParams = new Button();
            button_buildPrediction = new Button();
            tabControl_layerTabs.SuspendLayout();
            tabPage_layer1.SuspendLayout();
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
            textBox_cavasWidth.PlaceholderText = "400";
            textBox_cavasWidth.Size = new Size(50, 23);
            textBox_cavasWidth.TabIndex = 3;
            textBox_cavasWidth.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox_canvasHeight
            // 
            textBox_canvasHeight.Font = new Font("Segoe UI", 9F);
            textBox_canvasHeight.Location = new Point(124, 29);
            textBox_canvasHeight.Name = "textBox_canvasHeight";
            textBox_canvasHeight.PlaceholderText = "300";
            textBox_canvasHeight.Size = new Size(50, 23);
            textBox_canvasHeight.TabIndex = 4;
            textBox_canvasHeight.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox_numOfLayers
            // 
            textBox_numOfLayers.Font = new Font("Segoe UI", 9F);
            textBox_numOfLayers.Location = new Point(124, 58);
            textBox_numOfLayers.Name = "textBox_numOfLayers";
            textBox_numOfLayers.PlaceholderText = "3";
            textBox_numOfLayers.Size = new Size(50, 23);
            textBox_numOfLayers.TabIndex = 5;
            textBox_numOfLayers.TextAlign = HorizontalAlignment.Center;
            // 
            // tabControl_layerTabs
            // 
            tabControl_layerTabs.Controls.Add(tabPage_layer1);
            tabControl_layerTabs.Controls.Add(tabPage_layer2);
            tabControl_layerTabs.Location = new Point(0, 87);
            tabControl_layerTabs.Name = "tabControl_layerTabs";
            tabControl_layerTabs.SelectedIndex = 0;
            tabControl_layerTabs.Size = new Size(320, 284);
            tabControl_layerTabs.TabIndex = 6;
            // 
            // tabPage_layer1
            // 
            tabPage_layer1.Controls.Add(layerParametersPanel1);
            tabPage_layer1.Location = new Point(4, 24);
            tabPage_layer1.Name = "tabPage_layer1";
            tabPage_layer1.Padding = new Padding(3);
            tabPage_layer1.Size = new Size(312, 256);
            tabPage_layer1.TabIndex = 0;
            tabPage_layer1.Text = "Layer 1";
            tabPage_layer1.UseVisualStyleBackColor = true;
            // 
            // layerParametersPanel1
            // 
            layerParametersPanel1.Location = new Point(0, 0);
            layerParametersPanel1.MaximumSize = new Size(312, 256);
            layerParametersPanel1.MinimumSize = new Size(312, 256);
            layerParametersPanel1.Name = "layerParametersPanel1";
            layerParametersPanel1.Size = new Size(312, 256);
            layerParametersPanel1.TabIndex = 0;
            // 
            // tabPage_layer2
            // 
            tabPage_layer2.Location = new Point(4, 24);
            tabPage_layer2.Name = "tabPage_layer2";
            tabPage_layer2.Padding = new Padding(3);
            tabPage_layer2.Size = new Size(312, 256);
            tabPage_layer2.TabIndex = 1;
            tabPage_layer2.Text = "Layer 2";
            tabPage_layer2.UseVisualStyleBackColor = true;
            // 
            // button_propagateParams
            // 
            button_propagateParams.Location = new Point(-1, 377);
            button_propagateParams.Name = "button_propagateParams";
            button_propagateParams.Size = new Size(175, 23);
            button_propagateParams.TabIndex = 7;
            button_propagateParams.Text = "Propagate Layer 1 parameters";
            button_propagateParams.UseVisualStyleBackColor = true;
            // 
            // button_buildPrediction
            // 
            button_buildPrediction.Location = new Point(180, 377);
            button_buildPrediction.Name = "button_buildPrediction";
            button_buildPrediction.Size = new Size(140, 23);
            button_buildPrediction.TabIndex = 8;
            button_buildPrediction.Text = "Build prediction";
            button_buildPrediction.UseVisualStyleBackColor = true;
            // 
            // ParametersPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(button_buildPrediction);
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
            tabControl_layerTabs.ResumeLayout(false);
            tabPage_layer1.ResumeLayout(false);
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
        private TabPage tabPage_layer1;
        private TabPage tabPage_layer2;
        private Button button_propagateParams;
        private LayerParametersPanel layerParametersPanel1;
        private Button button_buildPrediction;
    }
}
