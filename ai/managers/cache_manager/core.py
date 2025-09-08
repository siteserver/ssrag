import time
import threading
from typing import Dict, Any, Optional
from configs import app_configs
import redis


class MemoryCache:
    """内存缓存类，使用字典存储，支持过期时间"""

    def __init__(self):
        self._cache: Dict[str, Dict[str, Any]] = {}
        self._lock = threading.RLock()

    def set(self, key: str, value: str, ex: int = 300) -> None:
        """设置缓存，ex为过期时间（秒），默认5分钟"""
        with self._lock:
            self._cache[key] = {"value": value, "expire_at": time.time() + ex}

    def get(self, key: str) -> Optional[str]:
        """获取缓存值，如果过期则返回None"""
        with self._lock:
            if key not in self._cache:
                return None

            cache_item = self._cache[key]
            if time.time() > cache_item["expire_at"]:
                # 过期，删除并返回None
                del self._cache[key]
                return None

            return cache_item["value"]

    def delete(self, key: str) -> None:
        """删除缓存"""
        with self._lock:
            self._cache.pop(key, None)

    def clear(self) -> None:
        """清空所有缓存"""
        with self._lock:
            self._cache.clear()

    def cleanup_expired(self) -> None:
        """清理过期的缓存项"""
        current_time = time.time()
        with self._lock:
            expired_keys = [
                key
                for key, item in self._cache.items()
                if current_time > item["expire_at"]
            ]
            for key in expired_keys:
                del self._cache[key]


class CacheManager:
    def __init__(self):
        # 初始化内存缓存
        self.memory_cache = MemoryCache()

        # 初始化Redis连接
        self.redis_client = redis.Redis(
            host=app_configs.REDIS_HOST,
            port=app_configs.REDIS_PORT,
            db=app_configs.REDIS_DB,
            password=app_configs.REDIS_PASSWORD,
            ssl=app_configs.REDIS_SSL,
            decode_responses=True,
        )

        # 缓存配置
        self.redis_prefix = (
            app_configs.REDIS_PREFIX + ":" if app_configs.REDIS_PREFIX else ""
        )
        self.memory_default_ttl = 300  # 5分钟
        self.redis_default_ttl = 3600  # 60分钟

    @property
    def client(self) -> redis.Redis:
        return self.redis_client

    def _get_final_key(self, key: str) -> str:
        return f"{self.redis_prefix}{key}"

    def get_class_key(self, class_name: str, *args: str) -> str:
        if not args:
            key = class_name
        else:
            args_str = ":".join(args)
            key = f"{class_name}:{args_str}"
        return key

    def get_entity_key(self, table_name: str, *args: str) -> str:
        if not args:
            key = f"{table_name}:entity"
        else:
            args_str = ":".join(args)
            key = f"{table_name}:entity:{args_str}"
        return key

    def get_list_key(self, table_name: str, *args: str) -> str:
        if not args:
            key = f"{table_name}:list"
        else:
            args_str = ":".join(args)
            key = f"{table_name}:list:{args_str}"
        return key

    def set(
        self,
        key: str,
        value: str,
        memory_ttl: Optional[int] = None,
        redis_ttl: Optional[int] = None,
    ):
        """
        设置二级缓存
        :param key: 缓存键
        :param value: 缓存值
        :param memory_ttl: 内存缓存过期时间（秒），默认5分钟
        :param redis_ttl: Redis缓存过期时间（秒），默认60分钟
        """
        final_key = self._get_final_key(key)

        # 设置内存缓存
        memory_expire = (
            memory_ttl if memory_ttl is not None else self.memory_default_ttl
        )
        self.memory_cache.set(final_key, value, memory_expire)

        # 设置Redis缓存
        redis_expire = redis_ttl if redis_ttl is not None else self.redis_default_ttl
        self.redis_client.set(final_key, value, redis_expire)

    def get(self, key: str) -> str | None:
        """
        获取缓存值，先查内存缓存，再查Redis缓存
        :param key: 缓存键
        :return: 缓存值，如果不存在返回None
        """
        final_key = self._get_final_key(key)

        # 先查内存缓存
        memory_value = self.memory_cache.get(final_key)
        if memory_value is not None:
            return memory_value

            # 内存缓存未命中，查Redis缓存
        redis_value = self.redis_client.get(final_key)
        if redis_value is not None:
            # Redis命中，回填到内存缓存
            redis_value_str = str(redis_value)
            self.memory_cache.set(final_key, redis_value_str, self.memory_default_ttl)
            return redis_value_str

        return None

    def delete(self, key: str):
        """
        删除缓存，同时删除内存和Redis中的缓存
        :param key: 缓存键
        """
        final_key = self._get_final_key(key)

        # 删除内存缓存
        self.memory_cache.delete(final_key)

        # 删除Redis缓存
        self.redis_client.delete(final_key)

    def clear_all(self):
        """
        清空所有缓存，同时删除内存和Redis中的缓存
        """
        self.redis_client.flushall()
        self.memory_cache.clear()
