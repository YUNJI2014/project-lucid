using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 손님의 트라우마 기억 하나. 하나 이상의 공격 패턴(AttackPattern)과 연결된다. (GDD 6~7장)
/// </summary>
public sealed class TraumaMemory
{
    public string Id { get; }
    public string DisplayName { get; }
    public IReadOnlyList<string> LinkedAttackPatternIds { get; }
    public bool IsDeleted { get; private set; }

    public TraumaMemory(string id, string displayName, IEnumerable<string> linkedAttackPatternIds)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("기억 id는 비어 있을 수 없습니다.", nameof(id));

        Id = id;
        DisplayName = displayName;
        LinkedAttackPatternIds = (linkedAttackPatternIds ?? Enumerable.Empty<string>()).ToList();
    }

    /// <summary>EnemyCombatState.DeleteMemory 를 통해서만 호출되어야 한다.</summary>
    internal void MarkDeleted()
    {
        IsDeleted = true;
    }
}
