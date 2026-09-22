using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 정화 조건(핵심 단서·증거물) 충족 여부를 추적한다. (GDD 8장)
/// 필요한 증거를 모두 제출해야 정화가 가능해진다 — "중요한 단서를 충분히 확보한 경우에만 정화가 가능".
/// </summary>
public sealed class PurificationTracker
{
    private readonly HashSet<string> _requiredEvidenceIds;
    private readonly HashSet<string> _submittedEvidenceIds = new HashSet<string>();

    public PurificationTracker(IEnumerable<string> requiredEvidenceIds)
    {
        if (requiredEvidenceIds == null) throw new ArgumentNullException(nameof(requiredEvidenceIds));

        _requiredEvidenceIds = new HashSet<string>(requiredEvidenceIds);
        if (_requiredEvidenceIds.Count == 0)
            throw new ArgumentException("정화에 필요한 증거물이 최소 1개는 있어야 합니다.", nameof(requiredEvidenceIds));
    }

    public bool IsPurificationUnlocked => _requiredEvidenceIds.All(_submittedEvidenceIds.Contains);

    public IReadOnlyCollection<string> MissingEvidenceIds =>
        _requiredEvidenceIds.Where(id => !_submittedEvidenceIds.Contains(id)).ToList();

    /// <summary>
    /// 증거물을 제시한다. 정화 조건과 무관한 증거를 제시하면 무시하고 false 를 반환한다.
    /// </summary>
    public bool SubmitEvidence(string evidenceId)
    {
        if (!_requiredEvidenceIds.Contains(evidenceId))
            return false;

        _submittedEvidenceIds.Add(evidenceId);
        return true;
    }
}
