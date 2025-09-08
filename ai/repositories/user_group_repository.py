from sqlmodel import Session, select, asc
from models import UserGroup
from utils.db_utils import engine
import uuid


class UserGroupRepository:

    def get_all(self) -> list[UserGroup]:
        """
        Get all user groups, ordered by taxis and id.
        If no default or manager group exists, create them.
        """
        with Session(engine) as session:
            statement = select(UserGroup).order_by(
                asc(UserGroup.taxis), asc(UserGroup.id)
            )
            groups = session.exec(statement).all()

            is_changed = False
            if not any(group.isDefault for group in groups):
                default_group = UserGroup(
                    uuid=str(uuid.uuid4()),
                    groupName="默认",
                    description="所有未设置用户组的用户，将自动属于默认用户组",
                    isDefault=True,
                    isManager=False,
                    taxis=0,
                    homePermissions="",
                )
                session.add(default_group)
                is_changed = True

            if not any(group.isManager for group in groups):
                manager_group = UserGroup(
                    uuid=str(uuid.uuid4()),
                    groupName="负责人",
                    description="用户被设置为部门负责人后，将自动属于负责人用户组",
                    isManager=True,
                    isDefault=False,
                    taxis=0,
                    homePermissions="",
                )
                session.add(manager_group)
                is_changed = True

            if is_changed:
                session.commit()
                statement = select(UserGroup).order_by(
                    asc(UserGroup.taxis), asc(UserGroup.id)
                )
                groups = session.exec(statement).all()

            return list(groups)
