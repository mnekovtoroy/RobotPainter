using RobotPainter.ConsoleTest;

//var image_gen = new VKRImageGenerator();
//image_gen.GenerateImages();

//TestCases.ColorCalibration();
//TestCases.PhotoTransformingTest();
await TestCases.TakePaintTest();
//await TestCases.BoundsFinder();


/*var calib = await Calibration.Create();
await calib.StraightLinesAsync();
await calib.CurvedLinesAsync();*/