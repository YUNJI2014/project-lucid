using NUnit.Framework;

public class SkillFragmentCatalogTests
{
    [Test]
    public void 최종보스_파편은_안식2_용기2로_구성된다()
    {
        var fragments = SkillFragmentCatalog.FinalBossFragments;

        Assert.AreEqual(4, fragments.Count);
        Assert.AreEqual(2, System.Linq.Enumerable.Count(fragments, f => f.Type == FragmentType.Repose));
        Assert.AreEqual(2, System.Linq.Enumerable.Count(fragments, f => f.Type == FragmentType.Courage));
    }
}
