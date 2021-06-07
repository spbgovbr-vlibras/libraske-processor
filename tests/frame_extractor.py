
import pika
import json
import cv2
import fire
import re
from tqdm import trange


class Extract_Frame:

    def __init__(self):
        self.resize_width = 854
        self.resize_height = 480
        self.server = RabbitmqConfigure(queue='Mediapipe',
                                        host='localhost',
                                        routingKey='Mediapipe',
                                        exchange='')

    def imageSender(self, message):

        with RabbitMq(self.server) as rabbitmq:
            rabbitmq.publish(payload=message)

    def reduce_image(self, image):

        dim = (self.resize_width, self.resize_height)
        resized = cv2.resize(image, dim, interpolation=cv2.INTER_AREA)
        return resized

    def getFrame(self, sec, frame_id, video_id, vidcap, reduce_size):

        out_frame_path = f'input_frames/{video_id}_frame_{str(frame_id)}.jpg'
        vidcap.set(cv2.CAP_PROP_POS_MSEC, sec*1000)
        hasFrames, image = vidcap.read()

        if hasFrames:

            file_image = image if not reduce_size else self.reduce_image(image)

            cv2.imwrite(out_frame_path, cv2.cvtColor(
                file_image, cv2.COLOR_BGR2GRAY))

            if out_frame_path:

                json_payload = {
                    "video_id": video_id,
                    "frame_id": str(frame_id),
                    "frame_link": out_frame_path
                }
                message = json.dumps(json_payload)
                self.imageSender(message)

        return hasFrames

    def frame_extractor(self, video_path, reduce_size=False):

        print(f'IMAGE REDUCE={reduce_size}')

        video_id = re.search(r'[^/]+(?=\.)', video_path).group(0)
        vidcap = cv2.VideoCapture(video_path)
        sec = 0
        frameRate = 0.5
        frame_id = 1
        success = self.getFrame(
            sec, frame_id, video_id, vidcap, reduce_size)

        frames_number = round((int(vidcap.get(cv2.CAP_PROP_FRAME_COUNT)) /
                               vidcap.get(cv2.CAP_PROP_FPS))/0.5)

        progress_bar = trange(frames_number, colour='#8B71F6',
                              desc='Sending Frames 🖼️:')

        while success:
            progress_bar.update(1)
            frame_id = frame_id + 1
            sec = sec + frameRate
            sec = round(sec, 2)
            success = self.getFrame(
                sec, frame_id, video_id, vidcap, reduce_size)


class MetaClass(type):

    _instance = {}

    def __call__(cls, *args, **kwargs):
        """ Singelton Design Pattern  """

        if cls not in cls._instance:
            cls._instance[cls] = super(
                MetaClass, cls).__call__(*args, **kwargs)
            return cls._instance[cls]


class RabbitmqConfigure(metaclass=MetaClass):

    def __init__(self, queue='hello', host='localhost', routingKey='hello', exchange=''):
        """ Configure Rabbit Mq Server  """
        self.queue = queue
        self.host = host
        self.routingKey = routingKey
        self.exchange = exchange


class RabbitMq():

    __slots__ = ["server", "_channel", "_connection"]

    def __init__(self, server):
        """
        :param server: Object of class RabbitmqConfigure
        """

        self.server = server
        self._connection = pika.BlockingConnection(
            pika.ConnectionParameters(host=self.server.host))
        self._channel = self._connection.channel()
        self._channel.queue_declare(queue=self.server.queue)

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        self._connection.close()

    def publish(self, payload={}):
        """
        :param payload: JSON payload
        :return: None
        """

        self._channel.basic_publish(exchange=self.server.exchange,
                                    routing_key=self.server.routingKey,
                                    body=payload)


class Image(object):

    __slots__ = ["filename"]

    def __init__(self, filename):
        self.filename = filename

    @property
    def get(self):
        with open(self.filename, "rb") as f:
            data = f.read()
        return data


if __name__ == "__main__":

    frame_extractor = Extract_Frame()
    fire.Fire(frame_extractor.frame_extractor)
