from datetime import datetime
import os
import posixpath


def get_upload_dir_path():
    current_date = datetime.now()
    dir_path = f"./wwwroot/{current_date.strftime('%Y/%m/%d')}"
    if not os.path.exists(dir_path):
        os.makedirs(dir_path)
    return dir_path


def combine_url(*args: str) -> str:
    return posixpath.join(*args)


def get_date_path() -> str:
    current_date = datetime.now()
    return current_date.strftime("%Y/%m/%d")
