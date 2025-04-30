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
    public partial class ControlPanel : UserControl
    {
        public EventHandler? StartButtonClicked;

        public ControlPanel()
        {
            InitializeComponent();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            StartButtonClicked?.Invoke(this, e);
        }

        public void Enable()
        {
            button_start.Enabled = true;
        }

        public void Disable()
        {
            button_start.Enabled = false;
        }
    }
}
