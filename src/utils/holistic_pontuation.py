import cv2
import json
import math
import mediapipe as mp
from utils.scheme.SchemeEvaluation import SchemeEvaluation
mp_drawing = mp.solutions.drawing_utils
mp_holistic = mp.solutions.holistic


class GetScore():

    def get_score(self, results, video_id, frameID):

        with open(f'references_data/{video_id}.txt') as json_file:
            reference = json.load(json_file)

        body_scheme = SchemeEvaluation().evaluation_scheme

        pose_weight = 1
        hand_weight = 2

        sum_pose_miss = 0
        sum_left_hand_miss = 0
        sum_right_hand_miss = 0
        # frameID = str(frameID)

        for key, values in body_scheme.items():

            if 'POSE' == key:

                for value in values:

                    try:
                        miss_landmark_x = math.fabs(
                            results.pose_landmarks.landmark[mp_holistic.PoseLandmark[value]].x - reference[frameID][key][value][0])
                        miss_landmark_y = math.fabs(
                            results.pose_landmarks.landmark[mp_holistic.PoseLandmark[value]].y - reference[frameID][key][value][1])
                        sum_pose_miss += (miss_landmark_x + miss_landmark_y)/2
                    except:
                        sum_pose_miss += 1

                average_pose_miss = sum_pose_miss/len(values)

            if 'LEFT_HAND' == key:

                for value in values:

                    try:
                        miss_landmark_x = math.fabs(
                            results.left_hand_landmarks.landmark[mp_holistic.HandLandmark[value]].x - reference[frameID][key][value][0])
                        miss_landmark_y = math.fabs(
                            results.left_hand_landmarks.landmark[mp_holistic.HandLandmark[value]].y - reference[frameID][key][value][1])
                        sum_left_hand_miss += (miss_landmark_x +
                                               miss_landmark_y)/2
                    except:
                        sum_left_hand_miss += 1

                average_left_hand_miss = sum_left_hand_miss/len(values)

            if 'RIGHT_HAND' == key:

                for value in values:

                    try:
                        miss_landmark_x = math.fabs(
                            results.right_hand_landmarks.landmark[mp_holistic.HandLandmark[value]].x - reference[frameID][key][value][0])
                        miss_landmark_y = math.fabs(
                            results.right_hand_landmarks.landmark[mp_holistic.HandLandmark[value]].y - reference[frameID][key][value][1])
                        sum_right_hand_miss += (miss_landmark_x +
                                                miss_landmark_y)/2
                    except:
                        sum_right_hand_miss += 1

                average_right_hand_miss = sum_right_hand_miss/len(values)

        total_average_miss = (average_pose_miss*pose_weight + average_left_hand_miss *
                              hand_weight + average_right_hand_miss*hand_weight)/(pose_weight + hand_weight*2)

        print("Total average miss:", "{:.2f}".format(
            total_average_miss*100), "%")

        return "{:.2f}".format(total_average_miss*100)
