from celery import Celery
import os, sys
from configs import app_configs

# from configs.configs import celery_broker_url, celery_backend_url

from dto import TaskDocumentProcess, TaskContentProcess
from tasks import document_process, embedding_changed, content_process

sys.path.append(os.path.dirname(os.path.abspath(__file__)))

redis_protocol = "rediss" if app_configs.REDIS_SSL else "redis"
celery_broker_url = f"{redis_protocol}://:{app_configs.REDIS_PASSWORD}@{app_configs.REDIS_HOST}:{app_configs.REDIS_PORT}/{app_configs.CELERY_BROKER_DB}"
celery_backend_url = f"{redis_protocol}://:{app_configs.REDIS_PASSWORD}@{app_configs.REDIS_HOST}:{app_configs.REDIS_PORT}/{app_configs.CELERY_BACKEND_DB}"

celery_app = Celery(
    "worker",
    broker=celery_broker_url,  # 使用 Redis 作为消息队列
    backend=celery_backend_url,  # 使用 Redis 存储任务结果
    # broker_use_ssl={"ssl_cert_reqs": ssl.CERT_NONE},  # 禁用证书验证
    # backend_use_ssl={"ssl_cert_reqs": ssl.CERT_NONE},
)

celery_app.conf.update(
    # 任务序列化方式
    task_serializer="json",
    # 结果序列化方式
    accept_content=["json"],
    # 结果序列化方式
    result_serializer="json",
    # 时区
    timezone="Asia/Shanghai",
    # 是否启用 UTC
    enable_utc=True,
)

if app_configs.REDIS_PREFIX:
    celery_app.conf.broker_transport_options = {
        "global_keyprefix": f"{app_configs.REDIS_PREFIX}:"
    }


@celery_app.task(bind=True)
def task_document_process(
    self,
    uuid: str,
    siteId: int,
    channelId: int,
    contentId: int,
    dirPath: str,
    fileName: str,
    extName: str,
):
    task = TaskDocumentProcess(
        uuid=uuid,
        siteId=siteId,
        channelId=channelId,
        contentId=contentId,
        dirPath=dirPath,
        fileName=fileName,
        extName=extName,
    )
    return document_process(self, task)


@celery_app.task(bind=True)
def task_content_process(
    self,
    siteId: int,
    channelId: int,
    contentId: int,
):
    task = TaskContentProcess(
        siteId=siteId,
        channelId=channelId,
        contentId=contentId,
    )
    return content_process(self, task)


@celery_app.task(bind=True)
def task_embedding_changed(
    self,
):
    return embedding_changed(self)
