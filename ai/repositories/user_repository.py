from sqlmodel import Session, select, or_
from models import User
from utils.db_utils import engine
from .department_repository import DepartmentRepository
from utils import string_utils


class UserRepository:

    def get_user_by_id(self, user_id: int):
        with Session(engine) as session:
            statement = select(User).where(User.id == user_id)
            user = session.exec(statement).first()
        return user

    def get_user_by_name(self, user_name: str | None = None) -> User | None:
        if not user_name:
            return None
        with Session(engine) as session:
            statement = select(User).where(User.userName == user_name)
            user = session.exec(statement).first()
        return user

    def get_users(
        self,
        keyword: str,
    ):
        with Session(engine) as session:
            statement = select(User)
            statement = statement.where(User.checked == True).where(
                User.locked == False
            )
            if keyword:
                like_keyword = f"%{keyword}%"
                statement = statement.where(
                    or_(
                        User.UserName.like(like_keyword),  # type: ignore
                        User.DisplayName.like(like_keyword),  # type: ignore
                        User.Mobile.like(like_keyword),  # type: ignore
                        User.Email.like(like_keyword),  # type: ignore
                    )
                )
            users = session.exec(statement).all()
        return users

    def update_user(self, user: User):
        with Session(engine) as session:
            session.add(user)
            session.commit()
            session.refresh(user)
        return user

    def get_display_by_user_name(self, user_name: str) -> str:
        if not user_name:
            return ""
        user = self.get_user_by_name(user_name)
        return self.get_display_by_user(user)

    def get_display_by_user(self, user: User | None) -> str:
        if not user:
            return ""

        display_name = user.displayName if user.displayName else user.userName
        department_name = ""

        if user.departmentId:
            department_repository = DepartmentRepository()
            department_name = department_repository.get_full_name_by_id(
                user.departmentId
            )

        return (
            f"{display_name}（{department_name}）" if department_name else display_name
        )

    def get_user_names_by_department_ids(self, department_ids: list[int]) -> list[str]:
        if not department_ids:
            return []

        with Session(engine) as session:
            statement = (
                select(User.userName)
                .where(
                    User.departmentId.in_(department_ids),  # type: ignore
                    User.checked == True,
                    User.locked == False,
                )
                .order_by(User.userName)
            )
            user_names = list(session.exec(statement).all())
        return user_names

    def get_user_names_of_manager_by_department_ids(
        self, department_ids: list[int]
    ) -> list[str]:
        if not department_ids:
            return []

        with Session(engine) as session:
            statement = (
                select(User.userName)
                .where(
                    User.manager == True,
                    User.departmentId.in_(department_ids),  # type: ignore
                    User.checked == True,
                    User.locked == False,
                )
                .order_by(User.userName)
            )
            user_names = list(session.exec(statement).all())
        return user_names

    def get_manager_user_names_by_department_id(
        self, department_id: int, level: int
    ) -> list[str]:
        if department_id == 0 or level == 0:
            return []

        # 分管院领导
        if level == 1:
            department_repository = DepartmentRepository()
            department = department_repository.get(department_id)
            if department and department.managerUserNames:
                return string_utils.split(department.managerUserNames)
            return []

        with Session(engine) as session:
            statement = (
                select(User.userName)
                .where(
                    User.departmentId == department_id,
                    User.manager == True,
                    User.level == level,
                    User.checked == True,
                    User.locked == False,
                )
                .order_by(User.userName)
            )
            user_names = list(session.exec(statement).all())
        return user_names

    def get_manager_user_names_by_department_id_ascendant(
        self, department_id: int, level: int
    ) -> list[str]:
        user_names = self.get_manager_user_names_by_department_id(department_id, level)
        if user_names:
            return user_names

        department_repository = DepartmentRepository()
        parent_id = department_repository.get_parent_id(department_id)
        if parent_id > 0:
            user_names = self.get_manager_user_names_by_department_id_ascendant(
                parent_id, level
            )
        return user_names
