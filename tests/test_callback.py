import json
from pathlib import Path

import pytest

from src.utils.holistic_callback import HolisticCallback

PATH = Path(__file__).parent / "sample_data"
JSON_FILES = PATH.glob("*.json")  

@pytest.mark.parametrize("json_file", JSON_FILES)
def test_holistic_callback_process(json_file):

    body = json_file.read_bytes()
    body_dict = json.loads(body)

    results, video_id, frame_id, session_id = HolisticCallback.test_process(body)

    assert hasattr(results, "face_landmarks")
    assert hasattr(results, "left_hand_landmarks")
    assert hasattr(results, "right_hand_landmarks")
    assert hasattr(results, "pose_landmarks")
    assert hasattr(results, "segmentation_mask")


    assert video_id == body_dict["videoId"]
    assert frame_id == body_dict["idFrame"]
    assert session_id == body_dict["idSession"]