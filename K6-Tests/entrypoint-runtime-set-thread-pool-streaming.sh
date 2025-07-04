#!/bin/sh
# 執行 k6
k6 run -e TESTING_API=http://host.docker.internal:8080/upload/streaming /scripts/file-upload-test.js
