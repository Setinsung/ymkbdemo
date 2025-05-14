using AutoMapper;
using YmKB.Application.Features.JsFunctionCalls.Commands;
using YmKB.Application.Features.JsFunctionCalls.DTOs;
using YmKB.Domain.Entities;

namespace YmKB.Application.Mappings;

public class JsFunctionCallProfile : Profile
{
    public JsFunctionCallProfile()
    {
        CreateMap<CreateJsFunctionCallCommand, JsFunctionCall>();
        CreateMap<UpdateJsFunctionCallCommand, JsFunctionCall>();
        CreateMap<JsFunctionCall, JsFunctionCallDto>().ReverseMap();
    }
}