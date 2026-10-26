using AutoMapper;
using Buyora.Areas.Customer.ViewModel;
using Buyora.Helper;
using BuyoraModels;

namespace Buyora.Mapping
{
    public class ShoppingCartProfile : Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, DisplayShoppingCartViewModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => CultureHelper.IsEnglish() ? src.Product.NameEn : src.Product.NameAr))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ProductImages.Select(i => i.Url).FirstOrDefault()));
        }
    }
}
