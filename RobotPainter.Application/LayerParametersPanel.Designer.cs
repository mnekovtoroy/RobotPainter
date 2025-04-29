namespace RobotPainter.Application
{
    partial class LayerParametersPanel
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
            label_brushModel = new Label();
            comboBox_brushModel = new ComboBox();
            SuspendLayout();
            // 
            // label_brushModel
            // 
            label_brushModel.AutoSize = true;
            label_brushModel.Location = new Point(3, 6);
            label_brushModel.Name = "label_brushModel";
            label_brushModel.Size = new Size(77, 15);
            label_brushModel.TabIndex = 0;
            label_brushModel.Text = "Brush model:";
            // 
            // comboBox_brushModel
            // 
            comboBox_brushModel.FormattingEnabled = true;
            comboBox_brushModel.Items.AddRange(new object[] { "Malevich 6" });
            comboBox_brushModel.Location = new Point(86, 3);
            comboBox_brushModel.Name = "comboBox_brushModel";
            comboBox_brushModel.Size = new Size(121, 23);
            comboBox_brushModel.TabIndex = 1;
            // 
            // LayerParametersPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(comboBox_brushModel);
            Controls.Add(label_brushModel);
            MaximumSize = new Size(312, 256);
            MinimumSize = new Size(312, 256);
            Name = "LayerParametersPanel";
            Size = new Size(312, 256);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_brushModel;
        private ComboBox comboBox_brushModel;
    }
}
