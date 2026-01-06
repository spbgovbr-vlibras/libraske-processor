FROM python:3.11-slim

ENV PYTHONUNBUFFERED=1 \
    PYTHONDONTWRITEBYTECODE=1 \
    PIP_NO_CACHE_DIR=1

WORKDIR /app

RUN apt-get update \
 && apt-get install -y --no-install-recommends \
      libopencv-dev \
 && rm -rf /var/lib/apt/lists/*

RUN useradd -m appuser
USER appuser

COPY requirements.txt .
RUN pip install --upgrade pip \
 && pip install -r requirements.txt

COPY . .

CMD ["python", "src/worker.py"]
