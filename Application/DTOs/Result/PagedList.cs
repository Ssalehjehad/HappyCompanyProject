
using Microsoft.EntityFrameworkCore;

namespace Application.DTOs.Result
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IQueryable<T> source, int index, int pageSize)
        {

        }

        public PagedList(List<T> source, int index, int pageSize)
        {

        }
        public PagedList(List<T> data)
        {
            AddRange(data);
        }

        public static async Task<PagedList<T>> InitiatePaging(IQueryable<T> source, int index, int pageSize)
        {

            PagedList<T> PagedList = new PagedList<T>(await source.Skip(index * pageSize).Take(pageSize).ToListAsync(), index, pageSize);
            return PagedList;
        }


    }

}
