# base image
FROM docker.m.daocloud.io/python:3.12-slim-bookworm AS base

WORKDIR /app

# Install uv
RUN pip install uv -i https://mirrors.aliyun.com/pypi/simple/


FROM base AS packages

# if you located in China, you can use aliyun mirror to speed up
RUN sed -i 's@deb.debian.org@mirrors.aliyun.com@g' /etc/apt/sources.list.d/debian.sources

RUN apt-get update \
    && apt-get install -y --no-install-recommends gcc g++ libc-dev libffi-dev libgmp-dev libmpfr-dev libmpc-dev

# Install Python dependencies
COPY pyproject.toml uv.lock ./
RUN uv sync -i https://mirrors.aliyun.com/pypi/simple/

# production stage
FROM base AS production

# Set environment variables
ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1 \
    PYTHONPATH=/app \
    HF_ENDPOINT=https://hf-mirror.com

# Expose port
EXPOSE 8081

# set timezone
ENV TZ=UTC

# Set work directory
WORKDIR /app

# if you located in China, you can use aliyun mirror to speed up
RUN sed -i 's@deb.debian.org@mirrors.aliyun.com@g' /etc/apt/sources.list.d/debian.sources

# Install system dependencies
RUN \
    apt-get update \
    # Install dependencies
    && apt-get install -y --no-install-recommends \
        # basic environment
        curl libgmp-dev libmpfr-dev libmpc-dev \
        # For Security
        expat libldap-2.5-0 perl libsqlite3-0 zlib1g \
        # install a package to improve the accuracy of guessing mime type and file extension
        media-types \
        # install libmagic to support the use of python-magic guess MIMETYPE
        libmagic1 \
    && apt-get autoremove -y \
    && rm -rf /var/lib/apt/lists/*

# Copy Python environment and packages
ENV VIRTUAL_ENV=/app/.venv
COPY --from=packages ${VIRTUAL_ENV} ${VIRTUAL_ENV}
ENV PATH="${VIRTUAL_ENV}/bin:${PATH}"

# Copy project files
COPY . /app/

# Copy entrypoint
COPY docker/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/bin/bash", "/entrypoint.sh"]
