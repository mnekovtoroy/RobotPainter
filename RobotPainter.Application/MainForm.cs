namespace RobotPainter.Application
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private Bitmap? image;

        public void MainForm_Load(object? sender, EventArgs e)
        {
            parametersPanel.NewImageOpened += UpdateImage;
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
    }
}
