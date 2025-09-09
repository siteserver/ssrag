import alibabacloud_oss_v2 as oss
import posixpath
from ..storage_base import StorageBase
from ..storage_type import StorageType
from .aliyun_oss_config import AliyunOssConfig
from utils import file_utils
import urllib.parse


class AliyunOssStorage(StorageBase):
    def __init__(self, config: AliyunOssConfig):
        super().__init__()
        self.bucket_name = config.bucket_name
        self.bucket_url = config.bucket_url
        self.prefix = config.prefix
        self.endpoint = config.endpoint

        cfg = oss.config.load_default()
        cfg.endpoint = config.endpoint
        cfg.region = config.region
        cfg.credentials_provider = oss.credentials.StaticCredentialsProvider(
            access_key_id=config.access_key,
            access_key_secret=config.secret_key,
        )
        self.client = oss.Client(cfg)

    def get_type(self) -> str:
        return StorageType.ALIYUN_OSS

    def save_stream(self, filename: str, data: bytes, public: bool) -> int:
        self.client.put_object(
            oss.PutObjectRequest(
                bucket=self.bucket_name,
                key=self._combine_path(filename),
                body=data,
                acl="public-read" if public else "private",
            )
        )
        return len(data)

    def save_text(self, filename: str, text: str, public: bool) -> int:
        return self.save_stream(filename, text.encode("utf-8"), public)

    def get_file_url(self, filename: str, signed: bool) -> str:
        if signed:
            pre_result = self.client.presign(
                oss.GetObjectRequest(
                    bucket=self.bucket_name,
                    key=self._combine_path(filename),
                )
            )
            file_url = pre_result.url if pre_result.url else ""
            if file_url and self.bucket_url:
                # 将file_url中域名部分替换为self.bucket_url
                parsed_url = urllib.parse.urlparse(file_url)
                bucket_url_parsed = urllib.parse.urlparse(self.bucket_url)
                # 如果self.bucket_url没有scheme，补全
                if not bucket_url_parsed.scheme:
                    bucket_url_parsed = urllib.parse.urlparse("https://" + self.bucket_url)
                # 组装新的url
                file_url = urllib.parse.urlunparse((
                    bucket_url_parsed.scheme,
                    bucket_url_parsed.netloc,
                    parsed_url.path,
                    parsed_url.params,
                    parsed_url.query,
                    parsed_url.fragment
                ))
        elif self.bucket_url:
            file_url = file_utils.combine_url(self.bucket_url, self.prefix, filename)
        else:
            protocol = "http://" if self.endpoint.startswith("http://") else "https://"
            file_url = file_utils.combine_url(
                protocol,
                self.bucket_name
                + "."
                + self.endpoint.replace("http://", "").replace("https://", ""),
                self.prefix,
                filename,
            )

        return file_url

    def load_bytes(self, filename: str) -> bytes:
        result = self.client.get_object(
            oss.GetObjectRequest(
                bucket=self.bucket_name,
                key=self._combine_path(filename),
            )
        )
        if result.body is None:
            raise FileNotFoundError(
                f"Object {filename} not found in bucket {self.bucket_name}"
            )
        with result.body as body_stream:
            return body_stream.read()

    def load_text(self, filename: str) -> str:
        return self.load_bytes(filename).decode("utf-8")

    def exists(self, filename):
        result = self.client.head_object(
            oss.HeadObjectRequest(
                bucket=self.bucket_name,
                key=self._combine_path(filename),
            )
        )
        return result.status_code == 200

    def delete(self, filename):
        self.client.delete_object(
            oss.DeleteObjectRequest(
                bucket=self.bucket_name,
                key=self._combine_path(filename),
            )
        )

    def scan(
        self, path: str, files: bool = True, directories: bool = False
    ) -> list[str]:
        result = []
        paginator = self.client.list_objects_v2_paginator()
        for page in paginator.iter_page(
            oss.ListObjectsV2Request(
                bucket=self.bucket_name,
                prefix=self._combine_path(path),
            )
        ):
            if page.contents is None:
                continue
            for o in page.contents:
                if o.key is None:
                    continue
                if files:
                    if o.key.endswith("/"):
                        continue
                    result.append(o.key.replace(self._combine_path(path), ""))
                elif directories:
                    if not o.key.endswith("/"):
                        continue
                    result.append(o.key.replace(self._combine_path(path), ""))
        return result

    def _combine_path(self, filename: str) -> str:
        return posixpath.join(self.prefix, filename) if self.prefix else filename
