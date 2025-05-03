using RobotPainter.Application.CustomEventArgs;
using RobotPainter.Calculations;
using RobotPainter.Calculations.Brushes;
using RobotPainter.Calculations.StrokeGeneration;
using RobotPainter.Communications;
using System.Drawing.Imaging;

namespace RobotPainter.Application
{
    public partial class MainForm : Form
    {
        EventHandler<DrawingStartedEventArgs>? DrawingStarted;
        EventHandler? DrawingEnded;
        EventHandler? LayerStarted;
        EventHandler<LayerCompletedEventArgs>? LayerCompleted;
        EventHandler<StrokeCompletedEventArgs>? StrokeCompleted;

        IPainter painter;

        RobotPainterCalculator? calculator;
        private Bitmap? image;

        private Bitmap? lastPhoto;

        private bool prediction_isRelevant;

        public MainForm()
        {
            InitializeComponent();
            painter = new BrushPainter();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            prediction_isRelevant = false;
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

        private void OnPhotoUpdate()
        {
            pictureBox_lastPhoto.Invoke(() =>
            {
                pictureBox_lastPhoto.BackColor = Color.Transparent;
                pictureBox_lastPhoto.Image = TransformPhoto(lastPhoto);
            });
        }

        //to do: actually transform photo
        private Bitmap? TransformPhoto(Bitmap? photo)
        {
            return photo;
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

            ((BrushPainter)painter).InitializePainter(new Bitmap(image.Width, image.Height), image.Width / canvas_width, image.Height / canvas_height, new BasicBrushModel());

            var photo = await painter.GetFeedback();
            lastPhoto = photo;
            OnPhotoUpdate();

            var all_layers_options = parametersPanel.Invoke(() => parametersPanel.GetAllLayerOptions());

            await Task.Run(async () =>
            {
                calculator = new RobotPainterCalculator(image, canvas_width, canvas_height);
                calculator.AllLayersOptions = all_layers_options;
                calculator.SetInitialCanvas(TransformPhoto(photo));

                DrawingStarted?.Invoke(this, new DrawingStartedEventArgs() { TotalLayers = calculator.NumOfLayers });

                //for every layer
                for (int i = 0; i < calculator.NumOfLayers; i++)
                {
                    calculator.InitializeStrokeGenerator();
                    var brushstrokes = calculator.GetAllBrushstrokes();

                    LayerStarted?.Invoke(this, EventArgs.Empty);

                    for (int j = 0; j < brushstrokes.Count; j++)
                    {
                        await painter.ApplyStrokes([Mapper.Map(brushstrokes[j])]);

                        StrokeCompleted?.Invoke(this, new() { StrokeIndex = j, TotalStrokes = brushstrokes.Count });

                        if (j % 100 == 0)
                        {
                            Console.WriteLine("Updating photo...");
                            photo = await painter.GetFeedback();
                            lastPhoto = photo;
                            OnPhotoUpdate();
                        }
                    }

                    LayerCompleted?.Invoke(this, new() { LayerIndex = i, TotalLayers = calculator.NumOfLayers });

                    var feedback = await painter.GetFeedback();
                    lastPhoto = feedback;

                    calculator.ApplyFeedback(TransformPhoto(lastPhoto));
                    calculator.AdvanceLayer();

                    OnPhotoUpdate();
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
