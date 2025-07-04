# 專案說明

這個專案旨在比較 **.NET 預設執行緒集區 (Thread Pool) 設定**與**手動提高最小執行緒數**在高併發檔案上傳場景下的效能表現。

## 專案結構

- **`ThreadsPoolTest.DotnetControl`**: Web API 專案，作為**對照組**。此專案使用 .NET 預設的執行緒集區設定，執行緒會根據負載動態調整。
- **`ThreadsPoolTest.SetMinThreadsPool`**: Web API 專案，作為**實驗組**。此專案在啟動時透過 `ThreadPool.SetMinThreads(128, 128)` 明確設定了較高的最小執行緒數，旨在避免高併發時動態建立執行緒的延遲。
- **`ThreadsPoolTest.UseCases`**: 包含兩個 API 專案共用的核心業務邏輯 (例如檔案儲存服務)，確保了實驗的公平性。
- **`ThreadsPoolTest.CrossCutting.Observability`**: 提供基於 `System.Diagnostics.ActivitySource` 的可觀測性功能，方便使用 `dotnet-counters` 等工具進行深入的效能監控與分析。
- **`K6-Tests`**: 包含 `k6` 負載測試腳本 (`file-upload-test.js`)，用於模擬高併發的檔案上傳請求，以產生足夠的壓力來凸顯兩種設定下的效能差異。

## 核心假設

本專案的核心假設是：在高併發的 I/O 密集型操作 (如檔案上傳) 中，預先設定較高的最小執行緒數 (`SetMinThreadsPool` 專案) 將會比使用 .NET 預設設定 (`DotnetControl` 專案) 帶來更佳的效能，例如更低的回應延遲和更高的請求吞吐量。

# 實驗驗證方法

此測試方法旨在驗證上述核心假設。透過 A/B 測試，我們能比較兩種設定下的效能差異。

## 測試前準備

1.  **建立測試檔案**: 確保在 `K6-Tests` 資料夾中已有名為 `sample.pdf` 的測試檔案。
2.  **隔離環境**: 為了得到最純淨的數據，請關閉所有不必要的應用程式 (包含 Visual Studio IDE)。建議直接透過 `dotnet run` 指令來啟動 API 服務。

## 測試執行步驟

請**嚴格分開**執行對照組與實驗組的測試，切勿同時啟動兩個 API 專案。

### A. 測試對照組 (DotnetControl)

1.  **啟動 API**:
    ```shell
    cd ThreadsPoolTest.DotnetControl
    dotnet run
    ```
2.  **取得 Process ID**: 開啟新的終端機，使用 `dotnet-counters ps` 找到 `ThreadsPoolTest.DotnetControl` 的程序 ID (PID)。
3.  **啟動效能監控**:
    ```shell
    dotnet-counters monitor --process-id <PID> -o control-metrics.csv --counters System.Runtime,Microsoft.AspNetCore.Hosting
    ```
4.  **執行負載測試**: 開啟第三個終端機，進入 `K6-Tests` 資料夾，執行 k6 腳本。
    ```shell
    cd K6-Tests
    # 注意: k6 腳本中的 URL (http://host.docker.internal:5036/upload/fromform) 可能需要根據你執行的 API port 進行調整
    docker run --rm -v $PWD:/scripts -w /scripts grafana/k6 run file-upload-test.js
    ```
5.  **結束測試**: k6 執行完畢後，停止 `dotnet-counters` 與 `dotnet run`。

### B. 測試實驗組 (SetMinThreadsPool)

1.  **啟動 API**:
    ```shell
    cd ThreadsPoolTest.SetMinThreadsPool
    dotnet run
    ```
2.  **重複步驟 A.2 ~ A.5**: 取得新的 PID，將監控數據儲存到不同的檔案 (例如 `experiment-metrics.csv`)，並執行完全相同的 k6 測試。

## 如何分析與驗證結果

比較兩次測試的數據，是驗證假設的關鍵。

### 1. k6 測試結果分析

- **`http_req_duration` (請求延遲)**: 這是最重要的效能指標。比較兩份 k6 報告中的 `p(95)` (95% 的請求延遲) 和 `avg` (平均延遲)。
- **`http_req_failed` (請求失敗率)**: 檢查 `DotnetControl` 是否有因為來不及處理請求而導致失敗率顯著高於 `SetMinThreadsPool`。

### 2. dotnet-counters 數據分析

- **`thread-pool-thread-count` (執行緒計數)**: 這是**最直接的證據**。比較 `control-metrics.csv` 和 `experiment-metrics.csv` 中的此項數據。
- **`thread-pool-queue-length` (執行緒佇列長度)**: 如果 `DotnetControl` 的佇列長度在測試期間持續高於 `SetMinThreadsPool`，代表請求發生了積壓。
- **`requests-per-second` (RPS)**: 比較 ASP.NET Core 層實際處理的 RPS，驗證系統的吞吐量。

## 預期結果

如果實驗成功，你應該會觀察到以下現象：

- **`DotnetControl` (對照組)**:
  - **k6**: 在測試初期 (第一分鐘)，`http_req_duration` 會明顯偏高，之後可能趨於平穩。
  - **dotnet-counters**: `thread-pool-thread-count` 會從一個較低的值 (例如 CPU 核心數) 開始，隨著壓力增加而緩慢爬升。`thread-pool-queue-length` 在初期可能會出現一個明顯的峰值。

- **`SetMinThreadsPool` (實驗組)**:
  - **k6**: `http_req_duration` 從測試開始就應維持在一個相對較低且穩定的水準。
  - **dotnet-counters**: `thread-pool-thread-count` 從一開始就應維持在 128 或更高的水準。`thread-pool-queue-length` 應始終保持在非常低的值 (接近 0)。


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

for windows
```shell
docker run --rm --cap-add NET_ADMIN -v .\:/scripts -w /scripts --entrypoint /scripts/entrypoint.sh grafana/k6
```

# Observability

> ThreadsPoolTest.CrossCutting.Observability

此專案為 AOP 元件，提供 `System.Diagnostics.ActivitySource` 資源，讓專案擁有直接提供觀測資料的能力。
