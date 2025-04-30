using MudBlazor;

namespace YmKB.UI.ConstantConfigs;

public static class Theme
{
    public static MudTheme AppTheme = new()
    {
        PaletteLight = new()
        {
            // **主色调**
            Primary = "#7C4DFF", // 调整后的紫色，用于突出显示和关键元素
            // Secondary = "#9E9E9E", // 深灰色，次要颜色
            Black = "#110e2d",
            // **背景和表面颜色**
            // Background = "#FFFFFF", // 标准的浅色背景颜色
            // Surface = "#FFFFFF", // 浅灰色，用于卡片等表面
            // **文本颜色**
            // TextPrimary = "#424242", // 深灰色，主要文本颜色
            // TextSecondary = "#6D6D6D", // 中灰色，次要文本颜色
            // TextDisabled = "rgba(0,0,0,0.38)", // 半透明黑色，禁用文本颜色
            // **强调色**
            Info = "#4a86ff", // 蓝色，用于信息性消息
            Success = "#3dcb6c", // 绿色，用于成功消息
            Warning = "#ffb545", // 橙色，用于警告消息
            Error = "#ff3f5f", // 红色，用于错误消息
            // **强调色对应的对比文本颜色**
            // SuccessContrastText = "#FFFFFF", // 白色，成功消息的文本颜色
            // WarningContrastText = "#FFFFFF", // 白色，警告消息的文本颜色
            // ErrorContrastText = "#FFFFFF", // 白色，错误消息的文本颜色
            // InfoContrastText = "#FFFFFF", // 白色，信息性消息的文本颜色
            // **分隔线和边框颜色**
            Divider = "rgba(0,0,0,0.12)", // 半透明黑色，用于分隔线
            // **悬停和涟漪效果**
            HoverOpacity = 0.04, // 悬停效果的不透明度
            RippleOpacity = 0.08, // 涟漪效果的不透明度
            // **覆盖层**
            // OverlayLight = "rgba(255,255,255,0.5)", // 半透明白色，用于覆盖层
            // **应用栏和导航栏**
            AppbarBackground = "rgba(255,255,255,0.8)", // 浅色表面
            AppbarText = "#424242", // 深灰色，应用栏文本颜色
            DrawerBackground = "#FFFFFF", // 浅色表面
            // DrawerText = "#424242", // 深灰色，侧边栏文本颜色
            // **主色调对应的对比文本颜色**
            // PrimaryContrastText = "#FFFFFF", // 白色，主色调上的文本颜色
            GrayLight = "#e8e8e8",
            GrayLighter = "#f9f9f9"
        },
        PaletteDark = new()
        {
            // **主色调**
            Primary = "#7e6fff", // 深蓝色，用于突出显示和关键元素
            Secondary = "#B0BEC5", // 浅灰色，次要文本颜色
            // **背景和表面颜色**
            Background = "#1a1a27", // 标准的深色模式背景颜色
            Surface = "#1e1e2d", // 稍浅的深灰色，用于卡片等表面
            // **文本颜色**
            TextPrimary = "#b2b0bf", // 白色，主要文本颜色
            TextSecondary = "#B0BEC5", // 浅灰色，次要文本颜色
            // TextDisabled = "rgba(255,255,255,0.38)", // 半透明白色，禁用文本颜色
            // **强调色**
            Info = "#4a86ff", // 蓝色，用于信息性消息
            Success = "#3dcb6c", // 绿色，用于成功消息
            Warning = "#ffb545", // 琥珀色，用于警告消息
            Error = "#ff3f5f", // 红色，用于错误消息
            // **强调色对应的对比文本颜色**
            // SuccessContrastText = "#FFFFFF", // 白色，成功消息的文本颜色
            // WarningContrastText = "#FFFFFF", // 黑色，警告消息的文本颜色
            // ErrorContrastText = "#FFFFFF", // 白色，错误消息的文本颜色
            // InfoContrastText = "#FFFFFF", // 白色，信息性消息的文本颜色
            // **分隔线和边框颜色**
            Divider = "rgba(255,255,255,0.12)", // 半透明白色，用于分隔线
            // **悬停和涟漪效果**
            HoverOpacity = 0.08, // 悬停效果的不透明度
            RippleOpacity = 0.12, // 涟漪效果的不透明度
            // **覆盖层**
            OverlayDark = "rgba(0,0,0,0.5)", // 半透明黑色，用于覆盖层
            OverlayLight = "rgba(30,30,30,0.4)",
            // **应用栏和导航栏**
            AppbarBackground = "rgba(26,26,39,0.8)", // 与表面颜色相同
            AppbarText = "#92929f", // 白色，应用栏文本颜色
            DrawerBackground = "#1a1a27", // 与表面颜色相同
            DrawerText = "#92929f", // 白色，侧边栏文本颜色
            // **主色调对应的对比文本颜色**
            // PrimaryContrastText = "#FFFFFF", // 白色，主色调上的文本颜色
            GrayLight = "#2a2833",
            GrayLighter = "#1e1e2d",
            BackgroundGray = "#151521",
            ActionDefault = "#74718e",
            ActionDisabled = "#9999994d",
            ActionDisabledBackground = "#605f6d4d",
            TextDisabled = "#ffffff33",
            DrawerIcon = "#92929f",
            LinesDefault = "#33323e",
            TableLines = "#33323e",
        }
    };
}
