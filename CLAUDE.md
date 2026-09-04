# 13호 악몽 여관 — 작업 규칙

Unity 게임 프로젝트. 기획 전체는 [docs/GDD.md](docs/GDD.md), 아트 규칙은 [docs/art-guide.md](docs/art-guide.md) 참고.
이 문서는 **Claude Code가 이 레포에서 작업할 때 지켜야 하는 규칙**이다. 사람 협업자도 같은 규칙을 따른다.

---

## 1. Git Flow

```
main (배포/마일스톤)  ←  develop (개발 통합)  ←  feature/fix/... (작업 브랜치)
```

- **모든 작업은 `develop`에서 분기**한다. `main`에서 직접 분기하지 않는다.
- **`develop` → `main`은 배포·마일스톤 시점에만** 머지한다. 평소 작업 결과물은 `develop`까지만 간다.
- `main`, `develop`에는 **직접 push 금지**. 반드시 PR을 거친다. (GitHub 브랜치 보호 규칙으로 강제됨 — [4장](#4-github-브랜치-보호) 참고)
- PR 머지에는 **최소 1명의 승인**이 필요하다.

### 브랜치명

`종류/#이슈번호-기능설명` (하이픈으로 구분)

prefix: `feat` `fix` `docs` `style` `refactor` `test` `chore` `hotfix`

예: `feat/#3-room-inspection`, `fix/#7-fragment-slot-bug`

### 커밋 메시지

`prefix: 상세설명 (#이슈번호)` — **영어로 작성**.

예: `feat: add limited room inspection system (#3)`

---

## 2. 협업 규칙

- **팀원 코드는 직접 건드리지 않는다.** 본인이 맡은 파트 코드만 추가·수정한다. 다른 사람 코드를 고쳐야 하면 PR 코멘트나 채팅으로 먼저 상의한다.
- **`main`/`develop`으로의 머지·푸시는 사용자(윤지) 승인 없이 하지 않는다.** Claude는 feature 브랜치에서 작업하고, 커밋 메시지는 제안하되 공용 브랜치 반영은 승인 후 진행한다.
- 되돌릴 일이 있으면 `git reset --hard` 대신 **`git revert`**(비파괴)를 쓴다.
- `*.meta`는 **절대 gitignore하지 않는다.** 빠지면 씬/프리팹/UI 참조가 다른 사람 화면에서 전부 깨진다.
- `Edit > Project Settings > Editor` → Asset Serialization **Force Text**, Version Control **Visible Meta Files** 확인.

---

## 3. 하네스 엔지니어링 — Claude가 스스로 검증하는 방법

이 프로젝트는 로직 검증 가능성을 아키텍처 단계에서부터 챙긴다.

### 3.1 원칙 — 순수 로직은 MonoBehaviour에서 분리

**기억 삭제/정화 판정, 안식·용기 파편 스킬 효과, 여관 수입 계산** 같은 상태 전이가 있는 로직은
`MonoBehaviour`에 직접 넣지 말고 **순수 C# 클래스**로 분리해서 짠다.

- MonoBehaviour에 다 넣으면 Claude가 **컴파일 여부밖에 확인 못 한다.**
- 순수 C# 클래스로 분리하면 **EditMode 테스트로 동작을 100% 검증**하면서 작업할 수 있다.
- 특히 "기억 A 삭제 → 패턴 B·C 사라짐 → 현실 손님 상태 변화" 같은 다단계 상태 전이는 테스트 없이 가면 반드시 꼬인다.

### 3.2 검증 고리 (Claude가 스스로 확인)

- **컴파일 체크** — Unity batchmode CLI로 컴파일 에러를 직접 읽고 고친다. 에디터를 켜지 않아도 된다.
- **EditMode 테스트** — Unity Test Framework를 CLI로 실행해서 순수 로직 클래스를 검증한다.
- 위 둘은 코드를 수정한 뒤 커밋하기 전에 실행하는 것을 기본으로 한다.

### 3.3 가드레일 (되돌릴 수 없는 일 전에 멈춤)

- `.meta` 파일을 gitignore에 추가하는 변경은 절대 하지 않는다.
- `git push --force`, 브랜치 삭제, `gh repo delete`는 사용하지 않는다.
- Collaborator 초대/삭제, 레포 설정(Public/Private, 브랜치 보호 규칙 등) 변경 전에는 반드시 먼저 확인받는다.
- 이 가드레일들은 `.claude/settings.json`의 permission/hook 설정으로도 강제한다 — CLAUDE.md는 Claude가 "잊으면" 뚫리지만, hook/deny 규칙은 잊어도 안 뚫린다.

### 3.4 하네스로 못 덮는 것

게임 손맛, 아트 톤, 밸런스, "악몽이 생생하게 느껴지는가" 같은 건 테스트로 검증할 수 없다.
사람이 직접 플레이해서 판단해야 한다 — 여기 욕심내서 자동화하려 들지 않는다.

---

## 4. GitHub 브랜치 보호

`main`, `develop` 모두 다음 규칙이 걸려 있다 (2026-09-04 설정):

- PR 없이 직접 push 불가
- PR 머지에 승인 1명 이상 필요
- Force-push 금지, 브랜치 삭제 금지

규칙 자체를 바꾸는 것은 레포 관리자 권한이 필요한 작업이라, Claude가 임의로 수정하지 않는다.
