from sqlalchemy import create_engine
from configs import app_configs

database_url = f"postgresql://{app_configs.DB_USER}:{app_configs.DB_PASSWORD}@{app_configs.DB_HOST}:{app_configs.DB_PORT}/{app_configs.DB_DATABASE}"
if app_configs.DB_SCHEMA:
    engine = create_engine(
        str(database_url),
        connect_args={"options": f"-csearch_path={app_configs.DB_SCHEMA}"},
        pool_recycle=3600,  # 每小时主动回收连接（需小于wait_timeout）
        pool_pre_ping=True,  # 每次取连接前验证有效性
    )
else:
    engine = create_engine(
        str(database_url),
        pool_recycle=3600,  # 每小时主动回收连接（需小于wait_timeout）
        pool_pre_ping=True,  # 每次取连接前验证有效性
    )
