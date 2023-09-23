namespace HL2TextureExtraction
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;

    using MessageBox = System.Windows.MessageBox;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

    //TODO - ALl works, but just need to delete all vtfs now :)
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Start(object sender, RoutedEventArgs e)
        {
            var filePaths = Directory.GetFiles(rootMaterialsPath.Text, "*.*", SearchOption.AllDirectories);

            var c = 0;
            foreach (var filePath in filePaths)
            {
                var fn = Path.GetFileName(filePath);
                var root = filePath.Replace($"{fn}", "Images");
                Directory.CreateDirectory(root);

                var command = $"vtex2.exe extract -f png {filePath}";
                await Run(command, $"{vtex2Path.Text.Replace("vtex2.exe", "")}");

                try
                {
                    File.Move(filePath.Replace(".vtf", ".png"), Path.Combine(root, fn.Replace(".vtf", ".png")));
                    c++;
                    textBlock.Text += $"Converted {filePath} \n";
                    scrollViewer.ScrollToEnd();
                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception);
                    //throw;
                }

                if (c > 5)
                {
                    break;
                }
            }

            MessageBox.Show($"{c} files converted.");
        }

        public static async Task Run(string command, string workingDirectory)
        {
            await Task.Run(
            () =>
                {
                    var process = new Process
                                      {
                                          StartInfo = new ProcessStartInfo
                                                          {
                                                              FileName = "cmd.exe",
                                                              RedirectStandardInput = true,
                                                              RedirectStandardOutput = true,
                                                              RedirectStandardError = true,
                                                              UseShellExecute = false,
                                                              CreateNoWindow = false,
                                                              WorkingDirectory = workingDirectory
                                                          }
                                      };
                    process.Start();
                    using (var sw = process.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            sw.WriteLine(command);
                            sw.WriteLine("pause");
                        }
                    }

                    process.WaitForExit();
                });
        }

        //public static void Run(string command, string workingDirectory)
        //{
        //    var process = new Process
        //                      {
        //                          StartInfo = new ProcessStartInfo
        //                                          {
        //                                              FileName = "cmd.exe",
        //                                              RedirectStandardInput = true,
        //                                              RedirectStandardOutput = true,
        //                                              RedirectStandardError = true,
        //                                              UseShellExecute = false,
        //                                              CreateNoWindow = false,
        //                                              WorkingDirectory = workingDirectory
        //                                          }
        //                      };
        //    process.Start();
        //    using (var sw = process.StandardInput)
        //    {
        //        if (sw.BaseStream.CanWrite)
        //        {
        //            sw.WriteLine(command);
        //            sw.WriteLine("pause"); // This will pause the command window
        //            // vtex2.exe extract -f png C:\Users\P3k\Desktop\VTFs\materials\brick\brickfloor001a.vtf
        //        }
        //    }
        //}

        public static void RunGUI(string command, string workingDirectory)
        {
            var startInfo = new ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = $"/K {command}",
                                    WorkingDirectory = workingDirectory,
                                    CreateNoWindow = false,
                                    UseShellExecute = false
                                };

            var process = new Process {StartInfo = startInfo};
            process.Start();
        }

        private void DestroyVTFs(object sender, RoutedEventArgs e)
        {
            var filePaths = Directory.GetFiles(rootMaterialsPath.Text, "*.*", SearchOption.AllDirectories);

            var c = 0;
            foreach (var filePath in filePaths)
            {
                try
                {
                    if (!filePath.Contains(".png"))
                    {
                        File.Delete(filePath);
                        c++;
                    }
                }
                catch (Exception a)
                {
                }
                //if (c > 2)
                //{
                //    break;
                //}
            }

            MessageBox.Show($"{c} files deleted.");
        }

        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
                                     {
                                         Filter = "Executable Files (*.exe)|*.exe",
                                         InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                                     };

            if (openFileDialog.ShowDialog() == true)
            {
                vtex2Path.Text = openFileDialog.FileName;
            }
        }

        private void OpenFolderDialog(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a directory";
                folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderDialog.ShowNewFolderButton = true;

                var result = folderDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    rootMaterialsPath.Text = folderDialog.SelectedPath;
                }
            }
        }
    }
}