namespace FlowchartExplainer.Models;

/// <summary>
/// Описание входного параметра алгоритма с ограничениями и значением по умолчанию
/// </summary>
public class InputDefinition
{
    public required string Name { get; init; }
    public required string Label { get; init; }
    public double Min { get; init; }
    public double Max { get; init; }
    public double Step { get; init; } = 1;
    public double DefaultValue { get; init; }
    public required string Description { get; init; }
}
