using KASHOP.DAL.DTO.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/*
    page        limit   skip        
    1           10      0
    2           10      10
    3           10      20
--------------------------------------
    page        limit   skip        
    1           5      0
    2           5      5
    3           5      10
 
 */
namespace KASHOP.BLL.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginationResponse<T>> ToPaginationAsync<T>(this IQueryable<T> query, int page, int limit)
        {
            var totalCount = await query.CountAsync();
            var data = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

            return new PaginationResponse<T>
            {
                Data = data,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };

        }

    }
}
