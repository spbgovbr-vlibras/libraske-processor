from pathlib import Path

import pytest

from src.utils.holistic_pontuation import GetScore
from src.utils.holistic_callback import HolisticCallback

PATH = Path(__file__).parent / "sample_data"
JSON_FILES = PATH.glob("*.json")  

@pytest.mark.parametrize(
        "json_file, expected_score",
        zip(
            JSON_FILES, [
    0.9382496565580368,
    0.8093868319122564,
    0.902368642505081,
    1,
    0.8413587204225006,
    0.8441945370996282,
    0.9110318653462898,
    0.9473307507112623,
    0.8144037180801942,
    0.8030964102596044,
    0.8051319649842169,
    ]))
def test_score(json_file, expected_score):

    body = json_file.read_bytes()

    results, video_id, frame_id, session_id = HolisticCallback.test_process(body)

    score = GetScore().get_score(results, video_id, frame_id)
    score = float(score)

    assert (score - expected_score) < 0.0001