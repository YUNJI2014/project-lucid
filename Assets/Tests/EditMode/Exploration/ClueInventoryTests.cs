using System.Linq;
using NUnit.Framework;

public class ClueInventoryTests
{
    private static Clue Key(string id) => new Clue(id, id, isKey: true);
    private static Clue Evidence(string id) => new Clue(id, id, canPresent: true);
    private static Clue Reveals(string id, string patternId) =>
        new Clue(id, id, revealsPatternId: patternId);

    [Test]
    public void 처음_얻은_단서는_true_를_돌려준다()
    {
        var inv = new ClueInventory();

        Assert.IsTrue(inv.Add(Key("diary")));
        Assert.AreEqual(1, inv.Count);
    }

    [Test]
    public void 같은_단서를_다시_주우면_false_이고_개수도_그대로다()
    {
        var inv = new ClueInventory();
        inv.Add(Key("diary"));

        Assert.IsFalse(inv.Add(Key("diary")));
        Assert.AreEqual(1, inv.Count);
    }

    [Test]
    public void 핵심_단서만_세어진다()
    {
        var inv = new ClueInventory();
        inv.Add(Key("diary"));
        inv.Add(Key("letter"));
        inv.Add(Evidence("photo"));

        Assert.AreEqual(3, inv.Count);
        Assert.AreEqual(2, inv.KeyClueCount);
    }

    [Test]
    public void 제시_가능한_단서만_증거물로_나온다()
    {
        var inv = new ClueInventory();
        inv.Add(Key("diary"));
        inv.Add(Evidence("photo"));

        CollectionAssert.AreEquivalent(
            new[] { "photo" },
            inv.Presentable.Select(c => c.Id).ToList());
    }

    [Test]
    public void 갖고_있지_않은_단서는_제시할_수_없다()
    {
        var inv = new ClueInventory();

        Assert.IsFalse(inv.CanPresent("photo"));
    }

    [Test]
    public void 갖고_있어도_제시_불가_단서면_제시할_수_없다()
    {
        var inv = new ClueInventory();
        inv.Add(Key("diary"));

        Assert.IsTrue(inv.Has("diary"));
        Assert.IsFalse(inv.CanPresent("diary"));
    }

    [Test]
    public void 단서로_공격_패턴을_미리_알_수_있다()
    {
        var inv = new ClueInventory();
        inv.Add(Reveals("valve-note", "water-column"));

        Assert.IsTrue(inv.KnowsPattern("water-column"));
        Assert.IsFalse(inv.KnowsPattern("floor-flood"));
    }

    [Test]
    public void 같은_패턴을_알려주는_단서가_둘이어도_한_번만_나온다()
    {
        var inv = new ClueInventory();
        inv.Add(Reveals("note-a", "water-column"));
        inv.Add(Reveals("note-b", "water-column"));

        Assert.AreEqual(1, inv.RevealedPatternIds.Count());
    }
}
