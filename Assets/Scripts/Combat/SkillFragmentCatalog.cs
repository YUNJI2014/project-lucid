using System.Collections.Generic;

/// <summary>
/// 노션 기획서 11장에 확정된 초기 파편 4종.
/// 최종보스전에서는 이 4개만 장착 가능하며, 설명 시점이 3인칭에서 1인칭으로 바뀐다(연출 텍스트는 UI 쪽 담당).
/// </summary>
public static class SkillFragmentCatalog
{
    public const string CuttingId = "repose.cutting";            // 절단
    public const string StopId = "repose.stop";                  // 정지
    public const string CatchBreathId = "courage.catch_breath";  // 숨 고르기
    public const string HoldOnId = "courage.hold_on";            // 붙들기

    public static readonly SkillFragment Cutting = new SkillFragment(
        CuttingId, "절단", FragmentType.Repose,
        "짧은 범위의 강공격, 높은 피해. 끊어내고 싶었던 것이 기억인지 고통인지 알 수 없다.");

    public static readonly SkillFragment Stop = new SkillFragment(
        StopId, "정지", FragmentType.Repose,
        "적을 짧게 경직시키고 패턴을 중단시킨다. 시간을 되돌리고 싶었던 것인지 멈추고 싶었던 것인지는 알 수 없다.");

    public static readonly SkillFragment CatchBreath = new SkillFragment(
        CatchBreathId, "숨 고르기", FragmentType.Courage,
        "체력을 소량 회복한다. 누군가가 남긴 흔적처럼 희미하게 따뜻하다.");

    public static readonly SkillFragment HoldOn = new SkillFragment(
        HoldOnId, "붙들기", FragmentType.Courage,
        "체력이 일정 이하일 때 1회 죽지 않고 버틴다. 놓지 않으려 했던 마음이 희미하게 남아 있다.");

    /// <summary>최종보스전에서 사용 가능한 초기 4개 파편.</summary>
    public static readonly IReadOnlyList<SkillFragment> FinalBossFragments = new[]
    {
        Cutting, Stop, CatchBreath, HoldOn,
    };
}
