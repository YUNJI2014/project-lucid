using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 손님 한 명과의 전투 상태(공격 패턴, 연결된 기억, 정화 진행)를 관리하는 순수 로직 클래스.
/// MonoBehaviour 는 이 클래스의 인스턴스만 들고 있고, 판정 자체는 전부 여기서 한다.
/// 프레임 단위 물리/애니메이션과 분리해서 EditMode 테스트로 검증하기 위함이다. (CLAUDE.md 4.1)
///
/// 기억 삭제와 정화의 배타성(FR-504)을 호출자가 잊을 수 없도록, PurificationTracker 를 이 클래스가 들고
/// DeleteMemory 안에서 직접 잠근다. 둘을 따로 두면 "기억만 지우고 잠그는 걸 안 한" 경로가 생긴다.
/// </summary>
public sealed class EnemyCombatState
{
    private readonly Dictionary<string, TraumaMemory> _memories;
    private readonly Dictionary<string, AttackPattern> _patterns;

    /// <summary>이 손님의 정화 진행 상태. (FR-305 · FR-410 · FR-503)</summary>
    public PurificationTracker Purification { get; }

    public EnemyCombatState(
        IEnumerable<TraumaMemory> memories,
        IEnumerable<AttackPattern> patterns,
        PurificationTracker purification)
    {
        if (memories == null) throw new ArgumentNullException(nameof(memories));
        if (patterns == null) throw new ArgumentNullException(nameof(patterns));

        _memories = memories.ToDictionary(m => m.Id);
        _patterns = patterns.ToDictionary(p => p.Id);
        Purification = purification ?? throw new ArgumentNullException(nameof(purification));
    }

    public IReadOnlyCollection<AttackPattern> AllPatterns => _patterns.Values;
    public IEnumerable<AttackPattern> ActivePatterns => _patterns.Values.Where(p => p.IsActive);
    public int ActivePatternCount => ActivePatterns.Count();

    /// <summary>삭제된 기억이 하나라도 있는지.</summary>
    public bool HasDeletedAnyMemory => _memories.Values.Any(m => m.IsDeleted);

    /// <summary>
    /// 기억을 삭제한다. 연결된 공격 패턴을 모두 비활성화하고, 그 즉시 정화를 잠근다(FR-504).
    /// 삭제 성공 시 호출자가 안식 파편을 지급한다.
    /// </summary>
    public MemoryDeletionResult DeleteMemory(string memoryId)
    {
        if (!_memories.TryGetValue(memoryId, out var memory))
            return MemoryDeletionResult.Failed(memoryId, "존재하지 않는 기억입니다.");

        if (memory.IsDeleted)
            return MemoryDeletionResult.Failed(memoryId, "이미 삭제된 기억입니다.");

        var deactivated = new List<string>();
        foreach (var patternId in memory.LinkedAttackPatternIds)
        {
            if (_patterns.TryGetValue(patternId, out var pattern) && pattern.IsActive)
            {
                pattern.Deactivate();
                deactivated.Add(patternId);
            }
        }

        memory.MarkDeleted();
        Purification.Lock();

        return MemoryDeletionResult.Succeeded(memoryId, deactivated);
    }
}
