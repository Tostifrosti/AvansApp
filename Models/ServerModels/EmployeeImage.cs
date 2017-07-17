using System;
using Windows.UI.Xaml.Media.Imaging;

namespace AvansApp.Models.ServerModels
{
    public class EmployeeImage
    {
        public double height { get; set; }
        public double width { get; set; }
        public string image { get; set; } // De afbeelding is base64 gecodeerd
        public bool isPlaceholder { get; set; }
        public DateTime lastUpdated { get; set; }
        public double size { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public BitmapImage bitmap { get; set; }

        public async System.Threading.Tasks.Task<BitmapImage> Base64StringToBitmap(string source)
        {
            var ims = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            var bytes = Convert.FromBase64String(source);
            var dataWriter = new Windows.Storage.Streams.DataWriter(ims);
            dataWriter.WriteBytes(bytes);
            await dataWriter.StoreAsync();
            ims.Seek(0);
            var img = new BitmapImage();
            img.SetSource(ims);
            return img;
        }
    }
}
