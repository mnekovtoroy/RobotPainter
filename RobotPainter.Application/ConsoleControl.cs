using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotPainter.Application
{
    public partial class ConsoleControl : UserControl
    {
        public class TextBoxWriter : TextWriter
        {
            private TextBox _textBox;

            public int MaxChars { get; set; } = 2048;

            public TextBoxWriter(TextBox textBox)
            {
                _textBox = textBox;
            }

            public override Encoding Encoding => Encoding.UTF8;

            public override void Write(char value)
            {
                AppendToTextBox(value);
            }

            public override void Write(string? value)
            {
                AppendToTextBox(value);
            }

            public override void WriteLine(string? value)
            {
                AppendToTextBox(value + NewLine);
            }

            private void AppendToTextBox(char value)
            {
                AppendToTextBox(value.ToString());
            }

            private void AppendToTextBox(string? value)
            {
                _textBox.Invoke(() =>
                {
                    if (value != null && _textBox.TextLength + value.Length > MaxChars)
                    {
                        _textBox.Text = _textBox.Text.Remove(0, value.Length - (MaxChars - _textBox.TextLength));
                    }
                    _textBox.AppendText(value);
                });
            }
        }

        public ConsoleControl()
        {
            InitializeComponent();
            Console.SetOut(new TextBoxWriter(textBox_consoleOutput));
        }

        private void ConsoleControl_ControlRemoved(object sender, ControlEventArgs e)
        {
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
        }
    }
}
