using System;
using System.Collections.Generic;

/// <summary>
/// DAY1 잔소리 공포 — 잔소리 한 줄이 증거 제시를 통해 단계적으로 칭찬으로 바뀌는 정화 연출. (GDD 13장)
/// 예: "왜 이것밖에 못 해?" → "정말 이것밖에 못 한 걸까?" → "그래도 해낸 것도 있었어"
/// 각 단계 전환은 올바른 증거 제시 1회에 대응한다. 마지막 단계에 도달하면 이 잔소리와 연결된
/// 공격 패턴이 정화된 것으로 간주한다 (연결은 상위 레벨의 EnemyCombatState 쪽에서 처리).
/// </summary>
public sealed class NaggingLineTransformation
{
    private readonly IReadOnlyList<string> _stages;

    public string LinkedMemoryId { get; }
    public IReadOnlyList<string> Stages => _stages;
    public int CurrentStageIndex { get; private set; }
    public string CurrentText => _stages[CurrentStageIndex];
    public bool IsFullyTransformed => CurrentStageIndex == _stages.Count - 1;

    public NaggingLineTransformation(string linkedMemoryId, IReadOnlyList<string> stages)
    {
        if (stages == null || stages.Count < 2)
            throw new ArgumentException("변환 단계는 최소 2단계(원본→변환) 이상이어야 합니다.", nameof(stages));

        LinkedMemoryId = linkedMemoryId;
        _stages = stages;
        CurrentStageIndex = 0;
    }

    /// <summary>증거물을 제시해 다음 단계로 넘어간다. 이미 끝까지 변환됐으면 아무 일도 일어나지 않고 false 를 반환한다.</summary>
    public bool Advance()
    {
        if (IsFullyTransformed)
            return false;

        CurrentStageIndex++;
        return true;
    }
}
