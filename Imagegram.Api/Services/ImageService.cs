using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Imagegram.Api.Services
{
    public class ImageService
    {
        private readonly string _baseDirectory;

        public ImageService(IConfiguration configuration)
        {
            _baseDirectory = configuration["ImagesBaseDirectory"];
        }

        public string Save(Stream data)
        {
            var name = CreateNewName();
            var path = GetFilePath(name);

            using (var file = File.OpenWrite(path))
            {
                ConvertImageToJpg(data, file);
            }

            return name;        
        } 
        
        public bool Exists(string name)
        {
            var path = GetFilePath(name);
            return File.Exists(path);
        }

        public Stream Get(string name)
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

        public void Delete(string name)
        {
            var path = GetFilePath(name);
            File.Delete(path);
        }

        private void ConvertImageToJpg(Stream imageStream, Stream destinationStream)
        {
            using var image = Image.FromStream(imageStream, false);
            image.Save(destinationStream, ImageFormat.Jpeg);
        }

        private string CreateNewName()
        {
            return $@"{Guid.NewGuid()}.jpg";
        }

        private string GetFilePath(string name)
        {
            return Path.Combine(GetBaseDirectory(), name);
        }

        private string GetBaseDirectory()
        {
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
            return _baseDirectory;
        }
    }
}
