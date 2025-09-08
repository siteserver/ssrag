# import base64
# import mimetypes
# from langchain_openai import ChatOpenAI
# from configs import configs
# from pydantic import SecretStr
# from typing import Union


# def llm_manager_get_image_description(
#     filePath, extName, prompt=None
# ) -> Union[str, list]:
#     llm = ChatOpenAI(
#         base_url="https://api.siliconflow.cn/v1/",
#         api_key=SecretStr(""),
#         model="QwenQVQ",
#     )

#     if prompt is None or prompt.strip() == "":
#         prompt = "为这张图片写一个详细的说明文字。"

#     data_uri = ""
#     with open(filePath, "rb") as imageFile:
#         contentType, encoding = mimetypes.guess_type("_dummy" + extName)
#         if contentType is None:
#             contentType = "image/jpeg"
#         imageBase64 = base64.b64encode(imageFile.read()).decode("utf-8")
#         dataUri = f"data:{contentType};base64,{imageBase64}"

#     messages = [
#         {
#             "role": "user",
#             "content": [
#                 {"type": "text", "text": prompt},
#                 {
#                     "type": "image_url",
#                     "image_url": {
#                         "url": data_uri,
#                     },
#                 },
#             ],
#         }
#     ]

#     response = llm.invoke(messages)
#     return (
#         response.content if isinstance(response.content, str) else str(response.content)
#     )
