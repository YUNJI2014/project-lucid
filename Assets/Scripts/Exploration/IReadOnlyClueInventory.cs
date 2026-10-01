using System.Collections.Generic;

/// <summary>
/// 단서 보관함의 조회 전용 창구. 넣는 기능(<see cref="ClueInventory.Add"/>)이 빠져 있다.
///
/// <see cref="GuestInvestigation.Clues"/> 가 이 타입으로 노출되는 이유 —
/// 단서를 넣는 길이 <see cref="GuestInvestigation.CollectClue"/> 하나뿐이어야
/// 정화 해금을 빠뜨리는 경로가 안 생긴다. 보관함을 통째로 내주면
/// <c>investigation.Clues.Add(clue)</c> 로 해금을 건너뛸 수 있다.
/// </summary>
public interface IReadOnlyClueInventory
{
    int Count { get; }

    /// <summary>정화 해금 개수에 포함되는 핵심 단서의 수. (FR-305)</summary>
    int KeyClueCount { get; }

    IEnumerable<Clue> All { get; }

    /// <summary>전투에서 제시할 수 있는 증거물. (FR-304)</summary>
    IEnumerable<Clue> Presentable { get; }

    bool Has(string clueId);

    Clue Get(string clueId);

    /// <summary>이 단서를 제시할 수 있는지. 갖고 있고, 제시 가능한 것이어야 한다.</summary>
    bool CanPresent(string clueId);

    /// <summary>탐색으로 미리 알아낸 공격 패턴 id 목록. (FR-303)</summary>
    IEnumerable<string> RevealedPatternIds { get; }

    /// <summary>해당 공격 패턴을 전투 전에 미리 알고 있는지. (FR-303)</summary>
    bool KnowsPattern(string patternId);
}
