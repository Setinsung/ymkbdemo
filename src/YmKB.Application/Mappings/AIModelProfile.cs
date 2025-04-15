using AutoMapper;
using YmKB.Application.Features.AIModels.Commands;
using YmKB.Application.Features.AIModels.DTOs;
using YmKB.Domain.Entities;

namespace YmKB.Application.Mappings;

public class AIModelProfile : Profile
{
    public AIModelProfile()
    {
        CreateMap<CreateAIModelCommand, AIModel>();
        CreateMap<AIModel, AIModelDto>().ReverseMap();
    }
}