import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import { Outlet } from "react-router-dom";

function MUITest() {
  return (
    <Box
      sx={{
        p: 4,
        display: "flex",
        flexDirection: "column",
        gap: 4,
        maxWidth: "400px",
        margin: "auto",
      }}
    >
      <h1 style={{ fontFamily: "Roboto, sans-serif" }}>
        MUI TextField 核心 Props
      </h1>

      {/* 1. 不同的变体 (variant) */}
      <TextField label="Outlined (默认)" variant="outlined" />
      <TextField label="Filled" variant="filled" />
      <TextField label="Standard" variant="standard" />

      {/* 2. 辅助文本和错误状态 */}
      <TextField
        label="密码"
        type="password"
        helperText="请输入至少8个字符" // 显示在下方的辅助文本
      />
      <TextField
        error // 将这个 prop 设置为 true 来触发错误状态
        label="确认密码"
        type="password"
        helperText="两次输入的密码不一致" // 错误状态下，辅助文本会自动变红
      />

      {/* 3. 其他常用 props */}
      <TextField label="个人简介" multiline rows={4} />
      <TextField label="不可用" disabled />
    </Box>
  );
}

export default MUITest;
