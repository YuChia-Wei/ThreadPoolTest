import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 200,
  duration: '5m',
  // 可以評估移除 rps 限制，讓 200 個 VUs 盡可能地發送請求，以模擬高併發壓力
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
    const url = __ENV.TESTING_API || "http://host.docker.internal:5036/api/cpu-intensive-work?iterations=100";
    const res = http.post(url);
    check(res, {
        'status was 200': (r) => r.status === 200}
    );
}
