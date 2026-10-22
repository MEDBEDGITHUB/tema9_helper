namespace FlowchartExplainer.Models;

/// <summary>
/// Фиксация изменения переменной на конкретном шаге выполнения.
/// Показывает старое и новое значения, а также подстановку формулы.
/// </summary>
public class VariableChange
{
    public required string Name { get; init; }
    public VariableValue OldValue { get; init; }
    public VariableValue NewValue { get; init; }
    public string? Formula { get; init; }
    public string? Description { get; init; }

    public bool HasChanged => !OldValue.Equals(NewValue);
}
