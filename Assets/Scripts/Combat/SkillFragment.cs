/// <summary>
/// 안식/용기 파편 스킬 하나. 구체적인 수치 효과는 GDD 22장 TODO("안식/용기 파편 각각의 구체 스킬 목록과 수치")
/// 확정 전이라, 여기서는 식별 정보와 슬롯 장착 가능 여부 판정에 필요한 정보만 다룬다.
/// </summary>
public sealed class SkillFragment
{
    public string Id { get; }
    public string DisplayName { get; }
    public FragmentType Type { get; }
    public string Description { get; }

    public SkillFragment(string id, string displayName, FragmentType type, string description)
    {
        Id = id;
        DisplayName = displayName;
        Type = type;
        Description = description;
    }
}
