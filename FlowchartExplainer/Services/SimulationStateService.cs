using FlowchartExplainer.Models;

namespace FlowchartExplainer.Services;

/// <summary>
/// Реактивный сервис управления состоянием симулятора и пользовательского сеанса
/// </summary>
public class SimulationStateService : IDisposable
{
    private readonly FlowchartExecutionEngine _engine = new();
    private CancellationTokenSource? _autoplayCts;

    public event Action? OnChange;

    public FlowchartDefinition CurrentFlowchart { get; private set; }
    public Dictionary<string, double> UserInputs { get; private set; } = new();
    public List<FlowchartStepSnapshot> History { get; private set; } = new();
    public int CurrentStepIndex { get; private set; } = 0;

    public FlowchartStepSnapshot CurrentSnapshot => History.Count > 0 && CurrentStepIndex < History.Count
        ? History[CurrentStepIndex]
        : _engine.Initialize(CurrentFlowchart, UserInputs);

    public ActiveQuestionState? ActiveQuestion => CurrentSnapshot.QuestionState;
    public bool IsCompleted => CurrentSnapshot.IsCompleted;
    public bool CanStepForward => !IsCompleted && (ActiveQuestion == null || ActiveQuestion.IsAnswered);
    public bool CanStepBackward => CurrentStepIndex > 0;
    public bool IsPlaying { get; private set; } = false;
    public int PlaySpeedMs { get; set; } = 1500;

    // Режим работы: "explore" | "practice"
    public string ActiveTab { get; private set; } = "main";
    public bool IsPracticeMode { get; private set; } = false;

    // Состояние самостоятельной задачи
    public PracticeTask CurrentPracticeTask { get; } = FlowchartRepository.MainPracticeTask;
    public Dictionary<string, double> PracticeInputs { get; private set; } = new();
    public PracticeValidationResult? PracticeResult { get; private set; }

    public SimulationStateService()
    {
        CurrentFlowchart = FlowchartRepository.MainFlowchart;
        InitFlowchart(CurrentFlowchart);
        InitPractice();
    }

    private void InitPractice()
    {
        PracticeInputs.Clear();
        foreach (var inp in CurrentPracticeTask.Inputs)
        {
            PracticeInputs[inp.Name] = inp.DefaultValue;
        }
        PracticeResult = null;
    }

    public void SelectFlowchart(string flowchartId)
    {
        StopAutoplay();
        var fc = FlowchartRepository.GetById(flowchartId) ?? FlowchartRepository.MainFlowchart;
        CurrentFlowchart = fc;
        ActiveTab = fc.Id;
        IsPracticeMode = false;
        InitFlowchart(fc);
        NotifyStateChanged();
    }

    public void SwitchToPractice()
    {
        StopAutoplay();
        IsPracticeMode = true;
        ActiveTab = "practice";
        NotifyStateChanged();
    }

    private void InitFlowchart(FlowchartDefinition flowchart)
    {
        UserInputs.Clear();
        foreach (var input in flowchart.Inputs)
        {
            UserInputs[input.Name] = input.DefaultValue;
        }
        ResetSimulation();
    }

    public void SetInput(string name, double value)
    {
        var inputDef = CurrentFlowchart.Inputs.FirstOrDefault(i => i.Name == name);
        if (inputDef != null)
        {
            var clamped = Math.Clamp(value, inputDef.Min, inputDef.Max);
            UserInputs[name] = clamped;
            ResetSimulation();
            NotifyStateChanged();
        }
    }

    public void ResetSimulation()
    {
        StopAutoplay();
        History.Clear();
        var initialSnapshot = _engine.Initialize(CurrentFlowchart, UserInputs);
        History.Add(initialSnapshot);
        CurrentStepIndex = 0;
        NotifyStateChanged();
    }

    public void StepForward()
    {
        if (ActiveQuestion != null && !ActiveQuestion.IsAnswered)
        {
            // Нельзя идти дальше, пока пользователь не ответил на вопрос
            return;
        }

        if (CurrentStepIndex < History.Count - 1)
        {
            CurrentStepIndex++;
            NotifyStateChanged();
            return;
        }

        if (!CurrentSnapshot.IsCompleted)
        {
            var nextSnapshot = _engine.ExecuteNextStep(CurrentFlowchart, CurrentSnapshot);
            History.Add(nextSnapshot);
            CurrentStepIndex++;
            NotifyStateChanged();
        }
        else
        {
            StopAutoplay();
        }
    }

    public void StepBackward()
    {
        StopAutoplay();
        if (CurrentStepIndex > 0)
        {
            CurrentStepIndex--;
            NotifyStateChanged();
        }
    }

    public void AnswerQuestion(string answer)
    {
        if (ActiveQuestion == null) return;

        var q = ActiveQuestion.Question;
        var correctAnswer = q.GetCorrectAnswer(CurrentSnapshot.Variables);
        var isCorrect = string.Equals(answer.Trim(), correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase);

        ActiveQuestion.SelectedAnswer = answer;
        ActiveQuestion.IsCorrect = isCorrect;
        ActiveQuestion.FeedbackMessage = isCorrect ? q.ExplanationOnCorrect : q.ExplanationOnWrong;

        NotifyStateChanged();
    }

    public void ToggleAutoplay()
    {
        if (IsPlaying)
        {
            StopAutoplay();
        }
        else
        {
            StartAutoplay();
        }
    }

    public void StartAutoplay()
    {
        if (IsCompleted)
        {
            ResetSimulation();
        }

        IsPlaying = true;
        _autoplayCts?.Cancel();
        _autoplayCts = new CancellationTokenSource();
        var token = _autoplayCts.Token;

        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested && IsPlaying)
            {
                try
                {
                    await Task.Delay(PlaySpeedMs, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                if (token.IsCancellationRequested || !IsPlaying) break;

                if (!CanStepForward)
                {
                    StopAutoplay();
                    break;
                }

                StepForward();
            }
        }, token);

        NotifyStateChanged();
    }

    public void StopAutoplay()
    {
        if (IsPlaying)
        {
            IsPlaying = false;
            _autoplayCts?.Cancel();
            _autoplayCts = null;
            NotifyStateChanged();
        }
    }

    public void SetPracticeInput(string name, double value)
    {
        var def = CurrentPracticeTask.Inputs.FirstOrDefault(i => i.Name == name);
        if (def != null)
        {
            PracticeInputs[name] = Math.Clamp(value, def.Min, def.Max);
            PracticeResult = null;
            NotifyStateChanged();
        }
    }

    public void CheckPracticeTask()
    {
        var inputVars = PracticeInputs.ToDictionary(k => k.Key, v => (VariableValue)v.Value);
        PracticeResult = CurrentPracticeTask.ValidateInputs(inputVars);
        NotifyStateChanged();
    }

    public void ResetPracticeTask()
    {
        InitPractice();
        NotifyStateChanged();
    }

    public void NotifyStateChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        StopAutoplay();
    }
}
