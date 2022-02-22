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
        private int removedfiles;
        private int assetCounter;
        private int packagesExtracted;
        private bool deleteMalwareC = true;
        
        public MainWindow()
        {
            InitializeComponent();
            removedfiles = 0;
            assetCounter = 0;
            packagesExtracted = 0;
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
            foreach (string file in files)
            {
                if (System.IO.Path.GetExtension(file).Equals(".unitypackage"))
                {
                    ExtractUnitypackage(file);
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
                else
                {
                    InfoText.Content = "not an .unitypackage";
                    Mouse.OverrideCursor = Cursors.No;
                }

            }

            MessageBox.Show(packagesExtracted + " .unitypackage files proccessed\n" +  assetCounter + " Files Extracted \n" + removedfiles + " Files Removed", "Unitypackage Extract & Clean");
            //InfoText.Content = "Completed";

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
            string root = targetFolder;


            string[] subdirectoryEntries = Directory.GetDirectories(root);


            foreach (string subdirectory in subdirectoryEntries) {

                LoadSubDirs(subdirectory);
            }

            }


            Directory.Delete(tempFolder, true);
            packagesExtracted++;
            Mouse.OverrideCursor = Cursors.Arrow;
            
        }

        void LoadSubDirs(string dir)

        {

            string[] subdirectoryEntries = Directory.GetDirectories(dir);

            foreach (string subdirectory in subdirectoryEntries)

            {

                foreach (string f in Directory.GetFiles(subdirectory))
                {
                    if (f.EndsWith(".dll") || f.EndsWith(".cs") || f.EndsWith(".dll.meta"))
                    {
                        File.Delete(f);
                        removedfiles++;
                    }

                }

                LoadSubDirs(subdirectory);

            }

        }

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
