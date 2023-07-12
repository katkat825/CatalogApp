using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CatalogApp.Models;

namespace CatalogApp;

public class PaginatedList<Book> : List<Book>
{
    public int PageIndex {get; private set;}
    public int TotalPages {get; private set;}

    public PaginatedList(List<Book> book, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        this.AddRange(book);
    }

    public bool HasPrevious => PageIndex > 1;
    public bool HasNext => PageIndex < TotalPages;

    public static async Task<PaginatedList<Book>> CreateAsync(IQueryable<Book> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<Book>(items, count, pageIndex, pageSize);
    }
}
