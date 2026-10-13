using AutoMapper;
using Buyora.Helper;
using Buyora.Models;
using Buyora.Models.ViewModel.Category;

namespace Buyora.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, GetCategoriesListViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => CultureHelper.IsEnglish() ? src.NameEn : src.NameAr));
        }
    }
}
