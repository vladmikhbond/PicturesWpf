using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PicturesWpf.Models
{
    public class PictureCollection : ObservableCollection<Picture>
    {
        private string _path;
       
        const string INDEX = "index.txt";

        public PictureCollection() {
            Add(new Picture { 
                FileName = "", Title = "Anonimus", 
                ImageSrc = new BitmapImage(new Uri("pack://application:,,,/Images/Anonimus.png")) 
            });
            _path = "";
        }

        public void Load(string path)
        {

            try
            {
                var lst = ReadData(path);
                ClearItems();
                lst.ForEach(p => this.Add(p));
                _path = path;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("No pictures.", ex);
            }
        }

        public void Save()
        {
            if (_path == "")
                return;
            // save index
            string text = string.Join("\r\n", this.Select(p => p.FileName + "\r\n" + p.Title).ToArray());
            string filePath = Path.Combine(_path, INDEX);
            File.WriteAllText(filePath, text);
            // save pictures
            foreach (var picture in this)
            {
                WriteImage(picture.ImageSrc, Path.Combine(_path, picture.FileName));
            }
        }


        public Picture New(string fileName)
        {
            var newPicture = new Picture
            {
                FileName = Path.GetFileName(fileName),
                Title = "No Title",
                ImageSrc = new BitmapImage(new Uri(fileName))
            };
            Add(newPicture);
            return newPicture;
        }

        private static List<Picture> ReadData(string path)
        {
            string filePath = Path.Combine(path, INDEX);
            string[] lines = File.ReadAllText(filePath).Trim().Split('\n');

            var pictures = new List<Picture>();
            for (int i = 0; i < lines.Length; i += 2)
            {
                string id = lines[i].Trim();
                string title = lines[i + 1].Trim();
                string imgPath = Path.Combine(path, id);
                var bitmap = new BitmapImage();
                using (var stream = File.OpenRead(imgPath))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }

                pictures.Add(new Picture
                {
                    FileName = id,
                    Title = title,
                    ImageSrc = bitmap
                });
            }
            return pictures;
        }

        private static void WriteImage(BitmapImage bmp, string fname)
        {
            WriteableBitmap wbitmap = new WriteableBitmap(bmp);
            using (FileStream stream = new FileStream(fname, FileMode.Create))
            {
                BitmapEncoder encoder = null;
                switch (Path.GetExtension(fname))
                {
                    case "png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case "jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case "gif":
                        encoder = new GifBitmapEncoder();
                        break;
                }
                encoder.Frames.Add(BitmapFrame.Create(wbitmap));
                encoder.Save(stream);
            }
        }


    }

}
