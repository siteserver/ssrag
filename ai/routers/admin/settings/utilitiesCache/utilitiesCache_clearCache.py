from managers import cache_manager
from dto import BoolResult


def utilitiesCache_clearCache():
    cache_manager.clear_all()

    return BoolResult(value=True)
