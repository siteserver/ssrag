from fastapi import HTTPException


def combile(url: str, route: str):
    route = route.lstrip("/")
    if url.endswith("/"):
        return url + route
    else:
        return url + "/" + route


def error(message: str):
    return HTTPException(status_code=400, detail=message)
