import os
import cv2
import json
import getopt
import sys
import mediapipe as mp
from utils.scheme.SchemeEvaluation import SchemeEvaluation
import pickle


mp_drawing = mp.solutions.drawing_utils
mp_holistic = mp.solutions.holistic

# Catch arguments
full_cmd_arguments = sys.argv
argument_list = full_cmd_arguments[1:]

short_options = "i:o:"
long_options = ["input=", "output="]

try:
    arguments, values = getopt.getopt(
        argument_list, short_options, long_options)
except getopt.error as err:
    # Output error, and return with an error code
    print("Argument error. Use -i for input path and -o for output path")
    sys.exit(2)

for argument, value in arguments:
    if argument in ("-i", "--input"):
        input_path = value
    elif argument in ("-o", "--output"):
        output_path = value


def getFrame(sec, count, vidcap):

    with mp_holistic.Holistic(static_image_mode=True, upper_body_only=True) as holistic:
        vidcap.set(cv2.CAP_PROP_POS_MSEC, sec*1000)
        hasFrames, image = vidcap.read()

        if hasFrames:
            results = holistic.process(cv2.cvtColor(image, cv2.COLOR_BGR2RGB))
            write_data(results, count)
            print("saved frame data")

    return hasFrames


def reference_generator(video_path):

    vidcap = cv2.VideoCapture(video_path)
    sec = 0
    frameRate = 0.5
    count = 1
    success = getFrame(sec, count, vidcap)

    while success:
        count = count + 1
        sec = sec + frameRate
        sec = round(sec, 2)
        success = getFrame(sec, count, vidcap)


def data_generate(results, body_schema, index):

    data = {}
    data[index] = {}

    for key, values in body_schema.items():

        data[index][key] = {}

        if 'POSE' == key:

            for value in values:
                try:
                    data[index][key][value] = []
                    data[index][key][value].append(
                        results.pose_landmarks.landmark[mp_holistic.PoseLandmark[value]].x)
                    data[index][key][value].append(
                        results.pose_landmarks.landmark[mp_holistic.PoseLandmark[value]].y)
                except:
                    print(f'KEYPOINT:{value} not found')

        elif 'LEFT_HAND' == key:

            for value in values:
                try:
                    data[index][key][value] = []
                    data[index][key][value].append(
                        results.left_hand_landmarks.landmark[mp_holistic.HandLandmark[value]].x)
                    data[index][key][value].append(
                        results.left_hand_landmarks.landmark[mp_holistic.HandLandmark[value]].y)
                except:
                    print(f'KEYPOINT:{value} not found')

        elif 'RIGHT_HAND' == key:

            for value in values:
                try:
                    data[index][key][value] = []
                    data[index][key][value].append(
                        results.right_hand_landmarks.landmark[mp_holistic.HandLandmark[value]].x)
                    data[index][key][value].append(
                        results.right_hand_landmarks.landmark[mp_holistic.HandLandmark[value]].y)
                except:
                    print(f'KEYPOINT:{value} not found')

    return data


def write_data(results, count):

    body_scheme = SchemeEvaluation().evaluation_scheme

   # Check if file already exists
    if os.path.isfile(output_path):
        with open(output_path, 'r+') as json_file:
            full_data = json.load(json_file)
    else:
        # Creates a blank file
        with open(output_path, 'w') as json_file:
            full_data = {}
            json.dump(full_data, json_file)

    index = str(count)

    data = data_generate(results, body_scheme, index)

    with open(output_path, 'r+') as outfile:
        full_data.update(data)
        json.dump(full_data, outfile)


reference_generator(input_path)