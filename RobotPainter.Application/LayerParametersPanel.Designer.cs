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
            label_ssbOptions = new Label();
            label_maxStrokeLength = new Label();
            textBox_maxStrokeLength = new TextBox();
            textBox_L_tol = new TextBox();
            label_L_tol = new Label();
            textBox_maxNormAngle = new TextBox();
            label_maxNormAngle = new Label();
            textBox_maxTurnAngle = new TextBox();
            label_maxTurnAngle = new Label();
            label_bsbOptions = new Label();
            textBox_maxStrokeWidth = new TextBox();
            label_maxStrokeWidth = new Label();
            textBox_overlap = new TextBox();
            label_overlap = new Label();
            textBox_startOverheadCoeff = new TextBox();
            label_startOverheadCoeff = new Label();
            textBox_endOverheadCoeff = new TextBox();
            label_endOverheadCoeff = new Label();
            textBox_safeHeight = new TextBox();
            label_safeHeight = new Label();
            textBox_startRunawayAngle = new TextBox();
            label_startRunawayAngle = new Label();
            textBox_endRunawayAngle = new TextBox();
            label_endRunawayAngle = new Label();
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
            comboBox_brushModel.Location = new Point(86, 3);
            comboBox_brushModel.Name = "comboBox_brushModel";
            comboBox_brushModel.Size = new Size(121, 23);
            comboBox_brushModel.TabIndex = 1;
            // 
            // label_ssbOptions
            // 
            label_ssbOptions.AutoSize = true;
            label_ssbOptions.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label_ssbOptions.Location = new Point(3, 34);
            label_ssbOptions.Name = "label_ssbOptions";
            label_ssbOptions.Size = new Size(154, 15);
            label_ssbOptions.TabIndex = 2;
            label_ssbOptions.Text = "StrokeSiteBuilder options:";
            // 
            // label_maxStrokeLength
            // 
            label_maxStrokeLength.AutoSize = true;
            label_maxStrokeLength.Location = new Point(3, 63);
            label_maxStrokeLength.Name = "label_maxStrokeLength";
            label_maxStrokeLength.Size = new Size(138, 15);
            label_maxStrokeLength.TabIndex = 3;
            label_maxStrokeLength.Text = "Max stroke length (mm):";
            // 
            // textBox_maxStrokeLength
            // 
            textBox_maxStrokeLength.Location = new Point(147, 60);
            textBox_maxStrokeLength.Name = "textBox_maxStrokeLength";
            textBox_maxStrokeLength.Size = new Size(145, 23);
            textBox_maxStrokeLength.TabIndex = 4;
            textBox_maxStrokeLength.Validating += textBox_DoubleValidating;
            // 
            // textBox_L_tol
            // 
            textBox_L_tol.Location = new Point(147, 89);
            textBox_L_tol.Name = "textBox_L_tol";
            textBox_L_tol.Size = new Size(145, 23);
            textBox_L_tol.TabIndex = 6;
            textBox_L_tol.Validating += textBox_DoubleValidating;
            // 
            // label_L_tol
            // 
            label_L_tol.AutoSize = true;
            label_L_tol.Location = new Point(106, 92);
            label_L_tol.Name = "label_L_tol";
            label_L_tol.Size = new Size(35, 15);
            label_L_tol.TabIndex = 5;
            label_L_tol.Text = "L_tol:";
            // 
            // textBox_maxNormAngle
            // 
            textBox_maxNormAngle.Location = new Point(147, 118);
            textBox_maxNormAngle.Name = "textBox_maxNormAngle";
            textBox_maxNormAngle.Size = new Size(145, 23);
            textBox_maxNormAngle.TabIndex = 8;
            textBox_maxNormAngle.Validating += textBox_DoubleValidating;
            // 
            // label_maxNormAngle
            // 
            label_maxNormAngle.AutoSize = true;
            label_maxNormAngle.Location = new Point(44, 121);
            label_maxNormAngle.Name = "label_maxNormAngle";
            label_maxNormAngle.Size = new Size(97, 15);
            label_maxNormAngle.TabIndex = 7;
            label_maxNormAngle.Text = "Max norm angle:";
            // 
            // textBox_maxTurnAngle
            // 
            textBox_maxTurnAngle.Location = new Point(147, 147);
            textBox_maxTurnAngle.Name = "textBox_maxTurnAngle";
            textBox_maxTurnAngle.Size = new Size(145, 23);
            textBox_maxTurnAngle.TabIndex = 10;
            textBox_maxTurnAngle.Validating += textBox_DoubleValidating;
            // 
            // label_maxTurnAngle
            // 
            label_maxTurnAngle.AutoSize = true;
            label_maxTurnAngle.Location = new Point(51, 150);
            label_maxTurnAngle.Name = "label_maxTurnAngle";
            label_maxTurnAngle.Size = new Size(90, 15);
            label_maxTurnAngle.TabIndex = 9;
            label_maxTurnAngle.Text = "Max turn angle:";
            // 
            // label_bsbOptions
            // 
            label_bsbOptions.AutoSize = true;
            label_bsbOptions.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label_bsbOptions.Location = new Point(3, 179);
            label_bsbOptions.Name = "label_bsbOptions";
            label_bsbOptions.Size = new Size(162, 15);
            label_bsbOptions.TabIndex = 11;
            label_bsbOptions.Text = "BrushstrokeBuilder options:";
            // 
            // textBox_maxStrokeWidth
            // 
            textBox_maxStrokeWidth.Location = new Point(147, 205);
            textBox_maxStrokeWidth.Name = "textBox_maxStrokeWidth";
            textBox_maxStrokeWidth.Size = new Size(145, 23);
            textBox_maxStrokeWidth.TabIndex = 13;
            textBox_maxStrokeWidth.Validating += textBox_DoubleValidating;
            // 
            // label_maxStrokeWidth
            // 
            label_maxStrokeWidth.AutoSize = true;
            label_maxStrokeWidth.Location = new Point(7, 208);
            label_maxStrokeWidth.Name = "label_maxStrokeWidth";
            label_maxStrokeWidth.Size = new Size(134, 15);
            label_maxStrokeWidth.TabIndex = 12;
            label_maxStrokeWidth.Text = "Max stroke width (mm):";
            // 
            // textBox_overlap
            // 
            textBox_overlap.Location = new Point(147, 234);
            textBox_overlap.Name = "textBox_overlap";
            textBox_overlap.Size = new Size(145, 23);
            textBox_overlap.TabIndex = 15;
            textBox_overlap.Validating += textBox_DoubleValidating;
            // 
            // label_overlap
            // 
            label_overlap.AutoSize = true;
            label_overlap.Location = new Point(57, 237);
            label_overlap.Name = "label_overlap";
            label_overlap.Size = new Size(84, 15);
            label_overlap.TabIndex = 14;
            label_overlap.Text = "Overlap (mm):";
            // 
            // textBox_startOverheadCoeff
            // 
            textBox_startOverheadCoeff.Location = new Point(147, 263);
            textBox_startOverheadCoeff.Name = "textBox_startOverheadCoeff";
            textBox_startOverheadCoeff.Size = new Size(145, 23);
            textBox_startOverheadCoeff.TabIndex = 17;
            textBox_startOverheadCoeff.Validating += textBox_DoubleValidating;
            // 
            // label_startOverheadCoeff
            // 
            label_startOverheadCoeff.AutoSize = true;
            label_startOverheadCoeff.Location = new Point(25, 266);
            label_startOverheadCoeff.Name = "label_startOverheadCoeff";
            label_startOverheadCoeff.Size = new Size(116, 15);
            label_startOverheadCoeff.TabIndex = 16;
            label_startOverheadCoeff.Text = "Start overhead coeff:";
            // 
            // textBox_endOverheadCoeff
            // 
            textBox_endOverheadCoeff.Location = new Point(147, 292);
            textBox_endOverheadCoeff.Name = "textBox_endOverheadCoeff";
            textBox_endOverheadCoeff.Size = new Size(145, 23);
            textBox_endOverheadCoeff.TabIndex = 19;
            textBox_endOverheadCoeff.Validating += textBox_DoubleValidating;
            // 
            // label_endOverheadCoeff
            // 
            label_endOverheadCoeff.AutoSize = true;
            label_endOverheadCoeff.Location = new Point(29, 295);
            label_endOverheadCoeff.Name = "label_endOverheadCoeff";
            label_endOverheadCoeff.Size = new Size(112, 15);
            label_endOverheadCoeff.TabIndex = 18;
            label_endOverheadCoeff.Text = "End overhead coeff:";
            // 
            // textBox_safeHeight
            // 
            textBox_safeHeight.Location = new Point(147, 321);
            textBox_safeHeight.Name = "textBox_safeHeight";
            textBox_safeHeight.Size = new Size(145, 23);
            textBox_safeHeight.TabIndex = 21;
            textBox_safeHeight.Validating += textBox_DoubleValidating;
            // 
            // label_safeHeight
            // 
            label_safeHeight.AutoSize = true;
            label_safeHeight.Location = new Point(72, 324);
            label_safeHeight.Name = "label_safeHeight";
            label_safeHeight.Size = new Size(69, 15);
            label_safeHeight.TabIndex = 20;
            label_safeHeight.Text = "Safe height:";
            // 
            // textBox_startRunawayAngle
            // 
            textBox_startRunawayAngle.Location = new Point(147, 350);
            textBox_startRunawayAngle.Name = "textBox_startRunawayAngle";
            textBox_startRunawayAngle.Size = new Size(145, 23);
            textBox_startRunawayAngle.TabIndex = 23;
            textBox_startRunawayAngle.Validating += textBox_DoubleValidating;
            // 
            // label_startRunawayAngle
            // 
            label_startRunawayAngle.AutoSize = true;
            label_startRunawayAngle.Location = new Point(27, 353);
            label_startRunawayAngle.Name = "label_startRunawayAngle";
            label_startRunawayAngle.Size = new Size(114, 15);
            label_startRunawayAngle.TabIndex = 22;
            label_startRunawayAngle.Text = "Start runaway angle:";
            // 
            // textBox_endRunawayAngle
            // 
            textBox_endRunawayAngle.Location = new Point(147, 379);
            textBox_endRunawayAngle.Name = "textBox_endRunawayAngle";
            textBox_endRunawayAngle.Size = new Size(145, 23);
            textBox_endRunawayAngle.TabIndex = 25;
            textBox_endRunawayAngle.Validating += textBox_DoubleValidating;
            // 
            // label_endRunawayAngle
            // 
            label_endRunawayAngle.AutoSize = true;
            label_endRunawayAngle.Location = new Point(31, 382);
            label_endRunawayAngle.Name = "label_endRunawayAngle";
            label_endRunawayAngle.Size = new Size(110, 15);
            label_endRunawayAngle.TabIndex = 24;
            label_endRunawayAngle.Text = "End runaway angle:";
            // 
            // LayerParametersPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            Controls.Add(textBox_endRunawayAngle);
            Controls.Add(label_endRunawayAngle);
            Controls.Add(textBox_startRunawayAngle);
            Controls.Add(label_startRunawayAngle);
            Controls.Add(textBox_safeHeight);
            Controls.Add(label_safeHeight);
            Controls.Add(textBox_endOverheadCoeff);
            Controls.Add(label_endOverheadCoeff);
            Controls.Add(textBox_startOverheadCoeff);
            Controls.Add(label_startOverheadCoeff);
            Controls.Add(textBox_overlap);
            Controls.Add(label_overlap);
            Controls.Add(textBox_maxStrokeWidth);
            Controls.Add(label_maxStrokeWidth);
            Controls.Add(label_bsbOptions);
            Controls.Add(textBox_maxTurnAngle);
            Controls.Add(label_maxTurnAngle);
            Controls.Add(textBox_maxNormAngle);
            Controls.Add(label_maxNormAngle);
            Controls.Add(textBox_L_tol);
            Controls.Add(label_L_tol);
            Controls.Add(textBox_maxStrokeLength);
            Controls.Add(label_maxStrokeLength);
            Controls.Add(label_ssbOptions);
            Controls.Add(comboBox_brushModel);
            Controls.Add(label_brushModel);
            MaximumSize = new Size(295, 0);
            MinimumSize = new Size(259, 0);
            Name = "LayerParametersPanel";
            Size = new Size(295, 415);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_brushModel;
        private ComboBox comboBox_brushModel;
        private Label label_ssbOptions;
        private Label label_maxStrokeLength;
        private TextBox textBox_maxStrokeLength;
        private TextBox textBox_L_tol;
        private Label label_L_tol;
        private TextBox textBox_maxNormAngle;
        private Label label_maxNormAngle;
        private TextBox textBox_maxTurnAngle;
        private Label label_maxTurnAngle;
        private Label label_bsbOptions;
        private TextBox textBox_maxStrokeWidth;
        private Label label_maxStrokeWidth;
        private TextBox textBox_overlap;
        private Label label_overlap;
        private TextBox textBox_startOverheadCoeff;
        private Label label_startOverheadCoeff;
        private TextBox textBox_endOverheadCoeff;
        private Label label_endOverheadCoeff;
        private TextBox textBox_safeHeight;
        private Label label_safeHeight;
        private TextBox textBox_startRunawayAngle;
        private Label label_startRunawayAngle;
        private TextBox textBox_endRunawayAngle;
        private Label label_endRunawayAngle;
    }
}
