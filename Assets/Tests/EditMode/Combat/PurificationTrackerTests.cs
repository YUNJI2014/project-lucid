using NUnit.Framework;

public class PurificationTrackerTests
{
    [Test]
    public void 필요한_증거를_모두_제출하면_정화가_해금된다()
    {
        var tracker = new PurificationTracker(new[] { "evidence.diary", "evidence.receipt" });

        tracker.SubmitEvidence("evidence.diary");
        tracker.SubmitEvidence("evidence.receipt");

        Assert.IsTrue(tracker.IsPurificationUnlocked);
        Assert.IsEmpty(tracker.MissingEvidenceIds);
    }

    [Test]
    public void 일부만_제출하면_아직_해금되지_않는다()
    {
        var tracker = new PurificationTracker(new[] { "evidence.diary", "evidence.receipt" });

        tracker.SubmitEvidence("evidence.diary");

        Assert.IsFalse(tracker.IsPurificationUnlocked);
        CollectionAssert.AreEquivalent(new[] { "evidence.receipt" }, tracker.MissingEvidenceIds);
    }

    [Test]
    public void 조건과_무관한_증거를_제출하면_무시된다()
    {
        var tracker = new PurificationTracker(new[] { "evidence.diary" });

        var accepted = tracker.SubmitEvidence("evidence.unrelated");

        Assert.IsFalse(accepted);
        Assert.IsFalse(tracker.IsPurificationUnlocked);
    }
}
