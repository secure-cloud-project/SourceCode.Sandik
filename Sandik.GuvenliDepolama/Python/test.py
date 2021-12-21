from cryption import Encryption,FileEncyrption

file_loc=r"\\?\C:\Users\oguzc\Desktop\Guvenli Depolama Yeni\Guvenli_Depolama\Sandik.GuvenliDepolama\Sandik.GuvenliDepolama\UploadFileTemp"
data_cryptiption = FileEncyrption(file_loc)
data_cryptiption.encrypt_files()