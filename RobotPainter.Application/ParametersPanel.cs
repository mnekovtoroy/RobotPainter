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
    public partial class ParametersPanel : UserControl
    {
        private readonly int defaultNumOfLayers = 3;
        private readonly int minNumOfLayers = 1;
        private readonly int maxNumOfLayers = 5;

        private int _numOfLayers;

        public int NumOfLayers
        {
            get
            {
                return _numOfLayers;
            }
            set
            {
                if(value != _numOfLayers)
                {
                    _numOfLayers = value;
                    OnNumOfLayersChanged();
                }
            }
        }

        public ParametersPanel()
        {
            InitializeComponent();
            textBox_numOfLayers.Text = defaultNumOfLayers.ToString();
            NumOfLayers = defaultNumOfLayers;
        }

        private void textBox_numOfLayers_Validating(object sender, CancelEventArgs e)
        {
            int new_numOfLayers;
            if (!int.TryParse(textBox_numOfLayers.Text, out new_numOfLayers))
            {
                MessageBox.Show("Field must be an integer.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ((TextBox)sender).Focus();
                return;
            }
            if(new_numOfLayers < minNumOfLayers || new_numOfLayers > maxNumOfLayers)
            {
                MessageBox.Show($"Number of layers must be in range {minNumOfLayers} to {maxNumOfLayers}.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ((TextBox)sender).Focus();
                return;
            }
            NumOfLayers = new_numOfLayers;
        }

        private void OnNumOfLayersChanged()
        {
            //adjust tabs to according number of layer tabs
            if(NumOfLayers < tabControl_layerTabs.TabPages.Count)
            {
                for(int i = tabControl_layerTabs.TabPages.Count - 1; i >= NumOfLayers; i--)
                {
                    tabControl_layerTabs.TabPages.RemoveAt(i);
                }
            }
            if(NumOfLayers > tabControl_layerTabs.TabPages.Count)
            {
                for(int i = tabControl_layerTabs.TabPages.Count; i < NumOfLayers; i++)
                {
                    var tab = new TabPage($"Layer {i + 1}");
                    tab.Controls.Add(new LayerParametersPanel());
                    tab.Controls[0].Location = new Point(0, 0);
                    //tab.Controls[0].Show();
                    tabControl_layerTabs.TabPages.Add(tab);
                }
            }
        }
    }
}
