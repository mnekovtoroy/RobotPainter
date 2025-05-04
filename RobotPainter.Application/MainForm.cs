using RobotPainter.Application.CustomEventArgs;
using RobotPainter.Application.PhotoTransforming;
using RobotPainter.Calculations;
using RobotPainter.Calculations.Brushes;
using RobotPainter.Communications;
using RobotPainter.Communications.Converting;
using RobotPainter.Core;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace RobotPainter.Application
{
    public partial class MainForm : Form
    {
        private EventHandler<DrawingStartedEventArgs>? DrawingStarted;
        private EventHandler? DrawingEnded;
        private EventHandler? LayerStarted;
        private EventHandler<LayerCompletedEventArgs>? LayerCompleted;
        private EventHandler<StrokeCompletedEventArgs>? StrokeCompleted;

        private IPainter? painter;
        private IPhotoTransformer photoTransformer;

        private RobotPainterCalculator? calculator;
        private Palette palette;
        private Bitmap? image;

        private Bitmap? lastPhoto;

        private bool prediction_isRelevant;

        public MainForm()
        {
            InitializeComponent();
            //test
            //photoTransformer = new DummyTransformer();

            //real
            photoTransformer = new PhotoTransformer();

            FormBorderStyle = FormBorderStyle.FixedSingle;
            prediction_isRelevant = false;
            palette = ManualPalette.GetPalette();
        }

        public void MainForm_Load(object? sender, EventArgs e)
        {
            parametersPanel.NewImageOpened += UpdateImage;
            parametersPanel.CalculatePredictionButtonClicked += button_calculatePrediction_clicked;
            parametersPanel.ParameterChanged += (object? sender, EventArgs e) => prediction_isRelevant = false;
            controlPanel.StartButtonClicked += button_startDrawing_clicked;

            DrawingStarted += controlPanel.onDrawingStarted;
            DrawingEnded += controlPanel.onDrawingEnded;
            LayerStarted += controlPanel.onLayerStarted;
            LayerCompleted += controlPanel.onLayerCompletion;
            StrokeCompleted += controlPanel.onStrokeCompletion;
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
            pictureBox_sourceImage.Invoke(() =>
            {
                pictureBox_sourceImage.BackColor = Color.Transparent;
                pictureBox_sourceImage.Image = image;
            });
        }


        private async Task UpdatePhoto(Bitmap photo)
        {
            if (photo == null || image == null) return;
            var transformed = await Task.Run(() =>
            {
                return photoTransformer.Transform(photo, image.Width, image.Height);
            });
            photo.Dispose();
            pictureBox_lastPhoto.Invoke(() =>
            {
                pictureBox_lastPhoto.BackColor = Color.Transparent;
                pictureBox_lastPhoto.Image = transformed;
            });
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Image == null)
            {
                return;
            }

            var picbox = new PictureBox();
            picbox.Image = ((PictureBox)sender).Image;
            picbox.SizeMode = PictureBoxSizeMode.Zoom;
            picbox.Dock = DockStyle.Fill;
            picbox.ContextMenuStrip = contextMenuStrip_pictureBox;

            var newform = new Form();
            newform.WindowState = FormWindowState.Maximized;
            newform.Text = "Image view";
            newform.Controls.Add(picbox);

            newform.ShowDialog();
        }

        private void EnableControls()
        {
            parametersPanel.Invoke(() =>
            {
                parametersPanel.Enable();
            });

            controlPanel.Invoke(() =>
            {
                controlPanel.Enable();
            });
        }

        private void DisableControls()
        {
            parametersPanel.Invoke(() =>
            {
                parametersPanel.Disable();
            });

            controlPanel.Invoke(() =>
            {
                controlPanel.Disable();
            });
        }

        private async void button_calculatePrediction_clicked(object? sender, EventArgs e)
        {
            if (sender == null) return;

            DisableControls();
            var prediction = await CalculatePrediction((ParametersPanel)sender);
            pictureBox_prediction.Invoke(() =>
            {
                pictureBox_prediction.BackColor = Color.Transparent;
                pictureBox_prediction.Image = prediction;
                prediction_isRelevant = true;
            });
            EnableControls();
        }

        //to do: change for multi-layer
        private async Task<Bitmap> CalculatePrediction(ParametersPanel parametersPanel)
        {
            if (image == null)
                throw new Exception("Image must not be null");

            Console.WriteLine("Prediction calculation started...");
            double canvas_width = Convert.ToDouble(parametersPanel.CanvasWidth);
            double canvas_height = Convert.ToDouble(parametersPanel.CanvasHeight);

            var all_layers_options = parametersPanel.Invoke(() => parametersPanel.GetAllLayerOptions());

            Bitmap result = new Bitmap(image.Width, image.Height);

            await Task.Run(() =>
            {
                calculator = new RobotPainterCalculator(image, canvas_width, canvas_height);
                calculator.SavedPalette = palette;
                calculator.AllLayersOptions = all_layers_options;
                calculator.SetInitialCanvas(result);

                int[] num_of_strokes = new int[calculator.NumOfLayers];
                //for every layer
                for (int i = 0; i < calculator.NumOfLayers; i++)
                {
                    Console.WriteLine($"Layer {i + 1} prediction calculation...");
                    calculator.InitializeStrokeGenerator();
                    Console.WriteLine($"Layer {i + 1} prediction calculation: calculator initialized");
                    var brushstrokes = calculator.GetAllBrushstrokes();
                    calculator.ApplyPalette(brushstrokes);
                    num_of_strokes[i] = brushstrokes.Count;
                    Console.WriteLine($"Layer {i + 1} prediction calculation: brushstrokes calculated");

                    using (var g = Graphics.FromImage(result))
                    {
                        foreach (var stroke in brushstrokes)
                        {
                            calculator.AllLayersOptions[i].BrushModel.DrawStroke(g, new SolidBrush(stroke.Color.ToRgb()), stroke.RootPath, result.Width / canvas_width, result.Height / canvas_height);
                        }
                    }
                    calculator.ApplyFeedback(result);
                    calculator.AdvanceLayer();
                    Console.WriteLine($"Layer {i + 1} prediction calculation: layer applied");
                }

                int total_strokes = 0;
                Console.WriteLine("Stroke count:");
                for (int i = 0; i < num_of_strokes.Length; i++)
                {
                    Console.WriteLine($"Layer {i + 1}: {num_of_strokes[i]} strokes");
                    total_strokes += num_of_strokes[i];
                }
                Console.WriteLine($"Total predicted strokes: {total_strokes}");
            });
            Console.WriteLine("Prediction calculated.");
            return result;
        }

        private async void button_startDrawing_clicked(object? sender, EventArgs e)
        {
            DisableControls();
            await StartDrawing();
            EnableControls();
        }

        //to do: convert for every layer
        private async Task StartDrawing()
        {
            if (image == null) return;

            if (!prediction_isRelevant)
            {
                var prediction = await CalculatePrediction(parametersPanel);
                pictureBox_prediction.Invoke(() =>
                {
                    pictureBox_prediction.BackColor = Color.Transparent;
                    pictureBox_prediction.Image = prediction;
                    prediction_isRelevant = true;
                });
            }

            double canvas_width = Convert.ToDouble(parametersPanel.CanvasWidth);
            double canvas_height = Convert.ToDouble(parametersPanel.CanvasHeight);

            //  real
            var colorToCoord = new ManualColorToCoord(palette.Colors, new PointD(0, 0), 1.5, 1.5, 5, 2);
            var pltConverter = new PltConverter(colorToCoord);
            string temp_path = @"C:\Users\User\source\repos\RobotPainter\Temp";
            string photo_path = @"C:\Users\User\source\repos\RobotPainter\Photos";
            painter = await RobotController.Create(pltConverter, temp_path, photo_path, canvas_width, canvas_height);
            //  testing
            //painter = new BrushPainter();
            //((BrushPainter)painter).InitializePainter(new Bitmap(image.Width, image.Height), image.Width / canvas_width, image.Height / canvas_height, new BasicBrushModel());

            var photo = await painter.GetFeedback();
            lastPhoto = photo;
            _ = UpdatePhoto(new Bitmap(lastPhoto));

            var all_layers_options = parametersPanel.Invoke(() => parametersPanel.GetAllLayerOptions());

            await Task.Run(async () =>
            {
                calculator = new RobotPainterCalculator(image, canvas_width, canvas_height);
                calculator.SavedPalette = palette;
                calculator.AllLayersOptions = all_layers_options;
                calculator.SetInitialCanvas(photoTransformer.Transform(photo, image.Width, image.Height));

                DrawingStarted?.Invoke(this, new DrawingStartedEventArgs() { TotalLayers = calculator.NumOfLayers });

                //for every layer
                for (int i = 0; i < calculator.NumOfLayers; i++)
                {
                    calculator.InitializeStrokeGenerator();
                    var brushstrokes = calculator.GetAllBrushstrokes();
                    calculator.ApplyPalette(brushstrokes);

                    LayerStarted?.Invoke(this, EventArgs.Empty);
                    for (int j = 0; j < brushstrokes.Count; j++)
                    {
                        await painter.ApplyStrokes([Mapper.Map(brushstrokes[j])]);

                        StrokeCompleted?.Invoke(this, new() { StrokeIndex = j, TotalStrokes = brushstrokes.Count });

                        if (j % 100 == 0)
                        {
                            Console.WriteLine("Updating photo...");
                            photo = await painter.GetFeedback();
                            lastPhoto.Dispose();
                            lastPhoto = photo;
                            _ = UpdatePhoto(new Bitmap(lastPhoto));
                        }
                    }

                    LayerCompleted?.Invoke(this, new() { LayerIndex = i, TotalLayers = calculator.NumOfLayers });

                    var feedback = await painter.GetFeedback();
                    lastPhoto.Dispose();
                    lastPhoto = feedback;
                    _ = UpdatePhoto(new Bitmap(lastPhoto));

                    calculator.ApplyFeedback(photoTransformer.Transform(feedback, image.Width, image.Height));
                    calculator.AdvanceLayer();
                }
                DrawingEnded?.Invoke(this, EventArgs.Empty);
            });
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var toolStripItem = (ToolStripItem?)sender;
            var contextMenuStrip = (ContextMenuStrip?)toolStripItem?.Owner;
            var picture_box = contextMenuStrip?.SourceControl as PictureBox;

            if (picture_box == null || picture_box.Image == null)
                return;

            var image_toSave = new Bitmap(picture_box.Image);

            SaveImage(image_toSave);
        }

        private void SaveImage(Bitmap bmp)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var stream = saveFileDialog.OpenFile();
                if(stream != null)
                {
                    bmp.Save(stream, ImageFormat.Png);
                }
            }
        }
    }
}
