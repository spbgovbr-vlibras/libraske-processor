FROM python:3.11-slim

ENV PYTHONUNBUFFERED=1 \
    PYTHONDONTWRITEBYTECODE=1 \
    PIP_NO_CACHE_DIR=1

WORKDIR /app

RUN echo "deb http://deb.debian.org/debian sid main" >> /etc/apt/sources.list && \
    apt-get update && \
    apt-get install -y --no-install-recommends \
    openssl/unstable \
    libssl3/unstable \
    libgl1 libglib2.0-0 make && \
    rm -rf /var/lib/apt/lists/*

RUN useradd -m appuser
USER appuser

COPY requirements.txt .
RUN pip install --upgrade pip \
 && pip install -r requirements.txt

COPY . .

CMD ["make", "start"]
