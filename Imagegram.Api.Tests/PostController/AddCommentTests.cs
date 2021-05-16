using FluentAssertions;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Imagegram.Api.Tests
{
    public class AddCommentTests : BaseControllerTests
    {
        private readonly AccountModel _account;        

        public AddCommentTests()
        {
            var db = ConnectToDatabase();
            _account = new AccountModel
            {
                Name = "Test Account"
            };
            db.Accounts.Add(_account);            
            db.SaveChanges();
        }

        [Fact]
        public async Task CommentShouldBeAddedSuccessfully()
        {
            // Arrange
            var content = "Comment";
            var createdDate = DateTime.Now;

            _dateTimeService.Setup(s => s.Now())
                .Returns(createdDate);

            var db = ConnectToDatabase();
            var post = new PostModel
            {
                CreatorId = _account.Id,
                CommentsCount = 0,
                CommentBeforeLast = null,
                CommentLast = null,
                ImageUrl = "/images/image.jpg"
            };
            db.Posts.Add(post);
            db.SaveChanges();

            // Act
            var request = new AddCommentRequest
            {
                Content = content
            };
            var response = await Client.PostJson($"posts/{post.Id}/comments", request, _account.Id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var payload = await response.GetBody<CommentDto>();
            payload.Should().NotBeNull();
            payload.Id.Should().NotBe(0);
            payload.Content.Should().Be(content);
            payload.CreatedAt.Should().Be(createdDate);            

            // Check database
            db = ConnectToDatabase();
            var comment = db.Comments.Single();
            comment.Content.Should().Be(content);
            comment.CreatedAt.Should().Be(createdDate);
            comment.CreatorId.Should().Be(_account.Id);
            comment.PostId.Should().Be(post.Id);

            post = db.Posts.Single();            
            post.CommentBeforeLastId.Should().BeNull();
            post.CommentLastId.Should().Be(comment.Id);
            post.CommentsCount.Should().Be(1);            

            // Check mocks
            _dateTimeService.Verify(s => s.Now(), Times.Once);
            _dateTimeService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CommentShouldNotBeAddedIfWrongBody()
        {            
            // Act            
            var response = await Client.PostJson($"posts/1/comments", "WRONG_BODY", _account.Id);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var db = ConnectToDatabase();
            db.Comments.Count().Should().Be(0);
        }

        [Fact]
        public async Task CommentShouldNotBeAddedIfPostDoesntExist()
        {
            // Act            
            var request = new AddCommentRequest
            {
                Content = "Comment"
            };

            var response = await Client.PostJson($"posts/9999/comments", request, _account.Id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var db = ConnectToDatabase();
            db.Comments.Count().Should().Be(0);
        }

        [Fact]
        public async Task CommentShouldNotBeAddedIfUnauthorized()
        {
            // Arrange
            var wrongAccountId = Guid.Empty;

            // Act
            var request = new AddCommentRequest
            {
                Content = "Comment"
            };

            var response = await Client.PostJson($"posts/1/comments", request, wrongAccountId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var db = ConnectToDatabase();
            db.Comments.Count().Should().Be(0);
        }
    }
}
