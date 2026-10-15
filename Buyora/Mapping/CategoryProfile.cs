using AutoMapper;
using Buyora.Areas.Admin.ViewModels.Category;
using Buyora.Helper;
using BuyoraModels;

namespace Buyora.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, GetCategoriesListViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => CultureHelper.IsEnglish() ? src.NameEn : src.NameAr));

            CreateMap<AddCategoryViewModel, Category>();

            CreateMap<Category, EditCategoryViewModel>().ReverseMap();

            CreateMap<Category,GetCategoryByIdViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => CultureHelper.IsEnglish() ? src.NameEn : src.NameAr));
  
        }
    }
}
