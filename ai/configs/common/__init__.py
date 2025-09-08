from .celery_config import CeleryConfig
from .db_config import DbConfig
from .redis_config import RedisConfig
from .golbal_config import GlobalConfig


class CommonConfig(
    # place the configs in alphabet order
    CeleryConfig,
    DbConfig,
    RedisConfig,
    GlobalConfig,
):
    pass
