## Docker 部署说明文档

欢迎使用 `docker` 目录，通过 Docker Compose 部署 SSRAG。本说明文档将介绍更新内容、部署步骤以及为现有用户提供的迁移细节。

### 如何使用 `docker-compose.yaml` 部署 SSRAG

1. **前置条件**：请确保您的系统已安装 Docker 和 Docker Compose。
2. **环境配置**：
    - 进入 `docker` 目录。
    - 通过命令 `cp .env.example .env` 将 `.env.example` 文件复制为 `.env` 文件。
    - 根据需要自定义 `.env` 文件。详细配置选项请参考 `.env.example` 文件。
3. **启动服务**：
    - 在 `docker` 目录下执行 `docker compose up` 启动所有服务。

### `.env` 文件概览

Docker 部署中提供的 `.env.example` 文件内容丰富，涵盖了广泛的配置选项。该文件分为多个部分，每一部分对应应用及其服务的不同配置。以下是部分关键配置及变量说明：

1. **数据库配置**：
    - `DB_USERNAME`、`DB_PASSWORD`、`DB_HOST`、`DB_PORT`、`DB_DATABASE`：PostgreSQL 数据库的凭证及连接信息。

2. **Redis 配置**：
    - `REDIS_HOST`、`REDIS_PORT`、`REDIS_PASSWORD`：Redis 服务器连接设置。

5. **Celery 配置**：
    - `CELERY_BROKER_URL`：Celery 消息中间件的配置。

6. **存储配置**：
    - `STORAGE_TYPE`、`S3_BUCKET_NAME`、`AZURE_BLOB_ACCOUNT_NAME`：本地、S3、Azure Blob 等文件存储选项的相关设置。

7. **向量数据库配置**：
    - `VECTOR_STORE`：向量数据库类型（如 `weaviate`、`milvus`）。
    - 各向量数据库的专用设置，如 `WEAVIATE_ENDPOINT`、`MILVUS_URI`。

8. **CORS 配置**：
    - `WEB_API_CORS_ALLOW_ORIGINS`、`CONSOLE_CORS_ALLOW_ORIGINS`：跨域资源共享相关设置。

9. **OpenTelemetry 配置**：
    - `ENABLE_OTEL`：是否在 API 中启用 OpenTelemetry 采集器。
    - `OTLP_BASE_ENDPOINT`：OTLP 导出器的终端地址。
  
10. **其他服务专用环境变量**：
    - 各服务（如 `nginx`、`redis`、`db` 及向量数据库）均有专用环境变量，这些变量会在 `docker-compose.yaml` 文件中直接引用。

### 其他信息

- **持续改进阶段**：我们正在积极收集社区反馈，以不断优化和完善部署流程。随着更多用户采用此新方法，我们将根据您的体验和建议持续改进。
- **支持**：有关详细的配置选项和环境变量设置，请参考 `.env.example` 文件及 `docker` 目录下的 Docker Compose 配置文件。

本说明文档旨在帮助您通过全新的 Docker Compose 方式完成部署。如有任何问题或需要进一步协助，请查阅官方文档或联系支持团队。
