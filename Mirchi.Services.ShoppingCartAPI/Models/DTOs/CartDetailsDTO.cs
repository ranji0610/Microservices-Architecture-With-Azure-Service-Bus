using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mirchi.Services.ShoppingCartAPI.Models.DTOs
{
    public class CartDetailsDTO
    {        
        public int CartDetailsId { get; set; }
        
        public int CartHeaderId { get; set; }

        public virtual CartHeaderDTO CartHeader { get; set; }
        
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Count { get; set; }
    }
}
