import pika


class MetaClass(type):

    _instance = {}

    def __call__(cls, *args, **kwargs):
        """ Singelton Design Pattern  """

        if cls not in cls._instance:
            cls._instance[cls] = super(
                MetaClass, cls).__call__(*args, **kwargs)
            return cls._instance[cls]


class RabbitMqServerConfigure(metaclass=MetaClass):

    def __init__(self, host='localhost', port="5672", user="Guest" , password="Guest", queue='hello', persistent=True):
        """ Server initialization   """

        self.host = host
        self.port = port
        self.queue = queue
        self.user = user
        self.password = password
        self.persistent = persistent

class RabbitmqServer():

    def __init__(self, server):
        """
        :param server: Object of class RabbitMqServerConfigure
        """
        self.server = server
        cred = pika.PlainCredentials(self.server.user, self.server.password)
        self._parameters = pika.ConnectionParameters(host=self.server.host, port=self.server.port, credentials=cred)
        self._connection = pika.BlockingConnection(self._parameters)
        self._channel = self._connection.channel()
#        self._tem = self._channel.queue_declare(queue=self.server.queue, arguments={"persistent":True}, durable=True)
        self._tem = self._channel.queue_declare(queue=self.server.queue, arguments={"persistent":True}, durable=False)
        print("Server started waiting for Messages!🚀")

    def startserver(self, queue, callback):
        self._channel.basic_consume(
            queue=queue,
            on_message_callback=callback,
            auto_ack=True)
        self._channel.start_consuming()
