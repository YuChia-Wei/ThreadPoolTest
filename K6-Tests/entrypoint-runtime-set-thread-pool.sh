#!/bin/sh
# 1) 設定頻寬
tc qdisc add dev eth0 root tbf rate 256kbit burst 32kbit latency 400ms

# 2) 執行 k6
k6 run -e TESTING_API=http://host.docker.internal:8080/upload/fromform /scripts/file-upload-test.js
