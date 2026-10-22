namespace FlowchartExplainer.Models;

public enum QuestionType
{
    Choice,
    NumberInput
}

/// <summary>
/// Интерактивный контрольный вопрос для пользователя на шаге алгоритма
/// </summary>
public class StepQuestion
{
    public required string Id { get; init; }
    public required string Prompt { get; init; }
    public QuestionType Type { get; init; } = QuestionType.Choice;
    public List<string> Options { get; init; } = new();
    
    /// <summary>
    /// Функция вычисления правильного ответа на основе текущих переменных
    /// </summary>
    public required Func<IReadOnlyDictionary<string, VariableValue>, string> GetCorrectAnswer { get; init; }
    
    public required string ExplanationOnCorrect { get; init; }
    public required string ExplanationOnWrong { get; init; }
}
