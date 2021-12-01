#FROM python:3.6-slim-stretch
FROM python:3

RUN apt update

RUN apt-get install build-essential -y

COPY . /mediapipe/

WORKDIR /mediapipe/

RUN make install

CMD make start
