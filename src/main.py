from queues import queueconsume
from queues import queuepublisher
from utils import holistic_callback
from utils import holistic_pontuation


class Worker:

    def __init__(self):

        self.__publisherconfigure = queuepublisher.RabbitmqConfigure(queue='Mediapipe_out',
                                                                     host='localhost',
                                                                     routingKey='Mediapipe_out',
                                                                     exchange='')

        self.__consumerconfigure = queueconsume.RabbitMqServerConfigure(host='localhost',
                                                                        queue='Mediapipe')

        self.__consumer = queueconsume.RabbitmqServer(self.__consumerconfigure)

        self.__score_eval = holistic_pontuation.GetScore()
        self.__holistic_process = holistic_callback.HolisticCallback.test_process

    def __publisher_message(self, msg):

        with queuepublisher.RabbitMq(self.__publisherconfigure) as rabbitmq:
            rabbitmq.publish(payload=msg)

    def __callback(self, _ch, _method, _properties, body):

        results, video_id, frame_id = self.__holistic_process(body)

        msg = self.__score_eval.get_score(
            results, video_id, frame_id)

        self.__publisher_message(msg)

    def start(self, queue):

        self.__consumer.startserver(queue, self.__callback)


if __name__ == "__main__":

    worker = Worker()
    worker.start("Mediapipe")
