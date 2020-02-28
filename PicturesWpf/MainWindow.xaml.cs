using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using PicturesWpf.Models;
using System.Configuration;

namespace PicturesWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly PictureCollection pictures = new PictureCollection();

        public MainWindow()
        {
            InitializeComponent();

            string path = ConfigurationManager.AppSettings.Get("path");
            pictures.Load(path);
            ResetPath(path);
            picBox.ItemsSource = pictures;
            picBox.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        protected void FileExit_Click(object sender, RoutedEventArgs args)
        {            
            Close();
        }

 
        private void NewCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCmdExecuted(object sender, RoutedEventArgs e)
        {
            pictures.Clear();
        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCmdExecuted(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = ConfigurationManager.AppSettings.Get("path"); 
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        pictures.Load(dialog.SelectedPath);
                        picBox.ItemsSource = pictures;
                        picBox.SelectedIndex = 0;
                        ResetPath(dialog.SelectedPath);
                    }
                    catch (ApplicationException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void ResetPath(string path)
        {

            ConfigurationManager.AppSettings.Set("path", path);
            Title = $"{path} - Pictures";
        }


        private void SaveCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(pictures.Path);
        }

        private void SaveCmdExecuted(object sender, RoutedEventArgs e)
        {
            pictures.Save();
        }

        private void SaveAsCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveAsCmdExecuted(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = ConfigurationManager.AppSettings["path"];
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {   
                        pictures.Save(dialog.SelectedPath);
                        ResetPath(dialog.SelectedPath);
                    }
                    catch (ApplicationException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

        }


        private void EditNewCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void EditNewCmdExecuted(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                picBox.SelectedItem = pictures.New(openFileDialog.FileName);
            }
        }

        private void DeleteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = picBox.SelectedIndex > -1;
        }

        private void DeleteCmdExecuted(object sender, RoutedEventArgs e)
        {
            pictures.RemoveAt(picBox.SelectedIndex);
        }

        #region Dragging  --------------------------------------------

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int iSource = IndexOf(sender);
            DragDrop.DoDragDrop(sender as Image, iSource.ToString(), DragDropEffects.Move);
        }


        private void Image_Drop(object sender, DragEventArgs e)
        {                        
            int iSource = Convert.ToInt32(e.Data.GetData(DataFormats.Text));
            int iDest = IndexOf(sender);
            if (iDest != iSource)
            {
                var t = pictures[iSource];
                pictures.RemoveAt(iSource);
                pictures.Insert(iDest, t);
                picBox.SelectedIndex = iDest;                    
            }            
        }


        private int IndexOf(object sender) => 
            pictures.TakeWhile(p => p.ImageSrc != (sender as Image).Source).Count();

        #endregion


    }


    public class WindowCommands
    {
        static WindowCommands()
        {
            Add = new RoutedUICommand("Add...", "Add", typeof(MainWindow));
        }
        public static RoutedUICommand Add { get; set; }
    }
}
