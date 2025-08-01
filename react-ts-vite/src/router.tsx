import { createBrowserRouter, RouterProvider } from "react-router-dom";
import App from "./App";
import { useQueryClient, type QueryClient } from "@tanstack/react-query";
import { useMemo } from "react";
import type { LazyRouteModule } from "./shared/shared-models";
import RouterNewpageTest from "./components/RouterNewpageTest";

const converter = (queryClient: QueryClient) => (m: LazyRouteModule) => {
  const { clientLoader, clientAction, default: Component, ...rest } = m;
  return {
    ...rest,
    loader: m.clientLoader?.(queryClient),
    action: m.clientAction,
    Component,
  };
};

export const createAppRouter = (queryClient: QueryClient) =>
  createBrowserRouter([
    {
      path: "/",
      element: <App />,
      children: [
        {
          path: "/slow",
          lazy: () =>
            import("./components/RouterLoadingTest/SlowLoadingTest").then(
              converter(queryClient)
            ),
        },
        {
          path: "/fast",
          lazy: () =>
            import("./components/RouterLoadingTest/FastElementTest").then(
              converter(queryClient)
            ),
        },
      ],
    },
    {
      path: "/newPage",
      element: <RouterNewpageTest />,
    },
  ]);

export const AppRouter = () => {
  const queryClient = useQueryClient();

  const router = useMemo(() => createAppRouter(queryClient), [queryClient]);

  return <RouterProvider router={router} />;
};
