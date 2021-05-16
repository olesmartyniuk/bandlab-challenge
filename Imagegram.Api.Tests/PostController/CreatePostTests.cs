using FluentAssertions;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Imagegram.Api.Tests
{
    public class CreatePostTests : BaseControllerTests
    {
        private readonly Guid AccountId;

        public CreatePostTests()
        {
            var db = ConnectToDatabase();
            var account = new AccountModel
            {
                Name = "Test Account"
            };
            db.Accounts.Add(account);
            db.SaveChanges();
            AccountId = account.Id;           
        }

        [Theory]
        [InlineData("image.bmp")]
        [InlineData("image.png")]
        [InlineData("image.jpg")]
        public async Task PostShouldBeCreatedSuccessfully(string imageFileName)
        {
            // Arrange
            using var stream = new FileStream(Path.Combine("Data", imageFileName), FileMode.Open);
            stream.Position = 0;

            var createdDate = DateTime.Now;
            var fileName = Guid.NewGuid().ToString();

            _dateTimeService.Setup(s => s.Now())
                .Returns(createdDate);
            _fileService.Setup(s => s.Save(It.IsAny<Stream>()))
                .ReturnsAsync(fileName);

            // Act
            var response = await Client.PostImage("posts", stream, AccountId);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Check payload
            var payload = await response.GetBody<PostDto>();
            payload.Id.Should().NotBe(0);
            payload.ImageUrl.Should().NotBeNullOrEmpty();
            payload.Creator.Id.Should().Be(AccountId);
            payload.CreatedAt.Should().Be(createdDate);

            // Check database
            var db = ConnectToDatabase();
            var posts = db
                .Posts
                .Include(p => p.CommentBeforeLast)
                .Include(p => p.CommentLast)
                .Include(p => p.Creator)
                .ToList();
            var post = posts.Single();            
            post.Id.Should().Be(payload.Id);
            post.CommentBeforeLast.Should().BeNull();
            post.CommentLast.Should().BeNull();
            post.CommentsCount.Should().Be(0);
            post.Creator.Id.Should().Be(AccountId);
            post.CreatedAt.Should().Be(createdDate);
            post.ImageUrl.Should().Be($"/images/{fileName}");

            // Check mocks
            _fileService
                .Verify(s => s.Save(It.IsAny<Stream>()), Times.Once);
            _fileService.VerifyNoOtherCalls();
            _dateTimeService
                .Verify(s => s.Now(), Times.Once);
            _dateTimeService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task PostShouldNotBeCreatedIfBadImage()
        {
            // Arrange
            using var stream = new MemoryStream();            

            // Act
            var response = await Client.PostImage("posts", stream, AccountId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var db = ConnectToDatabase();
            db.Posts.Count().Should().Be(0);
                
            _fileService.VerifyNoOtherCalls();
            _dateTimeService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task PostShouldNotBeCreatedIfUnauthorized()
        {
            // Arrange
            var wrongAccountId = Guid.Empty;
            using var stream = new MemoryStream();

            // Act
            var response = await Client.PostImage("posts", stream, wrongAccountId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var db = ConnectToDatabase();
            db.Posts.Count().Should().Be(0);

            _fileService.VerifyNoOtherCalls();
            _dateTimeService.VerifyNoOtherCalls();
        }
    }
}
