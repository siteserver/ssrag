from pydantic import BaseModel
import requests


class BochaWebSearchWebPage(BaseModel):
    name: str | None
    url: str | None
    snippet: str | None
    summary: str | None
    siteName: str | None
    siteIcon: str | None
    dateLastCrawled: str | None


class BochaWebSearchResponse(BaseModel):
    success: bool = False
    webPages: list[BochaWebSearchWebPage] | None = None
    errorMessage: str = ""


def bocha_websearch_tool(
    api_key: str,
    query: str,
    freshness: str = "noLimit",
    summary: bool = True,
    include: str = "",
    exclude: str = "",
    count: int = 10,
) -> BochaWebSearchResponse:
    """
    使用Bocha Web Search API 进行网页搜索。

    参数:
    - query: 搜索关键词
    - freshness: 搜索的时间范围
    - summary: 是否显示文本摘要
    - include: 包含的网站
    - exclude: 排除的网站
    - count: 返回的搜索结果数量

    返回:
    - 搜索结果的详细信息，包括网页标题、网页URL、网页摘要、网站名称、网站Icon、网页发布时间等。
    """

    url = "https://api.bochaai.com/v1/web-search"
    headers = {
        "Authorization": f"Bearer {api_key}",  # 请替换为你的API密钥
        "Content-Type": "application/json",
    }
    data = {
        "query": query,
        "freshness": freshness,
        "summary": summary,
        "include": include,
        "exclude": exclude,
        "count": count,
    }

    response = requests.post(url, headers=headers, json=data)

    if response.status_code == 200:
        json_response = response.json()
        try:
            if json_response["code"] != 200 or not json_response["data"]:
                return BochaWebSearchResponse(
                    success=False,
                    errorMessage=json_response.get("msg", "未知错误"),
                )

            webpages = json_response["data"]["webPages"]["value"]
            if not webpages:
                return BochaWebSearchResponse(
                    success=False,
                    errorMessage="未找到相关结果。",
                )
            webPages: list[BochaWebSearchWebPage] = []
            for idx, page in enumerate(webpages, start=1):
                webPages.append(
                    BochaWebSearchWebPage(
                        name=page["name"],
                        url=page["url"],
                        snippet=page["snippet"],
                        summary=page["summary"] if summary else None,
                        siteName=page["siteName"],
                        siteIcon=page["siteIcon"],
                        dateLastCrawled=page["dateLastCrawled"],
                    )
                )
            return BochaWebSearchResponse(
                success=True,
                webPages=webPages,
            )

        except Exception as e:
            return BochaWebSearchResponse(
                success=False,
                errorMessage=f"搜索API请求失败，原因是：搜索结果解析失败 {str(e)}",
            )
    else:
        return BochaWebSearchResponse(
            success=False,
            errorMessage=f"搜索API请求失败，状态码: {response.status_code}, 错误信息: {response.text}",
        )
