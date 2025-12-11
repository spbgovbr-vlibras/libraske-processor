FROM python:3.7-slim

RUN apt update && apt install -y --no-install-recommends \
    build-essential python3-opencv \
    && apt clean && rm -rf /var/lib/apt/lists/*

WORKDIR /mediapipe

COPY requirements.txt .
RUN python -m pip install --upgrade pip
RUN python -m pip install -r requirements.txt

COPY . .

CMD ["make", "start"]
