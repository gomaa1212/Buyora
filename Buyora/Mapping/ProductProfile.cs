using AutoMapper;
using Buyora.Areas.Admin.ViewModels.Product;
using Buyora.Helper;
using BuyoraModels;

namespace Buyora.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, GetProductsListViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => CultureHelper.IsEnglish() ? src.NameEn : src.NameAr))
                .ForMember(des => des.MainUrl, opt => opt.MapFrom(src => src.ProductImages.Where(x => x.IsMain == true).Select(x => x.Url).FirstOrDefault()));

            CreateMap<AddProductViewModel, Product>();

            CreateMap<Product, GetProductByIdViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => CultureHelper.IsEnglish() ? src.NameEn : src.NameAr))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => CultureHelper.IsEnglish() ? src.DescriptionEn : src.DescriptionAr));
            
            CreateMap<ProductImage, ProductImageViewModel>();

            CreateMap<EditProductViewModel, Product>().ForMember(d => d.ProductImages, o => o.Ignore()).ReverseMap();



        }
    }
}
