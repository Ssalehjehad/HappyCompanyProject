
namespace Application.Store
{
    public enum StatusCode
    {
        Success = 200,
        NoContent = 204,
        BadRequest = 400,
        Unauthenticated = 401,
        Unauthorized = 403,
        NotFound = 404,
        InternalError = 500,
        AlreadyExist = 409
    }

}
