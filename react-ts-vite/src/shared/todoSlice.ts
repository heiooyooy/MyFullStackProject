import { createSlice } from "@reduxjs/toolkit";

const initialState = {
  todoId: 0,
};

const todoReducer = createSlice({
  name: "todo",
  initialState,
  reducers: {
    // 同步的 logout action
    updateTodoId: (state, action) => {
      state.todoId = action.payload;
    },
  },
});

export const { updateTodoId } = todoReducer.actions;

export default todoReducer.reducer;
