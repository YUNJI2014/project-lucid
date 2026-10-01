using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 제한된 슬롯에 파편 스킬을 장착하는 로직. (GDD 9장 — "안식/안식/용기/용기" 같은 빌드 구성)
/// 최종보스전 모드에서는 SkillFragmentCatalog.FinalBossFragments 4종만 장착을 허용한다. (GDD 11장)
/// </summary>
public sealed class SkillLoadout
{
    private readonly SkillFragment[] _slots;

    public int SlotCount => _slots.Length;
    public bool IsFinalBossMode { get; }

    public SkillLoadout(int slotCount, bool finalBossMode = false)
    {
        if (slotCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(slotCount), "슬롯 개수는 1개 이상이어야 합니다.");

        _slots = new SkillFragment[slotCount];
        IsFinalBossMode = finalBossMode;
    }

    public SkillFragment GetSlot(int index)
    {
        ValidateSlotIndex(index);
        return _slots[index];
    }

    public IReadOnlyList<SkillFragment> EquippedFragments =>
        _slots.Where(f => f != null).ToList();

    /// <summary>최종보스 제한에 걸리지 않는 한 장착 가능한지 여부.</summary>
    public bool CanEquip(SkillFragment fragment)
    {
        if (fragment == null) return false;
        if (!IsFinalBossMode) return true;
        return SkillFragmentCatalog.FinalBossFragments.Any(f => f.Id == fragment.Id);
    }

    /// <summary>지정한 슬롯에 파편을 장착한다. 최종보스 제한에 걸리면 장착하지 않고 false 를 반환한다.</summary>
    public bool Equip(int slotIndex, SkillFragment fragment)
    {
        ValidateSlotIndex(slotIndex);
        if (!CanEquip(fragment))
            return false;

        _slots[slotIndex] = fragment;
        return true;
    }

    public void Unequip(int slotIndex)
    {
        ValidateSlotIndex(slotIndex);
        _slots[slotIndex] = null;
    }

    private void ValidateSlotIndex(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Length)
            throw new ArgumentOutOfRangeException(nameof(slotIndex));
    }
}
