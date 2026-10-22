namespace FlowchartExplainer.Models;

/// <summary>
/// Результат проверки самостоятельной задачи пользователем
/// </summary>
public class PracticeValidationResult
{
    public bool IsSuccess { get; init; }
    public required string ActualBranchOrResult { get; init; }
    public required string Explanation { get; init; }
    public string? Hint { get; init; }
    public Dictionary<string, VariableValue> FinalVariables { get; init; } = new();
}

/// <summary>
/// Описание задачи для самостоятельного решения (Тип 2: подбор входных данных)
/// </summary>
public class PracticeTask
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string TargetCondition { get; init; }
    public required string FlowchartId { get; init; }
    public List<InputDefinition> Inputs { get; init; } = new();
    
    /// <summary>
    /// Функция проверки параметров, подобранных пользователем
    /// </summary>
    public required Func<IReadOnlyDictionary<string, VariableValue>, PracticeValidationResult> ValidateInputs { get; init; }
}
