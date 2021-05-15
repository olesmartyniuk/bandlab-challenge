using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using System;

namespace Imagegram.Api.Exceptions
{
    public static class ExceptionToStatusCodeMapper
    {
        public static void Map(ProblemDetailsOptions options)
        {
            options.MapToStatusCode<InvalidParameterException>(StatusCodes.Status400BadRequest);
            options.MapToStatusCode<PostNotFoundException>(StatusCodes.Status404NotFound);

            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        }
    }
}
