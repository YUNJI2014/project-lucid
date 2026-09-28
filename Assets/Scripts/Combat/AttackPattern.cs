using System;

/// <summary>
/// 손님의 공격 패턴 하나. 특정 트라우마 기억(TraumaMemory)과 연결되어 있으며,
/// 연결된 기억이 삭제되면 전투에서 사라진다. (GDD 6장 — "물에 빠졌던 기억 삭제 → 물 관련 공격 패턴 삭제")
/// </summary>
public sealed class AttackPattern
{
    public string Id { get; }
    public string DisplayName { get; }
    public bool IsActive { get; private set; }

    public AttackPattern(string id, string displayName)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("공격 패턴 id는 비어 있을 수 없습니다.", nameof(id));

        Id = id;
        DisplayName = displayName;
        IsActive = true;
    }

    /// <summary>
    /// 이 패턴을 전투에서 제거한다. EnemyCombatState.DeleteMemory 를 통해서만 호출되어야 한다.
    /// </summary>
    internal void Deactivate()
    {
        IsActive = false;
    }
}
