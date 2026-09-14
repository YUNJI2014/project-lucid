# 13호의 여관 — 작업 규칙

Unity 게임 프로젝트. 기획 전체는 [docs/GDD.md](docs/GDD.md), 아트 규칙은 [docs/art-guide.md](docs/art-guide.md) 참고.
이 문서는 **Claude Code가 이 레포에서 작업할 때 지켜야 하는 규칙**이다. 사람 협업자도 같은 규칙을 따른다.

---

## 1. Git Flow

```
main (배포/마일스톤)  ←  develop (개발 통합)  ←  feature/fix/... (작업 브랜치)
```

- **모든 작업은 `develop`에서 분기**한다. `main`에서 직접 분기하지 않는다.
- **`develop` → `main`은 배포·마일스톤 시점에만** 머지한다. 평소 작업 결과물은 `develop`까지만 간다.
- `main`, `develop`에는 **직접 push 금지**. 반드시 PR을 거친다. (GitHub 브랜치 보호 규칙으로 강제됨 — [5장](#5-github-브랜치-보호) 참고)
- PR 머지에는 **최소 1명의 승인**이 필요하다.
- `develop` 또는 `main`을 대상으로 PR을 올리면 **CodeRabbit**이 자동으로 리뷰를 붙인다. 사람 승인과 별개로 참고할 것.

### 브랜치명

`종류/기능설명` (하이픈으로 구분). 이슈 트래커를 아직 안 써서 `#이슈번호`는 생기면 붙이고, 지금은 생략 가능.

prefix: `feat` `fix` `docs` `style` `refactor` `test` `chore` `hotfix`

예: `feat/room-inspection`, `fix/fragment-slot-bug`

### 커밋 메시지

`prefix: 상세설명` — **한국어로 작성**. PR 제목·본문도 한국어.

예: `feat: 제한된 객실 조사 시스템 추가`

---

## 2. 협업 규칙

- **팀원 코드는 직접 건드리지 않는다.** 본인이 맡은 파트 코드만 추가·수정한다. 다른 사람 코드를 고쳐야 하면 PR 코멘트나 채팅으로 먼저 상의한다.
- **`main`/`develop`으로의 머지·푸시는 사용자(윤지) 승인 없이 하지 않는다.** Claude는 feature 브랜치에서 작업하고, 커밋 메시지는 제안하되 공용 브랜치 반영은 승인 후 진행한다.
- 되돌릴 일이 있으면 `git reset --hard` 대신 **`git revert`**(비파괴)를 쓴다.
- `*.meta`는 **절대 gitignore하지 않는다.** 빠지면 씬/프리팹/UI 참조가 다른 사람 화면에서 전부 깨진다.
- `Edit > Project Settings > Editor` → Asset Serialization **Force Text**, Version Control **Visible Meta Files** 확인.

---

## 3. 개발 환경 확정 사항

| 항목 | 값 |
|---|---|
| Unity 버전 | **6000.3.23f1 (Unity 6.3 LTS)** |
| Color Space | **Linear** |
| Active Input Handling | **Input System Package (New)** — `com.unity.inputsystem` |
| 렌더 파이프라인 | 미정 (빌트인 상태로 시작, GDD TODO 참고) |

위 값들은 `ProjectSettings/`에 커밋돼 있어서 레포를 받으면 자동으로 적용된다. **임의로 바꾸지 말고, 바꿔야 하면 PR로 올려서 팀 합의를 받는다.** (에디터에서 실수로 바꾸면 `ProjectSettings.asset` diff로 올라오니 리뷰에서 걸러낸다.)

특히 입력 코드는 구 Input Manager(`Input.GetKeyDown` 등)가 아니라 새 Input System 기준으로 짠다.

---

## 4. 하네스 엔지니어링 — Claude가 스스로 검증하는 방법

이 프로젝트는 로직 검증 가능성을 아키텍처 단계에서부터 챙긴다.

### 4.1 원칙 — 순수 로직은 MonoBehaviour에서 분리

**기억 삭제/정화 판정, 안식·용기 파편 스킬 효과, 퍼즐/단서 조사 상태, 전투 공격 패턴-트리거 상태** 같은 상태 전이가 있는 로직은
`MonoBehaviour`에 직접 넣지 말고 **순수 C# 클래스**로 분리해서 짠다.

- MonoBehaviour에 다 넣으면 Claude가 **컴파일 여부밖에 확인 못 한다.**
- 순수 C# 클래스로 분리하면 **EditMode 테스트로 동작을 100% 검증**하면서 작업할 수 있다.
- 특히 "기억 A 삭제 → 패턴 B 사라짐 → 파편 B 획득" 같은 다단계 상태 전이는 테스트 없이 가면 반드시 꼬인다.
- 실시간 전투라 해도 **판정 자체(스턴 여부, 패턴 활성/비활성, 파편 지급)는 프레임 단위 물리/애니메이션과 분리**해서 로직만 따로 테스트 가능하게 짠다.

### 4.2 검증 고리 (Claude가 스스로 확인)

- **컴파일 체크** — Unity batchmode CLI로 컴파일 에러를 직접 읽고 고친다. 에디터를 켜지 않아도 된다.
- **EditMode 테스트** — Unity Test Framework를 CLI로 실행해서 순수 로직 클래스를 검증한다 (`com.unity.test-framework` 설치 완료).
- 위 둘은 코드를 수정한 뒤 커밋하기 전에 실행하는 것을 기본으로 한다.

### 4.3 가드레일 (되돌릴 수 없는 일 전에 멈춤)

- `.meta` 파일을 gitignore에 추가하는 변경은 절대 하지 않는다.
- `git push --force`, 브랜치 삭제, `gh repo delete`는 사용하지 않는다.
- Collaborator 초대/삭제, 레포 설정(Public/Private, 브랜치 보호 규칙 등) 변경 전에는 반드시 먼저 확인받는다.
이 가드레일들은 문구로만 있는 게 아니라 `.claude/settings.json`으로 실제 강제된다 — CLAUDE.md는 Claude가 "잊으면" 뚫리지만, deny/hook 규칙은 잊어도 안 뚫린다.

| 계층 | 대상 | 동작 |
|---|---|---|
| `permissions.deny` | `git push --force`, `git push -f`, `git push --delete`, `git branch -D`, `gh repo delete/archive` | 실행 자체가 차단됨 |
| `permissions.ask` | `gh api`의 PUT/POST/PATCH/DELETE, `gh repo edit`, `gh pr merge` | 매번 사람에게 확인을 받음 |
| `PreToolUse` 훅 | 위 위험 패턴을 명령어 어느 위치에서든 탐지 | 차단 (deny 규칙은 앞부분만 매칭돼서 `git push origin develop --force` 같은 형태를 놓치는데, 훅이 이걸 잡는다) |

> 단, 이건 **Claude Code 세션에만** 적용된다. 사람이 터미널에서 직접 `git push --force`를 치는 건 못 막는다 — 그건 [5장](#5-github-브랜치-보호)의 GitHub 브랜치 보호가 막는다.

### 4.4 하네스로 못 덮는 것

게임 손맛, 아트 톤, 밸런스, 실시간 전투의 타격감 같은 건 테스트로 검증할 수 없다.
사람이 직접 플레이해서 판단해야 한다 — 여기 욕심내서 자동화하려 들지 않는다.

---

## 5. GitHub 브랜치 보호

`main`, `develop` 모두 다음 규칙이 걸려 있다 (2026-09-04 설정):

- PR 없이 직접 push 불가
- PR 머지에 승인 1명 이상 필요
- Force-push 금지, 브랜치 삭제 금지
- `enforce_admins` 켜짐 — 레포 관리자(윤지)도 예외 없이 PR을 거쳐야 한다

규칙 자체를 바꾸는 것은 레포 관리자 권한이 필요한 작업이라, Claude가 임의로 수정하지 않는다.
