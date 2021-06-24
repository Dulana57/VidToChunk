using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reflection;

namespace VidToChunk {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        string inputfile;
        string outputfile;

        public MainWindow() {
            InitializeComponent();
            MinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            CloseButton.Click += (s, e) => Application.Current.Shutdown();
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);
            txt_Version.Text = "v" + version;
        }

        private void btn_run_Click(object sender, RoutedEventArgs e) {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/K ffmpeg -i \"" + inputfile + "\" -c:v libvpx-vp9 -crf 30 -b:v 0 -b:a 128k -c:a libopus \"" + outputfile.Replace(".chunk", ".webm") + "\"";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            File.Delete(outputfile);
            FileInfo fi = new FileInfo(outputfile.Replace(".chunk", ".webm"));
            fi.MoveTo(outputfile);
            long size = new FileInfo(outputfile).Length;
            txtb_filesize.Text = size.ToString();
            string message = "Conversion is finished! Open file folder?";
            string title = "Finished!";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBoxResult result = MessageBox.Show(message, title, buttons, icon);
            switch (result) {
                case MessageBoxResult.Yes:
                    Process.Start("explorer.exe", Path.GetDirectoryName(outputfile));
                    break;
            }
        }

        private void btn_inputchoose_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            {
                openFileDlg.Filter = "Input video file (*.mp4)|*.mp4";
                openFileDlg.FilterIndex = 2;
                openFileDlg.RestoreDirectory = true;

                Nullable<bool> result = openFileDlg.ShowDialog();

                if (result == true) {
                    inputfile = openFileDlg.FileName;
                    txtb_inputpath.Text = inputfile;
                }
            }
            checknull();
        }

        public void checknull() {
            if (inputfile != null && outputfile != null)
                btn_run.IsEnabled = true;
        }

        private void btn_outputchoose_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Chunk file|*.chunk";
            saveFileDialog1.Title = "Save a Chunk File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != null) {
                outputfile = saveFileDialog1.FileName;
                txtb_outputpath.Text = outputfile;
            }
            checknull();
        }
    }
}
