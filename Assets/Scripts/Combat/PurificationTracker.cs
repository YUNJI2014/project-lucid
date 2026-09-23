using System;

/// <summary>
/// 한 손님의 정화 진행 상태를 관리한다. 정화는 두 단계를 거친다. (FR-305 · FR-410 · FR-503 · FR-504)
///
/// 1단계 — 탐색에서 핵심 단서를 정해진 만큼 확보하면 Unlock() 으로 "정화 가능 상태"가 열린다.
/// 2단계 — 전투에서 증거물 제시와 지정된 패턴 회피로 게이지를 채운다. 최대치에 도달하면 정화할 수 있다.
///
/// 게이지는 누적 수치이고 절대 감소하지 않는다. 열리지 않았거나 잠긴 상태에서는 오르지 않는다.
/// 기억을 하나라도 삭제하면 Lock() 이 걸려 되돌릴 수 없다 — 이 판정은 EnemyCombatState 가 대신 호출한다.
/// </summary>
public sealed class PurificationTracker
{
    public int MaxGauge { get; }
    public int Gauge { get; private set; }

    /// <summary>탐색 단서로 정화 가능 상태가 열렸는지. (FR-305)</summary>
    public bool IsUnlocked { get; private set; }

    /// <summary>기억 삭제로 정화가 잠겼는지. 한 번 잠기면 해제되지 않는다. (FR-504)</summary>
    public bool IsLocked { get; private set; }

    /// <summary>게이지를 채울 수 있는 상태인지.</summary>
    public bool CanFillGauge => IsUnlocked && !IsLocked;

    /// <summary>게이지가 최대치에 도달해 정화를 실행할 수 있는지. (FR-503)</summary>
    public bool IsPurificationReady => CanFillGauge && Gauge >= MaxGauge;

    public PurificationTracker(int maxGauge)
    {
        if (maxGauge <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxGauge), "정화 게이지 최대치는 1 이상이어야 합니다.");

        MaxGauge = maxGauge;
        Gauge = 0;
        IsUnlocked = false;
        IsLocked = false;
    }

    /// <summary>
    /// 핵심 단서를 충분히 확보해 정화 가능 상태를 연다. 이미 잠긴 뒤에는 열어도 게이지를 채울 수 없다.
    /// </summary>
    public void Unlock()
    {
        IsUnlocked = true;
    }

    /// <summary>
    /// 기억 삭제로 정화를 잠근다. 되돌릴 수 없다. EnemyCombatState.DeleteMemory 가 호출한다.
    /// </summary>
    public void Lock()
    {
        IsLocked = true;
    }

    /// <summary>
    /// 증거물 제시나 지정된 패턴 회피로 게이지를 올린다.
    /// 채울 수 없는 상태면 아무 일도 하지 않고 0 을 반환한다. 최대치를 넘기면 최대치에서 멈춘다.
    /// </summary>
    /// <returns>실제로 오른 양.</returns>
    public int AddGauge(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "정화 게이지는 감소하지 않습니다.");

        if (!CanFillGauge || amount == 0)
            return 0;

        var before = Gauge;
        Gauge = Math.Min(MaxGauge, Gauge + amount);
        return Gauge - before;
    }
}
