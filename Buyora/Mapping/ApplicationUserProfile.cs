using AutoMapper;
using Buyora.Areas.Admin.ViewModels.User;
using Buyora.Areas.Identity.ViewModels;
using BuyoraModels;
using Microsoft.AspNetCore.Identity;

namespace Buyora.Mapping
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<ResetPasswordViewModel, ApplicationBuilder>();

            CreateMap<ApplicationUser, DisplayUsersViewModel>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<IdentityRole, ManageUserRolesViewModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsSelected, opt => opt.Ignore());
        }
    }
}
