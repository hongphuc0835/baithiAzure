using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using baithi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using baithi.Data;

namespace baithi.Controller
{
    [ApiController]
    [Route("api/book")]
    public class ComicBookController : ControllerBase
    {
        private readonly ComicSystem _context;

        public ComicBookController(ComicSystem context)
        {
            _context = context;
        }

        // CREATE: Thêm một cuốn sách mới
        [HttpPost]
        public async Task<ActionResult<ComicBooks>> CreateComicBook(ComicBooks comicBook)
        {
            _context.ComicBooks.Add(comicBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComicBook), new { id = comicBook.ComicBookID }, comicBook);
        }

        // READ: Lấy tất cả các cuốn sách
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComicBooks>>> GetComicBooks()
        {
            return await _context.ComicBooks.ToListAsync();
        }

        // READ: Lấy thông tin của một cuốn sách cụ thể theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ComicBooks>> GetComicBook(int id)
        {
            var comicBook = await _context.ComicBooks.FindAsync(id);

            if (comicBook == null)
            {
                return NotFound();
            }

            return comicBook;
        }

        // UPDATE: Cập nhật thông tin cuốn sách
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComicBook(int id, ComicBooks comicBook)
        {
            if (id != comicBook.ComicBookID)
            {
                return BadRequest();
            }

            _context.Entry(comicBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComicBookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: Xóa một cuốn sách theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComicBook(int id)
        {
            var comicBook = await _context.ComicBooks.FindAsync(id);
            if (comicBook == null)
            {
                return NotFound();
            }

            _context.ComicBooks.Remove(comicBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ComicBookExists(int id)
        {
            return _context.ComicBooks.Any(e => e.ComicBookID == id);
        }
    }
}
