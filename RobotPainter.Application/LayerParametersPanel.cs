using RobotPainter.Calculations;
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
    public partial class LayerParametersPanel : UserControl
    {
        public EventHandler? ParameterChanged;

        private readonly Type[] availableBrushModels = new Type[]
        {
            typeof(BasicBrushModel),
            typeof(DummyBrushModel)
        };

        public IBrushModel? BrushModel
        {
            get
            {
                var typeof_model = availableBrushModels[comboBox_brushModel.SelectedIndex];
                return (IBrushModel?)Activator.CreateInstance(typeof_model);
            }
            set
            {
                if (value == null) return;
                int i = Array.IndexOf(availableBrushModels, value.GetType());
                if (i != -1)
                {
                    comboBox_brushModel.SelectedIndex = i;
                }
            }
        }

        public LayerParametersPanel()
        {
            InitializeComponent();
            for (int i = 0; i < availableBrushModels.Length; i++)
            {
                comboBox_brushModel.Items.Add(availableBrushModels[i].Name);
            }
            comboBox_brushModel.SelectedIndex = 0;

            SetFieldsFromOptions(RobotPainterCalculator.CreateLayerOptions());
        }

        public RobotPainterCalculator.LayerOptions GetLayerOptions()
        {
            return new RobotPainterCalculator.LayerOptions()
            {
                //StrokeSitesBuilder
                CanvasMaxStrokeLength = double.Parse(textBox_maxStrokeLength.Text),
                L_tol = double.Parse(textBox_L_tol.Text),
                MaxNormAngle = double.Parse(textBox_maxNormAngle.Text),
                MaxBrushAngle = double.Parse(textBox_maxTurnAngle.Text),

                //BrushstrokeBuilder
                BrushModel = this.BrushModel,
                MaxWidth = double.Parse(textBox_maxStrokeWidth.Text),
                Overlap = double.Parse(textBox_overlap.Text),
                StartOverheadCoeff = double.Parse(textBox_startOverheadCoeff.Text),
                EndOverheadCoeff = double.Parse(textBox_endOverheadCoeff.Text),
                SafeHeight = double.Parse(textBox_safeHeight.Text),
                StartRunawayAngle = double.Parse(textBox_startRunawayAngle.Text),
                EndRunawayAngle = double.Parse(textBox_endRunawayAngle.Text)
            };
        }

        public void SetFieldsFromOptions(RobotPainterCalculator.LayerOptions options)
        {
            BrushModel = options.BrushModel;

            //ssb options
            textBox_maxStrokeLength.Text = options.CanvasMaxStrokeLength.ToString();
            textBox_L_tol.Text = options.L_tol.ToString();
            textBox_maxNormAngle.Text = options.MaxNormAngle.ToString();
            textBox_maxTurnAngle.Text = options.MaxBrushAngle.ToString();

            //bsb options
            textBox_maxStrokeWidth.Text = options.MaxWidth.ToString();
            textBox_overlap.Text = options.Overlap.ToString();
            textBox_startOverheadCoeff.Text = options.StartOverheadCoeff.ToString();
            textBox_endOverheadCoeff.Text = options.EndOverheadCoeff.ToString();
            textBox_safeHeight.Text = options.SafeHeight.ToString();
            textBox_startRunawayAngle.Text = options.StartRunawayAngle.ToString();
            textBox_endRunawayAngle.Text = options.EndRunawayAngle.ToString();
        }

        private void textBox_DoubleValidating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(((TextBox)sender).Text, out _))
            {
                MessageBox.Show("Field must be a double.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void onParameterChanged(object sender, EventArgs e)
        {
            ParameterChanged?.Invoke(this, e);
        }
    }
}
