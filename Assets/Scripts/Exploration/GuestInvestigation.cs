using System;

/// <summary>
/// 한 손님의 탐색 진행 상태. 단서 보관과 정화 해금을 함께 들고 있다. (GDD 5·8장 · FR-305)
///
/// 핵심 단서가 필요 개수에 도달하면 <see cref="PurificationTracker.Unlock"/> 을 여기서 직접 호출한다.
/// 해금 판정을 호출자에게 맡기면 "단서는 다 모았는데 해금을 안 한" 경로가 생기기 때문이다.
/// 전투 쪽에서 <c>EnemyCombatState</c> 가 기억 삭제 시 Lock() 을 직접 부르는 것과 같은 이유다.
///
/// 탐색(이 클래스)과 전투(EnemyCombatState)가 <b>같은 PurificationTracker 인스턴스를 공유</b>한다.
/// 탐색이 열고, 전투가 채우거나 잠근다. 그 트래커가 두 파트의 유일한 접점이다.
/// </summary>
public sealed class GuestInvestigation
{
    /// <summary>정화 해금에 필요한 핵심 단서 개수. (FR-305)</summary>
    public int RequiredKeyClues { get; }

    private readonly ClueInventory _clues = new ClueInventory();

    /// <summary>
    /// 모은 단서. <b>조회 전용</b>이다 — 단서를 넣는 길은 <see cref="CollectClue"/> 하나뿐이다.
    /// 보관함을 그대로 내주면 <c>Clues.Add(clue)</c> 로 정화 해금을 건너뛸 수 있다.
    /// </summary>
    public IReadOnlyClueInventory Clues => _clues;

    /// <summary>탐색과 전투가 공유하는 정화 상태.</summary>
    public PurificationTracker Purification { get; }

    public GuestInvestigation(int requiredKeyClues, PurificationTracker purification)
    {
        if (requiredKeyClues <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(requiredKeyClues), "정화 해금에 필요한 핵심 단서는 1개 이상이어야 합니다.");

        RequiredKeyClues = requiredKeyClues;
        Purification = purification ?? throw new ArgumentNullException(nameof(purification));
    }

    /// <summary>핵심 단서를 필요한 만큼 모았는지.</summary>
    public bool HasEnoughKeyClues => Clues.KeyClueCount >= RequiredKeyClues;

    /// <summary>정화 해금까지 남은 핵심 단서 수. 다 모았으면 0.</summary>
    public int RemainingKeyClues => Math.Max(0, RequiredKeyClues - Clues.KeyClueCount);

    /// <summary>
    /// 단서를 획득한다. 핵심 단서가 필요 개수를 채우면 그 자리에서 정화를 해금한다.
    /// </summary>
    /// <returns>처음 얻은 단서면 true. 획득 연출 표시 여부 판단에 쓴다 (FR-302).</returns>
    public bool CollectClue(Clue clue)
    {
        var isNew = _clues.Add(clue);

        if (isNew && HasEnoughKeyClues)
            Purification.Unlock();

        return isNew;
    }
}
