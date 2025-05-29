using PainterCore;
using RobotPainter.Communications.Converting;
using RobotPainter.Core;
using System.Drawing;

namespace RobotPainter.Communications
{
    public class RobotController : IPainter
    {
        private string _photoFolderPath;
        public string PhotoFolderPath
        {
            get
            {
                return _photoFolderPath;
            }
            set
            {
                _photoFolderPath = value;
            }
        }

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

        private double _canvasWidth;
        private double _canvasHeight;

        private RobotController(IPltConverter pltConverter, string temp_path, string photo_folder_path, double canvas_width, double canvas_height, PaintingController controller)
        {
            _tempPath = temp_path;
            _photoFolderPath = photo_folder_path;
            _pltConverter = pltConverter;
            _controller = controller;

            _canvasWidth = canvas_width; ;
            _canvasHeight = canvas_height;
        }

        public static async Task<RobotController> Create(IPltConverter pltConverter, string temp_path, string photo_folder_path, double canvas_width, double canvas_height, string controllerIp = null)
        {
            PaintingController controller;
            if (controllerIp == null)
                controller = await PaintingController.CreateController();
            else
                controller = await PaintingController.CreateController(controllerIp);
            return new RobotController(pltConverter, temp_path, photo_folder_path, canvas_width, canvas_height, controller);
        }

        public async Task ApplyStrokes(List<BrushstrokeInfo> strokes)
        {
            var strokes_mapped = RemapCoordinates(strokes);
            var pltCommandes = _pltConverter.ToPlt(strokes_mapped);
            var path_plt = _tempPath + "commands.plt";

            _pltConverter.SavePlt(pltCommandes, path_plt);

            await SendPltCommands(path_plt);
        }

        public async Task<Bitmap> GetFeedback()
        {
            await _controller.TakeaPhoto();

            //waiting a sec for the photo to be sent to pc
            Thread.Sleep(10000);

            //retrieve the photo
            string[] files = Directory.GetFiles(_photoFolderPath);

            var file_creation_times = new Dictionary<string, DateTime>();

            Bitmap photo = null;
            int k = 0;
            int k_max = 10;
            while(photo == null && k < k_max)
            {
                k++;
                try
                {
                    foreach (string file in files)
                    {
                        file_creation_times.Add(file, File.GetCreationTime(file));
                    }
                    var files_sorted = file_creation_times.OrderByDescending(f => f.Value).ToList();

                    photo = new Bitmap(files_sorted[0].Key);

                    //deleting old files
                    for (int i = 1; i < files_sorted.Count; i++)
                    {
                        File.Delete(files_sorted[i].Key);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to open or delete a file:");
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(1000);
                }
            }
            if (photo == null)
                throw new Exception("Couldn't retrieve a photo.");
            return photo;
        }

        private List<BrushstrokeInfo> RemapCoordinates(List<BrushstrokeInfo> strokes)
        {
            var remapped = strokes.Select(s => new BrushstrokeInfo()
            {
                Color = s.Color,
                RootPath = s.RootPath.Select(p => new Point3D(p.x, _canvasHeight - p.y, p.z)).ToList()
            }).ToList();
            return remapped;
        }

        private async Task SendPltCommands(string path_plt)
        {
            await _controller.Start(path_plt);
        }
    }
}
