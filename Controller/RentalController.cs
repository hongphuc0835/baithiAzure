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
    [Route("api/rental")]
    public class RentalsController : ControllerBase
    {
        private readonly ComicSystem _context;

        public RentalsController(ComicSystem context)
        {
            _context = context;
        }

        // CREATE: Thêm một bản ghi thuê mới
        [HttpPost]
        public async Task<ActionResult<Rentals>> CreateRental(Rentals rental)
        {
            // Thêm rental vào DbContext
            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            // Trả về kết quả với mã trạng thái Created và URL mới của rental
            return CreatedAtAction(nameof(GetRental), new { id = rental.RentalID }, rental);
        }

        // READ: Lấy tất cả các bản ghi thuê
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rentals>>> GetRentals()
        {
            // Trả về tất cả các rental từ cơ sở dữ liệu
            return await _context.Rentals.Include(r => r.Customers).Include(r => r.RentalDetails).ToListAsync();
        }

        // READ: Lấy thông tin của một rental cụ thể theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Rentals>> GetRental(int id)
        {
            // Tìm rental theo ID
            var rental = await _context.Rentals.Include(r => r.Customers).Include(r => r.RentalDetails).FirstOrDefaultAsync(r => r.RentalID == id);

            // Nếu không tìm thấy, trả về lỗi 404
            if (rental == null)
            {
                return NotFound();
            }

            return rental;
        }

        // UPDATE: Cập nhật thông tin của rental
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRental(int id, Rentals rental)
        {
            // Kiểm tra ID rental có khớp không
            if (id != rental.RentalID)
            {
                return BadRequest();
            }

            // Cập nhật trạng thái của rental
            _context.Entry(rental).State = EntityState.Modified;

            try
            {
                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Kiểm tra xem rental có tồn tại không
                if (!RentalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Trả về mã trạng thái NoContent khi cập nhật thành công
        }

        // DELETE: Xóa rental theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRental(int id)
        {
            // Tìm rental theo ID
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            // Xóa rental khỏi cơ sở dữ liệu
            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về mã trạng thái NoContent khi xóa thành công
        }

        // Kiểm tra xem rental có tồn tại không
        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.RentalID == id);
        }
         [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<RentalReport>>> GetRentalReport(DateTime startDate, DateTime endDate)
        {
            // Truy vấn các rental trong khoảng thời gian
            var rentals = await _context.Rentals
                .Where(r => r.RentalDate >= startDate && r.RentalDate <= endDate)
                .Include(r => r.RentalDetails)
                .ThenInclude(rd => rd.ComicBooks)
                .Include(r => r.Customers)
                .ToListAsync();

            // Tạo danh sách báo cáo theo định dạng yêu cầu
            var rentalReports = rentals.SelectMany(r => r.RentalDetails.Select(rd => new RentalReport
            {
                BookTitle = rd.ComicBooks.Title,
                RentalDate = r.RentalDate,
                ReturnDate = r.ReturnDate,
                FullName = r.Customers.FullName,
                Quantity = rd.Quantity
            })).ToList();

            // Trả về danh sách báo cáo
            return Ok(rentalReports);
        }
    }
    public class RentalReport
    {
        public string? BookTitle { get; set; }
        public DateTime? RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? FullName { get; set; }
        public int? Quantity { get; set; }
    }
}
