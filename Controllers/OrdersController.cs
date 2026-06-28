using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KinetiqueAPI.Data;
using KinetiqueAPI.Models;
using KinetiqueAPI.Services;

namespace KinetiqueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public OrdersController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            return Ok(await _context.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync());
        }

        // GET: api/orders/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetCustomerOrders(string customerId)
        {
            // Cleanly parse the incoming text string into a Guid format
            if (!Guid.TryParse(customerId, out Guid customerGuid))
            {
                return BadRequest("Invalid Customer ID format. Must be a valid UUID.");
            }

            // Compare Guid to Guid
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerGuid)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }

        // PUT: api/orders/updatestatus/{id}
        [HttpPut("updatestatus/{id}")]
        public async Task<ActionResult<Order>> UpdateOrderStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound($"Order {id} not found");

            order.Status = request.Status;
            await _context.SaveChangesAsync();

            // Send email notification
            try
            {
                await _emailService.SendOrderStatusEmailAsync(
                    request.CustomerEmail,
                    id,
                    request.Status
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }

            return Ok(order);
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }
}

