from .__base import SubmitModelRequest
from repositories import model_repository
from utils import string_utils
from models import Model
from dto import BoolResult
import uuid


async def configsModels_submit_model(request: SubmitModelRequest) -> BoolResult:
    if request.id > 0:
        model = model_repository.get_by_id(request.id)
        if not model:
            return BoolResult(value=False)

        model.modelId = request.modelId
        model.modelType = request.modelType
        model.skill_list = request.skills
        model.extendValues = string_utils.to_json_str(request.extendValues)
        model_repository.update(model)
    else:
        model = Model(
            uuid=str(uuid.uuid4()),
            providerId=request.providerId,
            modelId=request.modelId,
            modelType=request.modelType,
            extendValues=string_utils.to_json_str(request.extendValues),
        )
        model.skill_list = request.skills

        model_repository.create(model)

    return BoolResult(value=True)
