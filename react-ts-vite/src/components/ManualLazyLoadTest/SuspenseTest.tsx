import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import React, { Suspense, useState } from "react";

const SuspenseTest = () => {
  const [show, setShow] = useState(false);

  const LazyComponentWithDelay = React.lazy(() => {
    console.log("开始加载组件代码...");
    return new Promise<{ default: React.ComponentType<any> }>((resolve) => {
      setTimeout(() => {
        resolve(import("./MyLazyComponent"));
      }, 2000);
    });
  });

  return (
    <Box sx={{ padding: "20px" }}>
      <div>Suspense 示例</div>
      <Button sx={{ marginTop: "10px" }} onClick={() => setShow(true)}>
        Show lazy load component
      </Button>

      <Box sx={{ marginTop: "20px" }}>
        <Suspense
          fallback={
            <Box sx={{ padding: "20px", border: "2px dashed orange" }}>
              <h2>⏳ 正在加载组件...</h2>
            </Box>
          }
        >
          {show && <LazyComponentWithDelay />}
        </Suspense>
      </Box>
    </Box>
  );
};

export default SuspenseTest;
