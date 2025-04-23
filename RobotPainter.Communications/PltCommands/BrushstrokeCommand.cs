using RobotPainter.Core;
using System.Text;

namespace RobotPainter.Communications.PltCommands
{
    public class BrushstrokeCommand : IPltCommand
    {
        private static int scf = 40; // scale to PLT

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
                strBuilder.Append(Convert.ToInt32(p.x * scf));
                strBuilder.Append(',');
                strBuilder.Append(Convert.ToInt32(p.y * scf));
                strBuilder.Append(',');
                strBuilder.Append(Convert.ToInt32(p.z * scf));
                strBuilder.Append(',');
            }
            strBuilder.Remove(strBuilder.Length - 1, 1); //removing last ','
            strBuilder.Append(';');
            return strBuilder.ToString();
        }
    }
}
