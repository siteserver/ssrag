from models import ChannelSummary
from repositories import channel_repository

channel_summaries: list[ChannelSummary] = channel_repository.get_summaries(651)

print(channel_summaries)
