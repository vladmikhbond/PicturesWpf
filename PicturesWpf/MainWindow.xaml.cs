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

namespace PicturesWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PictureCollection pictures = new PictureCollection();

        public MainWindow()
        {
            InitializeComponent();
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

        #region Command handlers  --------------------------------------------

        private void NewCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCmdExecuted(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCmdExecuted(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = @"C:\temp\dinos";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        pictures.Load(dialog.SelectedPath);
                        picBox.ItemsSource = pictures;
                        picBox.SelectedIndex = 0;
                    }
                    catch (ApplicationException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void SaveCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
            throw new NotImplementedException();
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

        #endregion

        #region Dragging  --------------------------------------------

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int iSource = picBox.SelectedIndex;
            if (iSource != -1 && e.RightButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as Image, iSource.ToString(), DragDropEffects.Move);
            }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            var image = sender as Image;
            int iSource = Convert.ToInt32(e.Data.GetData(DataFormats.Text));
            int iDest = 0;

            for (;  iDest < pictures.Count; iDest++)
            {
                if (image.Source == pictures[iDest].ImageSrc && iDest != iSource) {
                    // move item
                    var t = pictures[iSource];
                    pictures.RemoveAt(iSource);
                    pictures.Insert(iDest, t);
                    picBox.SelectedIndex = iDest;
                    return;
                }
            }            
        }
        #endregion

    }
}
