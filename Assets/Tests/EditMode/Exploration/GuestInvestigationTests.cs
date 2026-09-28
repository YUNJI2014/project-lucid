using NUnit.Framework;

public class GuestInvestigationTests
{
    private static Clue Key(string id) => new Clue(id, id, isKey: true);
    private static Clue Plain(string id) => new Clue(id, id);

    private static GuestInvestigation Investigation(int required = 2, int maxGauge = 100) =>
        new GuestInvestigation(required, new PurificationTracker(maxGauge));

    [Test]
    public void 핵심_단서가_모자라면_정화가_해금되지_않는다()
    {
        var inv = Investigation(required: 2);

        inv.CollectClue(Key("diary"));

        Assert.IsFalse(inv.HasEnoughKeyClues);
        Assert.IsFalse(inv.Purification.IsUnlocked);
        Assert.AreEqual(1, inv.RemainingKeyClues);
    }

    [Test]
    public void 핵심_단서를_다_모으면_그_자리에서_정화가_해금된다()
    {
        var inv = Investigation(required: 2);

        inv.CollectClue(Key("diary"));
        inv.CollectClue(Key("letter"));

        Assert.IsTrue(inv.HasEnoughKeyClues);
        Assert.IsTrue(inv.Purification.IsUnlocked);
        Assert.AreEqual(0, inv.RemainingKeyClues);
    }

    [Test]
    public void 핵심이_아닌_단서는_아무리_모아도_해금되지_않는다()
    {
        var inv = Investigation(required: 2);

        inv.CollectClue(Plain("poster"));
        inv.CollectClue(Plain("mug"));
        inv.CollectClue(Plain("chair"));

        Assert.IsFalse(inv.Purification.IsUnlocked);
    }

    [Test]
    public void 같은_핵심_단서를_두_번_주워도_해금되지_않는다()
    {
        var inv = Investigation(required: 2);

        Assert.IsTrue(inv.CollectClue(Key("diary")));
        Assert.IsFalse(inv.CollectClue(Key("diary")));

        Assert.AreEqual(1, inv.Clues.KeyClueCount);
        Assert.IsFalse(inv.Purification.IsUnlocked);
    }

    [Test]
    public void 해금된_뒤에_단서를_더_주워도_문제없다()
    {
        var inv = Investigation(required: 1);
        inv.CollectClue(Key("diary"));

        inv.CollectClue(Key("letter"));

        Assert.IsTrue(inv.Purification.IsUnlocked);
        Assert.AreEqual(0, inv.RemainingKeyClues);
    }

    [Test]
    public void 탐색이_연_정화를_전투에서_채울_수_있다()
    {
        var inv = Investigation(required: 1, maxGauge: 10);
        inv.CollectClue(Key("diary"));

        var added = inv.Purification.AddGauge(10);

        Assert.AreEqual(10, added);
        Assert.IsTrue(inv.Purification.IsPurificationReady);
    }

    [Test]
    public void 전투에서_기억을_삭제하면_탐색으로_연_정화가_잠긴다()
    {
        var inv = Investigation(required: 1, maxGauge: 10);
        inv.CollectClue(Key("diary"));

        // 탐색과 전투가 같은 트래커를 공유한다 — 이 접점이 이 테스트의 핵심이다
        var combat = new EnemyCombatState(
            new[] { new TraumaMemory("m1", "기억", new[] { "p1" }) },
            new[] { new AttackPattern("p1", "패턴") },
            inv.Purification);

        combat.DeleteMemory("m1");

        Assert.IsTrue(inv.Purification.IsLocked);
        Assert.AreEqual(0, inv.Purification.AddGauge(10));
        Assert.IsFalse(inv.Purification.IsPurificationReady);
    }
}
