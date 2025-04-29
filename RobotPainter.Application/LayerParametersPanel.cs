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
                return (IBrushModel?) Activator.CreateInstance(typeof_model);
            }
            set
            {
                if (value == null) return;
                int i = Array.IndexOf(availableBrushModels, value.GetType());
                if(i != -1)
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
        }

        public StrokeSitesBuilder.Options GetStrokeSitesBuilderOptions()
        {
            return new StrokeSitesBuilder.Options();
        }

        public BrushstrokeBuilder.Options GetBrushstrokeBuilderOptions()
        {
            return new BrushstrokeBuilder.Options();
        }

        public void SetFieldsFromOptions(IBrushModel brushModel, StrokeSitesBuilder.Options ssbOptions, BrushstrokeBuilder.Options bsbOptions)
        {
            this.BrushModel = brushModel;
        }

    }
}
