import React from "react";
import type { ComponentCardProps } from "./shared-models";

const ComponentCard = ({ children, title }: ComponentCardProps) => {
  return (
    <div className="flex flex-col bg-white border border-gray-200 rounded-xl shadow-lg overflow-hidden font-sans flex-1 basis-[300px] min-w-[280px]">
      {/* 卡片头部：只有在提供了 'title' prop 时才会渲染 */}
      {title && (
        <div className="px-6 py-4 bg-gray-50 border-b border-gray-200">
          <h2 className="text-xl font-bold text-gray-800">{title}</h2>
        </div>
      )}

      {/* 卡片主体：包裹的内容会在这里显示 */}
      {/* - `flex-grow`: 让这个区域填充剩余的垂直空间，使得高度不一的卡片也能保持底部对齐 */}
      <div className="p-6 flex-grow">{children}</div>
    </div>
  );
};

export default ComponentCard;
