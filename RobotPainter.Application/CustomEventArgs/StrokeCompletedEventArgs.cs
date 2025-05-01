using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPainter.Application.CustomEventArgs
{
    public class StrokeCompletedEventArgs : EventArgs
    {
        public int StrokeIndex { get; set; }

        public int TotalStrokes { get; set; }
    }
}
