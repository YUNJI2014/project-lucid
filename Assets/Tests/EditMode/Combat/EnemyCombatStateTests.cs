using System.Linq;
using NUnit.Framework;

public class EnemyCombatStateTests
{
    private static EnemyCombatState CreateFloodState()
    {
        var memory = new TraumaMemory("mem.flood", "물에 빠졌던 기억", new[] { "pattern.water_wave", "pattern.door_seal" });
        var otherMemory = new TraumaMemory("mem.silence", "아무도 도와주지 않았던 기억", new[] { "pattern.silence" });

        var patterns = new[]
        {
            new AttackPattern("pattern.water_wave", "물결 공격"),
            new AttackPattern("pattern.door_seal", "문 봉쇄"),
            new AttackPattern("pattern.silence", "정적"),
        };

        return new EnemyCombatState(new[] { memory, otherMemory }, patterns);
    }

    [Test]
    public void 기억을_삭제하면_연결된_모든_공격패턴이_비활성화된다()
    {
        var state = CreateFloodState();

        var result = state.DeleteMemory("mem.flood");

        Assert.IsTrue(result.Success);
        CollectionAssert.AreEquivalent(new[] { "pattern.water_wave", "pattern.door_seal" }, result.DeactivatedAttackPatternIds);
        Assert.IsFalse(state.AllPatterns.Single(p => p.Id == "pattern.water_wave").IsActive);
        Assert.IsFalse(state.AllPatterns.Single(p => p.Id == "pattern.door_seal").IsActive);
    }

    [Test]
    public void 연결되지_않은_다른_기억의_공격패턴은_영향받지_않는다()
    {
        var state = CreateFloodState();

        state.DeleteMemory("mem.flood");

        Assert.IsTrue(state.AllPatterns.Single(p => p.Id == "pattern.silence").IsActive);
        Assert.AreEqual(1, state.ActivePatternCount);
    }

    [Test]
    public void 이미_삭제한_기억을_다시_삭제하면_실패한다()
    {
        var state = CreateFloodState();
        state.DeleteMemory("mem.flood");

        var result = state.DeleteMemory("mem.flood");

        Assert.IsFalse(result.Success);
    }

    [Test]
    public void 존재하지_않는_기억을_삭제하면_실패한다()
    {
        var state = CreateFloodState();

        var result = state.DeleteMemory("mem.unknown");

        Assert.IsFalse(result.Success);
        Assert.AreEqual(3, state.ActivePatternCount);
    }
}
