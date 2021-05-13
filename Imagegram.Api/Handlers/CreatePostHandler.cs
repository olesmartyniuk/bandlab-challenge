using AutoMapper;
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
        private readonly IMapper _mapper;

        public CreatePostHandler(ImageService imageService, ApplicationContext db, IMapper mapper)
        {
            _imageService = imageService;
            _db = db;
            _mapper = mapper;
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

            return _mapper.Map<CreatePostResponse>(post);
        }
    }
}
