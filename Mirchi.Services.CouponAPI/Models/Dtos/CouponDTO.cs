using System.ComponentModel.DataAnnotations;

namespace Mirchi.Services.CouponAPI.Models.Dtos
{
    public class CouponDTO
    {        
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
    }
}
