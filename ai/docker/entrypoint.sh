#!/bin/bash

set -e

if [[ "${MODE}" == "worker" ]]; then
  exec celery -A celery_app worker --loglevel info --pool=solo

else
  exec uvicorn main:app --host 0.0.0.0 --port 8081
fi
