FROM python:3

RUN apt update && apt install -y build-essential python3-opencv && apt clean

WORKDIR /mediapipe

COPY . .

RUN python -m pip install --upgrade pip
RUN python -m pip install -r requirements.txt

CMD ["make", "start"]
