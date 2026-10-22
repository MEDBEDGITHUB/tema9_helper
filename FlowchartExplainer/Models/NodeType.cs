namespace FlowchartExplainer.Models;

/// <summary>
/// Типы узлов блок-схемы по ГОСТ 19.701-90 / ISO 5807:
/// Start/End (Овал), Input/Output (Параллелограмм), Process (Прямоугольник), Condition (Ромб)
/// </summary>
public enum NodeType
{
    Start,
    End,
    Input,
    Process,
    Condition,
    Output
}
