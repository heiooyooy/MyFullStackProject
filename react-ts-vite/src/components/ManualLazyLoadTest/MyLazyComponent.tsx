import React from "react";

// 这是一个普通的组件，它本身没有任何特殊之处
const MyLazyComponent = () => {
  return (
    <div style={{ padding: "20px", border: "2px dashed green" }}>
      <h2>✅ 我是一个被延迟加载的组件！</h2>
      <p>我的代码在 2 秒后才被加载进来。</p>
    </div>
  );
};

export default MyLazyComponent;
