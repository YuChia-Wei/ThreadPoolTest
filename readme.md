# test file generate

```shell
dd if=/dev/zero of=sample.pdf bs=1024 count=5120
```

## for mac

```shell
mkfile 5m sample.pdf
```

# dotnet run

> 爲了避免 IDE 開著造成的額外效能損失，可以單獨用 dotnet run 來執行 api

# dotnet counters monitor

```shell
# 可能會需要更新 workload
dotnet workload update
dotnet tool install --global dotnet-counters
# 用 ps 列出 dotnet 程式的處理程序，並找出 process id
dotnet-counters ps
dotnet-counters monitor --process-id <pid> \
   --counters System.Runtime \
   --refresh-interval 3
```

# k6 test command

```shell
cd K6-Tests                       # 回到存有 upload.js 的資料夾
```

> 這個版本沒有掛到測試檔案，只是備份參考而已
```shell
docker run --rm -i grafana/k6 run - < file-upload-test.js
```

```shell
docker run --rm -i \
  -v $PWD:/scripts \                 # 把目前資料夾掛到 /scripts
  -w /scripts \                      # 工作目錄設 /scripts
  --network host \                   # Linux 建議；Win/macOS 可以省略
  grafana/k6 run file-upload-test.js
```

> 實際測試時使用
```shell
docker run --rm -i \
  -v $PWD:/scripts \
  -w /scripts \
  grafana/k6 run file-upload-test.js
```

!!!網路限制的部分待測!!!

> 包含網路限制
> 利用 `--cap-add NET_ADMIN` 參數來讓 container 內可以用 tc
```shell
docker run --rm \
  --cap-add NET_ADMIN \
  -v $PWD:/scripts \
  -w /scripts \
  grafana/k6 \
  sh -c 'tc qdisc add dev eth0 root tbf rate 256kbit burst 32kbit latency 400ms && \
	 k6 run file-upload-test.js'
```

> 包含網路限制，網路限制寫在 entrypoint.sh 中
sh file
```sh
#!/bin/sh
# 1) 設定頻寬
tc qdisc add dev eth0 root tbf rate 256kbit burst 32kbit latency 400ms

# 2) 執行 k6
k6 run /scripts/file-upload-test.js
```

```shell
docker run --rm --cap-add NET_ADMIN \
  -v $PWD:/scripts \
  -w /scripts \
  --entrypoint /scripts/entrypoint.sh \
  grafana/k6

```

# Observability

> ThreadsPoolTest.CrossCutting.Observability

此專案為 AOP 元件，提供 `System.Diagnostics.ActivitySource` 資源，讓專案擁有直接提供觀測資料的能力。