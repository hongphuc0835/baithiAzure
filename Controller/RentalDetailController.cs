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
    [Route("api/rentaldetails")]
    public class RentalDetailsController : ControllerBase
    {
        private readonly ComicSystem _context;

        public RentalDetailsController(ComicSystem context)
        {
            _context = context;
        }

        // CREATE: Thêm một rental detail mới
        [HttpPost]
        public async Task<ActionResult<RentalDetails>> CreateRentalDetail(RentalDetails rentalDetail)
        {
            // Thêm rental detail vào DbContext
            _context.RentalDetails.Add(rentalDetail);
            await _context.SaveChangesAsync();

            // Trả về kết quả với mã trạng thái Created và URL của rental detail mới
            return CreatedAtAction(nameof(GetRentalDetail), new { id = rentalDetail.RentalDetailID }, rentalDetail);
        }

        // READ: Lấy tất cả rental details
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentalDetails>>> GetRentalDetails()
        {
            // Trả về tất cả rental details từ cơ sở dữ liệu
            return await _context.RentalDetails.Include(rd => rd.Rentals)
                                               .Include(rd => rd.ComicBooks)
                                               .ToListAsync();
        }

        // READ: Lấy thông tin của một rental detail cụ thể theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<RentalDetails>> GetRentalDetail(int id)
        {
            // Tìm rental detail theo ID
            var rentalDetail = await _context.RentalDetails.Include(rd => rd.Rentals)
                                                           .Include(rd => rd.ComicBooks)
                                                           .FirstOrDefaultAsync(rd => rd.RentalDetailID == id);

            // Nếu không tìm thấy, trả về lỗi 404
            if (rentalDetail == null)
            {
                return NotFound();
            }

            return rentalDetail;
        }

        // UPDATE: Cập nhật rental detail theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRentalDetail(int id, RentalDetails rentalDetail)
        {
            // Kiểm tra ID rental detail có khớp không
            if (id != rentalDetail.RentalDetailID)
            {
                return BadRequest();
            }

            // Cập nhật trạng thái của rental detail
            _context.Entry(rentalDetail).State = EntityState.Modified;

            try
            {
                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Kiểm tra xem rental detail có tồn tại không
                if (!RentalDetailExists(id))
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

        // DELETE: Xóa rental detail theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRentalDetail(int id)
        {
            // Tìm rental detail theo ID
            var rentalDetail = await _context.RentalDetails.FindAsync(id);
            if (rentalDetail == null)
            {
                return NotFound();
            }

            // Xóa rental detail khỏi cơ sở dữ liệu
            _context.RentalDetails.Remove(rentalDetail);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về mã trạng thái NoContent khi xóa thành công
        }
        [HttpGet("calculate-fee/{rentalDetailId}")]
        public async Task<ActionResult<decimal>> CalculateRentalFee(int rentalDetailId)
        {
            // Lấy rentalDetail theo ID
            var rentalDetail = await _context.RentalDetails
                                              .Include(rd => rd.Rentals)  // Bao gồm thông tin thuê
                                              .Include(rd => rd.ComicBooks)  // Bao gồm thông tin sách
                                              .FirstOrDefaultAsync(rd => rd.RentalDetailID == rentalDetailId);

            // Kiểm tra nếu không tìm thấy rental detail
            if (rentalDetail == null)
            {
                return NotFound();
            }

            // Lấy ngày thuê và ngày trả
            var rentalDate = rentalDetail.Rentals.RentalDate;
            var returnDate = rentalDetail.Rentals.ReturnDate;

            // Kiểm tra nếu ngày trả và ngày thuê đều hợp lệ, nếu không gán giá trị mặc định cho rentalDuration
            int rentalDuration = 1;  // Giá trị mặc định nếu không có ngày trả hợp lệ
            if (rentalDate.HasValue && returnDate.HasValue)
            {
                // Tính số ngày thuê
                rentalDuration = (returnDate.Value - rentalDate.Value).Days;

                // Kiểm tra nếu rentalDuration <= 0, có thể trả về một lỗi nếu ngày trả trước ngày thuê
                if (rentalDuration <= 0)
                {
                    return BadRequest("Ngày trả không hợp lệ (trước ngày thuê).");
                }
            }

            // Lấy giá thuê mỗi cuốn sách (giả sử giá có thể là null, nên sử dụng giá trị mặc định 0)
            var rentalPricePerBook = rentalDetail.ComicBooks.PricePerDay ?? 0;

            // Tính phí thuê (Giả sử phí thuê là: số ngày thuê * số lượng sách * giá thuê mỗi cuốn sách)
            var totalFee = rentalDuration * rentalDetail.Quantity * rentalPricePerBook;

            // Trả về phí thuê
            return Ok(totalFee);
        }



        // Kiểm tra xem rental detail có tồn tại không
        private bool RentalDetailExists(int id)
        {
            return _context.RentalDetails.Any(e => e.RentalDetailID == id);
        }



    }
}
