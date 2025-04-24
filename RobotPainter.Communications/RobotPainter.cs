using PainterCore;
using RobotPainter.Communications.Converting;
using System.Drawing;

namespace RobotPainter.Communications
{
    public class RobotPainter
    {
        private string _tempPath;

        public string TempPath { 
            get 
            { 
                return _tempPath; 
            }
            set
            {
                _tempPath = value;
            }
        }

        private PaintingController _controller;
        private IPltConverter _pltConverter;

        private RobotPainter(IPltConverter pltConverter, string temp_path, PaintingController controller)
        {
            _tempPath = temp_path;
            _pltConverter = pltConverter;
            _controller = controller;
        }

        public static async Task<RobotPainter> Create(IPltConverter pltConverter, string temp_path, string controllerIp = null)
        {
            PaintingController controller;
            if (controllerIp == null)
                controller = await PaintingController.CreateController();
            else
                controller = await PaintingController.CreateController(controllerIp);
            return new RobotPainter(pltConverter, temp_path, controller);
        }

        public async Task ApplyStrokes(List<BrushstrokeInfo> strokes)
        {
            var pltCommandes = _pltConverter.ToPlt(strokes);
            var path_plt = _tempPath + "commands.plt";

            _pltConverter.SavePlt(pltCommandes, path_plt);

            await SendPltCommands(path_plt);
        }

        public async Task<Bitmap> TakePhoto(string photo_folder_path)
        {
            await _controller.TakeaPhoto();

            //retrieve the photo
            string[] files = Directory.GetFiles(photo_folder_path);

            var file_creation_times = new Dictionary<string, DateTime>();

            foreach(string file in files)
            {
                file_creation_times.Add(file, File.GetCreationTime(file));
            }
            var files_sorted = file_creation_times.OrderBy(f => f.Value).ToList();

            Bitmap photo = new Bitmap(files_sorted[0].Key);
            
            //deleting old files
            for(int i = 1; i < files_sorted.Count; i++)
            {
                File.Delete(files_sorted[i].Key);
            }

            return photo;
        }

        private async Task SendPltCommands(string path_plt)
        {
            await _controller.Start(path_plt);
        }
    }
}
