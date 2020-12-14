using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListExtension
    {

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IEnumerable<T> ts,
            int pageIndex,
            int pageSize)
        {
            var Skip = (pageIndex - 1) * pageSize;
            if (Skip < 0)
            {
                Skip = 0;
            }
            var page = new Page<T>();
            var totalItems = ts.Count();
            page.Items = ts.Skip(Skip).Take(pageSize).ToList();
            var totalPages = totalItems != 0 ? (totalItems % pageSize) == 0 ? (totalItems / pageSize) : (totalItems / pageSize) + 1 : 0;
            page.CurrentPage = pageIndex;
            page.ItemsPerPage = pageSize;
            page.TotalItems = totalItems;
            page.TotalPages = totalPages;
            return page;
        }
    }
}
