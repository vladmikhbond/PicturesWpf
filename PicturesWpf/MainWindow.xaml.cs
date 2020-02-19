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
using System.Windows.Forms;
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
        Pictures pictures;

        public MainWindow()
        {
            InitializeComponent();
            pictures = new Pictures();
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

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCmdExecuted(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = @"C:\temp";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pictures = new Pictures(dialog.SelectedPath);
                    if (pictures.Count > 0)
                    {
                        picBox.ItemsSource = pictures;
                        picBox.SelectedIndex = 0;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Error. No pictures.");
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

        private void NewCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCmdExecuted(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
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

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int n = picBox.SelectedIndex;
            if (n != -1 && e.RightButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as Image, n.ToString(), System.Windows.DragDropEffects.Move);
            }
        }

        private void Image_Drop(object sender, System.Windows.DragEventArgs e)
        {
            int iSour = Convert.ToInt32(e.Data.GetData(System.Windows.DataFormats.Text));
            int iDest = -1;

            var image = sender as Image;
            for (int i = 0; i < pictures.Count; i++)
            {
                if (image.Source == pictures[i].ImageSrc && i != iSour) {
                    iDest = i;
                    break;
                }
            }
            if (iDest != -1) {
                var t = pictures[iSour];
                pictures.RemoveAt(iSour);
                pictures.Insert(iDest, t);
                picBox.SelectedIndex = iDest;
            }
            
        }
    }
}
