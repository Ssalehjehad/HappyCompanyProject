
using Microsoft.EntityFrameworkCore;

namespace Application.DTOs.Result
{
    public static class PagedListExtension
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int index, int pageSize)
        {
            return await PagedList<T>.InitiatePaging(source, index, pageSize);
        }


        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, PagingInfo pagingInfo)
        {

            //Get Total count
            pagingInfo.TotalCount = await source.CountAsync();

            var PagedList = new PagedList<T>(await source.Skip(pagingInfo.CurrentPage * pagingInfo.PageSize)
                .Take(pagingInfo.PageSize).ToListAsync());

            return PagedList;

        }
    }

}
