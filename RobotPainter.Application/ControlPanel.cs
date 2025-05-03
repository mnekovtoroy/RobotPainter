using RobotPainter.Application.CustomEventArgs;
using SharpVoronoiLib.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotPainter.Application
{
    public partial class ControlPanel : UserControl
    {
        public EventHandler? StartButtonClicked;

        private DateTime drawingStartTime;
        private DateTime layerStartTime;
        private int totalStrokesCompleted;
        private int lastStrokeIndex;
        private DateTime lastStrokeCompletedTime;

        private bool isTimerRunning;

        public ControlPanel()
        {
            InitializeComponent();

            isTimerRunning = false;
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            StartButtonClicked?.Invoke(this, e);
        }

        public void Enable()
        {
            button_start.Enabled = true;
        }

        public void Disable()
        {
            button_start.Enabled = false;
        }

        public void onDrawingStarted(object? sender, DrawingStartedEventArgs e)
        {
            Invoke(() =>
            {
                drawingStartTime = DateTime.UtcNow;

                totalStrokesCompleted = 0;
                lastStrokeIndex = -1;
                label_timePassedDisplay.Text = "--h:--m:--s";
                label_progressDisplay.Text = $"0/{e.TotalLayers} layers";

                //start the timer
                isTimerRunning = true;
                Task.Run(Timer);
            });
        }

        public void onLayerStarted(object? sender, EventArgs e)
        {
            Invoke(() =>
            {
                layerStartTime = DateTime.UtcNow;
            });
        }

        public void onDrawingEnded(object? sender, EventArgs e)
        {
            Invoke(() =>
            {
                label_timeEstCurrLayerDisplay.Text = $"--h:--m:--s";

                //stop the timer
                isTimerRunning = false;
            });
        }

        public void onLayerCompletion(object? sender, LayerCompletedEventArgs e)
        {
            Invoke(() =>
            {
                lastStrokeIndex = -1;
                label_timeEstCurrLayerDisplay.Text = $"--h:--m:--s";
                UpdateLayerFrames(e);
            });
        }

        public void onStrokeCompletion(object? sender, StrokeCompletedEventArgs e)
        {
            Invoke(() =>
            {
                var stroke_i = e.StrokeIndex;
                totalStrokesCompleted += stroke_i - lastStrokeIndex;
                lastStrokeIndex = stroke_i;
                lastStrokeCompletedTime = DateTime.UtcNow;

                var strokes_left = e.TotalStrokes - lastStrokeIndex - 1;
                var time_left = CalculateTimePrediction(stroke_i + 1, strokes_left, lastStrokeCompletedTime, layerStartTime);

                UpdateCurrentLayerFrames(e, time_left);
                UpdateTotalStrokesCompletedFrame(totalStrokesCompleted);
            });
        }

        private void UpdateLayerFrames(LayerCompletedEventArgs e)
        {
            label_progressDisplay.Text = $"{e.LayerIndex + 1}/{e.TotalLayers} layers";
        }

        private void UpdateCurrentLayerFrames(StrokeCompletedEventArgs e, TimeSpan time_estimation)
        {
            label_currentLayerDisplay.Text = $"{e.StrokeIndex + 1}/{e.TotalStrokes} strokes";

            int hours = Convert.ToInt32(Math.Floor(time_estimation.TotalHours));
            int minutes = time_estimation.Minutes;
            int seconds = time_estimation.Seconds;

            string hours_string = hours >= 10 ? hours.ToString() : "0" + hours.ToString();
            string minutes_string = minutes >= 10 ? minutes.ToString() : "0" + minutes.ToString();
            string seconds_string = seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString();

            label_timeEstCurrLayerDisplay.Text = $"{hours_string}h:{minutes_string}m:{seconds_string}s";
        }

        private void UpdateTotalStrokesCompletedFrame(int total_strokes)
        {
            label_totalStrokesDisplay.Text = total_strokes.ToString();
        }

        private void UpdateTimeFrame()
        {
            if (!isTimerRunning) return;

            var time_lapsed = DateTime.UtcNow - drawingStartTime;

            int hours = Convert.ToInt32(Math.Floor(time_lapsed.TotalHours));
            int minutes = time_lapsed.Minutes;
            int seconds = time_lapsed.Seconds;

            string hours_string = hours >= 10 ? hours.ToString() : "0" + hours.ToString();
            string minutes_string = minutes >= 10 ? minutes.ToString() : "0" + minutes.ToString();
            string seconds_string = seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString();

            label_timePassedDisplay.Text = $"{hours_string}h:{minutes_string}m:{seconds_string}s";
        }

        private void Timer()
        {
            bool timerRunning = true;
            while(timerRunning)
            {
                Thread.Sleep(1000);
                timerRunning = Invoke(() =>
                {
                    UpdateTimeFrame();
                    return isTimerRunning;
                });
            }
        }

        private static TimeSpan CalculateTimePrediction(int strokes_completed, int strokes_left, DateTime last_stroke_completion, DateTime layer_start)
        {
            TimeSpan time_passed = last_stroke_completion - layer_start;
            TimeSpan avg_stroke_time = time_passed / strokes_completed;
            TimeSpan time_left = strokes_left * avg_stroke_time;
            return time_left;
        }
    }
}
