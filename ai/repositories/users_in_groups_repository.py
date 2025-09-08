from sqlmodel import Session, select, asc
from models import UsersInGroups, User, UserGroup
from utils.db_utils import engine
from .user_group_repository import UserGroupRepository


class UsersInGroupsRepository:

    def get_user_ids(self, group_id: int) -> list[int]:
        """
        Get all user IDs in a specific group.

        Args:
            group_id (int): The ID of the group to get users from

        Returns:
            list[int]: List of user IDs in the group
        """
        with Session(engine) as session:
            statement = (
                select(UsersInGroups.userId)
                .where(UsersInGroups.groupId == group_id)
                .distinct()
                .order_by(asc(UsersInGroups.userId))
            )
            result = session.exec(statement)
            return list(result)

    def get_group_ids(self, user_id: int) -> list[int]:
        """
        Get all group IDs for a specific user.

        Args:
            user_id (int): The ID of the user to get groups for

        Returns:
            list[int]: List of group IDs the user belongs to
        """
        with Session(engine) as session:
            statement = (
                select(UsersInGroups.groupId)
                .where(UsersInGroups.userId == user_id)
                .distinct()
                .order_by(asc(UsersInGroups.groupId))
            )
            result = session.exec(statement)
            return list(result)

    def exists(self, group_ids: list[int], user_id: int) -> bool:
        """
        Check if a user exists in any of the specified groups.

        Args:
            group_ids (list[int]): List of group IDs to check
            user_id (int): The ID of the user to check

        Returns:
            bool: True if user exists in any of the groups, False otherwise
        """
        with Session(engine) as session:
            statement = (
                select(UsersInGroups)
                .where(UsersInGroups.userId == user_id)
                .where(UsersInGroups.groupId.in_(group_ids))  # type: ignore
            )
            result = session.exec(statement)
            return result.first() is not None

    def get_groups(self, user: User | None) -> list[UserGroup]:
        """
        Get all groups for a user.
        """
        groups: list[UserGroup] = []

        if user is None:
            return groups

        group_ids = self.get_group_ids(user.id)
        all_groups = UserGroupRepository().get_all()

        if len(group_ids) > 0:
            groups = [group for group in all_groups if group.id in group_ids]

        if user.manager:
            manager_group = next(
                (group for group in all_groups if group.isManager), None
            )
            if manager_group:
                groups.append(manager_group)

        if len(groups) == 0:
            default_group = next(
                (group for group in all_groups if group.isDefault), None
            )
            if default_group:
                groups.append(default_group)

        return groups
