using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using static System.IO.Path;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PicturesWpf.Models
{
    public class PictureCollection : ObservableCollection<Picture>
    {
        const string INDEX = "index.txt";

        public string Path { get; private set; }

        public PictureCollection() {
            Add(new Picture { 
                FileName = "", Title = "Anonimus", 
                ImageSrc = new BitmapImage(new Uri("pack://application:,,,/Images/Anonimus.png")) 
            });
            Path = "";
        }

        public new void Clear()
        {
            base.Clear();
            Path = "";
        }

        public void Load(string path)
        {
            try
            {
                var lst = ReadData(path);
                ClearItems();
                lst.ForEach(p => Add(p));
                Path = path;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("No pictures.", ex);
            }
        }

        public void Save(string path=null)
        {
            if (path != null)
                Path = path;
            try {
                string text = string.Join("\r\n", this.Select(p => p.FileName + "\r\n" + p.Title).ToArray());
                string filePath = Combine(Path, INDEX);
                File.WriteAllText(filePath, text); 
                foreach (var picture in this)
                {
                    WriteImage(picture.ImageSrc, Combine(Path, picture.FileName));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Cannot save the collection.", ex);
            }
        }


        public Picture New(string fileName)
        {
            var newPicture = new Picture
            {
                FileName = GetFileName(fileName),
                Title = "No Title",
                ImageSrc = new BitmapImage(new Uri(fileName))
            };
            Add(newPicture);
            return newPicture;
        }

        private static List<Picture> ReadData(string path)
        {
            string filePath = Combine(path, INDEX);
            string[] lines = File.ReadAllText(filePath).Trim().Split('\n');

            var pictures = new List<Picture>();
            for (int i = 0; i < lines.Length; i += 2)
            {
                string id = lines[i].Trim();
                string title = lines[i + 1].Trim();
                string imgPath = Combine(path, id);
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
                switch (GetExtension(fname))
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ".gif":
                        encoder = new GifBitmapEncoder();
                        break;
                }
                encoder.Frames.Add(BitmapFrame.Create(wbitmap));
                encoder.Save(stream);
            }
        }


    }

}
