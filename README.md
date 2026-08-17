# project-lucid

여관 주인이 손님의 **악몽 속으로 들어가 그 악몽을 치료**하는 게임. (Unity)

- 현실 = 여관 운영 **2D**
- 악몽 = 탐색/전투 **3D** (탐색은 1인칭)

## 문서

- [기획 정리 (GDD)](docs/GDD.md) — 코어 루프, 서사, 몹 목록, 보스 기믹
- [아트 작업 체크리스트](docs/art-guide.md) — 프롬프트 일관성, 복선 심기 규칙

## 개발 환경

- Unity `(버전 미정)`
- 렌더 파이프라인 `(미정)`

## 주의

- `*.meta` 는 **절대 gitignore 하지 않는다.** 빼면 씬/프리팹/UI 참조가 다른 사람 화면에서 전부 깨진다.
- Unity Editor 설정: `Edit > Project Settings > Editor` → Asset Serialization을 **Force Text**, Version Control을 **Visible Meta Files** 로 둘 것.
