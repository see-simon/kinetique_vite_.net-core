using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KinetiqueAPI.Data;
using KinetiqueAPI.Services;

namespace KinetiqueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayFastController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public PayFastController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // PayFast ITN notification endpoint
        [HttpPost("notify")]
        public async Task<IActionResult> Notify([FromForm] IFormCollection form)
        {
            try
            {
                var paymentStatus = form["payment_status"].ToString();
                var mPaymentId = form["m_payment_id"].ToString();
                var emailAddress = form["email_address"].ToString();

                if (!int.TryParse(mPaymentId, out int orderId))
                    return BadRequest("Invalid order ID");

                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return NotFound("Order not found");

                if (paymentStatus == "COMPLETE")
                {
                    order.Status = "processing";
                    await _context.SaveChangesAsync();

                    // Send email notification
                    try
                    {
                        await _emailService.SendOrderStatusEmailAsync(
                            emailAddress,
                            orderId,
                            "processing"
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Email failed: {ex.Message}");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PayFast notify error: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}