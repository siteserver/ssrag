from .__base import GetManifestsResult
from utils import yaml_utils
from enums import ProviderType
import glob


async def configsModels_get_manifests() -> GetManifestsResult:
    manifests = []
    provider_types = ProviderType
    for provider_type in provider_types:
        manifest = yaml_utils.yaml_to_json(
            f"./providers/{provider_type.value}/manifest.yaml"
        )
        model = yaml_utils.yaml_to_json(
            f"./providers/{provider_type.value}/{manifest["plugins"]["models"][0]}"
        )

        models = []
        if "models" in model:
            for model_type in model["models"]:
                predefined = model["models"][model_type]["predefined"]
                if not predefined:
                    continue
                for path in predefined:
                    model_path_patten = (
                        f"./providers/{provider_type.value}/{path}"  # models/llm/*.yaml
                    )
                    files_path = glob.glob(model_path_patten)
                    for file_path in files_path:
                        if "_position.yaml" in file_path:
                            continue
                        model_file_json = yaml_utils.yaml_to_json(file_path)
                        models.append(model_file_json)

        model["models"] = models
        manifest["model"] = model
        manifests.append(manifest)

    return GetManifestsResult(manifests=manifests)
