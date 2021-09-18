from queues import queueconsume
from queues import queuepublisher
from utils import holistic_callback
from utils import holistic_pontuation
from utils import configreader
import json


class Worker:

    def __init__(self):


        self._rabbitcfg = configreader.load_configs("RabbitMQ")
        self.workercfg = configreader.load_configs("Worker")

        queueConf = self.workercfg.get("ReceiveQueue", "frame_receiver")
        hostConf = self._rabbitcfg.get("Host", "localhost")
        portConf = self._rabbitcfg.get("Port", "5672")
        userConf = self._rabbitcfg.get("Username", "Guest")
        pwdConf = self._rabbitcfg.get("Password", "Guest")
        print ("Queue, Host, Port, User, PWD = " + queueConf + " " + hostConf+ " " + portConf+ " " + userConf+ " " + pwdConf)


        self.__publisherconfigure = queuepublisher.RabbitmqConfigure(queue=queueConf,
                                                                     host=hostConf,
                                                                     port=portConf,
                                                                     user=userConf,
                                                                     password=pwdConf,
                                                                     routingKey=queueConf,
                                                                     exchange='')

        self.__consumerconfigure = queueconsume.RabbitMqServerConfigure(queue=queueConf,
                                                                        host=hostConf,
                                                                        port=portConf,
                                                                        user=userConf,
                                                                        password=pwdConf,
                                                                        persistent=True
                                                                        )

        self.__consumer = queueconsume.RabbitmqServer(self.__consumerconfigure)

        self.__score_eval = holistic_pontuation.GetScore()
        self.__holistic_process = holistic_callback.HolisticCallback.test_process

    def __publisher_message(self, msg):

        with queuepublisher.RabbitMq(self.__publisherconfigure) as rabbitmq:
            rabbitmq.publish(payload=msg)

    def __callback(self, _ch, _method, _properties, body):

        results, video_id, frame_id, session_id = self.__holistic_process(body)

        score = self.__score_eval.get_score(
            results, video_id, frame_id)

        msg = {
            "idSession": session_id,
            "score":score
        }

        # print(msg)

        payload = json.dumps(msg)

        self.__publisher_message(payload)

    def start(self, queue):

        self.__consumer.startserver(queue, self.__callback)


if __name__ == "__main__":

    workercfg = configreader.load_configs("Worker")
    
    worker = Worker()
    worker.start(workercfg.get("ReceiveQueue"))
