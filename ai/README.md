# uv

pip install uv
uv venv
.venv\Scripts\activate.bat
uv sync -i https://pypi.tuna.tsinghua.edu.cn/simple

# start

.venv\Scripts\activate
uvicorn main:app --reload --port 6603
fastapi dev main.py
celery -A celery_app worker --loglevel=info --pool=solo

# others

(Get-NetTCPConnection -LocalPort 8000).OwningProcess
Stop-Process -Id 22428 -Force

[mypy-celery.*]
ignore_missing_imports = True


在Windows系统中使用WinSW运行Python FastAPI应用，需通过服务化部署实现后台运行和开机自启。以下是具体步骤及技术要点，结合多个搜索结果整理而成：

# 开启知识库

celery -A celery_app worker --loglevel=info --pool=solo

---

### 一、环境准备
1. **安装Python与依赖**  
   - 确保安装Python 3.7+，勾选“Add Python to environment variables”以自动配置环境变量。  
   - 通过虚拟环境管理依赖：  
     ```cmd
     python -m venv .venv
     .venv\Scripts\activate.bat
     pip install fastapi uvicorn
     ```  
     虚拟环境可避免全局依赖冲突。

2. **FastAPI入口文件配置**  
   创建`main.py`文件，示例代码：  
   ```python
   from fastapi import FastAPI
   app = FastAPI()
   
   @app.get("")
   def read_root():
       return {"Hello": "World"}
   ```  
   测试运行：`uvicorn main:app --reload`。

---

### 二、WinSW服务化部署
1. **下载与配置WinSW**  
   - 从[GitHub Release](https://github.com/winsw/winsw/releases)下载`WinSW-x64.exe`，重命名为`fastapi-service.exe`，与`main.py`放在同一目录（如`C:\fastapi`）。  
   - 创建`fastapi-service.xml`配置文件：  
     ```xml
     <service>
       <id>FastAPI</id>
       <name>FastAPI Service</name>
       <description>FastAPI Web服务</description>
       <executable>%BASE%\venv\Scripts\python.exe</executable>
       <arguments>-m uvicorn main:app --host 0.0.0.0</arguments>
       <log mode="none" /> <!-- 可选禁用日志 -->
     </service>
     ```  
     关键参数说明：  
     - `executable`：指向虚拟环境中的Python解释器路径。  
     - `arguments`：启动命令，`--host 0.0.0.0`允许任意IP访问。

2. **安装与启动服务**  
   - 以管理员权限打开CMD，执行：  
     ```cmd
     fastapi-service.exe install
     fastapi-service.exe start
     ```  
   - 验证服务状态：通过`services.msc`查看服务“FastAPI Service”是否运行。

---


## 打包：

uv pip compile pyproject.toml -o requirements.txt