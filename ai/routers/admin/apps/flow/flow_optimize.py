from .__base import OptimizeRequest, OptimizeResult
from services import llm_manager


async def flow_optimize(request: OptimizeRequest) -> OptimizeResult:
    if not request.items or len(request.items) == 0:
        raise Exception("请输入文字！")

    items = []
    for item in request.items:
        if not item:
            continue
        items.append(item)

    items = llm_manager.optimize(items)

    return OptimizeResult(items=items)
