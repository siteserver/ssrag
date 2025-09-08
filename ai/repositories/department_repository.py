from sqlmodel import Session, select
from sqlalchemy import func
from models import Department, User
from utils.db_utils import engine
from enums import ScopeType


class DepartmentRepository:

    def get(self, department_id: int):
        if department_id == 0:
            return None
        with Session(engine) as session:
            statement = select(Department).where(Department.id == department_id)
            department = session.exec(statement).first()
        return department

    def get_by_code(self, code: str):
        if not code:
            return None
        with Session(engine) as session:
            statement = select(Department).where(Department.code == code)
            department = session.exec(statement).first()
        return department

    def get_parent_id(self, department_id: int) -> int:
        department = self.get(department_id)
        return department.parentId if department else 0

    def get_departments(self) -> list[Department]:
        with Session(engine) as session:
            statement = select(Department)
            departments = session.exec(statement).all()
        return list(departments)

    def get_departments_by_keyword(self, keyword: str) -> list[Department]:
        with Session(engine) as session:
            statement = select(Department).where(
                func.lower(Department.name).like(f"%{keyword.lower()}%")
            )
            departments = session.exec(statement).all()
        return list(departments)

    def get_department_with_full_name(self, department: Department) -> dict:
        fullName = self.get_full_name_by_id(department.id)
        return {
            "id": department.id,
            "code": department.code,
            "name": department.name,
            "parentId": department.parentId,
            "parentsCount": department.parentsCount,
            "childrenCount": department.childrenCount,
            "count": department.count,
            "wxCount": department.wxCount,
            "managerUserNames": department.managerUserNames,
            "taxis": department.taxis,
            "description": department.description,
            "fullName": fullName,
        }

    def get_parent_ids_recursive(self, departments, parent_ids, department_id):
        """
        Recursively collects parent department IDs for a given department.

        Args:
            departments: List of Department objects
            parent_ids: List to collect parent IDs
            department_id: ID of the department to find parents for
        """
        department = next(
            (dept for dept in departments if dept.id == department_id), None
        )
        if department is not None and department.parentId > 0:
            parent_ids.append(department.parentId)
            self.get_parent_ids_recursive(departments, parent_ids, department.parentId)

    def get_full_name_by_id(self, department_id: int | None) -> str:
        """
        Gets the full hierarchical name of a department without a current department context.

        Args:
            department_id: ID of the department to get the full name for

        Returns:
            String representation of the department hierarchy
        """
        if department_id is None or department_id <= 0:
            return ""

        return self.get_full_name(0, department_id)

    def get_full_name(self, current_department_id: int, department_id: int) -> str:
        """
        Gets the full hierarchical name of a department.

        Args:
            current_department_id: ID of the current department
            department_id: ID of the department to get the full name for

        Returns:
            String representation of the department hierarchy
        """
        if department_id <= 0:
            return ""

        department_names = []

        # Check if we're looking at the current department
        if department_id == current_department_id:
            department = self.get(current_department_id)
            return department.name if department else ""

        # Get all departments and build parent hierarchy
        departments = self.get_departments()
        parent_ids = [department_id]
        self.get_parent_ids_recursive(departments, parent_ids, department_id)
        parent_ids.reverse()

        # Build the department names list
        for parent_id in parent_ids:
            if parent_id == current_department_id:
                department_names.clear()

            department = next(
                (dept for dept in departments if dept.id == parent_id), None
            )
            if department and department.name:
                department_names.append(department.name)

        # Join department names with separator
        return " > ".join(department_names)

    def get_user_with_department(self, user: User):
        departmentName = self.get_full_name_by_id(user.departmentId)
        return {
            "id": user.id,
            "userName": user.userName,
            "departmentId": user.departmentId,
            "manager": user.manager,
            "checked": user.checked,
            "locked": user.locked,
            "displayName": user.displayName,
            "mobile": user.mobile,
            "email": user.email,
            "avatarUrl": user.avatarUrl,
            "departmentName": departmentName,
        }

    def get_child_ids(self, departments: list[Department], parent_id: int) -> list[int]:
        """Get child department IDs for a given parent ID.

        Args:
            departments: List of all departments
            parent_id: ID of the parent department

        Returns:
            List of child department IDs
        """
        return [dept.id for dept in departments if dept.parentId == parent_id]

    def get_child_ids_recursive(
        self, departments: list[Department], department_ids: list[int], parent_id: int
    ) -> None:
        """Recursively get all child department IDs for a given parent ID.

        Args:
            departments: List of all departments
            department_ids: List to store the found department IDs
            parent_id: ID of the parent department
        """
        child_ids = self.get_child_ids(departments, parent_id)
        if child_ids:
            department_ids.extend(child_ids)
            for child_id in child_ids:
                self.get_child_ids_recursive(departments, department_ids, child_id)

    def get_all_department_ids(self, department_ids: list[int]) -> list[int]:
        """Get all unique department IDs including their descendants.

        Args:
            department_ids: List of parent department IDs

        Returns:
            List of all unique department IDs including descendants
        """
        all_department_ids = []
        if department_ids:
            for parent_id in department_ids:
                department_ids = self.get_department_ids(parent_id, ScopeType.ALL)
                for department_id in department_ids:
                    if department_id not in all_department_ids:
                        all_department_ids.append(department_id)
        return all_department_ids

    def get_department_ids(
        self, department_id: int, scope_type: ScopeType
    ) -> list[int]:
        """Get department IDs based on scope type and optional query.

        Args:
            department_id: The department ID to start from
            scope_type: The scope type to determine which departments to include
            query: Optional query to filter results

        Returns:
            List of department IDs
        """
        department_ids = []

        if department_id == 0:
            return department_ids

        if scope_type == ScopeType.SELF:
            department_ids = [department_id]
        elif scope_type == ScopeType.SELF_AND_CHILDREN:
            departments = self.get_departments()
            department_ids = self.get_child_ids(departments, department_id)
            department_ids.append(department_id)
        elif scope_type == ScopeType.CHILDREN:
            departments = self.get_departments()
            department_ids = self.get_child_ids(departments, department_id)
        elif scope_type == ScopeType.DESCENDANT:
            departments = self.get_departments()
            self.get_child_ids_recursive(departments, department_ids, department_id)
        elif scope_type == ScopeType.ALL:
            departments = self.get_departments()
            department_ids = [department_id]
            self.get_child_ids_recursive(departments, department_ids, department_id)

        return department_ids

    def is_in_tree(self, department_ids: list[int], department_id: int) -> bool:
        """Check if a department is in the tree of given department IDs.

        Args:
            department_ids: List of department IDs to check against
            department_id: Department ID to check

        Returns:
            bool: True if department is in the tree, False otherwise
        """
        if department_id in department_ids:
            return True

        for id in department_ids:
            descendant_ids = self.get_department_ids(id, ScopeType.ALL)
            if department_id in descendant_ids:
                return True

        return False
