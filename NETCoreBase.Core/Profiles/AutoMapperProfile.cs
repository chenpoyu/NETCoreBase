using AutoMapper;
using NETCoreBase.Common.Helpers;
using NETCoreBase.Core.Commands.Features;
using NETCoreBase.Core.Commands.Roles;
using NETCoreBase.Core.Commands.Users;
using NETCoreBase.Database;
using NETCoreBase.Database.Models;

namespace NETCoreBase.Core.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            #region User 使用者
            CreateMap<User, User>();
            CreateMap<User, UserByIdResponse>()
                .ForMember(c => c.Name, c => c.MapFrom(source => source.NormalizedUserName));
            CreateMap<User, UserListResponse>()
                .ForMember(c => c.Name, c => c.MapFrom(source => source.NormalizedUserName));
            // CreateMap<UserLogin, UserLogin>();
            CreateMap<CreateUserRequest, User>();
            #endregion

            #region Role 角色
            CreateMap<Role, Role>();
            CreateMap<Role, RoleByIdResponse>();
            CreateMap<Role, RoleListResponse>();
            CreateMap<CreateRoleRequest, Role>();
            CreateMap<Role, CreateRoleRequest>();
            #endregion

            #region Role 角色
            CreateMap<Feature, Feature>();
            CreateMap<Feature, FeatureByIdResponse>();
            CreateMap<Feature, FeatureListResponse>();
            CreateMap<CreateFeatureRequest, Feature>();
            CreateMap<Feature, CreateFeatureRequest>();
            #endregion

        }
    }
}