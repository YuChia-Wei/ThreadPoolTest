import http from 'k6/http';
import { check, sleep } from 'k6';

// 讀取同一層的 sample.pdf（二進位）
const fileBin = open('./sample.pdf', 'b');

// 測試目標：5 分鐘內持續 200 使用者
export const options = {
  vus: 200,
  duration: '5m',
  // 限制每秒請求數
  rps: 50,  // 每秒最多 50 個請求
  // 若要漸增 VU，可改用 stages:
  // stages: [
  //   { duration: '1m', target: 100 },
  //   { duration: '8m', target: 100 },
  //   { duration: '1m', target: 0 },
  // ],
  thresholds: {
    http_req_failed: ['rate<0.01'],      // 失敗率 < 1 %
    http_req_duration: ['p(95)<3000'],   // 95% 請求 < 3 秒（依需求自行調整）
  },
};

export default function () {
  // multipart-form 資料
  const data = {
    file: http.file(fileBin, 'sample.pdf'),  // 欄位名稱 file，與 API 參數一致
  };

  const url = __ENV.TESTING_API || "http://host.docker.internal:5036/upload/fromform";

  const res = http.post(url, data);

  check(res, {
    'HTTP 200': (r) => r.status === 200,
  });
}
