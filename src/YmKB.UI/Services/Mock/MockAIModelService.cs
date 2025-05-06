// using YmKB.UI.Models;
//
// namespace YmKB.UI.Services;
//
// public class MockAIModelService : IAIModelService
// {
//     private List<AIModel> _models;
//
//     public MockAIModelService()
//     {
//         _models = new List<AIModel>
//         {
//             new AIModel
//             {
//                 Id = "a3d801f0-09bd-4337-b6b4-56d612f03",
//                 Description = "doubao",
//                 AIType = "OpenAI",
//                 ModelType = "会话模型",
//                 Endpoint = "https://ark.cn-beijing.volces.com/api/v1",
//                 Name = "ep-202407140250"
//             },
//             new AIModel
//             {
//                 Id = "1c6efa5c-1c95-46a2-a32f-7419ccbbe9",
//                 Description = "doubaoet",
//                 AIType = "OpenAI",
//                 ModelType = "向量模型",
//                 Endpoint = "https://ark.cn-beijing.volces.com/api/v1",
//                 Name = "ep-202412261258"
//             }
//         };
//     }
//
//     public async Task<List<AIModel>> GetAllModelsAsync()
//     {
//         return await Task.FromResult(_models);
//     }
//
//     public async Task<AIModel> GetModelByIdAsync(string id)
//     {
//         return await Task.FromResult(_models.FirstOrDefault(m => m.Id == id));
//     }
//
//     public async Task<AIModel> CreateModelAsync(AIModel model)
//     {
//         model.Id = Guid.NewGuid().ToString();
//         _models.Add(model);
//         return await Task.FromResult(model);
//     }
//
//     public async Task<AIModel> UpdateModelAsync(string id, AIModel model)
//     {
//         var existingModel = _models.FirstOrDefault(m => m.Id == id);
//         if (existingModel != null)
//         {
//             existingModel.Description = model.Description;
//             existingModel.AIType = model.AIType;
//             existingModel.ModelType = model.ModelType;
//             existingModel.Endpoint = model.Endpoint;
//             existingModel.Name = model.Name;
//             existingModel.Secret = model.Secret;
//         }
//         return await Task.FromResult(existingModel);
//     }
//
//     public async Task DeleteModelAsync(string id)
//     {
//         var model = _models.FirstOrDefault(m => m.Id == id);
//         if (model != null)
//         {
//             _models.Remove(model);
//         }
//         await Task.CompletedTask;
//     }
// }
