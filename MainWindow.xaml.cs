using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace AudioPlayer
{
    public partial class MainWindow : Window
    {
        private int mediaIndex = 0;
        private int playStopIndex = 0;
        private string[] files;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FindMusic(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                MusicList.Items.Clear();

                files = Directory.GetFiles(dialog.FileName, "*.*")
                    .Where(s => s.EndsWith(".mp3") || s.EndsWith(".mp4"))
                    .ToArray();

                foreach (string file in files)
                {
                    MusicList.Items.Add(Path.GetFileName(file));
                }

                PlayStop.IsEnabled = true;
                SkipNext.IsEnabled = true;
                SkipPrevious.IsEnabled = true;

                PlayMusic();
            }
        }

        private void PlayMusic()
        {
            string file = files[mediaIndex];
            Player.Source = new Uri(file);
            Player.Play();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Player.Source != null)
            {
                MediaSlider.Value = Player.Position.Ticks;
            }
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaSlider.Value = 0;
            MediaSlider.Maximum = Player.NaturalDuration.TimeSpan.Ticks;
        }

        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Position = new TimeSpan(Convert.ToInt64(MediaSlider.Value));
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (mediaIndex == files.Length - 1)
            {
                mediaIndex = 0;
            }
            else
            {
                mediaIndex++;
            }

            PlayMusic();
        }

        private void PlayStop_Click(object sender, RoutedEventArgs e)
        {
            if (playStopIndex == 0)
            {
                Player.Pause();
                playStopIndex = 1;
            }
            else if (playStopIndex == 1)
            {
                Player.Play();
                playStopIndex = 0;
            }
        }

        private void SkipPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (mediaIndex != 0)
            {
                mediaIndex--;
            }

            PlayMusic();
        }

        private void SkipNext_Click(object sender, RoutedEventArgs e)
        {
            if (mediaIndex != files.Length - 1)
            {
                mediaIndex++;
            }

            PlayMusic();
        }
    }
}