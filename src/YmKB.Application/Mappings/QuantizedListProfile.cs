using AutoMapper;
using YmKB.Application.Features.QuantizedLists.Commands;
using YmKB.Application.Features.QuantizedLists.DTOs;
using YmKB.Domain.Entities;

namespace YmKB.Application.Mappings;

public class QuantizedListProfile : Profile
{
    public QuantizedListProfile()
    {
        CreateMap<CreateQuantizedListCommand, QuantizedList>();
        CreateMap<UpdateQuantizedListCommand, QuantizedList>();
        CreateMap<QuantizedList, QuantizedListDto>().ReverseMap();
    }
}