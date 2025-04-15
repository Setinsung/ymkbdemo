using AutoMapper;
using YmKB.Application.Features.KnowledgeDbs.Commands;
using YmKB.Application.Features.KnowledgeDbs.DTOs;
using YmKB.Domain.Entities;

namespace YmKB.Application.Mappings;

public class KnowledgeDbProfile : Profile
{
    public KnowledgeDbProfile()
    {
        CreateMap<CreateKnowledgeDbCommand, KnowledgeDb>();
        CreateMap<UpdateKnowledgeDbCommand, KnowledgeDb>();
        CreateMap<KnowledgeDb, KnowledgeDbDto>().ReverseMap();
    }
}