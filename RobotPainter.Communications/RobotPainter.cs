using PainterCore;
using RobotPainter.Communications.Converting;

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

        public RobotPainter(string controllerIp = null)
        {
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

            SendPltCommands(path_plt);
        }

        private async void SendPltCommands(string path_plt)
        {
            //await _controller.Start(path_plt);
        }
    }
}
