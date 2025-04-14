using System.Drawing;
using RobotPainter.Calculations;
using RobotPainter.ConsoleTest;

/*//color conversion test
Color test = Color.FromArgb(56, 23, 117);
ColorLab colorLab = ColorLab.FromRgb(test);
Color reverse = colorLab.ToRgb();

Console.WriteLine($"Original:\n\tL: {test.R}\ta: {test.G}\tb: {test.B}");
Console.WriteLine($"L*a*b*:\n\tL: {colorLab.L}\ta: {colorLab.a}\tb: {colorLab.b}");
Console.WriteLine($"Reversed:\n\tL: {reverse.R}\ta: {reverse.G}\tb: {reverse.B}");*/

/*//Palette appliance test
string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
Bitmap original = new Bitmap(path + "test5.jpg");
original.Save(path + "1_original.png");

LabBitmap lbm =  new LabBitmap(original);
Bitmap reverse = lbm.ToBitmap();
reverse.Save(path + "2_reverse.png");

Palette plt = new Palette();
plt.Colors.Add(ColorLab.FromRgb(Color.White));
plt.Colors.Add(ColorLab.FromRgb(Color.LightGray));
plt.Colors.Add(ColorLab.FromRgb(Color.Gray));
plt.Colors.Add(ColorLab.FromRgb(Color.DarkGray));
plt.Colors.Add(ColorLab.FromRgb(Color.Black));

Bitmap plted = plt.Apply(lbm).ToBitmap();
plted.Save(path + "3_paletted.png");*/

//Visualiser test
string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
Bitmap original = new Bitmap(path + "test3.jpg");

int n_avg = 3;
int n_arrows = 20;

int l_s = original.Width < original.Height ? original.Width : original.Height;
int arrow_length = (l_s / 20) / 3;
int tip_length = arrow_length / 4;

LabBitmap lbmp= new LabBitmap(original);

double[,] u, v;
(u, v) = ImageProcessor.LNormWithRollAvg(lbmp, n_avg);
//visualise
ImageVisualiser.VisualiseVectorsInline(original, u, v, n_arrows, Color.Blue, Color.Blue, arrow_length, tip_length);

original.Save(path + "norm_visualised.png");

