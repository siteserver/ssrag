import jwt
from gmssl.sm4 import CryptSM4, SM4_ENCRYPT, SM4_DECRYPT
import uuid


def jwt_encrypt(credentials: dict) -> tuple[str, str]:
    salt = str(uuid.uuid4())
    return jwt.encode(credentials, salt, algorithm="HS256"), salt


def jwt_decrypt(credentials: str, salt: str) -> dict:
    return jwt.decode(credentials, salt, algorithms=["HS256"])


def is_valid_security_key(security_key: str) -> tuple[bool, bytes, bytes]:
    """
    验证安全密钥格式是否正确

    Args:
        security_key: 格式为 "key.iv" 的安全密钥字符串

    Returns:
        tuple[bool, str, str]: (是否有效, key, iv)
    """
    if not security_key:
        return False, b"", b""

    parts = security_key.split(".")
    if len(parts) != 2:
        return False, b"", b""

    key, iv = parts[0], parts[1]
    if len(key) != 32 or len(iv) != 32:
        return False, b"", b""

    return True, key.encode("utf-8"), iv.encode("utf-8")


def sm4_encrypt(plaintext: str, security_key: str) -> str:
    is_valid, key, iv = is_valid_security_key(security_key)
    if not is_valid:
        raise ValueError("Invalid security key")

    sm4_crypt = CryptSM4()
    sm4_crypt.set_key(key, SM4_ENCRYPT)
    ciphertext = sm4_crypt.crypt_cbc(iv, plaintext.encode("utf-8"))
    ciphertext_hex = ciphertext.hex()
    return ciphertext_hex


def sm4_decrypt(ciphertext: str, security_key: str) -> str:
    is_valid, key, iv = is_valid_security_key(security_key)
    if not is_valid:
        raise ValueError("Invalid security key")

    sm4_crypt = CryptSM4()
    sm4_crypt.set_key(key, SM4_DECRYPT)
    decrypted = sm4_crypt.crypt_cbc(iv, bytes.fromhex(ciphertext))
    decrypted_text = decrypted.decode("utf-8").rstrip("\x00")
    return decrypted_text
