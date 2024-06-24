import ssl
import certifi
import easyocr
import re

def create_default_https_context():
    return ssl.create_default_context(cafile=certifi.where())

ssl._create_default_https_context = create_default_https_context
# Initialize the OCR reader
reader = easyocr.Reader(['de'], gpu=True)


def check_plate_format(text):
    pattern = r'^[A-Z]{1,3}[A-Z]{1,2}\d{1,4}[A-Z]{1,2}'

    if re.match(pattern, text):
        return True
    else:
        return False


def read_license_plate(license_plate_crop):
    detections = reader.readtext(license_plate_crop)
    plate_text = ''
    for detection in detections:
        bbox, text, score = detection
        text = text.upper().replace(' ', '').replace('\'', '').replace(':', '').replace('_', '')
        plate_text += text
    if check_plate_format(plate_text):
        return plate_text
    else:
        return None