
from uuid import uuid4
import cv2
import mediapipe as mp
import numpy as np
import json
import base64


class HolisticCallback():

    @staticmethod
    def test_process(body):

        message = json.loads(body)

        video_id = message["video_id"]
        frame_id = message["frame_id"]
        frame_link = message["frame_link"]

        with open(frame_link, "rb") as f:
            im_b64 = base64.b64encode(f.read())

        im64_bytes = base64.b64decode(im_b64)

        im_arr = np.frombuffer(im64_bytes, dtype=np.uint8)
        img = cv2.imdecode(im_arr, flags=cv2.COLOR_BGR2RGB)
        file_id = str(uuid4())[:5]
        mp_drawing = mp.solutions.drawing_utils
        mp_holistic = mp.solutions.holistic

        with mp_holistic.Holistic(static_image_mode=True) as holistic:

            results = holistic.process(img)
            if results.pose_landmarks:
                # print(
                #     f'Nose coordinates: ('
                #     f'{results.pose_landmarks.landmark}, '
                #     f'{results.pose_landmarks.landmark})'
                # )
                annotated_image = img.copy()
                mp_drawing.draw_landmarks(
                    annotated_image, results.left_hand_landmarks, mp_holistic.HAND_CONNECTIONS)
                mp_drawing.draw_landmarks(
                    annotated_image, results.right_hand_landmarks, mp_holistic.HAND_CONNECTIONS)
                mp_drawing.draw_landmarks(
                    annotated_image, results.pose_landmarks, mp_holistic.POSE_CONNECTIONS)

                cv2.imwrite('output/image' + file_id +
                            '.png', annotated_image)

        return results, video_id, frame_id
