# Dockerfile

# --- 阶段 1: 构建阶段 (Build Stage) ---
# 使用一个包含 Node.js 的官方镜像作为构建环境。
# 选用 alpine 版本体积更小。

FROM node:20-alpine AS build

# 设置工作目录
WORKDIR /app

# 复制 package.json 和 lock 文件，以便利用 Docker 的层缓存机制。
# 只有当这些文件变化时，才会重新执行 npm install。
COPY package*.json ./

# 安装项目依赖。
# 在 CI/CD 环境中，推荐使用 --frozen-lockfile 来确保安装与 lock 文件完全一致的依赖。
RUN npm install --frozen-lockfile

# 复制项目的所有源代码
COPY . .

# 执行 Vite 的构建命令，这会生成一个 /app/dist 目录
RUN npm run build


# --- 阶段 2: 生产阶段 (Production Stage) ---
# 使用一个非常轻量级的 Nginx 官方镜像作为最终的运行环境。
FROM nginx:stable-alpine

# 从“构建阶段”的镜像中，复制编译好的静态文件到 Nginx 的 Web 根目录。
COPY --from=build /app/dist /usr/share/nginx/html

# 复制我们自定义的 Nginx 配置文件，来覆盖默认配置并支持 SPA。
COPY nginx.conf /etc/nginx/conf.d/default.conf

# 暴露容器的 80 端口
EXPOSE 80

# Nginx 镜像的默认命令是启动 Nginx 服务。
# CMD ["nginx", "-g", "daemon off;"] 会在前台运行 Nginx，这是容器化应用的最佳实践。
# 这行通常是 Nginx 基础镜像自带的，所以可以省略，但写出来更清晰。
CMD ["nginx", "-g", "daemon off;"]