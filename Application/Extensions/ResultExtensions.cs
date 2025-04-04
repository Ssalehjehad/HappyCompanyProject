using Application.DTOs.Result;
using Application.Store;
using Microsoft.AspNetCore.Mvc;

namespace Application.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            switch (result.StatusCode)
            {
                case StatusCode.Success:
                    return new OkObjectResult(result);

                case StatusCode.NoContent:
                    return new NoContentResult();

                case StatusCode.BadRequest:
                    return new BadRequestObjectResult(result);

                case StatusCode.Unauthenticated:
                    return new ObjectResult(result) { StatusCode = 401 };

                case StatusCode.Unauthorized:
                    return new ObjectResult(result) { StatusCode = 403 };

                case StatusCode.NotFound:
                    return new NotFoundObjectResult(result);

                case StatusCode.AlreadyExist:
                    return new ConflictObjectResult(result);

                case StatusCode.InternalError:
                    return new ObjectResult(result) { StatusCode = 500 };

                default:
                    return new ObjectResult(result) { StatusCode = (int)result.StatusCode };
            }
        }
    }
}
