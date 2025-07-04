#!/bin/sh
# 1) 設定頻寬
tc qdisc add dev eth0 root tbf rate 256kbit burst 32kbit latency 400ms

# 2) 執行 k6
k6 run -e TESTING_API=http://host.docker.internal:8081/upload/fromform /scripts/file-upload-test.js
#                     ^^^^^^^^^^^^^^^^^^^^^^^
#                     · Windows/macOS Docker Desktop → host.docker.internal
#                     · Linux Docker → 可直接用 --network host ，改成 http://localhost:5000
