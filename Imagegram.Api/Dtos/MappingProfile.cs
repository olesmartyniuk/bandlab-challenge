using AutoMapper;
using Imagegram.Api.Database.Models;

namespace Imagegram.Api.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostModel, PostDto>();
            CreateMap<PostModel, CreatePostResponse>();
            CreateMap<CommentModel, CommentDto>();
            CreateMap<PostModel, AddCommentResponse>();
            CreateMap<AccountModel, AccountDto>();
            CreateMap<AccountModel, CreateAccountResponse>();
        }
    }
}
