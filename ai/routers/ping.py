from fastapi import APIRouter
from configs import router_prefix

router = APIRouter(prefix=router_prefix.PING, tags=["ping"])


@router.get("")
def read_root():
    return "pong"
