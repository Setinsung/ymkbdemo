using YmKB.Domain.Entities;

namespace YmKB.Application.Features.JsFunctionCalls.DTOs;

public class JsFunctionCallDto
{
    public string Id { get; set; }
    
    public string Name { get; set; }

    public string Description { get; set; }

    public string ScriptContent { get; set; }

    public string MainFuncName { get; set; }

    public List<JsFunctionParameter> Parameters { get; set; }

}