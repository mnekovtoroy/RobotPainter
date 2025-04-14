using System.Drawing;
using RobotPainter.Calculations;
/*
//color conversion test
Color test = Color.FromArgb(56, 23, 117);
ColorLab colorLab = ColorLab.FromRgb(test);
Color reverse = colorLab.ToRgb();

Console.WriteLine($"Original:\n\tL: {test.R}\ta: {test.G}\tb: {test.B}");
Console.WriteLine($"L*a*b*:\n\tL: {colorLab.L}\ta: {colorLab.a}\tb: {colorLab.b}");
Console.WriteLine($"Reversed:\n\tL: {reverse.R}\ta: {reverse.G}\tb: {reverse.B}");*/

//Palette appliance test
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
plted.Save(path + "3_paletted.png");