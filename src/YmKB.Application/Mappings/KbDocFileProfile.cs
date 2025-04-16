using AutoMapper;
using YmKB.Application.Features.KbDocFiles.Commands;
using YmKB.Domain.Entities;

namespace YmKB.Application.Mappings;

public class KbDocFileProfile : Profile
{
    public KbDocFileProfile()
    {
        CreateMap<CreateKbDocFileCommand, KbDocFile>();
        // CreateMap<AIModel, AIModelDto>().ReverseMap();
    }
}