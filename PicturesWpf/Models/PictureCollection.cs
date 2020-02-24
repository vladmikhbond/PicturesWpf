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
       
        const string TEXT = "index.txt";

        public PictureCollection() {
            Add(new Picture { 
                FileName = "", Title = "Anonimus", 
                ImageSrc = new BitmapImage(new Uri("pack://application:,,,/Images/Anonimus.png")) 
            });
            _path = "";
        }

        private List<Picture> ReadData(string path)
        {
            string filePath = Path.Combine(path, TEXT);
            string[] lines = File.ReadAllText(filePath).Trim().Split('\n');
            var lst = new List<Picture>();
            for (int i = 0; i < lines.Length; i += 2)
            {
                string id = lines[i].Trim();
                string imgPath = Path.Combine(path, id);
                lst.Add(new Picture
                {
                    FileName = id,
                    Title = lines[i + 1].Trim(),
                    ImageSrc = new BitmapImage(new Uri(imgPath))
                });
            }           
            return lst;
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
            string text = string.Join("\r\n", this.Select(p => p.FileName + "\r\n" + p.Title).ToArray());
            string filePath = Path.Combine(_path, TEXT);
            File.WriteAllText(filePath, text);           
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
    }

}
