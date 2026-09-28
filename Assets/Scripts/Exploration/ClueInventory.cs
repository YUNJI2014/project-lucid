using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 획득한 단서를 보관한다. (GDD 5장 · FR-302 · FR-206)
/// 같은 단서를 두 번 주워도 한 번만 들어간다 — 같은 오브젝트를 다시 조사하는 경우가 있기 때문이다.
/// </summary>
public sealed class ClueInventory
{
    private readonly Dictionary<string, Clue> _clues = new Dictionary<string, Clue>();

    public int Count => _clues.Count;

    /// <summary>정화 해금 개수에 포함되는 핵심 단서의 수. (FR-305)</summary>
    public int KeyClueCount => _clues.Values.Count(c => c.IsKey);

    public IEnumerable<Clue> All => _clues.Values;

    /// <summary>전투에서 제시할 수 있는 증거물. (FR-304)</summary>
    public IEnumerable<Clue> Presentable => _clues.Values.Where(c => c.CanPresent);

    /// <summary>
    /// 단서를 획득한다.
    /// </summary>
    /// <returns>처음 얻은 단서면 true, 이미 갖고 있던 것이면 false.
    /// 획득 연출을 한 번만 띄우려면 이 값으로 판단한다 (FR-302).</returns>
    public bool Add(Clue clue)
    {
        if (clue == null) throw new ArgumentNullException(nameof(clue));

        if (_clues.ContainsKey(clue.Id))
            return false;

        _clues.Add(clue.Id, clue);
        return true;
    }

    public bool Has(string clueId) =>
        !string.IsNullOrEmpty(clueId) && _clues.ContainsKey(clueId);

    public Clue Get(string clueId)
    {
        if (string.IsNullOrEmpty(clueId)) return null;
        _clues.TryGetValue(clueId, out var clue);
        return clue;
    }

    /// <summary>이 단서를 제시할 수 있는지. 갖고 있고, 제시 가능한 것이어야 한다.</summary>
    public bool CanPresent(string clueId) => Get(clueId)?.CanPresent == true;

    /// <summary>탐색으로 미리 알아낸 공격 패턴 id 목록. (FR-303)</summary>
    public IEnumerable<string> RevealedPatternIds =>
        _clues.Values
            .Where(c => c.RevealsPatternId != null)
            .Select(c => c.RevealsPatternId)
            .Distinct();

    /// <summary>해당 공격 패턴을 전투 전에 미리 알고 있는지. (FR-303)</summary>
    public bool KnowsPattern(string patternId) =>
        !string.IsNullOrEmpty(patternId) &&
        _clues.Values.Any(c => c.RevealsPatternId == patternId);
}
