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
    public class CreatePostHandler : IRequestHandler<CreatePostRequest, CreatePostResponse>
    {
        private readonly ImageService _imageService;
        private readonly ApplicationContext _db;

        public CreatePostHandler(ImageService imageService, ApplicationContext db)
        {
            _imageService = imageService;
            _db = db;
        }

        public async Task<CreatePostResponse> Handle(CreatePostRequest request, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.FindAsync(new object[] { request.AccountId }, cancellationToken: cancellationToken);

            var imageName = _imageService.SaveStream(request.ImageStream);

            var post = new PostModel
            {
                ImageUrl = $"/images/{imageName}",
                Creator = account,
                CreatedAt = DateTime.UtcNow
            };
            _db.Posts.Add(post);
            await _db.SaveChangesAsync(cancellationToken);

            return new CreatePostResponse
            {
                Id = post.Id,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                Creator = new AccountDto
                {
                    Id = post.Creator.Id,
                    Name = post.Creator.Name
                }
            };
        }
    }
}
