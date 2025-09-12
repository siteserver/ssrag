from dto import ChunkConfig
from models import Document
from llama_index.core.node_parser import SentenceSplitter
from llama_index.core.readers import StringIterableReader
import fitz
import pymupdf4llm
import re
import base64
from storages import Storage


# 替换掉连续的空格、换行符和制表符
def preprocessing_replaces(text):
    text = re.sub(r" +", " ", text)
    text = re.sub(r"\n+", "\n", text)
    text = re.sub(r"\t+", "\t", text)
    return text


# 删除所有 URL 和电子邮箱地址
def preprocessing_deletes(text):
    # 正则表达式匹配 URL
    url_pattern = r"https?://\S+|www\.\S+"
    # 正则表达式匹配电子邮箱地址
    email_pattern = r"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b"

    # 删除 URL
    text = re.sub(url_pattern, "", text)
    # 删除电子邮箱地址
    text = re.sub(email_pattern, "", text)

    # 清理可能留下的多余空格
    text = re.sub(r"\s+", " ", text).strip()

    return text


def chunk_document_md(
    document: Document,
    md_content: str,
) -> list[str]:
    config = ChunkConfig(
        separators=str(document.separators).split(","),
        chunkSize=document.chunkSize,
        chunkOverlap=document.chunkOverlap,
        isChunkReplaces=document.isChunkReplaces,
        isChunkDeletes=document.isChunkDeletes,
    )

    # # Process the separator(s) from config: for each list item,
    # # replace "double_new_line" with "\n\n" and "new_line" with "\n"
    # separators = []
    # for sep in config.separators:
    #     new_sep = sep.replace("double_new_line", "\n\n").replace("new_line", "\n")
    #     separators.append(new_sep)

    # reader = FlatReader()
    # md_documents = reader.load_data(Path(mdPath))  # 替换为你的Markdown路径
    md_documents = StringIterableReader().load_data(texts=[md_content])

    # 文本预处理
    if config.isChunkReplaces:
        for doc in md_documents:
            doc.set_content(preprocessing_replaces(doc.get_content()))
    if config.isChunkDeletes:
        for doc in md_documents:
            doc.set_content(preprocessing_deletes(doc.get_content()))

    # md_parser = MarkdownNodeParser()
    # base_nodes = md_parser.get_nodes_from_documents(md_documents)

    text_parser = SentenceSplitter(
        paragraph_separator="\n\n",
        chunk_size=config.chunkSize,
        chunk_overlap=config.chunkOverlap,
    )
    chunked_nodes = text_parser.get_nodes_from_documents(md_documents)

    # 合并过小的块
    merged_chunks = []
    buffer = ""

    for split in chunked_nodes:
        buffer += split.get_content()
        if len(buffer) >= config.chunkOverlap:
            merged_chunks.append(buffer)
            buffer = ""

    # 添加最后一个块（如果有剩余内容）
    if buffer:
        merged_chunks.append(buffer)

    taxis = 0
    chunks: list[str] = []
    for text in merged_chunks:
        taxis += 1
        chunks.append(text)

    return chunks


def save_base64_images_and_replace(storage: Storage, dirPath, uuid, content):
    # 匹配 ![title](data:image/xxx;base64,xxxx) 的正则
    pattern = r'!\[([^\]]*)\]\((data:image\/[a-zA-Z0-9+]+;base64,[^\)]+)\)'
    img_counter = [1]  # 使用列表作为可变对象以便在闭包中修改

    def repl(match):
        title = match.group(1)
        data_url = match.group(2)
        # 提取图片类型
        m = re.match(r'data:image/([a-zA-Z0-9+]+);base64,(.+)', data_url)
        if not m:
            return match.group(0)
        ext = m.group(1)
        base64_data = m.group(2)
        # 生成按自然数顺序的文件名
        filename = f"{dirPath}/{uuid}_{img_counter[0]}.{ext}"
        img_counter[0] += 1
        # 保存图片
        storage.save_stream(filename, base64.b64decode(base64_data), True)
        file_url = storage.get_file_url(filename, False)
        # 替换为本地图片url
        return f'![{title}]({file_url})'

    content = re.sub(pattern, repl, content)
    return content
  
  
def convert_pdf_to_md(storage: Storage, file_path: str) -> str:
    pdf_bytes = storage.load_bytes(file_path)
    doc = fitz.open(stream=pdf_bytes, filetype="pdf")
    md_text = pymupdf4llm.to_markdown(
        doc,
        embed_images=True
    )
    return md_text