#!/bin/sh
# 執行 k6
k6 run -e TESTING_API=http://host.docker.internal:8081/upload/raw-streaming /scripts/file-upload-raw-stream-test.js
#                     ^^^^^^^^^^^^^^^^^^^^^^^
#                     · Windows/macOS Docker Desktop → host.docker.internal
#                     · Linux Docker → 可直接用 --network host ，改成 http://localhost:5000
