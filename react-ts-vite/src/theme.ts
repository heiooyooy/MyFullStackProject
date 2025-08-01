// 在你的应用入口附近，比如 App.tsx 或一个专门的 theme.ts 文件
import { createTheme, ThemeProvider } from "@mui/material/styles";

// 使用 createTheme 创建一个自定义主题
export const theme = createTheme({
  // 我们要修改的是 components 部分
  components: {
    // 找到我们要修改的组件，MUI 组件的名字是 Mui + 组件名
    MuiTextField: {
      // 在这里设置默认 props
      defaultProps: {
        variant: "outlined", // 顺便把 variant 也设为默认值
        size: "small", // 这就是你想要的！
        fullWidth: true, // 还可以设置其他任何你想统一的 prop
      },
    },
    // 你也可以在这里为其他组件设置默认值
    MuiButton: {
      defaultProps: {
        variant: "contained",
        size: "medium",
      },
    },
  },
});
