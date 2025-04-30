using RobotPainter.Calculations;
using RobotPainter.Calculations.StrokeGeneration;
using SharpVoronoiLib.Exceptions;

namespace RobotPainter.Application
{
    public partial class MainForm : Form
    {
        RobotPainterCalculator? calculator;
        private Bitmap? image;

        public MainForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }


        public void MainForm_Load(object? sender, EventArgs e)
        {
            parametersPanel.NewImageOpened += UpdateImage;
            parametersPanel.CalculatePredictionButtonClicked += button_calculatePrediction_clicked;
        }

        private void UpdateImage(object? sender, EventArgs e)
        {
            var path = ((ParametersPanel?)sender)?.ImagePath;
            if (path != null)
            {
                image = new Bitmap(path);
                OnImageUpdate();
            }
        }

        private void OnImageUpdate()
        {
            pictureBox_sourceImage.BackColor = Color.Transparent;
            pictureBox_sourceImage.Image = image;
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            if(((PictureBox)sender).Image == null)
            {
                return;
            }

            var picbox = new PictureBox();
            picbox.Image = ((PictureBox)sender).Image;
            picbox.SizeMode = PictureBoxSizeMode.Zoom;
            picbox.Dock = DockStyle.Fill;

            var newform = new Form();
            newform.WindowState = FormWindowState.Maximized;
            newform.Text = "Image view";
            newform.Controls.Add(picbox);

            newform.ShowDialog();
        }

        private async void button_calculatePrediction_clicked(object? sender, EventArgs e)
        {
            if (sender == null) return;
            var prediction = await CalculatePrediction((ParametersPanel)sender);
            pictureBox_prediction.Invoke(() =>
            {
                pictureBox_prediction.Image = prediction;
            });
        }

        private async Task<Bitmap> CalculatePrediction(ParametersPanel parametersPanel)
        {
            if (image == null)
                throw new Exception("Image must not be null");

            Console.WriteLine("Prediction calculation started...");
            double canvas_width = Convert.ToDouble(parametersPanel.CanvasWidth);
            double canvas_height = Convert.ToDouble(parametersPanel.CanvasHeight);
            calculator = new RobotPainterCalculator(image, canvas_width, canvas_height);
            calculator.AllLayersOptions.Add(RobotPainterCalculator.CreateLayerOptions());

            var brush_models = parametersPanel.GetBrushModelsForAllLayers();

            Bitmap result = new Bitmap(image.Width, image.Height);
            //to do: change for multi-layer
            await Task.Run(() =>
            {
                using (var g = Graphics.FromImage(result))
                {
                    //for every layer
                    for (int i = 0; i < 1; i++)
                    {
                        Console.WriteLine($"Layer {i + 1} prediction calculation...");
                        calculator.InitializeStrokeGenerator(5000, new StrokeGenerator.Options());
                        Console.WriteLine($"Layer {i + 1} prediction calculation: calculator initialized");
                        var brushstrokes = calculator.GetAllBrushstrokes();
                        Console.WriteLine($"Layer {i + 1} prediction calculation: brushstrokes calculated");
                        foreach (var stroke in brushstrokes)
                        {
                            brush_models[i].DrawStroke(g, new SolidBrush(stroke.Color.ToRgb()), stroke.RootPath, result.Width / canvas_width, result.Height / canvas_height);
                        }
                        Console.WriteLine($"Layer {i + 1} prediction calculation: layer applied");
                    }
                }
            });

            Console.WriteLine("Prediction calculated.");
            return result;
        }
    }
}
