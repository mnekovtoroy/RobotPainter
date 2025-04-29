namespace RobotPainter.Application
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
    }
}
