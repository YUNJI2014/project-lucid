#!/bin/sh
# 커밋 메시지 형식 검사 — CLAUDE.md "커밋 메시지" 규칙을 실제로 강제한다.
#
# 사용법: scripts/check-commit-msg.sh <메시지 파일>
#
# 로컬 훅(.githooks/commit-msg)과 CI(.github/workflows/commit-lint.yml)가
# 같은 스크립트를 쓴다. 규칙이 두 군데로 갈라지지 않게 하기 위함이다.

set -e

MSG_FILE="$1"
[ -z "$MSG_FILE" ] && { echo "사용법: $0 <메시지 파일>" >&2; exit 2; }
[ -f "$MSG_FILE" ] || { echo "파일을 찾을 수 없음: $MSG_FILE" >&2; exit 2; }

PREFIXES='feat|fix|docs|style|refactor|test|chore|hotfix'

# 주석(#)과 빈 줄을 걷어낸 첫 줄이 제목
SUBJECT=$(grep -v '^#' "$MSG_FILE" | sed '/^[[:space:]]*$/d' | head -1)

# 자동 생성 커밋은 형식을 강제하지 않는다
case "$SUBJECT" in
  Merge\ *|Revert\ *|fixup!\ *|squash!\ *) exit 0 ;;
esac

fail() {
  echo ""
  echo "  ✗ 커밋 메시지 규칙 위반"
  echo ""
  echo "    제목: $SUBJECT"
  echo "    이유: $1"
  echo ""
  echo "  형식:  prefix: 무엇을 했는지"
  echo "  prefix: feat fix docs style refactor test chore hotfix"
  echo ""
  echo "  예시:  feat: 제한된 객실 조사 시스템 추가"
  echo "         fix: force push 가드레일이 뚫리던 구멍 수정"
  echo "         docs: 리뷰 반영 — 시점 1인칭 명시, TODO 2건 추가"
  echo ""
  echo "  자세한 규칙은 CLAUDE.md 1장 \"커밋 메시지\" 참고."
  echo ""
  exit 1
}

# 1) prefix: 설명  형식인가
echo "$SUBJECT" | grep -qE "^($PREFIXES): .+" \
  || fail "\"prefix: 설명\" 형식이 아니다. prefix 뒤에는 콜론과 공백이 와야 한다."

DESC=$(echo "$SUBJECT" | sed -E "s/^($PREFIXES): //")

# 2) 제목 끝 마침표
case "$DESC" in
  *.) fail "제목 끝에 마침표를 찍지 않는다." ;;
esac

# 3) 설명이 너무 짧음 (한글 2자 = 6바이트)
DESC_BYTES=$(printf '%s' "$DESC" | wc -c | tr -d ' ')
[ "$DESC_BYTES" -ge 6 ] \
  || fail "설명이 너무 짧다. 무엇을 했는지 알아볼 수 있게 쓴다."

# 4) 한글 자음/모음만으로 된 토큰 (ㅇㅈㄹ, ㅋㅋㅋ 등)
#    UTF-8 바이트로 판정한다. grep -P 가 없는 환경(Git Bash 등)에서도 동작하게 하기 위함.
JAMO=$(printf '\xe3\x84[\xb1-\xbf]|\xe3\x85[\x80-\xbf]|\xe3\x86[\x80-\x8e]')
for TOKEN in $DESC; do
  STRIPPED=$(printf '%s' "$TOKEN" | LC_ALL=C sed -E "s/($JAMO)//g")
  [ -n "$STRIPPED" ] \
    || fail "자음·모음만 쓴 말(\"$TOKEN\")은 쓰지 않는다."
done

# 5) 제목이 너무 길면 경고만 한다 (막지는 않음). 한글 50자 ≈ 150바이트
SUBJ_BYTES=$(printf '%s' "$SUBJECT" | wc -c | tr -d ' ')
if [ "$SUBJ_BYTES" -gt 150 ]; then
  echo "  ! 제목이 깁니다(${SUBJ_BYTES}바이트). 50자 내외로 줄이고 자세한 내용은 본문에 적는 걸 권장합니다." >&2
fi

exit 0
