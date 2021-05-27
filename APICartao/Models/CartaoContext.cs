using Microsoft.EntityFrameworkCore;


namespace APICartao.Models
{
    public class CartaoContext: DbContext
    {
        public CartaoContext(DbContextOptions<CartaoContext> options) 
            : base(options)
        {    
        }

        public DbSet<CartaoItem> CartaoItems { get; set; }

    }
}