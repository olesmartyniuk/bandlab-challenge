using Imagegram.Api.Database;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using Imagegram.Api.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class CreatePostHandler : IRequestHandler<CreatePostRequest, PostDto>
    {
        private readonly ImageService _imageService;
        private readonly ApplicationContext _db;
        private readonly Cash<AccountModel> _accountsCash;

        public CreatePostHandler(ImageService imageService, ApplicationContext db, Cash<AccountModel> accountsCash)
        {
            _imageService = imageService;
            _db = db;
            _accountsCash = accountsCash;
        }

        public async Task<PostDto> Handle(CreatePostRequest request, CancellationToken cancellationToken)
        {
            var account = await _accountsCash.GetOrCreate(request.AccountId, 
                async () => await _db.Accounts.FindAsync(request.AccountId, cancellationToken));
            
            var imageFile = _imageService.Save(request.ImageStream);

            var post = new PostModel
            {
                ImageUrl = $"/images/{imageFile}",
                CreatorId = account.Id,
                CreatedAt = DateTime.UtcNow
            };

            try
            {               
                _db.Posts.Add(post);
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                _imageService.Delete(imageFile);
                throw;
            }

            return DtosBuilder.Build(post, account);
        }
    }
}
