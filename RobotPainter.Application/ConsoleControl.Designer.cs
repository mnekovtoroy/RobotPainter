namespace RobotPainter.Application
{
    partial class ConsoleControl
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
            textBox_consoleOutput = new TextBox();
            SuspendLayout();
            // 
            // textBox_consoleOutput
            // 
            textBox_consoleOutput.BackColor = SystemColors.ControlLight;
            textBox_consoleOutput.Font = new Font("Segoe UI Variable Display", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBox_consoleOutput.Location = new Point(0, 0);
            textBox_consoleOutput.Multiline = true;
            textBox_consoleOutput.Name = "textBox_consoleOutput";
            textBox_consoleOutput.ReadOnly = true;
            textBox_consoleOutput.ScrollBars = ScrollBars.Vertical;
            textBox_consoleOutput.Size = new Size(320, 200);
            textBox_consoleOutput.TabIndex = 0;
            // 
            // ConsoleControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(textBox_consoleOutput);
            MaximumSize = new Size(320, 200);
            MinimumSize = new Size(320, 200);
            Name = "ConsoleControl";
            Size = new Size(320, 200);
            ControlRemoved += ConsoleControl_ControlRemoved;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox_consoleOutput;
    }
}
