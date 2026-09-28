using NUnit.Framework;

public class SkillLoadoutTests
{
    [Test]
    public void 일반_모드에서는_아무_파편이나_장착할_수_있다()
    {
        var loadout = new SkillLoadout(slotCount: 4);
        var custom = new SkillFragment("repose.custom", "임의 스킬", FragmentType.Repose, "테스트용");

        var equipped = loadout.Equip(0, custom);

        Assert.IsTrue(equipped);
        Assert.AreSame(custom, loadout.GetSlot(0));
    }

    [Test]
    public void 최종보스_모드에서는_초기_4종_파편만_장착할_수_있다()
    {
        var loadout = new SkillLoadout(slotCount: 4, finalBossMode: true);

        var equipped = loadout.Equip(0, SkillFragmentCatalog.Cutting);

        Assert.IsTrue(equipped);
    }

    [Test]
    public void 최종보스_모드에서는_초기_4종_이외_파편은_장착이_거부된다()
    {
        var loadout = new SkillLoadout(slotCount: 4, finalBossMode: true);
        var unlockedLater = new SkillFragment("repose.later_unlock", "나중에 얻는 스킬", FragmentType.Repose, "테스트용");

        var equipped = loadout.Equip(0, unlockedLater);

        Assert.IsFalse(equipped);
        Assert.IsNull(loadout.GetSlot(0));
    }

    [Test]
    public void 슬롯_해제하면_비워진다()
    {
        var loadout = new SkillLoadout(slotCount: 2);
        loadout.Equip(0, SkillFragmentCatalog.Stop);

        loadout.Unequip(0);

        Assert.IsNull(loadout.GetSlot(0));
        Assert.IsEmpty(loadout.EquippedFragments);
    }
}
