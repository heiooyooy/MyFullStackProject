import { useEffect, useState } from "react";

export interface TodoItem {
  userId: number;
  id: number;
  title: string;
  completed: boolean;
}

const useFetch = (url: string): TodoItem[] | null => {
  const [data, setData] = useState(null);
  useEffect(() => {
    fetch(url)
      .then((data) => data.json())
      .then((data) => setData(data));
  }, [url]);

  return data;
};

export default useFetch;
