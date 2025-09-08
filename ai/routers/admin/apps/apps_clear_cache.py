from repositories import site_repository
from dto import BoolResult


async def apps_clear_cache() -> BoolResult:
    site_repository.clear_cache()

    return BoolResult(value=True)
