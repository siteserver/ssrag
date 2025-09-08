from repositories import document_repository, segment_repository
from vectors import Vector


def embedding_changed(celery_app):
    current = 0
    vector = Vector()
    vector.create_collection()
    vector.delete_all()

    documents = document_repository.get_all()
    current = 0
    for document in documents:
        segments = segment_repository.get_all_by_document_id(document.id)
        vector.add_segments(document, segments)
        current += 1
        celery_app.update_state(
            state="PROGRESS",
            meta={
                "count": len(documents),
                "current": current,
            },
        )

    return {"status": "SUCCESS"}
