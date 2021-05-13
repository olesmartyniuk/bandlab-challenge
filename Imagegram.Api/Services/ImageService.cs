using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Imagegram.Api.Services
{
    public class ImageService
    {        
        public string SaveStream(Stream data)
        {
            var name = GenerateName();
            var path = GetFilePath(name);

            using (var file = File.OpenWrite(path))
            {
                ConvertImageToJpg(data, file);
            }

            return name;        
        }             

        public Stream GetStream(string name)
        {
            var path = GetFilePath(name);

            using (var file = File.OpenRead(path))
            {
                var stream = new MemoryStream();
                file.CopyTo(stream);
                stream.Position = 0;
                return stream;
            }
        }

        private void ConvertImageToJpg(Stream imageStream, Stream destinationStream)
        {
            using var image = Image.FromStream(imageStream, false);
            image.Save(destinationStream, ImageFormat.Jpeg);
        }

        private string GenerateName()
        {
            return $@"{Guid.NewGuid()}.jpg";
        }

        private string GetFilePath(string name)
        {
            return Path.Combine(GetBaseDirectory(), name);
        }

        private string GetBaseDirectory()
        {
            var baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "images");
            Directory.CreateDirectory(baseDirectory);
            return baseDirectory;
        }
    }
}
