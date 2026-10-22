namespace FlowchartExplainer.Models;

/// <summary>
/// Результат выполнения шага алгоритма на узле блок-схемы
/// </summary>
public class StepExecutionResult
{
    public required string Explanation { get; init; }
    public string? FormulaApplied { get; init; }
    public List<VariableChange> Changes { get; init; } = new();
    public string? NextNodeId { get; init; }
    public bool? ConditionResult { get; init; }
    public string? BranchTaken { get; init; }
}
