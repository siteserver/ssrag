import jwt
from configs import app_configs
from dto import Auth
from enums import AuthType
from fastapi import Header
from utils import string_utils


class AuthManager:
    def __init__(
        self,
        authentication: str,
    ):
        self.authentication = authentication
        self.auth = self.__get_auth()

    def __get_auth(self) -> Auth | None:
        try:
            if self.authentication is None:
                return None
            parts = self.authentication.split()
            token = ""
            if len(parts) == 2 and parts[0].lower() == "bearer":
                token = parts[1]
            if not token or token == "null" or not app_configs.SECURITY_KEY:
                return None

            payload = jwt.decode(token, app_configs.SECURITY_KEY, algorithms=["HS256"])
            role = []
            if payload.get("role") == AuthType.User.value:
                role = [AuthType.User]
            elif payload.get("role") == AuthType.Administrator.value:
                role = [AuthType.Administrator]
            elif isinstance(payload.get("role"), list):
                role = payload.get("role")
            admin_name = (
                str(payload.get("admin_name"))
                if payload.get("admin_name") is not None
                else ""
            )
            user_name = (
                str(payload.get("user_name"))
                if payload.get("user_name") is not None
                else ""
            )
            is_persistent = (
                string_utils.to_bool(payload.get("is_persistent"))
                if payload.get("is_persistent") is not None
                else False
            )
            return Auth(
                role=role,
                adminName=admin_name,
                userName=user_name,
                isPersistent=is_persistent,
                token=token,
            )
        except:
            return None

    @staticmethod
    def get_auth_manager(authorization: str | None = Header(None)):  # type: ignore
        auth_manager = AuthManager(authorization if authorization else "")
        return auth_manager

    def get_user_name(self) -> str | None:
        if self.auth is None:
            return None
        if self.auth.role and AuthType.User.value in self.auth.role:
            return self.auth.userName
        return None

    def get_admin_name(self) -> str | None:
        if self.auth is None:
            return None
        if self.auth.role and AuthType.Administrator.value in self.auth.role:
            return self.auth.adminName
        return None
