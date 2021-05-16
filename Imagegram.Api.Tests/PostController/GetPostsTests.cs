using Bogus;
using FluentAssertions;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Imagegram.Api.Tests
{
    public class GetPostsTests : BaseControllerTests
    {
        private readonly AccountModel _account;
        private readonly List<PostModel> _posts;
        private readonly List<CommentModel> _comments;

        public GetPostsTests()
        {
            var db = ConnectToDatabase();

            _account = new AccountModel
            {
                Name = "Test Account"
            };
            db.Accounts.Add(_account);
            db.SaveChanges();

            _posts = GenerateRandomPosts(5);
            db.AddRange(_posts);
            db.SaveChanges();

            _comments = GenerateRandomComments(8);
            db.Comments.AddRange(_comments);

            _posts[0].CommentsCount = 6;
            _posts[0].CommentBeforeLast = _comments[0];
            _posts[0].CommentLast = _comments[1];
            _comments[0].Post = _posts[0];
            _comments[1].Post = _posts[0];

            _posts[1].CommentsCount = 5;
            _posts[1].CommentBeforeLast = _comments[2];
            _posts[1].CommentLast = _comments[3];
            _comments[2].Post = _posts[1];
            _comments[3].Post = _posts[1];

            _posts[2].CommentsCount = 5;
            _posts[2].CommentBeforeLast = _comments[4];
            _posts[2].CommentLast = _comments[5];
            _comments[4].Post = _posts[2];
            _comments[5].Post = _posts[2];

            _posts[3].CommentsCount = 4;
            _posts[3].CommentBeforeLast = _comments[6];
            _posts[3].CommentLast = _comments[7];
            _comments[6].Post = _posts[3];
            _comments[7].Post = _posts[3];

            _posts[4].CommentsCount = 0;
            _posts[4].CommentBeforeLast = null;
            _posts[4].CommentLast = null;

            db.AddRange(_comments);
            db.SaveChanges();
        }

        [Fact]
        public async Task PostShouldBeGotSuccessfully()
        {
            // Act
            var response = await Client.Get("posts?limit=2", _account.Id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.GetBody<GetPostsResponse>();
            var posts = body.Posts;
            var cursor = body.Cursor;

            posts.Count.Should().Be(2);
            posts[0].Id.Should().Be(_posts[0].Id);
            posts[1].Id.Should().Be(_posts[1].Id);

            var comments1 = posts[0].Comments;
            comments1.Count.Should().Be(2);
            comments1[0].Id.Should().Be(_comments[1].Id);
            comments1[1].Id.Should().Be(_comments[0].Id);

            var comments2 = posts[1].Comments;
            comments2.Count.Should().Be(2);
            comments2[0].Id.Should().Be(_comments[3].Id);
            comments2[1].Id.Should().Be(_comments[2].Id);

            cursor.Should().Be("NToy");
        }

        [Fact]
        public async Task PostShouldBeGotSuccessfullyWithCursor()
        {
            // Act
            var response = await Client.Get("posts?cursor=NToy&limit=2", _account.Id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.GetBody<GetPostsResponse>();
            var posts = body.Posts;
            var cursor = body.Cursor;

            posts.Count.Should().Be(2);
            posts[0].Id.Should().Be(_posts[2].Id);
            posts[1].Id.Should().Be(_posts[3].Id);

            var comments1 = posts[0].Comments;
            comments1.Count.Should().Be(2);
            comments1[0].Id.Should().Be(_comments[5].Id);
            comments1[1].Id.Should().Be(_comments[4].Id);

            var comments2 = posts[1].Comments;
            comments2.Count.Should().Be(2);
            comments2[0].Id.Should().Be(_comments[7].Id);
            comments2[1].Id.Should().Be(_comments[6].Id);

            cursor.Should().Be("NDo0");
        }

        [Fact]
        public async Task PostShouldBeGotSuccessfullyWithCursorAtTheEnd()
        {
            // Act
            var response = await Client.Get("posts?cursor=NDo0&limit=2", _account.Id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.GetBody<GetPostsResponse>();
            var posts = body.Posts;
            var cursor = body.Cursor;

            posts.Count.Should().Be(1);
            posts[0].Id.Should().Be(_posts[4].Id);

            var comments = posts[0].Comments;
            comments.Count.Should().Be(0);

            cursor.Should().BeNull();
        }

        private List<PostModel> GenerateRandomPosts(int count)
        {
            var testPosts = new Faker<PostModel>()
                .RuleFor(p => p.Creator, f => _account)
                .RuleFor(p => p.CreatedAt, f => DateTime.Now);

            return testPosts.Generate(count);
        }

        private List<CommentModel> GenerateRandomComments(int count)
        {
            var testComments = new Faker<CommentModel>()
                .RuleFor(c => c.Creator, f => _account)
                .RuleFor(p => p.CreatedAt, f => DateTime.Now);

            return testComments.Generate(count);
        }
    }
}
