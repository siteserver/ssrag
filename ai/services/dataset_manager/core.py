class DatasetManager:
    def __init__(self):
        pass

    def index_all(self, siteId: int) -> None:
        from .dataset_manager_index_all import dataset_manager_index_all

        return dataset_manager_index_all(siteId)

    def index_channel(self, siteId: int, channelId: int) -> None:
        from .dataset_manager_index_channel import dataset_manager_index_channel

        return dataset_manager_index_channel(siteId, channelId)

    def index_content(self, siteId: int, channelId: int, contentId: int) -> None:
        from .dataset_manager_index_content import dataset_manager_index_content

        return dataset_manager_index_content(siteId, channelId, contentId)

    def remove_all(self, siteId: int) -> None:
        from .dataset_manager_remove_all import dataset_manager_remove_all

        return dataset_manager_remove_all(siteId)

    def remove_channel(self, siteId: int, channelId: int) -> None:
        from .dataset_manager_remove_channel import dataset_manager_remove_channel

        return dataset_manager_remove_channel(siteId, channelId)

    def remove_content(self, siteId: int, channelId: int, contentId: int) -> None:
        from .dataset_manager_remove_content import dataset_manager_remove_content

        return dataset_manager_remove_content(siteId, channelId, contentId)
