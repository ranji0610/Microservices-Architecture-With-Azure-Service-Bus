using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Mirchi.Services.ShoppingCartAPI.DBContexts;
using Mirchi.Services.ShoppingCartAPI.Models;
using Mirchi.Services.ShoppingCartAPI.Models.DTOs;

namespace Mirchi.Services.ShoppingCartAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly IMapper _mapper;

        public CartRepository(ApplicationDBContext applicationDBContext, IMapper mapper)
        {
            _applicationDBContext = applicationDBContext;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cartHeaderFromDb = await _applicationDBContext.CartHeaders.FirstOrDefaultAsync(cartHeader => cartHeader.UserId == userId);
            cartHeaderFromDb.CouponCode = couponCode;
            _applicationDBContext.CartHeaders.Update(cartHeaderFromDb);
            await _applicationDBContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _applicationDBContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
            if (cartHeaderFromDb == null)
            {
                return false;
            }
            else
            {
                _applicationDBContext.CartDetails.RemoveRange(await _applicationDBContext.CartDetails.Where(u => u.CartHeaderId == cartHeaderFromDb.CartHeaderId).ToListAsync());
                _applicationDBContext.CartHeaders.Remove(cartHeaderFromDb);
                await _applicationDBContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<CartDTO> CreateUpdateCart(CartDTO cartDTO)
        {
            Cart cart = _mapper.Map<Cart>(cartDTO);
            var productInDb = await _applicationDBContext.Products.AsNoTracking().FirstOrDefaultAsync(product => product.ProductId == cartDTO.CartDetails.FirstOrDefault().ProductId);
            if (productInDb == null)
            {
                await _applicationDBContext.Products.AddAsync(cart.CartDetails.FirstOrDefault().Product);
                await _applicationDBContext.SaveChangesAsync();
            }

            var cartHeaderFromDb = await _applicationDBContext.CartHeaders.AsNoTracking().
                FirstOrDefaultAsync(cartHeader => cartHeader.UserId == cartDTO.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                await _applicationDBContext.CartHeaders.AddAsync(cart.CartHeader);
                await _applicationDBContext.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                await _applicationDBContext.CartDetails.AddAsync(cart.CartDetails.FirstOrDefault());
                await _applicationDBContext.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb = await _applicationDBContext.CartDetails.AsNoTracking().
                    FirstOrDefaultAsync(u => u.Product.ProductId == cart.CartDetails.FirstOrDefault().ProductId && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (cartDetailsFromDb == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    await _applicationDBContext.CartDetails.AddAsync(cart.CartDetails.FirstOrDefault());
                    await _applicationDBContext.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                    _applicationDBContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _applicationDBContext.SaveChangesAsync();
                }
            }


            return _mapper.Map<CartDTO>(cart);
        }

        public async Task<CartDTO> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _applicationDBContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId)
            };

            cart.CartDetails = await _applicationDBContext.CartDetails.Include(u => u.Product).AsNoTracking().Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId).ToListAsync();

            return _mapper.Map<CartDTO>(cart);


        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartHeaderFromDb = await _applicationDBContext.CartHeaders.FirstOrDefaultAsync(cartHeader => cartHeader.UserId == userId);
            cartHeaderFromDb.CouponCode = string.Empty;
            _applicationDBContext.CartHeaders.Update(cartHeaderFromDb);
            await _applicationDBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                var cartDetailsFromDb = await _applicationDBContext.CartDetails.FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);
                if (cartDetailsFromDb != null)
                {
                    int totalCountofCartItems = await _applicationDBContext.CartDetails.Where(u => u.CartHeaderId == cartDetailsFromDb.CartHeaderId).CountAsync();
                    _applicationDBContext.CartDetails.Remove(cartDetailsFromDb);
                    if (totalCountofCartItems == 1)
                    {
                        var cardHeaderToRemove = await _applicationDBContext.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetailsFromDb.CartHeaderId);
                        _applicationDBContext.CartHeaders.Remove(cardHeaderToRemove);
                    }
                    await _applicationDBContext.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
