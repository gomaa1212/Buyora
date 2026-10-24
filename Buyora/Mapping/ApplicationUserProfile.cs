using AutoMapper;
using Buyora.Areas.Identity.ViewModels;
using BuyoraModels;

namespace Buyora.Mapping
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<ResetPasswordViewModel, ApplicationBuilder>();
        }
    }
}
