from fastapi import HTTPException


class AppException(HTTPException):
    def __init__(self, message: str):
        super().__init__(status_code=400, detail=message)
