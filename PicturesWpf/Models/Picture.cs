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
}
