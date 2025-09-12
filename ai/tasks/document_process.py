from repositories import document_repository, content_repository, site_repository
from utils import md_utils
from dto import TaskDocumentProcess
from markitdown import MarkItDown
from models import Document
from utils import string_utils, file_utils
from configs import constants
from vectors import Vector
from storages import Storage
import io
from enums import TaskStatus


def document_process(
    celery_app,
    task: TaskDocumentProcess,
):
    markItDown = MarkItDown()
    md_path = f"{task.dirPath}/{task.uuid}.md"

    # if ext_name.lower() in [".jpg", ".jpeg", ".png"]:
    #     text_content = llm_manager.get_image_description(save_path, ext_name)
    #     if isinstance(text_content, list):
    #         text_content = "\n".join(text_content)
    #     with open(md_path, "w", encoding="utf-8") as md_file:
    #         md_file.write(text_content)

    storage = Storage()
    file_path = file_utils.combine_url(
        task.dirPath or "", f"{task.uuid}{task.extName or ''}"
    )

    md_content = ""
    if task.extName and task.extName.lower() == ".pdf":
        md_content = md_utils.convert_pdf_to_md(storage, file_path)
    elif task.extName and task.extName.lower() != ".md":
        result = markItDown.convert_stream(
            io.BytesIO(storage.load_bytes(file_path)), file_extension=task.extName, keep_data_uris=True
        )
        md_content = result.markdown
    else:
        md_content = storage.load_text(file_path)
        
    md_content = md_utils.save_base64_images_and_replace(storage, task.dirPath, task.uuid, md_content)

    storage.save_text(md_path, md_content)

    site = site_repository.get(task.siteId or 0)
    if site is None:
        raise ValueError("site not found")
    site_values = site.site_values

    document = Document(
        uuid=task.uuid or "",
        siteId=task.siteId or 0,
        channelId=task.channelId or 0,
        contentId=task.contentId or 0,
        title=f"{task.fileName or ''}{task.extName or ''}",
        dirPath=task.dirPath or "",
        fileName=task.fileName or "",
        extName=task.extName or "",
        fileSize=len(md_content),
        separators=string_utils.join(
            site_values.separators or constants.DEFAULT_CHUNK_SEPARATORS, ","
        ),
        chunkSize=site_values.chunkSize or constants.DEFAULT_CHUNK_SIZE,
        chunkOverlap=site_values.chunkOverlap or constants.DEFAULT_CHUNK_OVERLAP,
        isChunkReplaces=site_values.isChunkReplaces
        or constants.DEFAULT_CHUNK_IS_CHUNK_REPLACES,
        isChunkDeletes=site_values.isChunkDeletes
        or constants.DEFAULT_CHUNK_IS_CHUNK_DELETES,
    )

    document = document_repository.insert(document)
    task_result = {
        "id": document.id,
        "fileName": document.fileName,
        "extName": document.extName,
        "fileSize": document.fileSize,
        "title": document.title,
    }

    celery_app.update_state(
        state=TaskStatus.PROGRESS,
        meta=task_result,
    )
    # result = vector_manager.chunk(document)
    # vector_manager.embed_chunks(result)
    chunks = md_utils.chunk_document_md(document, md_content)
    vector = Vector()
    vector.add_texts(document, chunks)

    if task.contentId and task.siteId and task.channelId and task.contentId > 0:
        content_repository.update_knowledge(
            site.tableName,
            task.siteId,
            task.channelId,
            task.contentId,
            True,
        )

    return task_result
