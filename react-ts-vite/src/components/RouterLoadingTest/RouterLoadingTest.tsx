import React from "react";
import { Link, Outlet, useNavigation } from "react-router-dom";

const RouterLoadingTest = () => {
  const navigate = useNavigation();

  const isLoading = navigate.state === "loading";

  return (
    <div>
      <Link to="/fast">首页</Link> | <Link to="/slow">慢页面</Link>|{" "}
      <Link to="/newPage">New页面</Link>
      {isLoading && (
        <div style={{ color: "red", margin: "10px" }}>Loading...</div>
      )}
      {!isLoading && (
        <div style={{ padding: "20px", border: "1px solid #ccc" }}>
          <Outlet />
        </div>
      )}
    </div>
  );
};

export default RouterLoadingTest;
