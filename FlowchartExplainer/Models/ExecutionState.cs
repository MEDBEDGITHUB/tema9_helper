namespace FlowchartExplainer.Models;

/// <summary>
/// Состояние интерактивного вопроса на текущем шаге
/// </summary>
public class ActiveQuestionState
{
    public required StepQuestion Question { get; init; }
    public required string NodeId { get; init; }
    public string? SelectedAnswer { get; set; }
    public bool? IsCorrect { get; set; }
    public bool IsAnswered => IsCorrect.HasValue;
    public string? FeedbackMessage { get; set; }
}

/// <summary>
/// Неизменяемый снимок состояния трассировки алгоритма на определенном шаге
/// </summary>
public class FlowchartStepSnapshot
{
    public int StepIndex { get; init; }
    public required string CurrentNodeId { get; init; }
    public List<string> VisitedNodeIds { get; init; } = new();
    public List<string> VisitedEdgeIds { get; init; } = new();
    public Dictionary<string, VariableValue> Variables { get; init; } = new();
    public List<VariableChange> RecentChanges { get; init; } = new();
    public required string Explanation { get; init; }
    public string? FormulaApplied { get; init; }
    public ActiveQuestionState? QuestionState { get; init; }
    public bool IsCompleted { get; init; }
}
