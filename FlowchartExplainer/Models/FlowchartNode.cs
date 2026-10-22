namespace FlowchartExplainer.Models;

/// <summary>
/// Узел блок-схемы (блок) с геометрическими параметрами и логикой исполнения
/// </summary>
public class FlowchartNode
{
    public required string Id { get; init; }
    public required NodeType Type { get; init; }
    public required string Text { get; init; }
    public string? Subtext { get; init; }
    
    // Координаты центра и размеры блока для SVG
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    
    // Идентификаторы следующих узлов
    public string? NextNodeId { get; init; }
    public string? NextTrueNodeId { get; init; }
    public string? NextFalseNodeId { get; init; }
    
    // Опциональный интерактивный вопрос на этом шаге
    public StepQuestion? Question { get; init; }
    
    // Делегат выполнения логики на узле
    public Func<IReadOnlyDictionary<string, VariableValue>, StepExecutionResult>? ExecuteFunc { get; init; }
}
