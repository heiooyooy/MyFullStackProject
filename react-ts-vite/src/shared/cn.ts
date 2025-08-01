import { type ClassValue, clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';

// 用来智能组合 className，避免 Tailwind 样式冲突，语法更简洁。
export function cn(...inputs: ClassValue[]) {
  // clsx 是一个小工具库，用于条件拼接 class 字符串:
  // clsx('a', false && 'b', ['c', null]) // 'a c'
  // 专门用来合并冲突的 Tailwind 类，例如：
  // twMerge('p-2 p-4') // 'p-4'（自动保留最后一个有效的 padding）
  return twMerge(clsx(inputs));
}
