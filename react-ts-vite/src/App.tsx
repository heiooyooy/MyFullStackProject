import BasicReactHookForm from "./components/BasicReactHookForm";
import FetchTest from "./components/FetchTest";
import LazyQueryDemo from "./components/LazyQueryDemo";
import Login from "./components/LoginTest";
import SuspenseTest from "./components/ManualLazyLoadTest/SuspenseTest";
import SignUpForm from "./components/ReactHookFormZod/SignUpForm";
import ReducerTest from "./components/ReducerTest";
import RouterLoadingTest from "./components/RouterLoadingTest/RouterLoadingTest";
import RTKQueryTest from "./components/RTKQueryTest";
import SearchWithAbortTest from "./components/SearchWithAbortTest";
import SignalRTest from "./components/SignalRTest/SignalRTest";
import SlotButtonTest from "./components/SlotButtonTest";
import SseTestComponent from "./components/SseTestComponent";
import TanstackMutationTest from "./components/TanstackQuery/TanstackMutationTest";
import TanstackPrefetchComponent from "./components/TanstackQuery/TanstackPrefetchTest";
import TanstackQueryTest from "./components/TanstackQuery/TanstackQueryTest";
import UseOptimisticTest from "./components/UseOptimisticTest";
import ComponentCard from "./shared/ComponentCard";

export default function App() {
  return (
    <>
      <div className="bg-gray-100 min-h-screen py-8 px-4 sm:px-6 lg:px-8">
        <div className="mb-20">
          <SignalRTest />
        </div>
        <div className="max-w-7xl mx-auto flex flex-wrap justify-center gap-8">
          {/* <ComponentCard title="SignalR Test">
            <SignalRTest />
          </ComponentCard> */}

          <ComponentCard title="SSE Test">
            <SseTestComponent />
          </ComponentCard>

          <ComponentCard title="SignUp Form">
            <SignUpForm />
          </ComponentCard>
          <ComponentCard title="Login Test">
            <Login />
          </ComponentCard>

          <ComponentCard title="Use Optimistic">
            <UseOptimisticTest />
          </ComponentCard>
          <ComponentCard title="Router Loading">
            <RouterLoadingTest />
          </ComponentCard>
          <ComponentCard title="Basic React Hook Form">
            <BasicReactHookForm />
          </ComponentCard>
          <ComponentCard title="Reducer Test">
            <ReducerTest />
          </ComponentCard>
          <ComponentCard title="Some Tests">
            <FetchTest />
          </ComponentCard>
          <ComponentCard title="Tanstack Query">
            <TanstackQueryTest />
            <TanstackMutationTest />
          </ComponentCard>
          <ComponentCard title="Tanstack Prefetch">
            <TanstackPrefetchComponent />
          </ComponentCard>
          <ComponentCard title="Suspense Test">
            <SuspenseTest />
          </ComponentCard>
          <ComponentCard title="RTK Query Test">
            <RTKQueryTest />
          </ComponentCard>
          <ComponentCard title="Lazy Query Test">
            <LazyQueryDemo />
          </ComponentCard>

          <ComponentCard title="Slot Button">
            <SlotButtonTest />
          </ComponentCard>
          <ComponentCard title="AbortController Test">
            <SearchWithAbortTest />
          </ComponentCard>

          {/* 
          <ComponentCard title="Label Test">
            <MyLabelTest />
          </ComponentCard>
          <ComponentCard title="MUI Test">
            <MUITest />
          </ComponentCard> */}
        </div>
      </div>
    </>
  );
}
