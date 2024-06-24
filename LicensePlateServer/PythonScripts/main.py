import json
from datetime import datetime
import httpx
from ultralytics import YOLO
import cv2
import logging
from sort.sort import *
from util import read_license_plate

logging.getLogger('ultralytics').setLevel(logging.WARNING)

mot_tracker = Sort()

# load models
coco_model = YOLO('/Users/iseaman/RiderProjects/LicensePlate/LicensePlateServer/TrainedModels/models/yolov8m.pt')
license_plate_detector = YOLO(
    '/Users/iseaman/RiderProjects/LicensePlate/LicensePlateServer/TrainedModels/models/license_plate_detector.pt')

# load pic for test
cap = cv2.imread('/Users/iseaman/RiderProjects/LicensePlate/LicensePlateServer/TrainedModels/models/test.jpeg')


class LicensePlate:
    def __init__(self, date_time, camera_name, plate_number, image):
        self.date_time = date_time
        self.camera_name = camera_name
        self.plate_number = plate_number
        self.image = image

    def to_json(self):
        return json.dumps({
            'Date': self.date_time.strftime('%Y-%m-%d'),
            'Time': self.date_time.strftime('%H:%M:%S'),
            'CameraName': self.camera_name,
            'PlateNumber': self.plate_number,
            'Image': self.image
        })


# read frames
def process_image_from_camera(ip, login, password, name):
    url = f"http://{ip}/axis-cgi/jpg/image.cgi"
    repeat = True
    frame_nr = 0
    while repeat:
        frame_nr += 1
        response = httpx.get(url, auth=httpx.DigestAuth(login, password))
        if response.status_code == 200:
            #image = cv2.imdecode(np.frombuffer(response.content, np.uint8), cv2.IMREAD_COLOR)
            image = cap
            # detect license plates
            license_plates = license_plate_detector(image)[0]
            for license_plate in license_plates.boxes.data.tolist():
                x1, y1, x2, y2, score, class_id = license_plate
                # crop license plate
                license_plate_crop = image[int(y1):int(y2), int(x1): int(x2), :]
                # process license plate
                license_plate_crop_gray = cv2.cvtColor(license_plate_crop, cv2.COLOR_BGR2GRAY)
                _, license_plate_crop_thresh = cv2.threshold(license_plate_crop_gray, 64, 255, cv2.THRESH_BINARY_INV)
                # read license plate number
                license_plate_text = read_license_plate(license_plate_crop_thresh)
                current_datetime = datetime.now()
                if license_plate_text is not None:
                    # save image
                    file_name = '/Users/iseaman/RiderProjects/LicensePlate/LicensePlateClient/wwwroot/Images/Plates/' + license_plate_text + '.jpg'
                    cv2.imwrite(file_name, license_plate_crop)
                    license_plate = LicensePlate(current_datetime, name, license_plate_text, license_plate_text + '.jpg')
                    print(license_plate.to_json())
        else:
            repeat = False
            raise Exception("Failed to get image from camera")


if __name__ == "__main__":
    import sys

    camera_ip = sys.argv[1]
    camera_login = sys.argv[2]
    camera_password = sys.argv[3]
    camera_name = sys.argv[4]

    #camera_ip = '192.168.0.159'
    #camera_login = 'root'
    #camera_password = 'pass'
    #camera_name = 'Test camera'

    try:
        process_image_from_camera(camera_ip, camera_login, camera_password, camera_name)

    except Exception as e:
        print(f"Error: {str(e)}", file=sys.stderr)
        sys.exit(1)
