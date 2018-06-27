using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows;
using System.IO;
using System.Windows.Media;

namespace FileExplorer
{
    class Helper
    {
        //Browse button load method (method used when user changes directory)
        public static string LoadDirectory()
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            string path = "";

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK) 
                path = dlg.SelectedPath;

            return path;
        }


        //Load directories method
        public static void LoadDirectories(TreeView view, string Path, ref int folders, ref int files)
        {
            if(Path != "")
            {
                view.Items.Clear();
                var rootDirectoryInfo = new DirectoryInfo(Path);
                view.Items.Add(CreateDir(rootDirectoryInfo, ref folders, ref files));
            }

        }

        //Load the files and folders into tree
        public static TreeViewItem CreateDir(DirectoryInfo info, ref int folder, ref int files)
        {
            CheckBox ch = new CheckBox();
            ch.Name = "cFolder";
            ch.IsChecked = true;
            //ch.IsThreeState = true;
            ch.Content = info.Name;

            ch.Checked += Ch_Checked;
            ch.Unchecked += Ch_Checked;
            ch.Indeterminate += Ch_Checked;
            //ch.Click += Ch_Click;

            var dirInfo = new TreeViewItem { Header = ch };
            dirInfo.IsExpanded = true;

            try
            {
                foreach (var dir in info.GetDirectories())
                {
                    ++folder;
                    dirInfo.Items.Add(CreateDir(dir, ref folder, ref files));
                }

                foreach (var file in info.GetFiles())
                {
                    ++files;
                    ch = new CheckBox();
                    ch.Name = "cFile";
                    ch.IsChecked = true;
                    ch.Content = file.Name;

                    ch.Checked += Ch_Checked;
                    ch.Unchecked += Ch_Checked;

                    dirInfo.Items.Add(new TreeViewItem { Header = ch });
                }
            }
            catch(UnauthorizedAccessException ex) { }

            return dirInfo;
        }

        //Checkbox click event
        private static void Ch_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            if (ch.IsChecked == null)
                ch.IsChecked = false;

        }

        //Checkbox checked event
        private static void Ch_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            bool? state = check.IsChecked;

            TreeViewItem view = findAncestor<TreeViewItem>(sender as DependencyObject);
            if (view != null)
            {
                if (view.Items != null)
                {
                    foreach (TreeViewItem i in view.Items)
                        ((CheckBox)i.Header).IsChecked = state;
                }

                if (view.Parent is TreeViewItem)
                    checkState((TreeViewItem)view.Parent);
            }

            App.Current.main.UpdateSelected();
        }

        //Check checkbox state
        public static void checkState(TreeViewItem view)
        {
            int numSelect = 0;
            bool childnull = false;

            foreach(TreeViewItem i in view.Items)
            {
                CheckBox check = (CheckBox)i.Header;
                bool? state = check.IsChecked;

                if (state == true || state == null)
                    ++numSelect;
                if (state == null)
                    childnull = true;
            }

            CheckBox me = (CheckBox)view.Header;
            // Dont respond to changes...
            me.Checked -= Ch_Checked;
            me.Unchecked -= Ch_Checked;
            me.Indeterminate -= Ch_Checked;

            if (numSelect == 0)
                me.IsChecked = false;
            else if (view.Items.Count == numSelect)
            {
                if (childnull)
                    me.IsChecked = null;
                else
                    me.IsChecked = true;
            }
            else
                me.IsChecked = null;

            if (view.Parent is TreeViewItem)
                checkState((TreeViewItem)view.Parent);

            me.Checked += Ch_Checked;
            me.Unchecked += Ch_Checked;
            me.Indeterminate += Ch_Checked;
        }

        //Count selected files
        public static int countFiles(TreeViewItem view, string path)
        {
            CheckBox check = (CheckBox)view.Header;
            if (check.IsChecked == false) return 0;
            int count = 0;

            foreach (TreeViewItem i in view.Items)
                count += countFiles(i, path + @"\" + check.Content);


            if (File.Exists(path + @"\" + check.Content))
                return 1;
            else
                return count;

        }

        //Count selected folders
        public static int countFolders(TreeViewItem view, string path)
        {
            CheckBox check = (CheckBox)view.Header;
            if (check.IsChecked == false) return 0;

            int count;
            if (check.IsChecked == null)
                count = 0;
            else
                count = 1;

            foreach (TreeViewItem i in view.Items)
                count += countFolders(i, path + @"\" + check.Content);


            if (Directory.Exists(path + @"\" + check.Content))
                return count;
            else
                return 0;
        }

        //Count file size 
        public static long countSize(TreeViewItem view, string path)
        {
            CheckBox check = (CheckBox)view.Header;
            if (check.IsChecked == false) return 0;
            long count = 0;

            foreach (TreeViewItem i in view.Items)
                count += countSize(i, path + @"\" + check.Content);


            if (File.Exists(path + @"\" + check.Content))
                return new FileInfo(path +@"\" + check.Content).Length;
            else
                return count;
        }

        public static T findAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }
}
