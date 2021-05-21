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
        private readonly Cache<AccountModel> _accountsCache;
        private readonly DateTimeService _dateTime;

        public CreatePostHandler(FileService fileService, ApplicationContext db, Cache<AccountModel> accountsCache, DateTimeService dateTime)
        {
            _fileService = fileService;
            _db = db;
            _accountsCache = accountsCache;
            _dateTime = dateTime;
        }

        public async Task<PostDto> Handle(CreatePostRequest request, CancellationToken cancellationToken)
        {
            var account = await GetAccount(request.AccountId, cancellationToken);
            var imageFile = await SaveImage(request.ImageData);

            var post = new PostModel
            {
                ImageUrl = $"/images/{imageFile}",
                CreatorId = account.Id,
                CreatedAt = _dateTime.Now()
            };
            await SavePost(post, imageFile, cancellationToken);

            return DtosBuilder.Build(post, account);
        }

        private async Task SavePost(PostModel post, string imageFile, CancellationToken cancellationToken)
        {
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
        }

        private async Task<string> SaveImage(MemoryStream imageData)
        {
            var imageFile = string.Empty;
            try
            {
                using var jpgStream = ConvertImageToJpg(imageData);
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

            return imageFile;
        }

        private async Task<AccountModel> GetAccount(Guid accountId, CancellationToken cancellationToken)
        {
            return await _accountsCache.GetOrCreate(accountId,
                async () => await _db.Accounts.FindAsync(accountId, cancellationToken));
        }

        private static MemoryStream ConvertImageToJpg(Stream imageStream)
        {
            using var image = Image.FromStream(imageStream, false);
            var result = new MemoryStream();
            image.Save(result, ImageFormat.Jpeg);
            result.Position = 0;
            return result;
        }
    }
}
