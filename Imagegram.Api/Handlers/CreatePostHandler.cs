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

        public CreatePostHandler(ImageService imageService, ApplicationContext db)
        {
            _imageService = imageService;
            _db = db;
        }

        public async Task<PostDto> Handle(CreatePostRequest request, CancellationToken cancellationToken)
        {
            var imageName = _imageService.Save(request.ImageStream);

            var post = new PostModel
            {
                ImageUrl = $"/images/{imageName}",
                CreatorId = request.AccountId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _db.Posts.Add(post);

                await _db.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                _imageService.Delete(imageName);
                throw;
            }

            return DtosBuilder.Build(post);
        }
    }
}
