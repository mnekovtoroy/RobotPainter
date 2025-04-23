using PainterCore;
using RobotPainter.Communications.Converting;
using System.Drawing;

namespace RobotPainter.Communications
{
    public class RobotPainter
    {
        private string _tempPath = "";

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

        public RobotPainter(IPltConverter pltConverter, string controllerIp = null)
        {
            _pltConverter = pltConverter;
            if(controllerIp == null)
                _controller = new PaintingController();
            else
                _controller = new PaintingController(controllerIp);
        }

        public async Task ApplyStrokes(List<BrushstrokeInfo> strokes)
        {
            var pltCommandes = _pltConverter.ToPlt(strokes);
            var path_plt = _tempPath + "commands.plt";

            _pltConverter.SavePlt(pltCommandes, path_plt);

            await SendPltCommands(path_plt);
        }

        public async Task<Bitmap> TakePhoto()
        {
            //await _controller.TakeaPhoto();
            //
            throw new NotImplementedException();
        }

        private async Task SendPltCommands(string path_plt)
        {
            //await _controller.Start(path_plt);
            throw new NotImplementedException();
        }
    }
}
