using NUnit.Framework;

public class NaggingLineTransformationTests
{
    private static readonly string[] Stages =
    {
        "왜 이것밖에 못 해?",
        "정말 이것밖에 못 한 걸까?",
        "그래도 해낸 것도 있었어",
    };

    [Test]
    public void 증거를_제시할때마다_한_단계씩_변환된다()
    {
        var line = new NaggingLineTransformation("mem.nagging_grades", Stages);

        Assert.AreEqual("왜 이것밖에 못 해?", line.CurrentText);

        line.Advance();
        Assert.AreEqual("정말 이것밖에 못 한 걸까?", line.CurrentText);
        Assert.IsFalse(line.IsFullyTransformed);

        line.Advance();
        Assert.AreEqual("그래도 해낸 것도 있었어", line.CurrentText);
        Assert.IsTrue(line.IsFullyTransformed);
    }

    [Test]
    public void 끝까지_변환된_후에는_더_진행되지_않는다()
    {
        var line = new NaggingLineTransformation("mem.nagging_grades", Stages);
        line.Advance();
        line.Advance();

        var advanced = line.Advance();

        Assert.IsFalse(advanced);
        Assert.AreEqual("그래도 해낸 것도 있었어", line.CurrentText);
    }
}
