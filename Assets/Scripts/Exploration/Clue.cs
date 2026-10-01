using System;

/// <summary>
/// 탐색에서 발견하는 단서 하나. (GDD 5장 · FR-302~305)
///
/// 단서는 스토리 아이템이 아니라 전투에 직접 영향을 준다. 하나의 단서가 아래 역할을 겹쳐서 가질 수 있다.
///
/// - <see cref="IsKey"/>        핵심 단서. 정해진 개수를 모으면 정화가 해금된다 (FR-305)
/// - <see cref="CanPresent"/>   전투 중 증거물로 제시할 수 있다 (FR-304 · FR-407)
/// - <see cref="RevealsPatternId"/>  이 단서를 얻으면 해당 공격 패턴을 전투 전에 미리 알 수 있다 (FR-303)
///
/// 세 역할을 따로 둔 타입으로 나누지 않은 이유는, 기획상 같은 물건이 여러 역할을 동시에 하기 때문이다.
/// 예를 들어 DAY 3 의 "사생팬 일기"는 핵심 단서이면서 전투에서 제시할 증거물이기도 하다.
/// </summary>
public sealed class Clue
{
    public string Id { get; }
    public string DisplayName { get; }
    public string Description { get; }

    /// <summary>정화 해금 개수에 포함되는 핵심 단서인지. (FR-305)</summary>
    public bool IsKey { get; }

    /// <summary>전투 중 증거물로 제시할 수 있는지. (FR-304)</summary>
    public bool CanPresent { get; }

    /// <summary>이 단서가 미리 알려주는 공격 패턴 id. 없으면 null. (FR-303)</summary>
    public string RevealsPatternId { get; }

    public Clue(
        string id,
        string displayName,
        string description = null,
        bool isKey = false,
        bool canPresent = false,
        string revealsPatternId = null)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("단서 id는 비어 있을 수 없습니다.", nameof(id));

        Id = id;
        DisplayName = displayName;
        Description = description;
        IsKey = isKey;
        CanPresent = canPresent;
        RevealsPatternId = string.IsNullOrEmpty(revealsPatternId) ? null : revealsPatternId;
    }
}
