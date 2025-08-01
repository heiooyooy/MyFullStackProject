import type { QueryClient } from "@tanstack/react-query";
import type { ComponentType } from "react";
import type { LoaderFunction, ActionFunction } from "react-router-dom";

export interface ComponentCardProps {
  children: React.ReactNode;
  title: string;
}

export type LazyRouteModule = {
  default: ComponentType;
  clientLoader?: (queryClient: QueryClient) => LoaderFunction;
  clientAction?: ActionFunction;
  ErrorBoundary?: ComponentType;
  handle?: unknown;
  [key: string]: unknown;
};
