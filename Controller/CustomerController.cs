using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using baithi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using baithi.Data;
using baithi.Models;

namespace baithi.Controller
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ComicSystem _context;

        public CustomerController(ComicSystem context)
        {
            _context = context;
        }

        // CREATE: Thêm khách hàng mới
        [HttpPost]
        public async Task<ActionResult<Customers>> CreateCustomer(Customers customer)
        {
            // Thêm khách hàng vào DbContext
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Trả về kết quả với mã trạng thái Created và URL mới của khách hàng
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerID }, customer);
        }

        // READ: Lấy tất cả khách hàng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customers>>> GetCustomers()
        {
            // Trả về tất cả khách hàng từ cơ sở dữ liệu
            return await _context.Customers.ToListAsync();
        }

        // READ: Lấy thông tin của một khách hàng cụ thể theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Customers>> GetCustomer(int id)
        {
            // Tìm khách hàng theo ID
            var customer = await _context.Customers.FindAsync(id);

            // Nếu không tìm thấy, trả về lỗi 404
            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // UPDATE: Cập nhật thông tin khách hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customers customer)
        {
            // Kiểm tra ID khách hàng có khớp không
            if (id != customer.CustomerID)
            {
                return BadRequest();
            }

            // Cập nhật trạng thái của khách hàng
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Kiểm tra xem khách hàng có tồn tại trong cơ sở dữ liệu không
                if (!CustomerExists(id))
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

        // DELETE: Xóa khách hàng theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            // Tìm khách hàng theo ID
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Xóa khách hàng khỏi cơ sở dữ liệu
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về mã trạng thái NoContent khi xóa thành công
        }

        // Kiểm tra xem khách hàng có tồn tại không
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }
         [HttpPost("register")]
        public async Task<ActionResult<Customers>> RegisterCustomer([FromBody] Customers customer)
        {
            // Kiểm tra xem số điện thoại đã tồn tại chưa
            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == customer.PhoneNumber);
            if (existingCustomer != null)
            {
                return BadRequest("Số điện thoại đã tồn tại.");
            }

            // Thêm khách hàng mới vào DbContext
            customer.Registration = DateTime.Now;
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerID }, customer);
        }

        // 2. Đăng nhập khách hàng (kiểm tra số điện thoại và mật khẩu)
        [HttpPost("login")]
        public async Task<ActionResult<Customers>> LoginCustomer([FromBody] Customers customer)
        {
            // Kiểm tra xem khách hàng có tồn tại với số điện thoại và mật khẩu khớp không
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == customer.PhoneNumber && c.Password == customer.Password);
            
            if (existingCustomer == null)
            {
                return Unauthorized("Số điện thoại hoặc mật khẩu không đúng.");
            }

            return Ok(existingCustomer); // Trả về thông tin khách hàng nếu đăng nhập thành công
        }
    }
}
