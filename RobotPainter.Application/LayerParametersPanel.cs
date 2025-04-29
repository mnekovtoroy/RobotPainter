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
        public LayerParametersPanel()
        {
            InitializeComponent();
        }

        public IBrushModel BrushModel { get; private set; }

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
