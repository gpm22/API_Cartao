using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICartao.Models;

namespace APICartao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoItemsController : ControllerBase
    {
        private readonly CartaoContext _context;

        public CartaoItemsController(CartaoContext context)
        {
            _context = context;
        }

        // GET: api/CartaoItems/id/5
        [HttpGet("id/{id}")]
        public async Task<ActionResult<CartaoItem>> GetCartaoItem(long id)
        {
            var cartaoItem = await _context.CartaoItems.FindAsync(id);

            if (cartaoItem == null)
            {
                return NotFound();
            }

            return cartaoItem;
        }

        // GET: api/CartaoItems/email/exemple@exemple.com
        [HttpGet("email/{Email}")]
        public async Task<ActionResult<IEnumerable<CartaoItem>>> GetCartaoList(String email)
        {
            var cartaoItemList = await _context.CartaoItems.Where(s => s.Email.Equals(email)).ToListAsync();

            if (cartaoItemList == null)
            {
                return NotFound();
            }

            return cartaoItemList;
        }


        // POST: api/CartaoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CartaoItem>> PostCartaoItem(CartaoItem cartaoItem)
        {
            cartaoItem.criarCartao();
            
            _context.CartaoItems.Add(cartaoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCartaoItem), new { id = cartaoItem.Id }, cartaoItem);
        }

    }
}
