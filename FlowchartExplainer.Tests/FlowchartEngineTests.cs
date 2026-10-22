using FlowchartExplainer.Models;
using FlowchartExplainer.Services;
using Xunit;

namespace FlowchartExplainer.Tests;

public class FlowchartEngineTests
{
    private readonly FlowchartExecutionEngine _engine = new();

    [Fact]
    public void Test1_MainFlowchart_StandardDelivery_CalculatesCorrectly()
    {
        // 1. Обычные данные: price = 120, qty = 10
        // total = 1200 > 1000 -> скидка 20% (240 руб.) -> final_sum = 960 < 1200 -> Стандартная доставка
        var fc = FlowchartRepository.MainFlowchart;
        var inputs = new Dictionary<string, double> { ["price"] = 120, ["qty"] = 10 };

        var state = _engine.Initialize(fc, inputs);
        Assert.Equal("m_start", state.CurrentNodeId);

        while (!state.IsCompleted)
        {
            state = _engine.ExecuteNextStep(fc, state);
        }

        Assert.Equal("m_end", state.CurrentNodeId);
        Assert.Equal(1200, state.Variables["total"].ToDouble());
        Assert.Equal(240, state.Variables["disc"].ToDouble());
        Assert.Equal(960, state.Variables["final_sum"].ToDouble());
        Assert.Equal("Стандартная доставка", state.Variables["delivery"].AsString);
    }

    [Fact]
    public void Test2_MainFlowchart_VipDelivery_CalculatesCorrectly()
    {
        // 2. Ветвь VIP: price = 200, qty = 10
        // total = 2000 > 1000 -> скидка 20% (400 руб.) -> final_sum = 1600 >= 1200 -> VIP доставка
        var fc = FlowchartRepository.MainFlowchart;
        var inputs = new Dictionary<string, double> { ["price"] = 200, ["qty"] = 10 };

        var state = _engine.Initialize(fc, inputs);
        while (!state.IsCompleted)
        {
            state = _engine.ExecuteNextStep(fc, state);
        }

        Assert.Equal("m_end", state.CurrentNodeId);
        Assert.Equal(2000, state.Variables["total"].ToDouble());
        Assert.Equal(400, state.Variables["disc"].ToDouble());
        Assert.Equal(1600, state.Variables["final_sum"].ToDouble());
        Assert.Equal("VIP (Бесплатно)", state.Variables["delivery"].AsString);
    }

    [Fact]
    public void Test3_SpecialFlowchart_ZeroDivisionGuard_ExecutesSafely()
    {
        // 3. Особый случай: b = 0 -> срабатывает защита от деления на 0, ветвь "Да"
        var fc = FlowchartRepository.SpecialFlowchart;
        var inputs = new Dictionary<string, double> { ["a"] = 42, ["b"] = 0 };

        var state = _engine.Initialize(fc, inputs);
        while (!state.IsCompleted)
        {
            state = _engine.ExecuteNextStep(fc, state);
        }

        Assert.Contains("s_err_div_zero", state.VisitedNodeIds);
        Assert.DoesNotContain("s_calc_div", state.VisitedNodeIds);
        Assert.Equal("Ошибка: деление на ноль!", state.Variables["result"].AsString);
    }

    [Fact]
    public void Test4_PracticeTask_Validation_SuccessAndFailureExplanations()
    {
        // 4. Проверка самостоятельной задачи (Тип 2)
        var task = FlowchartRepository.MainPracticeTask;

        // Неудача 1: total <= 1000 (скидка 5% вместо 20%)
        var res1 = task.ValidateInputs(new Dictionary<string, VariableValue> { ["price"] = 50, ["qty"] = 10 });
        Assert.False(res1.IsSuccess);
        Assert.Contains("не превышает 1000", res1.Explanation);

        // Неудача 2: total > 1000, но final_sum >= 1200 (VIP вместо Стандартной)
        var res2 = task.ValidateInputs(new Dictionary<string, VariableValue> { ["price"] = 200, ["qty"] = 10 });
        Assert.False(res2.IsSuccess);
        Assert.Contains("VIP", res2.ActualBranchOrResult);

        // Успех: price = 100, qty = 12 -> total = 1200 > 1000, final_sum = 960 < 1200
        var res3 = task.ValidateInputs(new Dictionary<string, VariableValue> { ["price"] = 100, ["qty"] = 12 });
        Assert.True(res3.IsSuccess);
        Assert.Contains("Отличная работа", res3.Explanation);
    }

    [Fact]
    public void Test5_ParameterChange_And_ResetSimulation_IsDeterministic()
    {
        // 5. Изменение параметров и повторный перезапуск
        var service = new SimulationStateService();
        service.SelectFlowchart("intro");
        service.SetInput("a", 8);
        service.SetInput("b", 12);

        // Первый проход
        while (!service.IsCompleted)
        {
            if (service.ActiveQuestion != null)
            {
                var ans = service.ActiveQuestion.Question.GetCorrectAnswer(service.CurrentSnapshot.Variables);
                service.AnswerQuestion(ans);
            }
            service.StepForward();
        }

        Assert.Equal(20, service.CurrentSnapshot.Variables["sum"].ToInt());
        Assert.Equal("Сумма чётная", service.CurrentSnapshot.Variables["result"].AsString);

        // Повторный запуск со сбросом
        service.ResetSimulation();
        Assert.Equal(0, service.CurrentStepIndex);
        Assert.Equal("n_start", service.CurrentSnapshot.CurrentNodeId);
    }
}
