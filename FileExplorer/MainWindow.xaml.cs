using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int numFolders = 0;
        int numFiles = 0;


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            string loadPath;

            //Check for saved path
            if (File.Exists(@".\saved path.txt"))
                loadPath = File.ReadAllText(@".\saved path.txt");
            else
                loadPath = System.IO.Path.GetFullPath(".");

            //Load directory
            tbStatus.Text = "Loading...";

            Path = loadPath;
            Helper.LoadDirectories(fileBox, Path, ref numFolders, ref numFiles);

            Files = numFiles;
            Folders = numFolders;
            UpdateSelected();

            tbStatus.Text = "Done";
        }


        //Dependency for the path textbox
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(MainWindow), new PropertyMetadata(""));


        //Dependency for num files
        public int Files
        {
            get { return (int)GetValue(filesProperty); }
            set { SetValue(filesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for files.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty filesProperty =
            DependencyProperty.Register("Files", typeof(int), typeof(MainWindow), new PropertyMetadata(0));


        //Dependency for num folders
        public int Folders
        {
            get { return (int)GetValue(foldersProperty); }
            set { SetValue(foldersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for folders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty foldersProperty =
            DependencyProperty.Register("Folders", typeof(int), typeof(MainWindow), new PropertyMetadata(0));


        //Dependency for selected files 
        public int SelectFiles
        {
            get { return (int)GetValue(selectFilesProperty); }
            set { SetValue(selectFilesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for selectFiles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty selectFilesProperty =
            DependencyProperty.Register("SelectFiles", typeof(int), typeof(MainWindow), new PropertyMetadata(0));



        //Dependency for selected folders
        public int SelectFolders
        {
            get { return (int)GetValue(selectFoldersProperty); }
            set { SetValue(selectFoldersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for selectFolders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty selectFoldersProperty =
            DependencyProperty.Register("SelectFolders", typeof(int), typeof(MainWindow), new PropertyMetadata(0));


        //Dependency for file size
        public long FileSize
        {
            get { return (long)GetValue(fileSizeProperty); }
            set { SetValue(fileSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for fileSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty fileSizeProperty =
            DependencyProperty.Register("FileSize", typeof(long), typeof(MainWindow), new PropertyMetadata(0L));



        //Browse button event (...)
        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {
            numFolders = 0;
            numFiles = 0;

            tbStatus.Text = "Loading...";
            string tempPath = Helper.LoadDirectory();

            if (tempPath != "")
            {
                Path = tempPath;
                Helper.LoadDirectories(fileBox, Path, ref numFolders, ref numFiles);
                tbFolders.Text = numFolders.ToString();
                tbFiles.Text = numFiles.ToString();
                UpdateSelected();
                tbStatus.Text = "Done";
            }
        }

        //Save when window closes
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.WriteAllText(@".\saved path.txt", Path);
        }

        public void UpdateSelected()
        {
            int c = 0, f = 0;
            long s = 0;
            foreach (TreeViewItem tvi in ((TreeViewItem)fileBox.Items[0]).Items)
            {
                c += Helper.countFiles(tvi, Path);
                f += Helper.countFolders(tvi, Path);
                s += Helper.countSize(tvi, Path);
            }
            SelectFiles = c;
            SelectFolders = f;
            FileSize = s;
        }
    }
}
