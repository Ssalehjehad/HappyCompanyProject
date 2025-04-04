
namespace Application.DTOs.Common
{
    public class PagingParams
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string? SortField { get; set; }
        public bool SortDesc { get; set; } = false;
    }
}
