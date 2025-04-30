using MudBlazor;
using YmKB.UI.Models.SideMenu;

namespace YmKB.UI.ConstantConfigs;

/// <summary>
/// 表示应用程序的默认导航菜单配置。
/// </summary>
public static class NavMenuConfig
{
    public static List<MenuSectionModel> Default =
    [
        new()
        {
            Title = "GENERAL",
            SectionItems =
            [
                new()
                {
                    Title = "仪表",
                    Icon = Icons.Material.Filled.Speed,
                    Href = "/"
                },
                new()
                {
                    Title = "聊天",
                    Icon = Icons.Material.Filled.Message,
                    Href = "/chat"
                },
                new()
                {
                    Title = "应用",
                    Icon = Icons.Material.Filled.Apps,
                    Href = "/applications",
                },
                new()
                {
                    Title = "知识",
                    IsParent = true,
                    Icon = Icons.Material.Filled.AutoStories,
                    MenuItems =
                    [
                        new()
                        {
                            Title = "知识库",
                            Href = "/knowledgebases",
                        },
                        new()
                        {
                            Title = "文档",
                            Href = "/docfiles",
                        },
                    ]
                },
                new()
                {
                    Title = "插件",
                    Icon = Icons.Custom.Brands.Calculator,
                    Href = "/plugins",
                },
            ]
        },
        new()
        {
            Title = "CONFIGURATION",
            SectionItems =
            [
                new()
                {
                    Title = "模型",
                    Icon = Icons.Material.Filled.AllInclusive,
                    Href = "/aimodels",
                },
                new()
                {
                    Title = "用户",
                    Icon = Icons.Material.Filled.PeopleAlt,
                    Href = "/users",
                }
            ]
        }
    ];
}
