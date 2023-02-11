using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mirchi.Services.ShoppingCartAPI.Models
{
    public class CartDetails
    {
        [Key]
        public int CartDetailsId { get; set; }

        [ForeignKey("CartHeaderId")]
        public int CartHeaderId { get; set; }

        public virtual CartHeader CartHeader { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Count { get; set; }
    }
}
