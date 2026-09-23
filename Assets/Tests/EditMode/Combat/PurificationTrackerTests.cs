using NUnit.Framework;

public class PurificationTrackerTests
{
    private static PurificationTracker UnlockedTracker(int max = 100)
    {
        var tracker = new PurificationTracker(max);
        tracker.Unlock();
        return tracker;
    }

    [Test]
    public void 열리지_않은_상태에서는_게이지가_오르지_않는다()
    {
        var tracker = new PurificationTracker(100);

        var added = tracker.AddGauge(30);

        Assert.AreEqual(0, added);
        Assert.AreEqual(0, tracker.Gauge);
        Assert.IsFalse(tracker.CanFillGauge);
    }

    [Test]
    public void 단서로_열린_뒤에는_게이지가_누적된다()
    {
        var tracker = UnlockedTracker();

        tracker.AddGauge(30);
        tracker.AddGauge(20);

        Assert.AreEqual(50, tracker.Gauge);
        Assert.IsFalse(tracker.IsPurificationReady);
    }

    [Test]
    public void 게이지가_최대치에_도달하면_정화할_수_있다()
    {
        var tracker = UnlockedTracker(50);

        tracker.AddGauge(50);

        Assert.IsTrue(tracker.IsPurificationReady);
    }

    [Test]
    public void 최대치를_넘겨도_최대치에서_멈춘다()
    {
        var tracker = UnlockedTracker(50);

        var added = tracker.AddGauge(80);

        Assert.AreEqual(50, tracker.Gauge);
        Assert.AreEqual(50, added, "실제로 오른 양만 반환해야 한다");
    }

    [Test]
    public void 게이지는_감소하지_않는다()
    {
        var tracker = UnlockedTracker();
        tracker.AddGauge(40);

        Assert.Throws<System.ArgumentOutOfRangeException>(() => tracker.AddGauge(-10));
        Assert.AreEqual(40, tracker.Gauge);
    }

    [Test]
    public void 잠긴_뒤에는_게이지가_오르지_않는다()
    {
        var tracker = UnlockedTracker();
        tracker.AddGauge(40);

        tracker.Lock();
        var added = tracker.AddGauge(30);

        Assert.AreEqual(0, added);
        Assert.AreEqual(40, tracker.Gauge, "이미 쌓인 게이지는 남지만 더 오르지 않는다");
        Assert.IsFalse(tracker.IsPurificationReady);
    }

    [Test]
    public void 잠김은_다시_열어도_풀리지_않는다()
    {
        var tracker = UnlockedTracker();
        tracker.Lock();

        tracker.Unlock();

        Assert.IsTrue(tracker.IsLocked);
        Assert.IsFalse(tracker.CanFillGauge);
    }

    [Test]
    public void 최대치가_가득_찬_뒤_잠기면_정화할_수_없다()
    {
        var tracker = UnlockedTracker(50);
        tracker.AddGauge(50);

        tracker.Lock();

        Assert.IsFalse(tracker.IsPurificationReady);
    }
}
