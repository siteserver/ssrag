# from models import Segment


# def set_embedding(segment: Segment, embedding: list[float]):
#     dimension = len(embedding)
#     if dimension == 4096:
#         segment.embedding4096 = embedding
#     elif dimension == 2048:
#         segment.embedding2048 = embedding
#     elif dimension == 1024:
#         segment.embedding1024 = embedding
#     elif dimension == 768:
#         segment.embedding768 = embedding
#     elif dimension == 512:
#         segment.embedding512 = embedding
#     elif dimension == 384:
#         segment.embedding384 = embedding
#     elif dimension == 256:
#         segment.embedding256 = embedding


# def get_column_name(embedding: list[float]) -> str:
#     dimension = len(embedding)
#     return f"Embedding{dimension}"
