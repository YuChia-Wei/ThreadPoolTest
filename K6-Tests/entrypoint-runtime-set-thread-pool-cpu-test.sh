#!/bin/sh
# 執行 k6
k6 run -e TESTING_API=http://host.docker.internal:8080/api/cpu-intensive-work?iterations=100 /scripts/cpu-intensive-work-test.js
