using System.Collections.Generic;

/// <summary>EnemyCombatState.DeleteMemory 호출 결과.</summary>
public sealed class MemoryDeletionResult
{
    public bool Success { get; }
    public string MemoryId { get; }
    public IReadOnlyList<string> DeactivatedAttackPatternIds { get; }
    public string FailureReason { get; }

    private MemoryDeletionResult(bool success, string memoryId, IReadOnlyList<string> deactivatedAttackPatternIds, string failureReason)
    {
        Success = success;
        MemoryId = memoryId;
        DeactivatedAttackPatternIds = deactivatedAttackPatternIds;
        FailureReason = failureReason;
    }

    public static MemoryDeletionResult Succeeded(string memoryId, IReadOnlyList<string> deactivatedAttackPatternIds) =>
        new MemoryDeletionResult(true, memoryId, deactivatedAttackPatternIds, failureReason: null);

    public static MemoryDeletionResult Failed(string memoryId, string reason) =>
        new MemoryDeletionResult(false, memoryId, System.Array.Empty<string>(), reason);
}
