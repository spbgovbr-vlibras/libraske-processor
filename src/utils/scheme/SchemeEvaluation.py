

class SchemeEvaluation():

    def __init__(self):

        self.evaluation_scheme = {}
        self.set_points()

    def set_points(self):

        pose_scheme = [
            'LEFT_SHOULDER',
            'RIGHT_SHOULDER',
            'LEFT_ELBOW',
            'RIGHT_ELBOW',
            'LEFT_HIP',
            'RIGHT_HIP',
            'LEFT_WRIST',
            'RIGHT_WRIST',
        ]

        general_hands = [
            'WRIST',
            'THUMB_CMC',
            'THUMB_MCP',
            'THUMB_IP',
            'THUMB_TIP',
            'INDEX_FINGER_MCP',
            'INDEX_FINGER_PIP',
            'INDEX_FINGER_DIP',
            'INDEX_FINGER_TIP',
            'MIDDLE_FINGER_MCP',
            'MIDDLE_FINGER_PIP',
            'MIDDLE_FINGER_DIP',
            'MIDDLE_FINGER_TIP',
            'RING_FINGER_MCP',
            'RING_FINGER_PIP',
            'RING_FINGER_DIP',
            'RING_FINGER_TIP',
            'PINKY_MCP',
            'PINKY_PIP',
            'PINKY_DIP',
            'PINKY_TIP',
        ]

        self.evaluation_scheme = {
            'POSE': pose_scheme, 'LEFT_HAND': general_hands, 'RIGHT_HAND': general_hands}
