
import pika
import os
import sys
import json
import cv2
import base64
import numpy as np
import fire
import re
import ast


def imageSender(out_frame_path, server, im64_bytes):

    with RabbitMq(server) as rabbitmq:
        rabbitmq.publish(payload=im64_bytes)

def getFrame(sec, frame_id, video_id, vidcap, server):

    vidcap.set(cv2.CAP_PROP_POS_MSEC,sec*1000)
    hasFrames, image = vidcap.read()
    out_frame_path = ''

    if hasFrames:

        out_frame_path = f'images_in/{str(frame_id)}.jpg'
        cv2.imwrite(out_frame_path, image)

        with open(out_frame_path, "rb") as f:
            im_b64 = base64.b64encode(f.read())

        im64_bytes = base64.b64decode(im_b64)
        if out_frame_path:


            json_payload = {
                "video_id": video_id,
                "frame_id": frame_id,
                "frame_link": out_frame_path 
            }
            message = json.dumps(json_payload)
            imageSender(out_frame_path, server, message)

    return hasFrames, out_frame_path


def frame_extractor(video_path):

    server = RabbitmqConfigure(queue='Mediapipe',
                            host='localhost',
                            routingKey='Mediapipe',
                            exchange='')

    files_list = []
    video_id = re.search(r'[^/]+(?=$)', video_path).group(0)
    vidcap = cv2.VideoCapture(video_path)
    sec = 0
    frameRate = 0.5
    frame_id=1
    success, out_frame_path = getFrame(sec, frame_id, video_id, vidcap, server)

    while success:
        frame_id = frame_id + 1
        sec = sec + frameRate
        sec = round(sec, 2)
        success, out_frame_path = getFrame(sec, frame_id, video_id, vidcap, server)
        if out_frame_path:
            files_list.append(out_frame_path)

    return files_list



class MetaClass(type):

    _instance ={}

    def __call__(cls, *args, **kwargs):

        """ Singelton Design Pattern  """

        if cls not in cls._instance:
            cls._instance[cls] = super(MetaClass, cls).__call__(*args, **kwargs)
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
        self._connection = pika.BlockingConnection(pika.ConnectionParameters(host=self.server.host))
        self._channel = self._connection.channel()
        self._channel.queue_declare(queue=self.server.queue)

    def __enter__(self):
        # print("__enter__")
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        # print("__exit__")
        self._connection.close()

    def publish(self, payload ={}):

        """
        :param payload: JSON payload
        :return: None
        """

        self._channel.basic_publish(exchange=self.server.exchange,
                                    routing_key=self.server.routingKey,
                                    body=payload)

        print("Sending Frame")

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

    

    # video_path = 'video/OFTAMOLOGISTA.mp4'
        
    fire.Fire(frame_extractor)

    