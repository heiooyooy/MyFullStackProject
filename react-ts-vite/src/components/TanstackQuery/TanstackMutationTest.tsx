import Button from "@mui/material/Button";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import React from "react";

const addNewTodoItem = async (): Promise<string> => {
  await new Promise((res) => setTimeout(res, 1000));
  return "Success";
};

const TanstackMutationTest = () => {
  const queryClient = useQueryClient();
  const update = useMutation({
    mutationFn: addNewTodoItem,
    onSuccess: () => {
      console.log("添加成功，正在让列表缓存失效...");
      queryClient.invalidateQueries({ queryKey: ["todos"] });
    },
  });

  const handleClick = () => {
    update.mutate();
  };
  return (
    <div>
      <Button onClick={handleClick}>Update</Button>
    </div>
  );
};

export default TanstackMutationTest;
