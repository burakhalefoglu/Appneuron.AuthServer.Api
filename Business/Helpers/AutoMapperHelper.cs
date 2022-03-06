using AutoMapper;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Entities.Concrete;

namespace Business.Helpers
{
    public class AutoMapperHelper : Profile
    {
        public AutoMapperHelper()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}