using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Imagegram.Api.Services
{
    public class FileService
    {
        private readonly string _baseDirectory;

        public FileService(IConfiguration configuration)
        {
            _baseDirectory = configuration["ImagesBaseDirectory"];
        }

        public virtual async Task<string> Save(Stream data)
        {
            var fileName = CreateNewName();
            var filePath = GetFilePath(fileName);

            using (var file = File.OpenWrite(filePath))
            {
                await data.CopyToAsync(file);
            }

            return fileName;
        }

        public virtual bool Exists(string name)
        {
            var path = GetFilePath(name);
            return File.Exists(path);
        }

        public virtual async Task<Stream> Get(string name)
        {
            var path = GetFilePath(name);

            using var file = File.OpenRead(path);
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;
            return stream;
        }

        public virtual void Delete(string name)
        {
            var path = GetFilePath(name);
            File.Delete(path);
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
