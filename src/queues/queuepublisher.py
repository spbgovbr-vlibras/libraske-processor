import pika


class MetaClass(type):

    _instance = {}

    def __call__(cls, *args, **kwargs):

        if cls not in cls._instance:
            cls._instance[cls] = super(
                MetaClass, cls).__call__(*args, **kwargs)
            return cls._instance[cls]


class RabbitmqConfigure(metaclass=MetaClass):

    def __init__(self, queue='hello', host='localhost', port="5672", user="Guest", password="Guest", routingKey='hello', exchange=''):

        self.queue = queue
        self.host = host
        self.port = port
        self.user = user
        self.password = password
        self.routingKey = routingKey
        self.exchange = exchange


class RabbitMq():

    __slots__ = ["server", "_parameters" , "_channel", "_connection"]

    def __init__(self, server):

        self.server = server
        cred = pika.PlainCredentials(self.server.user, self.server.password)
        self._parameters = pika.ConnectionParameters(host=self.server.host, port=self.server.port, credentials=cred)
        self._connection = pika.BlockingConnection(self._parameters)
        self._channel = self._connection.channel()
        self._channel.queue_declare(queue=self.server.queue)

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        self._connection.close()

    def publish(self, payload={}):

        self._channel.basic_publish(exchange=self.server.exchange,
                                    routing_key=self.server.routingKey,
                                    body=payload)

        # print("Sending Frame")
