using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevSteamAPI.Data;
using DevSteamAPI.Models;

namespace DevSteamAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensCarrinhoController : ControllerBase
    {
        private readonly DevSteamAPIContext _context;

        public ItensCarrinhoController(DevSteamAPIContext context)
        {
            _context = context;
        }

        // GET: api/ItensCarrinho
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemCarrinho>>> GetItensCarrinho()
        {
            return await _context.ItemCarrinho.ToListAsync();
        }

        // GET: api/ItensCarrinho/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemCarrinho>> GetItemCarrinho(Guid id)
        {
            var itemCarrinho = await _context.ItemCarrinho.FindAsync(id);

            if (itemCarrinho == null)
            {
                return NotFound();
            }

            return itemCarrinho;
        }

        // POST: api/ItensCarrinho/CalculateTotal
        [HttpPost("CalculateTotal")]
        public ActionResult<decimal> CalculateTotal(ItemCarrinho itemCarrinho)
        {
            var total = itemCarrinho.Quantidade * itemCarrinho.Valor;
            return Ok(total);
        }

        // POST: api/ItensCarrinho/AddItem
        [HttpPost("AddItem")]
        public async Task<ActionResult<ItemCarrinho>> AddItem(ItemCarrinho itemCarrinho)
        {
            var carrinho = await _context.Carrinho.FindAsync(itemCarrinho.CarrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }

            itemCarrinho.Total = itemCarrinho.Quantidade * itemCarrinho.Valor;
            _context.ItemCarrinho.Add(itemCarrinho);
            carrinho.Total += itemCarrinho.Total;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItemCarrinho", new { id = itemCarrinho.ItemCarrinhoId }, itemCarrinho);
        }

        // DELETE: api/ItensCarrinho/RemoveItem/5
        [HttpDelete("RemoveItem/{id}")]
        public async Task<IActionResult> RemoveItem(Guid id)
        {
            var itemCarrinho = await _context.ItemCarrinho.FindAsync(id);
            if (itemCarrinho == null)
            {
                return NotFound();
            }

            var carrinho = await _context.Carrinho.FindAsync(itemCarrinho.CarrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }

            carrinho.Total -= itemCarrinho.Total;
            _context.ItemCarrinho.Remove(itemCarrinho);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/ItensCarrinho/UpdateCartTotal
        [HttpPost("UpdateCartTotal/{carrinhoId}")]
        public async Task<IActionResult> UpdateCartTotal(Guid carrinhoId)
        {
            var carrinho = await _context.Carrinho.FindAsync(carrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }

            var itensCarrinho = await _context.ItemCarrinho
                .Where(ic => ic.CarrinhoId == carrinhoId)
                .ToListAsync();

            carrinho.Total = itensCarrinho.Sum(ic => ic.Total);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemCarrinhoExists(Guid id)
        {
            return _context.ItemCarrinho.Any(e => e.ItemCarrinhoId == id);
        }
    }
}
