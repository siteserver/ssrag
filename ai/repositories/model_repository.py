from sqlmodel import Session, select, delete, asc
from models import Model
from utils.db_utils import engine
from enums import ModelType


class ModelRepository:
    def get(
        self, provider_id: str | None = None, model_id: str | None = None
    ) -> Model | None:
        model = None
        statement = select(Model).where(
            Model.providerId == provider_id, Model.modelId == model_id
        )
        with Session(engine) as session:
            model = session.exec(statement).first()
        return model

    def get_all(self, model_type: ModelType) -> list[Model]:
        statement = (
            select(Model)
            .where(Model.modelType == model_type.value)
            .order_by(asc(Model.id))
        )

        with Session(engine) as session:
            models = session.exec(statement).all()
        return list(models)

    def get_by_model_id(self, model_id: str) -> Model | None:
        statement = select(Model).where(Model.modelId == model_id)
        with Session(engine) as session:
            model = session.exec(statement).first()
        return model

    def get_by_id(self, id: int) -> Model | None:
        statement = select(Model).where(Model.id == id)
        with Session(engine) as session:
            model = session.exec(statement).first()
        return model

    def get_all_by_provider_id(self, provider_id: str) -> list[Model]:
        statement = select(Model).where(Model.providerId == provider_id)
        with Session(engine) as session:
            models = session.exec(statement).all()
        return list(models)

    def update(self, model: Model):
        with Session(engine) as session:
            session.add(model)
            session.commit()

    def create(self, model: Model):
        with Session(engine) as session:
            session.add(model)
            session.commit()

    def delete(self, provider_id: str, model_id: str):
        statement = delete(Model).where(
            Model.providerId == provider_id, Model.modelId == model_id  # type: ignore
        )
        with Session(engine) as session:
            session.exec(statement)  # type: ignore
            session.commit()
