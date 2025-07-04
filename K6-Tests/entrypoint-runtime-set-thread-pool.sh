#!/bin/bash

k6 run -e TESTING_API=http://host.docker.internal:8080/upload/fromform /K6-Tests/file-upload-test.js
#                     ^^^^^^^^^^^^^^^^^^^^^^^
#                     · Windows/macOS Docker Desktop → host.docker.internal
#                     · Linux Docker → 可直接用 --network host ，改成 http://localhost:5000
