# SSRAG AI

### 一、安装Python与依赖

确保安装Python 3.7+，勾选“Add Python to environment variables”以自动配置环境变量。

通过虚拟环境管理依赖：  

```sh
pip install uv
uv venv
.venv\Scripts\activate.bat
uv sync -i https://pypi.tuna.tsinghua.edu.cn/simple
```  

虚拟环境可避免全局依赖冲突。

### 二、启动 API

```sh
.venv\Scripts\activate
uvicorn main:app --reload --port 6603
```

### 三、启动 Worker

```sh
.venv\Scripts\activate
celery -A celery_app worker --loglevel=info --pool=solo
```
