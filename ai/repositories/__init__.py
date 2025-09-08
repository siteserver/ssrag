from .celery_task_repository import CeleryTaskRepository
from .channel_repository import ChannelRepository
from .config_repository import ConfigRepository
from .chat_group_repository import ChatGroupRepository
from .chat_message_repository import ChatMessageRepository
from .content_repository import ContentRepository
from .dataset_repository import DatasetRepository
from .department_repository import DepartmentRepository
from .document_repository import DocumentRepository
from .error_log_repository import ErrorLogRepository
from .flow_edge_repository import FlowEdgeRepository
from .flow_node_repository import FlowNodeRepository
from .flow_variable_repository import FlowVariableRepository
from .message_repository import MessageRepository
from .model_provider_repository import ModelProviderRepository
from .model_repository import ModelRepository
from .prompt_repository import PromptRepository
from .segment_repository import SegmentRepository
from .site_repository import SiteRepository
from .user_group_repository import UserGroupRepository
from .user_repository import UserRepository
from .users_in_groups_repository import UsersInGroupsRepository


celery_task_repository = CeleryTaskRepository()
channel_repository = ChannelRepository()
config_repository = ConfigRepository()
chat_group_repository = ChatGroupRepository()
chat_message_repository = ChatMessageRepository()
content_repository = ContentRepository()
dataset_repository = DatasetRepository()
department_repository = DepartmentRepository()
document_repository = DocumentRepository()
error_log_repository = ErrorLogRepository()
flow_edge_repository = FlowEdgeRepository()
flow_node_repository = FlowNodeRepository()
flow_variable_repository = FlowVariableRepository()
message_repository = MessageRepository()
model_provider_repository = ModelProviderRepository()
model_repository = ModelRepository()
prompt_repository = PromptRepository()
segment_repository = SegmentRepository()
site_repository = SiteRepository()
user_group_repository = UserGroupRepository()
user_repository = UserRepository()
users_in_groups_repository = UsersInGroupsRepository()

__all__ = [
    "celery_task_repository",
    "channel_repository",
    "config_repository",
    "chat_group_repository",
    "chat_message_repository",
    "content_repository",
    "dataset_repository",
    "department_repository",
    "document_repository",
    "error_log_repository",
    "flow_edge_repository",
    "flow_node_repository",
    "flow_variable_repository",
    "message_repository",
    "model_provider_repository",
    "model_repository",
    "prompt_repository",
    "segment_repository",
    "site_repository",
    "user_group_repository",
    "user_repository",
    "users_in_groups_repository",
]
