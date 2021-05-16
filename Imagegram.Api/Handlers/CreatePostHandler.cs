using Imagegram.Api.Database;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using Imagegram.Api.Exceptions;
using Imagegram.Api.Services;
using MediatR;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class CreatePostHandler : IRequestHandler<CreatePostRequest, PostDto>
    {
        private readonly FileService _fileService;
        private readonly ApplicationContext _db;
        private readonly Cash<AccountModel> _accountsCash;
        private readonly DateTimeService _dateTime;

        public CreatePostHandler(FileService fileService, ApplicationContext db, Cash<AccountModel> accountsCash, DateTimeService dateTime)
        {
            _fileService = fileService;
            _db = db;
            _accountsCash = accountsCash;
            _dateTime = dateTime;
        }

        public async Task<PostDto> Handle(CreatePostRequest request, CancellationToken cancellationToken)
        {
            var account = await _accountsCash.GetOrCreate(request.AccountId, 
                async () => await _db.Accounts.FindAsync(request.AccountId, cancellationToken));
            
            var imageFile = string.Empty;
            try
            {
                using var jpgStream = ConvertImageToJpg(request.ImageData);
                imageFile = await _fileService.Save(jpgStream);
            }
            catch (Exception e)
            {
                if (e.Source == "System.Drawing.Common")
                {
                    throw new BadImageException("Provided image has unsupported format.");
                }

                throw;
            }

            var post = new PostModel
            {
                ImageUrl = $"/images/{imageFile}",
                CreatorId = account.Id,
                CreatedAt = _dateTime.Now()
            };

            try
            {               
                _db.Posts.Add(post);
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                _fileService.Delete(imageFile);
                throw;
            }

            return DtosBuilder.Build(post, account);
        }

        private MemoryStream ConvertImageToJpg(Stream imageStream)
        {
            using var image = Image.FromStream(imageStream, false);
            var result = new MemoryStream();
            image.Save(result, ImageFormat.Jpeg);
            result.Position = 0;
            return result;
        }
    }
}
