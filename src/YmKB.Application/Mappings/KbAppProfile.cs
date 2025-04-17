using AutoMapper;
using YmKB.Application.Features.KbApps.Commands;
using YmKB.Application.Features.KbApps.DTOs;
using YmKB.Application.Features.KnowledgeDbs.DTOs;
using YmKB.Domain.Entities;

namespace YmKB.Application.Mappings;

public class KbAppProfile : Profile
{
    public KbAppProfile()
    {
        CreateMap<CreateKbAppCommand, KbApp>();
        CreateMap<UpdateKbAppCommand, KbApp>();
        CreateMap<KbApp, KbAppDto>().ReverseMap();
    }
}