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
    public class Picture
    {
        public string FileName { set; get; }
        public string Title { set; get; }
        public BitmapImage ImageSrc { set; get; }
    }



    public class PictureCollection : ObservableCollection<Picture>
    {
        readonly string _path;
       
        const string TEXT = "titles.txt";

        public PictureCollection(string path)
        {
            Load(path);
            _path = path;
            
        }

        public PictureCollection() {
            Add(new Picture { 
                FileName = "", Title = "Anonimus", 
                ImageSrc = new BitmapImage(new Uri("pack://application:,,,/Images/Anonimus.png")) 
            });                
        }

        public void Load(string path)
        {
            string filePath = Path.Combine(path, TEXT);
            string[] lines = File.ReadAllText(filePath).Trim().Split('\n');
            this.Clear();
            for (int i = 0; i < lines.Length; i += 2)
            {
                string id = lines[i].Trim();
                string title = lines[i+1].Trim();
                string imgPath = Path.Combine(path, id);
                Add(new Picture
                {
                    FileName = id, Title = title,
                    ImageSrc = new BitmapImage(new Uri(imgPath))
                });
            } 
        }

        public void Save()
        {
            if (_path == null)
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
