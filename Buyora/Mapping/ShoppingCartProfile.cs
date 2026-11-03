using AutoMapper;
using Buyora.Areas.Customer.ViewModel;
using Buyora.Helper;
using Buyora.SharedViewModel;
using BuyoraModels;

namespace Buyora.Mapping
{
    public class ShoppingCartProfile : Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, DisplayShoppingCartViewModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => CultureHelper.IsEnglish() ? src.Product.NameEn : src.Product.NameAr))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ProductImages.Select(i => i.Url).FirstOrDefault()))
                .ReverseMap();

            CreateMap<OrderDetail, DisplayOrderDetailsViewModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => CultureHelper.IsEnglish() ? src.Product.NameEn : src.Product.NameAr));

            CreateMap<Order, DisplayOrderViewModel>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.ApplicationUser.Email))
                .ReverseMap();



        }
    }
}
