using Application.DTOs.Result;

namespace Application.Interfaces
{
    public interface IResult<T>
    {

        string Version { get; set; }
        List<string>? ErrorMessages { get; set; }
        PagingInfo? PagingInfo { get; set; }
        DateTime Timestamp { get; set; }

    }

}
