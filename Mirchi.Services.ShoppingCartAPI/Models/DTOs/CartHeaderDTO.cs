using System.ComponentModel.DataAnnotations;

namespace Mirchi.Services.ShoppingCartAPI.Models.DTOs
{
    public class CartHeaderDTO
    {        
        public int CartHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
    }
}
