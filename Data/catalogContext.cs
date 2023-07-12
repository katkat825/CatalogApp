using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CatalogApp.Models;
using Pomelo.EntityFrameworkCore.MySql;

namespace CatalogApp.Data
{
    public class catalogContext : DbContext
    {
        public catalogContext (DbContextOptions<catalogContext> options)
            : base(options)
        {
        }

        public DbSet<CatalogApp.Models.Book> Book { get; set; } = default!;
    }
}
