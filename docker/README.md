## README for docker Deployment

Welcome to the new `docker` directory for deploying SSRAG using Docker Compose. This README outlines the updates, deployment instructions, and migration details for existing users.

### How to Deploy SSRAG with `docker-compose.yaml`

1. **Prerequisites**: Ensure Docker and Docker Compose are installed on your system.
2. **Environment Setup**:
    - Navigate to the `docker` directory.
    - Copy the `.env.example` file to a new file named `.env` by running `cp .env.example .env`.
    - Customize the `.env` file as needed. Refer to the `.env.example` file for detailed configuration options.
3. **Running the Services**:
    - Execute `docker compose up` from the `docker` directory to start the services.

### Overview of `.env`

The `.env.example` file provided in the Docker setup is extensive and covers a wide range of configuration options. It is structured into several sections, each pertaining to different aspects of the application and its services. Here are some of the key sections and variables:

1. **Database Configuration**:
    - `DB_USERNAME`, `DB_PASSWORD`, `DB_HOST`, `DB_PORT`, `DB_DATABASE`: PostgreSQL database credentials and connection details.

2. **Redis Configuration**:
    - `REDIS_HOST`, `REDIS_PORT`, `REDIS_PASSWORD`: Redis server connection settings.

5. **Celery Configuration**:
    - `CELERY_BROKER_URL`: Configuration for Celery message broker.

6. **Storage Configuration**:
    - `STORAGE_TYPE`, `S3_BUCKET_NAME`, `AZURE_BLOB_ACCOUNT_NAME`: Settings for file storage options like local, S3, Azure Blob, etc.

7. **Vector Database Configuration**:
    - `VECTOR_STORE`: Type of vector database (e.g., `weaviate`, `milvus`).
    - Specific settings for each vector store like `WEAVIATE_ENDPOINT`, `MILVUS_URI`.

8. **CORS Configuration**:
    - `WEB_API_CORS_ALLOW_ORIGINS`, `CONSOLE_CORS_ALLOW_ORIGINS`: Settings for cross-origin resource sharing.

9. **OpenTelemetry Configuration**:
    - `ENABLE_OTEL`: Enable OpenTelemetry collector in api.
    - `OTLP_BASE_ENDPOINT`: Endpoint for your OTLP exporter.
  
10. **Other Service-Specific Environment Variables**:
    - Each service like `nginx`, `redis`, `db`, and vector databases have specific environment variables that are directly referenced in the `docker-compose.yaml`.

### Additional Information

- **Continuous Improvement Phase**: We are actively seeking feedback from the community to refine and enhance the deployment process. As more users adopt this new method, we will continue to make improvements based on your experiences and suggestions.
- **Support**: For detailed configuration options and environment variable settings, refer to the `.env.example` file and the Docker Compose configuration files in the `docker` directory.

This README aims to guide you through the deployment process using the new Docker Compose setup. For any issues or further assistance, please refer to the official documentation or contact support.
