using RobotPainter.Calculations.Brushes;
using RobotPainter.Calculations.StrokeGeneration;
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
        public EventHandler? CalculatePredictionButtonClicked;
        public EventHandler? NewImageOpened;

        private readonly int defaultNumOfLayers = 3;
        private readonly int minNumOfLayers = 1;
        private readonly int maxNumOfLayers = 5;

        private int _numOfLayers;

        public string? ImagePath { get; private set; }

        public int NumOfLayers
        {
            get
            {
                return _numOfLayers;
            }
            set
            {
                if (value != _numOfLayers)
                {
                    _numOfLayers = value;
                    OnNumOfLayersChanged();
                }
            }
        }

        public int CanvasWidth { get { return int.Parse(textBox_canvasWidth.Text); } }

        public int CanvasHeight { get { return int.Parse(textBox_canvasHeight.Text); } }

        public ParametersPanel()
        {
            InitializeComponent();
            textBox_numOfLayers.Text = defaultNumOfLayers.ToString();
            NumOfLayers = defaultNumOfLayers;
        }

        public List<StrokeSitesBuilder.Options> GetStrokeSitesBuilderOptionsForAllLayers()
        {
            return tabControl_layerTabs.Invoke(() =>
            {
                var result = new List<StrokeSitesBuilder.Options>();
                for (int i = 0; i < tabControl_layerTabs.TabCount; i++)
                {
                    var layer_parameters_panel = (LayerParametersPanel)tabControl_layerTabs.TabPages[i].Controls[0];
                    result.Add(layer_parameters_panel.GetStrokeSitesBuilderOptions());
                }
                return result;
            });
        }

        public List<BrushstrokeBuilder.Options> GetBrushstrokeBuilderOptionsForAllLayers()
        {
            return tabControl_layerTabs.Invoke(() =>
            {
                var result = new List<BrushstrokeBuilder.Options>();
                for (int i = 0; i < tabControl_layerTabs.TabCount; i++)
                {
                    var layer_parameters_panel = (LayerParametersPanel)tabControl_layerTabs.TabPages[i].Controls[0];
                    result.Add(layer_parameters_panel.GetBrushstrokeBuilderOptions());
                }
                return result;
            });
        }

        public List<IBrushModel> GetBrushModelsForAllLayers()
        {
            return tabControl_layerTabs.Invoke(() =>
            {
                var result = new List<IBrushModel>();
                for (int i = 0; i < tabControl_layerTabs.TabCount; i++)
                {
                    var layer_parameters_panel = (LayerParametersPanel)tabControl_layerTabs.TabPages[i].Controls[0];
                    var brush_model = layer_parameters_panel.BrushModel;
                    if (brush_model != null)
                    {
                        result.Add(brush_model);
                    }
                }
                return result;
            });
        }

        public void Disable()
        {
            button_openImage.Enabled = false;
            textBox_canvasWidth.Enabled = false;
            textBox_canvasHeight.Enabled = false;
            textBox_numOfLayers.Enabled = false;

            foreach(TabPage tab in tabControl_layerTabs.TabPages)
            {
                tab.Controls[0].Enabled = false;
            }

            button_propagateParams.Enabled = false;
            button_calculatePrediction.Enabled = false;
        }

        public void Enable()
        {
            button_openImage.Enabled = true;
            textBox_canvasWidth.Enabled = true;
            textBox_canvasHeight.Enabled = true;
            textBox_numOfLayers.Enabled = true;

            foreach (TabPage tab in tabControl_layerTabs.TabPages)
            {
                tab.Controls[0].Enabled = true;
            }

            button_propagateParams.Enabled = true;
            button_calculatePrediction.Enabled = true;
        }

        private void textBox_numOfLayers_Validating(object sender, CancelEventArgs e)
        {
            int new_numOfLayers;
            if (!int.TryParse(((TextBox)sender).Text, out new_numOfLayers))
            {
                MessageBox.Show("Field must be an integer.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }
            if (new_numOfLayers < minNumOfLayers || new_numOfLayers > maxNumOfLayers)
            {
                MessageBox.Show($"Number of layers must be in range {minNumOfLayers} to {maxNumOfLayers}.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }
            NumOfLayers = new_numOfLayers;
        }

        private void textBox_IntValidating(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(((TextBox)sender).Text, out _))
            {
                MessageBox.Show("Field must be an integer.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void OnNumOfLayersChanged()
        {
            //adjust tabs to according number of layer tabs
            if (NumOfLayers < tabControl_layerTabs.TabPages.Count)
            {
                for (int i = tabControl_layerTabs.TabPages.Count - 1; i >= NumOfLayers; i--)
                {
                    tabControl_layerTabs.TabPages.RemoveAt(i);
                }
            }
            if (NumOfLayers > tabControl_layerTabs.TabPages.Count)
            {
                for (int i = tabControl_layerTabs.TabPages.Count; i < NumOfLayers; i++)
                {
                    var tab = new TabPage($"Layer {i + 1}");
                    tab.AutoScroll = true;
                    tab.Controls.Add(new LayerParametersPanel());
                    tab.Controls[0].Location = new Point(0, 0);
                    tabControl_layerTabs.TabPages.Add(tab);
                }
            }
        }

        private void button_propagateParams_Click(object sender, EventArgs e)
        {
            var curr_layer_parameters_panel = (LayerParametersPanel)tabControl_layerTabs.TabPages[0].Controls[0];
            var brushModel = curr_layer_parameters_panel.BrushModel;
            var ssbOptions = curr_layer_parameters_panel.GetStrokeSitesBuilderOptions();
            var bsbOptions = curr_layer_parameters_panel.GetBrushstrokeBuilderOptions();

            for (int i = 1; i < tabControl_layerTabs.TabCount; i++)
            {
                curr_layer_parameters_panel = (LayerParametersPanel)tabControl_layerTabs.TabPages[i].Controls[0];
                curr_layer_parameters_panel.SetFieldsFromOptions(brushModel, ssbOptions, bsbOptions);

            }
        }

        private void button_calculatePrediction_Click(object sender, EventArgs e)
        {
            CalculatePredictionButtonClicked?.Invoke(this, e);
        }

        private void button_openImage_Click(object sender, EventArgs e)
        {
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImagePath = openFileDialog.FileName;
                label_fileName.Text = Path.GetFileName(ImagePath);
                NewImageOpened?.Invoke(this, e);
            }
        }
    }
}
