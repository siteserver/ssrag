import pymupdf4llm

# 使用pymupdf4llm将PDF转换为Markdown，并将图片以base64方式嵌入
md_text = pymupdf4llm.to_markdown(
    "test.pdf",  # 替换为你的PDF文件路径
    embed_images=True
)

# 保存到文件
with open("test.md", "w", encoding="utf-8") as f:
    f.write(md_text)
print("转换完成！Markdown文件已保存为 'test.md'")