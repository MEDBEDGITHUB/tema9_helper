namespace FlowchartExplainer.Models;

/// <summary>
/// Полное определение блок-схемы алгоритма
/// </summary>
public class FlowchartDefinition
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string TypeCategory { get; init; } // "intro" | "main" | "special"
    public required string Badge { get; init; }
    public required string Description { get; init; }
    public required string Goal { get; init; }
    public List<InputDefinition> Inputs { get; init; } = new();
    public List<FlowchartNode> Nodes { get; init; } = new();
    public List<FlowchartEdge> Edges { get; init; } = new();
    public double ViewBoxWidth { get; init; }
    public double ViewBoxHeight { get; init; }
}
