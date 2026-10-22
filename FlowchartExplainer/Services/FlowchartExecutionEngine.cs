using FlowchartExplainer.Models;

namespace FlowchartExplainer.Services;

/// <summary>
/// Чистый детерминированный движок пошагового выполнения блок-схемы
/// </summary>
public class FlowchartExecutionEngine
{
    /// <summary>
    /// Инициализация первого шага (узел «Начало»)
    /// </summary>
    public FlowchartStepSnapshot Initialize(FlowchartDefinition flowchart, IReadOnlyDictionary<string, double> userInputs)
    {
        var startNode = flowchart.Nodes.FirstOrDefault(n => n.Type == NodeType.Start)
            ?? throw new InvalidOperationException($"В блок-схеме '{flowchart.Id}' не найден узел Start.");

        var initialVars = new Dictionary<string, VariableValue>();
        foreach (var inp in flowchart.Inputs)
        {
            var val = userInputs.TryGetValue(inp.Name, out var userVal) ? userVal : inp.DefaultValue;
            initialVars[inp.Name] = val;
        }

        var startResult = startNode.ExecuteFunc?.Invoke(initialVars);

        var questionState = startNode.Question != null
            ? new ActiveQuestionState
            {
                NodeId = startNode.Id,
                Question = startNode.Question
            }
            : null;

        return new FlowchartStepSnapshot
        {
            StepIndex = 1,
            CurrentNodeId = startNode.Id,
            VisitedNodeIds = new List<string> { startNode.Id },
            VisitedEdgeIds = new List<string>(),
            Variables = new Dictionary<string, VariableValue>(initialVars),
            RecentChanges = new List<VariableChange>(),
            Explanation = startResult?.Explanation ?? "Старт алгоритма. Начало выполнения.",
            FormulaApplied = startResult?.FormulaApplied,
            QuestionState = questionState,
            IsCompleted = false
        };
    }

    /// <summary>
    /// Переход к следующему шагу алгоритма (ровно 1 узел за шаг)
    /// </summary>
    public FlowchartStepSnapshot ExecuteNextStep(FlowchartDefinition flowchart, FlowchartStepSnapshot current)
    {
        if (current.IsCompleted)
            return current;

        var currentNode = flowchart.Nodes.FirstOrDefault(n => n.Id == current.CurrentNodeId)
            ?? throw new InvalidOperationException($"Узел '{current.CurrentNodeId}' не найден.");

        // 1. Вычисляем действие текущего узла для определения следующего целевого узла
        var currentResult = currentNode.ExecuteFunc?.Invoke(current.Variables)
            ?? new StepExecutionResult { Explanation = "Переход...", NextNodeId = currentNode.NextNodeId };

        var nextNodeId = currentResult.NextNodeId;
        if (string.IsNullOrEmpty(nextNodeId) || currentNode.Type == NodeType.End)
        {
            // Завершение алгоритма
            return new FlowchartStepSnapshot
            {
                StepIndex = current.StepIndex,
                CurrentNodeId = current.CurrentNodeId,
                VisitedNodeIds = new List<string>(current.VisitedNodeIds),
                VisitedEdgeIds = new List<string>(current.VisitedEdgeIds),
                Variables = new Dictionary<string, VariableValue>(current.Variables),
                RecentChanges = new List<VariableChange>(),
                Explanation = "Алгоритм успешно завершён.",
                FormulaApplied = current.FormulaApplied,
                QuestionState = null,
                IsCompleted = true
            };
        }

        var nextNode = flowchart.Nodes.FirstOrDefault(n => n.Id == nextNodeId)
            ?? throw new InvalidOperationException($"Следующий узел '{nextNodeId}' не найден.");

        // 2. Выполняем действие на новом узле (вычисление формул, присваивание переменных)
        var newVars = new Dictionary<string, VariableValue>(current.Variables);
        var nextResult = nextNode.ExecuteFunc?.Invoke(newVars);

        var finalChanges = new List<VariableChange>();
        if (nextResult?.Changes != null && nextResult.Changes.Count > 0)
        {
            foreach (var ch in nextResult.Changes)
            {
                newVars[ch.Name] = ch.NewValue;
                finalChanges.Add(ch);
            }
        }

        // 3. Фиксируем пройденное ребро и пройденные узлы
        var traversedEdge = flowchart.Edges.FirstOrDefault(e => e.From == currentNode.Id && e.To == nextNode.Id);
        var newVisitedEdges = new List<string>(current.VisitedEdgeIds);
        if (traversedEdge != null && !newVisitedEdges.Contains(traversedEdge.Id))
        {
            newVisitedEdges.Add(traversedEdge.Id);
        }

        var newVisitedNodes = new List<string>(current.VisitedNodeIds);
        if (!newVisitedNodes.Contains(nextNode.Id))
        {
            newVisitedNodes.Add(nextNode.Id);
        }

        // 4. Проверяем вопрос на новом узле
        var questionState = nextNode.Question != null
            ? new ActiveQuestionState
            {
                NodeId = nextNode.Id,
                Question = nextNode.Question
            }
            : null;

        var isCompleted = nextNode.Type == NodeType.End;

        return new FlowchartStepSnapshot
        {
            StepIndex = current.StepIndex + 1,
            CurrentNodeId = nextNode.Id,
            VisitedNodeIds = newVisitedNodes,
            VisitedEdgeIds = newVisitedEdges,
            Variables = newVars,
            RecentChanges = finalChanges,
            Explanation = nextResult?.Explanation ?? $"Переход к блоку «{nextNode.Text}».",
            FormulaApplied = nextResult?.FormulaApplied,
            QuestionState = questionState,
            IsCompleted = isCompleted
        };
    }
}
