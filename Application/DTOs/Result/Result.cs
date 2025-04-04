using Application.Interfaces;
using Application.Store;

namespace Application.DTOs.Result
{
    public class Result<T> : IResult<T>
    {
        #region Properties
        public string Version { get; set; } = "1.1";
        public StatusCode StatusCode { get; set; }
        public List<string>? ErrorMessages { get; set; } = null;
        public T Data { get; set; }
        public PagingInfo? PagingInfo { get; set; }
        public string? SuccessMessege { get; set; } = null;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        #endregion

        #region Constructors
        public Result(T data)
        {
            Data = data;
        }


        public Result(T data, PagingInfo pagingInfo)
        {
            Data = data;
            PagingInfo = pagingInfo;
        }
        #endregion

        #region Methods
        public void Success(string successMessage)
        {
            StatusCode = StatusCode.Success;
            SuccessMessege = successMessage;
        }
        #endregion

    }

}
