class SearchScope:
    siteIds: list[int]
    channelIds: list[int]
    contentIds: list[int]

    def __init__(
        self,
        siteIds: list[int],
        channelIds: list[int],
        contentIds: list[int],
    ):
        self.siteIds = siteIds
        self.channelIds = channelIds
        self.contentIds = contentIds
