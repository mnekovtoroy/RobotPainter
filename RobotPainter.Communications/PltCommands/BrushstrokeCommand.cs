using RobotPainter.Core;
using System.Text;

namespace RobotPainter.Communications.PltCommands
{
    public class BrushstrokeCommand : IPltCommand
    {
        private static int scf = 40; // scale to PLT

        private static Point3D p0 = new Point3D(220.0, 20.0, 0.0);

        public List<Point3D> rootPath;

        public BrushstrokeCommand(List<Point3D> root_path)
        {
            rootPath = root_path;
        }

        public string ToPlt()
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append("BS");
            foreach(var p in rootPath)
            {
                var p_moved = p + p0;
                strBuilder.Append(Convert.ToInt32(p_moved.x * scf));
                strBuilder.Append(',');
                strBuilder.Append(Convert.ToInt32(p_moved.y * scf));
                strBuilder.Append(',');
                strBuilder.Append(Convert.ToInt32(p_moved.z * scf));
                strBuilder.Append(',');
            }
            strBuilder.Remove(strBuilder.Length - 1, 1); //removing last ','
            strBuilder.Append(';');
            return strBuilder.ToString();
        }
    }
}
