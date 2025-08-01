import axios from "axios";
import React, { useEffect, useRef, useState } from "react";

// 假设这是我们 API 返回的 Post 类型
interface Post {
  userId: number;
  id: number;
  title: string;
  body: string;
}

const searchApi = (query: string) => {
  return new Promise<string[]>((res) => {
    setTimeout(() => {
      console.log(`api request finished: ${query}`);
      const results = [
        `结果 1 for "${query}"`,
        `结果 2 for "${query}"`,
        `结果 3 for "${query}"`,
      ];
      res(results);
    }, 1000);
  });
};

const SearchWithAbortTest = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [results, setResults] = useState<Post[]>([]);
  const [loading, setLoading] = useState(false);

  // useRef 来保存 AbortController
  const controllerRef = useRef<AbortController | null>(null);

  useEffect(() => {
    if (!searchTerm) {
      setResults([]);
      return;
    }

    // 取消上一个请求
    controllerRef.current?.abort();

    // 创建新的控制器
    const newController = new AbortController();
    controllerRef.current = newController;

    setLoading(true);

    const fetchData = async () => {
      try {
        // 直接发起一个真实的 axios 请求
        const response = await axios.get<Post[]>(
          `https://jsonplaceholder.typicode.com/posts`,
          {
            params: { q: searchTerm }, // axios 会处理查询参数
            signal: newController.signal, // 传递 signal
          }
        );
        // 在真实场景中，我们从 response.data 中获取数据
        setResults(response.data);
      } catch (error) {
        if (axios.isCancel(error)) {
          console.log("请求被取消: ", searchTerm);
        } else {
          console.error("请求失败", error);
        }
      } finally {
        setLoading(false);
      }
    };

    // 设置一个防抖 (debounce)，避免过于频繁地请求
    const debounceTimeout = setTimeout(() => {
      fetchData();
    }, 300);

    // 清理函数
    return () => {
      clearTimeout(debounceTimeout);
      controllerRef.current?.abort();
    };
  }, [searchTerm]);

  return (
    <div className="p-6 max-w-lg mx-auto">
      <h1 className="text-2xl font-bold mb-4">即时搜索 (正确实现)</h1>
      <input
        type="text"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        placeholder="搜索帖子标题..."
        className="w-full p-2 border rounded"
      />
      <div className="mt-4">
        {loading && <p>加载中...</p>}
        <ul>
          {results.map((post) => (
            <li key={post.id}>{post.title}</li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default SearchWithAbortTest;
