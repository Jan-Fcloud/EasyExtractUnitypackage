using System.Windows;
using System.Windows.Shapes;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.Win32;
using System.Windows.Input;

namespace EasyExtractUnitypackage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>   
    public partial class MainWindow : Window
    {
        public int removedfiles = 0;
        private int assetCounter;
        private bool deleteMalwareC = true;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                Mouse.OverrideCursor = Cursors.AppStarting;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (System.IO.Path.GetExtension(files[0]).Equals(".unitypackage"))
            {
                ExtractUnitypackage(files[0]);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else
            {
                InfoText.Content = "not an .unitypackage";
                Mouse.OverrideCursor = Cursors.No;
            }
        }

        private void ExtractUnitypackage(string filename)
        {

            Mouse.OverrideCursor = Cursors.Wait;
            var tempFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "tmp_" + System.IO.Path.GetFileNameWithoutExtension(filename));
            var targetFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), System.IO.Path.GetFileNameWithoutExtension(filename) + "_extracted");

            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);
            if (Directory.Exists(targetFolder))
            {
                MessageBox.Show("Folder already exists", "Error");
                Mouse.OverrideCursor = Cursors.Arrow;
                return;
            }
            Directory.CreateDirectory(tempFolder);
            Directory.CreateDirectory(targetFolder);

            ExtractTGZ(filename, tempFolder);
            ProcessExtracted(tempFolder, targetFolder);

            if (deleteMalwareC)
            {
                // CUSTOM START
            string root = targetFolder;

            // Get all subdirectories

            string[] subdirectoryEntries = Directory.GetDirectories(root);

            // Loop through them to see if they have any other subdirectories

            foreach (string subdirectory in subdirectoryEntries) {

                LoadSubDirs(subdirectory);
            }
            // CUSTOM END

            }


            Directory.Delete(tempFolder, true);
            MessageBox.Show(assetCounter + " Files EasyExtracted and " + removedfiles + " files removed! ", "EasyExtractUnitypackage & Clean");
            removedfiles = 0;
            InfoText.Content = "Completed";
            Mouse.OverrideCursor = Cursors.Arrow;
            
        }

        // CUSTOM START
        void LoadSubDirs(string dir)

        {

            //Console.WriteLine(dir);

            string[] subdirectoryEntries = Directory.GetDirectories(dir);

            foreach (string subdirectory in subdirectoryEntries)

            {

                foreach (string f in Directory.GetFiles(subdirectory))
                {
                    if (f.EndsWith(".dll") || f.EndsWith(".cs"))
                    {
                        File.Delete(f);
                        removedfiles++;
                    }

                }

                LoadSubDirs(subdirectory);

            }

        }
        // CUSTOM END

        public void ExtractTGZ(string gzArchiveName, string destFolder)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            Stream inStream = File.OpenRead(gzArchiveName);
            Stream gzipStream = new GZipInputStream(inStream);

            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(destFolder);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
        }

        private void ProcessExtracted(string tempFolder, string targetFolder)
        {
            assetCounter = 0;
            foreach (string d in Directory.EnumerateDirectories(tempFolder))
            {
                string relativePath = "";
                string targetFullPath = "";
                string targetFullFile = "";
                try
                {
                    if (File.Exists(System.IO.Path.Combine(d, "pathname")))
                    {
                        relativePath = File.ReadAllText(System.IO.Path.Combine(d, "pathname"));
                        targetFullPath = System.IO.Path.GetDirectoryName(System.IO.Path.Combine(targetFolder, relativePath));
                        targetFullFile = System.IO.Path.Combine(targetFolder, relativePath);
                    }
                    if (File.Exists(System.IO.Path.Combine(d, "asset")))
                    {
                        Directory.CreateDirectory(targetFullPath);
                        File.Move(System.IO.Path.Combine(d, "asset"), targetFullFile);
                        assetCounter++;
                    }

                    if (File.Exists(System.IO.Path.Combine(d, "asset.meta")))
                    {
                        Directory.CreateDirectory(targetFullPath);
                        File.Move(System.IO.Path.Combine(d, "asset.meta"), targetFullFile + ".meta");
                    }

                    /*
                    if (File.Exists(System.IO.Path.Combine(d, "preview.png")))
                    {

                    }
                    */
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error", ex.Message);
                }
                
            }
        }

        private void deletemalware_Click(object sender, System.EventArgs e)
        {
            if (deletemalware.IsChecked == true)
            {
                deleteMalwareC = true;
            }
            else if(deletemalware.IsChecked == false)
            {
                deleteMalwareC = false;
            }
        }

        private void searchUnitybtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.Filter = "Unitypackages (*.Unitypackage)|*.Unitypackage";
            openFile.ShowDialog();
            if (string.IsNullOrEmpty(openFile.FileName))
            {
                return;
            }
            ExtractUnitypackage(openFile.FileName);
        }

        private void deletemalware_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
