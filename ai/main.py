from fastapi import FastAPI, HTTPException, Request, status
from fastapi.responses import JSONResponse
from fastapi.exceptions import RequestValidationError
from dto import AppException
import traceback
from fastapi.staticfiles import StaticFiles
from repositories import error_log_repository
from configs import router_prefix
from routers.ping import router as ping_router
from routers.upgrade import router as upgrade_router
from routers.open.home import home as open_home
from routers.open.chat import chat as open_chat
from routers.open.copilot import copilot as open_copilot
from routers.admin.apps import apps as admin_apps
from routers.admin.apps.flow import flow as admin_apps_flow
from routers.admin.apps.settings import settings as admin_apps_settings
from routers.admin.apps.logs import logs as admin_apps_logs
from routers.admin.apps.publish import publish as admin_apps_publish
from routers.admin.apps.modals.dataset_select import (
    dataset_select as admin_apps_modals_dataset_select,
)
from routers.admin.apps.modals.dataset_config import (
    dataset_config as admin_apps_modals_dataset_config,
)
from routers.admin.dataset.documents import documents as admin_dataset_documents
from routers.admin.dataset.segments import segments as admin_dataset_segments
from routers.admin.dataset.testing import testing as admin_dataset_testing
from routers.admin.dataset.settings import settings as admin_dataset_settings
from routers.admin.dataset.status import status as admin_dataset_status
from routers.admin.dataset.tasks import tasks as admin_dataset_tasks
from routers.admin.settings.configsModels import (
    configsModels as admin_settings_configsModels,
)
from routers.admin.settings.utilitiesCache import (
    utilitiesCache as admin_settings_utilitiesCache,
)

app = FastAPI()


@app.exception_handler(RequestValidationError)
async def validation_exception_handler(request: Request, exc: RequestValidationError):
    return JSONResponse(
        status_code=status.HTTP_422_UNPROCESSABLE_ENTITY,
        content={
            "message": "Validation Error",
            "success": False,
            "errors": exc.errors(),
        },
    )


@app.exception_handler(404)
async def not_found_exception_handler(request: Request, exc: HTTPException):
    return JSONResponse(
        status_code=status.HTTP_404_NOT_FOUND,
        content={
            "message": "未找到请求的资源，请检查URL是否正确。",
            "success": False,
        },
    )


# 自定义异常处理器
@app.exception_handler(AppException)
async def custom_exception_handler(request: Request, exc: AppException):
    return JSONResponse(
        status_code=400,
        content={
            "message": exc.detail,
            "success": False,
        },
    )


# 全局捕获 HTTPException
@app.exception_handler(HTTPException)
async def http_exception_handler(request: Request, exc: HTTPException):
    return JSONResponse(
        status_code=exc.status_code,
        content={
            "message": exc.detail,
            "detail": traceback.format_exc(),
            "success": False,
        },
    )


# 全局捕获所有未处理的异常
@app.exception_handler(Exception)
async def global_exception_handler(request: Request, exc: Exception):
    message: str = str(exc)
    summary: str = request.method + " " + request.url.path
    stackTrace: str = traceback.format_exc()
    try:
        error_log_repository.add(message, summary, stackTrace)
    except:
        pass
    return JSONResponse(
        status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
        content={
            "message": message,
            "detail": stackTrace,
            "success": False,
        },
    )


app.include_router(ping_router)
app.include_router(upgrade_router)

app.include_router(open_home.router)
app.include_router(open_chat.router)
app.include_router(open_copilot.router)

app.include_router(admin_apps.router)
app.include_router(admin_apps_flow.router)
app.include_router(admin_apps_settings.router)
app.include_router(admin_apps_logs.router)
app.include_router(admin_apps_publish.router)
app.include_router(admin_apps_modals_dataset_select.router)
app.include_router(admin_apps_modals_dataset_config.router)
app.include_router(admin_dataset_documents.router)
app.include_router(admin_dataset_segments.router)
app.include_router(admin_dataset_testing.router)
app.include_router(admin_dataset_settings.router)
app.include_router(admin_dataset_status.router)
app.include_router(admin_dataset_tasks.router)
app.include_router(admin_settings_configsModels.router)
app.include_router(admin_settings_utilitiesCache.router)

app.mount(
    router_prefix.ROOT,
    StaticFiles(directory="wwwroot"),
    name="wwwroot",
)
