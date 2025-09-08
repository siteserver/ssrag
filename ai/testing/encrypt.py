from utils import encrypt_utils
from gmssl.sm4 import CryptSM4, SM4_ENCRYPT, SM4_DECRYPT


# 创建密钥和IV
key = b"f8ca6f4a11aba05d98d781297ad38b15"  # 16字节密钥
iv = b"dbd5c7fa1dc2f4fe783f70a0df5e20f4"  # 16字节IV
plaintext = "Hello, SM4 Cross-Language!"

# 创建SM4加密对象
sm4_crypt = CryptSM4()
sm4_crypt.set_key(key, SM4_ENCRYPT)
# sm4_crypt.set_iv(iv)

# 加密
ciphertext = sm4_crypt.crypt_cbc(iv, plaintext.encode("utf-8"))
ciphertext_hex = ciphertext.hex()
print("密文 (Hex): " + ciphertext_hex)

# 解密
sm4_crypt.set_key(key, SM4_DECRYPT)
# sm4_crypt.set_iv(iv)
decrypted = sm4_crypt.crypt_cbc(iv, bytes.fromhex(ciphertext_hex))
decrypted_text = decrypted.decode("utf-8").rstrip("\x00")  # 移除填充
print("解密结果: " + decrypted_text)

security_key = "f8ca6f4a11aba05d98d781297ad38b15.dbd5c7fa1dc2f4fe783f70a0df5e20f4"
print(encrypt_utils.sm4_encrypt(plaintext, security_key))
print(
    encrypt_utils.sm4_decrypt(
        encrypt_utils.sm4_encrypt(plaintext, security_key), security_key
    )
)
