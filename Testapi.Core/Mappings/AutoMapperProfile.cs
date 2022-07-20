using AutoMapper;
using Testapi.Models;
using Testapi.DataTranserObjects;

namespace Testapi.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Person, PersonDTO>();
        CreateMap<PersonDTO, Person>();
    }
}