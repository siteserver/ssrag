from sqlmodel import Session, select
from utils.db_utils import engine
from models import Config, ConfigValues


class ConfigRepository:

    def get_values(self) -> ConfigValues:
        config_values = None
        with Session(engine) as session:
            statement = select(Config)
            config = session.exec(statement).first()
        if config:
            config_values = config.config_values

        if not config_values:
            config_values = ConfigValues(
                init=False,
                defaultLLMProviderId=None,
                defaultLLMModelId=None,
                defaultTextEmbeddingProviderId=None,
                defaultTextEmbeddingModelId=None,
            )

        return config_values

    def update_values(self, values: ConfigValues):
        with Session(engine) as session:
            statement = select(Config)
            config = session.exec(statement).first()
            if config:
                config.config_values = values
                # config.configs = values.model_dump_json()
                session.add(config)
                session.commit()
