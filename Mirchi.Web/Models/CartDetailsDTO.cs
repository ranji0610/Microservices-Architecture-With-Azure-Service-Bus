namespace Mirchi.Web.Models
{
    public class CartDetailsDTO
    {
        public int CartDetailsId { get; set; }

        public int CartHeaderId { get; set; }

        public virtual CartHeaderDTO CartHeader { get; set; }

        public int ProductId { get; set; }

        public virtual ProductDto Product { get; set; }

        public int Count { get; set; }
    }
}
