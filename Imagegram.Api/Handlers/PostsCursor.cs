using System;
using System.Text;

namespace Imagegram.Api.Handlers
{
    public class PostsCursor
    {
        public int CommentsCount { get; private set; }
        public int LastPostId { get; private set; }
        public bool IsEmpty { get; private set; }
        public bool IsInvalid { get; private set; }

        public static PostsCursor EmptyCursor()
        {
            return new PostsCursor
            {
                IsEmpty = true
            };
        }

        public static PostsCursor ParseCursor(string base64Value)
        {
            if (string.IsNullOrEmpty(base64Value))
            {
                return EmptyCursor();
            }
            else
            {
                var cursor = new PostsCursor();
                cursor.FromBase64(base64Value);
                return cursor;
            }
        }

        public static PostsCursor NewCursor(int commentsCount, int lastPostId)
        {
            return new PostsCursor
            {
                CommentsCount = commentsCount,
                LastPostId = lastPostId
            };
        }        

        public string ToBase64()
        {
            if (IsEmpty)
            {
                return null;
            }

            var text = $"{CommentsCount}:{LastPostId}";
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        private void FromBase64(string base64Value)
        {
            var bytes = Convert.FromBase64String(base64Value);
            var text = Encoding.UTF8.GetString(bytes);
            var values = text.Split(':');

            if (values.Length == 2 &&
                int.TryParse(values[0], out var commentsCount) &&
                int.TryParse(values[1], out var lastPostId))
            {
                CommentsCount = commentsCount;
                LastPostId = lastPostId;
            }
            else
            {
                IsInvalid = true;
            }
        }
    }
}
