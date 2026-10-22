namespace FlowchartExplainer.Models;

/// <summary>
/// Направленная связь (ребро / стрелка) между узлами блок-схемы
/// </summary>
public class FlowchartEdge
{
    public required string Id { get; init; }
    public required string From { get; init; }
    public required string To { get; init; }
    public string? Label { get; init; } // "Да", "Нет" или null
    public List<Waypoint> Waypoints { get; init; } = new();
}
