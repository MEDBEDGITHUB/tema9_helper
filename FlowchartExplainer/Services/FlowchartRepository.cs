using FlowchartExplainer.Models;
using System.Globalization;

namespace FlowchartExplainer.Services;

/// <summary>
/// Репозиторий определений блок-схем и задач проекта
/// </summary>
public static class FlowchartRepository
{
    public static FlowchartDefinition IntroFlowchart => new()
    {
        Id = "intro",
        Title = "1. Вводная: Чётность суммы двух чисел",
        TypeCategory = "intro",
        Badge = "1 условие · Базовый уровень",
        Goal = "Научиться отслеживать вычисление суммы и проверку делимости на 2 (ветвление Да/Нет)",
        Description = "Программа принимает два целых числа a и b, находит их сумму sum = a + b, а затем проверяет, делится ли сумма на 2 без остатка. В зависимости от условия выбирается левая или правая ветвь.",
        ViewBoxWidth = 560,
        ViewBoxHeight = 600,
        Inputs = new List<InputDefinition>
        {
            new() { Name = "a", Label = "Число a", Min = 1, Max = 20, DefaultValue = 7, Description = "Первое целое слагаемое (1..20)" },
            new() { Name = "b", Label = "Число b", Min = 1, Max = 20, DefaultValue = 4, Description = "Второе целое слагаемое (1..20)" }
        },
        Nodes = new List<FlowchartNode>
        {
            new()
            {
                Id = "n_start",
                Type = NodeType.Start,
                Text = "Начало",
                X = 280,
                Y = 40,
                Width = 140,
                Height = 48,
                NextNodeId = "n_input",
                ExecuteFunc = _ => new StepExecutionResult
                {
                    Explanation = "Старт алгоритма. Инициализация переменных исходными значениями.",
                    NextNodeId = "n_input"
                }
            },
            new()
            {
                Id = "n_input",
                Type = NodeType.Input,
                Text = "Ввод a, b",
                Subtext = "Считывание слагаемых",
                X = 280,
                Y = 120,
                Width = 160,
                Height = 50,
                NextNodeId = "n_sum",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Получены входные данные: a = {vars["a"]}, b = {vars["b"]}. Переменные сохранены в памяти.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "a", OldValue = VariableValue.Empty, NewValue = vars["a"], Description = "Ввод первого числа" },
                        new() { Name = "b", OldValue = VariableValue.Empty, NewValue = vars["b"], Description = "Ввод второго числа" }
                    },
                    NextNodeId = "n_sum"
                }
            },
            new()
            {
                Id = "n_sum",
                Type = NodeType.Process,
                Text = "sum = a + b",
                Subtext = "Сложение чисел",
                X = 280,
                Y = 210,
                Width = 180,
                Height = 54,
                NextNodeId = "n_cond",
                Question = new StepQuestion
                {
                    Id = "q_intro_sum",
                    Prompt = "Вопрос для самопроверки: чему равно значение sum = a + b?",
                    Type = QuestionType.NumberInput,
                    GetCorrectAnswer = vars => (vars["a"].ToInt() + vars["b"].ToInt()).ToString(),
                    ExplanationOnCorrect = "Верно! Складываем значение переменной a со значением переменной b.",
                    ExplanationOnWrong = "Неверно. Сложите текущие значения a и b из таблицы переменных."
                },
                ExecuteFunc = vars =>
                {
                    var a = vars["a"].ToInt();
                    var b = vars["b"].ToInt();
                    var sum = a + b;
                    return new StepExecutionResult
                    {
                        Explanation = $"Вычисляем сумму: sum = {a} + {b} = {sum}. Значение {sum} записывается в переменную sum.",
                        FormulaApplied = $"sum = {a} + {b} = {sum}",
                        Changes = new List<VariableChange>
                        {
                            new() { Name = "sum", OldValue = vars.GetValueOrDefault("sum"), NewValue = sum, Formula = $"{a} + {b} = {sum}" }
                        },
                        NextNodeId = "n_cond"
                    };
                }
            },
            new()
            {
                Id = "n_cond",
                Type = NodeType.Condition,
                Text = "sum mod 2 = 0 ?",
                Subtext = "Остаток от деления на 2",
                X = 280,
                Y = 320,
                Width = 180,
                Height = 70,
                NextTrueNodeId = "n_even",
                NextFalseNodeId = "n_odd",
                Question = new StepQuestion
                {
                    Id = "q_intro_branch",
                    Prompt = "Проверка условия: делится ли sum на 2 без остатка? Какую ветвь выберем?",
                    Type = QuestionType.Choice,
                    Options = new List<string> { "Да (чётное)", "Нет (нечётное)" },
                    GetCorrectAnswer = vars => vars["sum"].ToInt() % 2 == 0 ? "Да (чётное)" : "Нет (нечётное)",
                    ExplanationOnCorrect = "Точно! Проверяем делимость на 2: если остаток 0, идём по ветке «Да», иначе — «Нет».",
                    ExplanationOnWrong = "Внимательно посмотрите на остаток от деления sum на 2."
                },
                ExecuteFunc = vars =>
                {
                    var sum = vars["sum"].ToInt();
                    var rem = sum % 2;
                    var isEven = rem == 0;
                    return new StepExecutionResult
                    {
                        Explanation = $"Проверяем: {sum} mod 2 = {rem}. {(isEven ? "Остаток равен 0 — условие истинно (ветвь «Да»)." : "Остаток не равен 0 — условие ложно (ветвь «Нет»).")}",
                        FormulaApplied = $"{sum} % 2 == {rem} ({(isEven ? "True" : "False")})",
                        ConditionResult = isEven,
                        BranchTaken = isEven ? "Да" : "Нет",
                        NextNodeId = isEven ? "n_even" : "n_odd"
                    };
                }
            },
            new()
            {
                Id = "n_even",
                Type = NodeType.Output,
                Text = "Вывод: «Сумма чётная»",
                X = 130,
                Y = 430,
                Width = 190,
                Height = 52,
                NextNodeId = "n_end",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Выводим результат пользователю: сумма {vars["sum"]} является чётным числом.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "result", OldValue = vars.GetValueOrDefault("result"), NewValue = "Сумма чётная" }
                    },
                    NextNodeId = "n_end"
                }
            },
            new()
            {
                Id = "n_odd",
                Type = NodeType.Output,
                Text = "Вывод: «Сумма нечётная»",
                X = 430,
                Y = 430,
                Width = 190,
                Height = 52,
                NextNodeId = "n_end",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Выводим результат пользователю: сумма {vars["sum"]} является нечётным числом.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "result", OldValue = vars.GetValueOrDefault("result"), NewValue = "Сумма нечётная" }
                    },
                    NextNodeId = "n_end"
                }
            },
            new()
            {
                Id = "n_end",
                Type = NodeType.End,
                Text = "Конец",
                X = 280,
                Y = 530,
                Width = 140,
                Height = 48,
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Алгоритм успешно завершил работу. Итоговый результат: «{vars.GetValueOrDefault("result")}».",
                    NextNodeId = null
                }
            }
        },
        Edges = new List<FlowchartEdge>
        {
            new() { Id = "e1", From = "n_start", To = "n_input" },
            new() { Id = "e2", From = "n_input", To = "n_sum" },
            new() { Id = "e3", From = "n_sum", To = "n_cond" },
            new()
            {
                Id = "e_cond_true",
                From = "n_cond",
                To = "n_even",
                Label = "Да",
                Waypoints = new List<Waypoint>
                {
                    new(190, 320),
                    new(130, 320),
                    new(130, 404)
                }
            },
            new()
            {
                Id = "e_cond_false",
                From = "n_cond",
                To = "n_odd",
                Label = "Нет",
                Waypoints = new List<Waypoint>
                {
                    new(370, 320),
                    new(430, 320),
                    new(430, 404)
                }
            },
            new()
            {
                Id = "e_even_end",
                From = "n_even",
                To = "n_end",
                Waypoints = new List<Waypoint>
                {
                    new(130, 490),
                    new(280, 490),
                    new(280, 506)
                }
            },
            new()
            {
                Id = "e_odd_end",
                From = "n_odd",
                To = "n_end",
                Waypoints = new List<Waypoint>
                {
                    new(430, 490),
                    new(280, 490),
                    new(280, 506)
                }
            }
        }
    };

    public static FlowchartDefinition MainFlowchart => new()
    {
        Id = "main",
        Title = "2. Основная: Расчёт покупки со скидкой",
        TypeCategory = "main",
        Badge = "2 условия · Комплексный разбор",
        Goal = "Освоить чтение многоуровневого ветвления: расчёт процента скидки и присвоение статуса доставки",
        Description = "Программа вычисляет общую стоимость total = price × qty. Если total > 1000 руб., скидка 20%, иначе 5%. После вычитания скидки проверяется итоговая сумма: при final_sum ≥ 1200 руб. присваивается VIP-доставка, иначе — Стандартная.",
        ViewBoxWidth = 620,
        ViewBoxHeight = 780,
        Inputs = new List<InputDefinition>
        {
            new() { Name = "price", Label = "Цена за шт. (руб)", Min = 10, Max = 300, DefaultValue = 120, Description = "Стоимость единицы товара (10..300 руб.)" },
            new() { Name = "qty", Label = "Количество (шт)", Min = 1, Max = 20, DefaultValue = 10, Description = "Количество единиц товара (1..20 шт.)" }
        },
        Nodes = new List<FlowchartNode>
        {
            new()
            {
                Id = "m_start",
                Type = NodeType.Start,
                Text = "Начало",
                X = 310,
                Y = 35,
                Width = 140,
                Height = 44,
                NextNodeId = "m_input",
                ExecuteFunc = _ => new StepExecutionResult
                {
                    Explanation = "Старт программы оформления заказа покупателя.",
                    NextNodeId = "m_input"
                }
            },
            new()
            {
                Id = "m_input",
                Type = NodeType.Input,
                Text = "Ввод price, qty",
                Subtext = "Цена и количество",
                X = 310,
                Y = 105,
                Width = 180,
                Height = 48,
                NextNodeId = "m_total",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Получены параметры покупки: price = {vars["price"]} руб., qty = {vars["qty"]} шт.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "price", OldValue = VariableValue.Empty, NewValue = vars["price"], Description = "Цена за единицу" },
                        new() { Name = "qty", OldValue = VariableValue.Empty, NewValue = vars["qty"], Description = "Количество товаров" }
                    },
                    NextNodeId = "m_total"
                }
            },
            new()
            {
                Id = "m_total",
                Type = NodeType.Process,
                Text = "total = price * qty",
                Subtext = "Предварительная сумма",
                X = 310,
                Y = 185,
                Width = 210,
                Height = 52,
                NextNodeId = "m_cond_disc",
                Question = new StepQuestion
                {
                    Id = "q_main_total",
                    Prompt = "Вопрос 1: Вычислите предварительную стоимость покупки total = price * qty",
                    Type = QuestionType.NumberInput,
                    GetCorrectAnswer = vars => (vars["price"].ToDouble() * vars["qty"].ToDouble()).ToString("0.##", CultureInfo.InvariantCulture),
                    ExplanationOnCorrect = "Верно! Умножаем цену одной штуки на количество товаров.",
                    ExplanationOnWrong = "Перемножьте текущую цену price на количество qty."
                },
                ExecuteFunc = vars =>
                {
                    var p = vars["price"].ToDouble();
                    var q = vars["qty"].ToDouble();
                    var total = Math.Round(p * q, 2);
                    return new StepExecutionResult
                    {
                        Explanation = $"Умножаем цену на количество: total = {p} × {q} = {total} руб.",
                        FormulaApplied = $"total = {p} * {q} = {total}",
                        Changes = new List<VariableChange>
                        {
                            new() { Name = "total", OldValue = vars.GetValueOrDefault("total"), NewValue = total, Formula = $"{p} * {q} = {total}" }
                        },
                        NextNodeId = "m_cond_disc"
                    };
                }
            },
            new()
            {
                Id = "m_cond_disc",
                Type = NodeType.Condition,
                Text = "total > 1000 ?",
                Subtext = "Порог скидки 20%",
                X = 310,
                Y = 285,
                Width = 180,
                Height = 66,
                NextTrueNodeId = "m_disc_20",
                NextFalseNodeId = "m_disc_5",
                Question = new StepQuestion
                {
                    Id = "q_main_disc_branch",
                    Prompt = "Вопрос 2: Положена ли повышенная скидка (total > 1000)? Какую ветку выбираем?",
                    Type = QuestionType.Choice,
                    Options = new List<string> { "Да (скидка 20%)", "Нет (скидка 5%)" },
                    GetCorrectAnswer = vars => vars["total"].ToDouble() > 1000 ? "Да (скидка 20%)" : "Нет (скидка 5%)",
                    ExplanationOnCorrect = "Правильно! Сравниваем total с 1000: при строгом превышении (total > 1000) положено 20%.",
                    ExplanationOnWrong = "Обратите внимание: условие строгое (total > 1000)."
                },
                ExecuteFunc = vars =>
                {
                    var total = vars["total"].ToDouble();
                    var isBig = total > 1000;
                    return new StepExecutionResult
                    {
                        Explanation = $"Сравниваем: {total} > 1000. Результат: {(isBig ? "Да (True) → скидка 20%" : "Нет (False) → базовая скидка 5%")}.",
                        FormulaApplied = $"{total} > 1000 is {isBig}",
                        ConditionResult = isBig,
                        BranchTaken = isBig ? "Да" : "Нет",
                        NextNodeId = isBig ? "m_disc_20" : "m_disc_5"
                    };
                }
            },
            new()
            {
                Id = "m_disc_20",
                Type = NodeType.Process,
                Text = "disc = total * 0.20",
                Subtext = "Скидка 20%",
                X = 140,
                Y = 385,
                Width = 190,
                Height = 52,
                NextNodeId = "m_final_calc",
                ExecuteFunc = vars =>
                {
                    var total = vars["total"].ToDouble();
                    var disc = Math.Round(total * 0.20, 2);
                    return new StepExecutionResult
                    {
                        Explanation = $"Вычисляем 20% скидки: disc = {total} × 0.20 = {disc} руб.",
                        FormulaApplied = $"disc = {total} * 0.20 = {disc}",
                        Changes = new List<VariableChange>
                        {
                            new() { Name = "disc", OldValue = vars.GetValueOrDefault("disc"), NewValue = disc, Formula = $"{total} * 0.20 = {disc}" }
                        },
                        NextNodeId = "m_final_calc"
                    };
                }
            },
            new()
            {
                Id = "m_disc_5",
                Type = NodeType.Process,
                Text = "disc = total * 0.05",
                Subtext = "Скидка 5%",
                X = 480,
                Y = 385,
                Width = 190,
                Height = 52,
                NextNodeId = "m_final_calc",
                ExecuteFunc = vars =>
                {
                    var total = vars["total"].ToDouble();
                    var disc = Math.Round(total * 0.05, 2);
                    return new StepExecutionResult
                    {
                        Explanation = $"Вычисляем 5% скидки: disc = {total} × 0.05 = {disc} руб.",
                        FormulaApplied = $"disc = {total} * 0.05 = {disc}",
                        Changes = new List<VariableChange>
                        {
                            new() { Name = "disc", OldValue = vars.GetValueOrDefault("disc"), NewValue = disc, Formula = $"{total} * 0.05 = {disc}" }
                        },
                        NextNodeId = "m_final_calc"
                    };
                }
            },
            new()
            {
                Id = "m_final_calc",
                Type = NodeType.Process,
                Text = "final_sum = total - disc",
                Subtext = "Итог к оплате",
                X = 310,
                Y = 475,
                Width = 220,
                Height = 52,
                NextNodeId = "m_cond_vip",
                Question = new StepQuestion
                {
                    Id = "q_main_final_sum",
                    Prompt = "Вопрос 3: Вычислите итоговую сумму к оплате: final_sum = total - disc",
                    Type = QuestionType.NumberInput,
                    GetCorrectAnswer = vars => (vars["total"].ToDouble() - vars["disc"].ToDouble()).ToString("0.##", CultureInfo.InvariantCulture),
                    ExplanationOnCorrect = "Верно! Из предварительной стоимости вычитаем сумму скидки.",
                    ExplanationOnWrong = "Вычтите disc из total."
                },
                ExecuteFunc = vars =>
                {
                    var total = vars["total"].ToDouble();
                    var disc = vars["disc"].ToDouble();
                    var finalSum = Math.Round(total - disc, 2);
                    return new StepExecutionResult
                    {
                        Explanation = $"Вычитаем скидку: final_sum = {total} − {disc} = {finalSum} руб.",
                        FormulaApplied = $"final_sum = {total} - {disc} = {finalSum}",
                        Changes = new List<VariableChange>
                        {
                            new() { Name = "final_sum", OldValue = vars.GetValueOrDefault("final_sum"), NewValue = finalSum, Formula = $"{total} - {disc} = {finalSum}" }
                        },
                        NextNodeId = "m_cond_vip"
                    };
                }
            },
            new()
            {
                Id = "m_cond_vip",
                Type = NodeType.Condition,
                Text = "final_sum >= 1200 ?",
                Subtext = "Порог VIP-доставки",
                X = 310,
                Y = 565,
                Width = 200,
                Height = 66,
                NextTrueNodeId = "m_out_vip",
                NextFalseNodeId = "m_out_std",
                ExecuteFunc = vars =>
                {
                    var fs = vars["final_sum"].ToDouble();
                    var isVip = fs >= 1200;
                    return new StepExecutionResult
                    {
                        Explanation = $"Проверяем: {fs} >= 1200 ? {(isVip ? "Да → VIP-доставка" : "Нет → Стандартная доставка")}.",
                        FormulaApplied = $"{fs} >= 1200 is {isVip}",
                        ConditionResult = isVip,
                        BranchTaken = isVip ? "Да" : "Нет",
                        NextNodeId = isVip ? "m_out_vip" : "m_out_std"
                    };
                }
            },
            new()
            {
                Id = "m_out_vip",
                Type = NodeType.Output,
                Text = "Вывод: «VIP (Бесплатно)»",
                X = 140,
                Y = 665,
                Width = 200,
                Height = 48,
                NextNodeId = "m_end",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Назначаем статус: VIP клиент с бесплатной доставкой. Итог: {vars["final_sum"]} руб.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "delivery", OldValue = vars.GetValueOrDefault("delivery"), NewValue = "VIP (Бесплатно)" }
                    },
                    NextNodeId = "m_end"
                }
            },
            new()
            {
                Id = "m_out_std",
                Type = NodeType.Output,
                Text = "Вывод: «Стандартная»",
                X = 480,
                Y = 665,
                Width = 190,
                Height = 48,
                NextNodeId = "m_end",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Назначаем статус: Стандартная доставка. Итог к оплате: {vars["final_sum"]} руб.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "delivery", OldValue = vars.GetValueOrDefault("delivery"), NewValue = "Стандартная доставка" }
                    },
                    NextNodeId = "m_end"
                }
            },
            new()
            {
                Id = "m_end",
                Type = NodeType.End,
                Text = "Конец",
                X = 310,
                Y = 740,
                Width = 130,
                Height = 42,
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Расчёт завершён. К оплате: {vars.GetValueOrDefault("final_sum")} руб., доставка: «{vars.GetValueOrDefault("delivery")}».",
                    NextNodeId = null
                }
            }
        },
        Edges = new List<FlowchartEdge>
        {
            new() { Id = "me1", From = "m_start", To = "m_input" },
            new() { Id = "me2", From = "m_input", To = "m_total" },
            new() { Id = "me3", From = "m_total", To = "m_cond_disc" },
            new()
            {
                Id = "me_cond1_true",
                From = "m_cond_disc",
                To = "m_disc_20",
                Label = "Да",
                Waypoints = new List<Waypoint> { new(220, 285), new(140, 285), new(140, 359) }
            },
            new()
            {
                Id = "me_cond1_false",
                From = "m_cond_disc",
                To = "m_disc_5",
                Label = "Нет",
                Waypoints = new List<Waypoint> { new(400, 285), new(480, 285), new(480, 359) }
            },
            new()
            {
                Id = "me_disc20_merge",
                From = "m_disc_20",
                To = "m_final_calc",
                Waypoints = new List<Waypoint> { new(140, 430), new(310, 430), new(310, 449) }
            },
            new()
            {
                Id = "me_disc5_merge",
                From = "m_disc_5",
                To = "m_final_calc",
                Waypoints = new List<Waypoint> { new(480, 430), new(310, 430), new(310, 449) }
            },
            new() { Id = "me_final_to_cond2", From = "m_final_calc", To = "m_cond_vip" },
            new()
            {
                Id = "me_cond2_true",
                From = "m_cond_vip",
                To = "m_out_vip",
                Label = "Да",
                Waypoints = new List<Waypoint> { new(210, 565), new(140, 565), new(140, 641) }
            },
            new()
            {
                Id = "me_cond2_false",
                From = "m_cond_vip",
                To = "m_out_std",
                Label = "Нет",
                Waypoints = new List<Waypoint> { new(410, 565), new(480, 565), new(480, 641) }
            },
            new()
            {
                Id = "me_vip_to_end",
                From = "m_out_vip",
                To = "m_end",
                Waypoints = new List<Waypoint> { new(140, 705), new(310, 705), new(310, 719) }
            },
            new()
            {
                Id = "me_std_to_end",
                From = "m_out_std",
                To = "m_end",
                Waypoints = new List<Waypoint> { new(480, 705), new(310, 705), new(310, 719) }
            }
        }
    };

    public static FlowchartDefinition SpecialFlowchart => new()
    {
        Id = "special",
        Title = "3. Особый случай: Деление с защитой от деления на 0",
        TypeCategory = "special",
        Badge = "Особый случай · Защита от сбоя",
        Goal = "Понять роль защитных проверок (guard clauses) и обработку особых ситуаций до вычислений",
        Description = "При делении чисел a на b недопустимо значение b = 0. Алгоритм сначала проверяет: если b = 0, сразу выдаётся предупреждение и алгоритм прерывается. Иначе вычисляются частное q = ⌊a / b⌋ и остаток r = a mod b с проверкой на делимость нацело.",
        ViewBoxWidth = 620,
        ViewBoxHeight = 680,
        Inputs = new List<InputDefinition>
        {
            new() { Name = "a", Label = "Делимое a", Min = 1, Max = 100, DefaultValue = 25, Description = "Число, которое делим (1..100)" },
            new() { Name = "b", Label = "Делитель b", Min = 0, Max = 20, DefaultValue = 0, Description = "Число, на которое делим (0..20, по умолчанию 0!)" }
        },
        Nodes = new List<FlowchartNode>
        {
            new()
            {
                Id = "s_start",
                Type = NodeType.Start,
                Text = "Начало",
                X = 310,
                Y = 35,
                Width = 140,
                Height = 44,
                NextNodeId = "s_input",
                ExecuteFunc = _ => new StepExecutionResult
                {
                    Explanation = "Старт алгоритма деления с предварительной проверкой аргументов.",
                    NextNodeId = "s_input"
                }
            },
            new()
            {
                Id = "s_input",
                Type = NodeType.Input,
                Text = "Ввод a, b",
                Subtext = "Делимое и делитель",
                X = 310,
                Y = 105,
                Width = 180,
                Height = 48,
                NextNodeId = "s_check_zero",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Получены числа: делимое a = {vars["a"]}, делитель b = {vars["b"]}.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "a", OldValue = VariableValue.Empty, NewValue = vars["a"], Description = "Делимое" },
                        new() { Name = "b", OldValue = VariableValue.Empty, NewValue = vars["b"], Description = "Делитель" }
                    },
                    NextNodeId = "s_check_zero"
                }
            },
            new()
            {
                Id = "s_check_zero",
                Type = NodeType.Condition,
                Text = "b = 0 ?",
                Subtext = "Проверка деления на 0",
                X = 310,
                Y = 200,
                Width = 180,
                Height = 66,
                NextTrueNodeId = "s_err_div_zero",
                NextFalseNodeId = "s_calc_div",
                Question = new StepQuestion
                {
                    Id = "q_spec_zero",
                    Prompt = "Вопрос 1: Равен ли делитель b нулю? Безопасно ли выполнять деление?",
                    Type = QuestionType.Choice,
                    Options = new List<string> { "Да (b = 0, делить нельзя)", "Нет (b ≠ 0, можно делить)" },
                    GetCorrectAnswer = vars => vars["b"].ToInt() == 0 ? "Да (b = 0, делить нельзя)" : "Нет (b ≠ 0, можно делить)",
                    ExplanationOnCorrect = "Верно! Если b = 0, операция a / b в арифметике не определена и привела бы к критическому сбою.",
                    ExplanationOnWrong = "Посмотрите на текущее значение переменной b в таблице."
                },
                ExecuteFunc = vars =>
                {
                    var b = vars["b"].ToInt();
                    var isZero = b == 0;
                    return new StepExecutionResult
                    {
                        Explanation = $"Проверяем b = {b}. {(isZero ? "Делитель равен 0! Переходим по ветви «Да» на безопасную обработку ошибки." : "Делитель не равен 0. Переходим по ветви «Нет» к арифметическим операциям.")}",
                        FormulaApplied = $"b == 0 ({isZero})",
                        ConditionResult = isZero,
                        BranchTaken = isZero ? "Да" : "Нет",
                        NextNodeId = isZero ? "s_err_div_zero" : "s_calc_div"
                    };
                }
            },
            new()
            {
                Id = "s_err_div_zero",
                Type = NodeType.Output,
                Text = "Ошибка: деление на 0!",
                Subtext = "Аварийное завершение",
                X = 130,
                Y = 330,
                Width = 200,
                Height = 52,
                NextNodeId = "s_end",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = "Критическая ошибка предотвращена: выводим сообщение об ошибке деления на ноль и безопасно завершаем программу.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "result", OldValue = vars.GetValueOrDefault("result"), NewValue = "Ошибка: деление на ноль!" }
                    },
                    NextNodeId = "s_end"
                }
            },
            new()
            {
                Id = "s_calc_div",
                Type = NodeType.Process,
                Text = "q = ⌊a / b⌋, r = a mod b",
                Subtext = "Частное и остаток",
                X = 470,
                Y = 330,
                Width = 220,
                Height = 54,
                NextNodeId = "s_cond_rem",
                Question = new StepQuestion
                {
                    Id = "q_spec_rem",
                    Prompt = "Вопрос 2: Чему равен остаток r = a mod b?",
                    Type = QuestionType.NumberInput,
                    GetCorrectAnswer = vars => (vars["a"].ToInt() % vars["b"].ToInt()).ToString(),
                    ExplanationOnCorrect = "Верно! Остаток от деления вычисляется операцией mod (или % в языках программирования).",
                    ExplanationOnWrong = "Вычислите a % b (остаток от деления a на b)."
                },
                ExecuteFunc = vars =>
                {
                    var a = vars["a"].ToInt();
                    var b = vars["b"].ToInt();
                    var q = a / b;
                    var r = a % b;
                    return new StepExecutionResult
                    {
                        Explanation = $"Делим {a} на {b}: целая часть q = {q}, остаток r = {r}.",
                        FormulaApplied = $"q = {a} / {b} = {q}; r = {a} % {b} = {r}",
                        Changes = new List<VariableChange>
                        {
                            new() { Name = "q", OldValue = vars.GetValueOrDefault("q"), NewValue = q, Formula = $"⌊{a} / {b}⌋ = {q}" },
                            new() { Name = "r", OldValue = vars.GetValueOrDefault("r"), NewValue = r, Formula = $"{a} mod {b} = {r}" }
                        },
                        NextNodeId = "s_cond_rem"
                    };
                }
            },
            new()
            {
                Id = "s_cond_rem",
                Type = NodeType.Condition,
                Text = "r = 0 ?",
                Subtext = "Делимость нацело",
                X = 470,
                Y = 440,
                Width = 170,
                Height = 64,
                NextTrueNodeId = "s_out_exact",
                NextFalseNodeId = "s_out_rem",
                ExecuteFunc = vars =>
                {
                    var r = vars["r"].ToInt();
                    var isExact = r == 0;
                    return new StepExecutionResult
                    {
                        Explanation = $"Проверяем остаток r = {r}: {(isExact ? "Делится нацело без остатка (ветвь «Да»)" : "Есть ненулевой остаток (ветвь «Нет»)")}.",
                        FormulaApplied = $"r == 0 ({isExact})",
                        ConditionResult = isExact,
                        BranchTaken = isExact ? "Да" : "Нет",
                        NextNodeId = isExact ? "s_out_exact" : "s_out_rem"
                    };
                }
            },
            new()
            {
                Id = "s_out_exact",
                Type = NodeType.Output,
                Text = "Вывод: «Нацело: q»",
                X = 350,
                Y = 550,
                Width = 180,
                Height = 48,
                NextNodeId = "s_end",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Результат: делится нацело, частное = {vars["q"]}.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "result", OldValue = vars.GetValueOrDefault("result"), NewValue = $"Нацело: частное {vars["q"]}" }
                    },
                    NextNodeId = "s_end"
                }
            },
            new()
            {
                Id = "s_out_rem",
                Type = NodeType.Output,
                Text = "Вывод: «С остатком: q (ост. r)»",
                X = 540,
                Y = 550,
                Width = 180,
                Height = 48,
                NextNodeId = "s_end",
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Результат: частное = {vars["q"]}, остаток = {vars["r"]}.",
                    Changes = new List<VariableChange>
                    {
                        new() { Name = "result", OldValue = vars.GetValueOrDefault("result"), NewValue = $"{vars["q"]} (остаток {vars["r"]})" }
                    },
                    NextNodeId = "s_end"
                }
            },
            new()
            {
                Id = "s_end",
                Type = NodeType.End,
                Text = "Конец",
                X = 310,
                Y = 635,
                Width = 130,
                Height = 42,
                ExecuteFunc = vars => new StepExecutionResult
                {
                    Explanation = $"Работа алгоритма завершена. Статус: {vars.GetValueOrDefault("result")}.",
                    NextNodeId = null
                }
            }
        },
        Edges = new List<FlowchartEdge>
        {
            new() { Id = "se1", From = "s_start", To = "s_input" },
            new() { Id = "se2", From = "s_input", To = "s_check_zero" },
            new()
            {
                Id = "se_zero_true",
                From = "s_check_zero",
                To = "s_err_div_zero",
                Label = "Да",
                Waypoints = new List<Waypoint> { new(220, 200), new(130, 200), new(130, 304) }
            },
            new()
            {
                Id = "se_zero_false",
                From = "s_check_zero",
                To = "s_calc_div",
                Label = "Нет",
                Waypoints = new List<Waypoint> { new(400, 200), new(470, 200), new(470, 303) }
            },
            new()
            {
                Id = "se_err_to_end",
                From = "s_err_div_zero",
                To = "s_end",
                Waypoints = new List<Waypoint> { new(130, 382), new(130, 635), new(245, 635) }
            },
            new() { Id = "se_calc_to_rem", From = "s_calc_div", To = "s_cond_rem" },
            new()
            {
                Id = "se_rem_true",
                From = "s_cond_rem",
                To = "s_out_exact",
                Label = "Да",
                Waypoints = new List<Waypoint> { new(385, 440), new(350, 440), new(350, 526) }
            },
            new()
            {
                Id = "se_rem_false",
                From = "s_cond_rem",
                To = "s_out_rem",
                Label = "Нет",
                Waypoints = new List<Waypoint> { new(555, 440), new(540, 440), new(540, 526) }
            },
            new()
            {
                Id = "se_exact_to_end",
                From = "s_out_exact",
                To = "s_end",
                Waypoints = new List<Waypoint> { new(350, 598), new(350, 615), new(310, 615) }
            },
            new()
            {
                Id = "se_rem_to_end",
                From = "s_out_rem",
                To = "s_end",
                Waypoints = new List<Waypoint> { new(540, 598), new(540, 615), new(375, 615) }
            }
        }
    };

    public static PracticeTask MainPracticeTask => new()
    {
        Id = "practice_discount_standard",
        Title = "Самостоятельная задача: Подбор входных данных (Тип 2)",
        Description = "В схеме «Расчёт покупки со скидкой» подберите такие цену (price) и количество (qty), чтобы выполнялась ветвь повышенной скидки 20% (total > 1000 руб.), но при этом итоговая сумма к оплате (final_sum) оказалась строго меньше 1200 руб., обеспечив назначение «Стандартной доставки».",
        TargetCondition = "Ветвь скидки 20% (total > 1000) И ветвь «Стандартная доставка» (final_sum < 1200)",
        FlowchartId = "main",
        Inputs = new List<InputDefinition>
        {
            new() { Name = "price", Label = "Цена за шт. (руб)", Min = 50, Max = 250, DefaultValue = 100, Description = "Диапазон: 50..250 руб." },
            new() { Name = "qty", Label = "Количество (шт)", Min = 5, Max = 20, DefaultValue = 8, Description = "Диапазон: 5..20 шт." }
        },
        ValidateInputs = inputs =>
        {
            var price = inputs["price"].ToDouble();
            var qty = inputs["qty"].ToDouble();

            if (price <= 0 || qty <= 0)
            {
                return new PracticeValidationResult
                {
                    IsSuccess = false,
                    ActualBranchOrResult = "Недопустимые параметры",
                    Explanation = "Цена и количество должны быть строго положительными числами.",
                    Hint = "Задайте параметры в указанных диапазонах (цена 50..250 руб., количество 5..20 шт.)."
                };
            }

            var total = Math.Round(price * qty, 2);
            var isBig = total > 1000;
            var disc = isBig ? Math.Round(total * 0.20, 2) : Math.Round(total * 0.05, 2);
            var finalSum = Math.Round(total - disc, 2);
            var isVip = finalSum >= 1200;
            var delivery = isVip ? "VIP (Бесплатно)" : "Стандартная доставка";

            var finalVars = new Dictionary<string, VariableValue>
            {
                ["price"] = price,
                ["qty"] = qty,
                ["total"] = total,
                ["disc"] = disc,
                ["final_sum"] = finalSum,
                ["delivery"] = delivery
            };

            if (!isBig)
            {
                return new PracticeValidationResult
                {
                    IsSuccess = false,
                    ActualBranchOrResult = $"total = {total} руб. (скидка 5%, а не 20%)",
                    Explanation = $"Сумма total = {price} × {qty} = {total} руб. не превышает 1000 руб. Сработала ветвь скидки 5% вместо 20%.",
                    Hint = "Увеличьте цену или количество, чтобы предварительная сумма total = price × qty была строго больше 1000 руб.",
                    FinalVariables = finalVars
                };
            }

            if (isVip)
            {
                return new PracticeValidationResult
                {
                    IsSuccess = false,
                    ActualBranchOrResult = $"final_sum = {finalSum} руб. (VIP-доставка)",
                    Explanation = $"Скидка 20% получена (total = {total} руб.), но итоговая сумма final_sum = {finalSum} руб. превысила лимит 1200 руб. Назначена VIP-доставка, а требовалась «Стандартная»!",
                    Hint = "Итоговая сумма со скидкой 20% равна total × 0.8. Нам нужно 1000 < total < 1500 (так как 1500 × 0.8 = 1200). Попробуйте параметры с общей суммой, например, 1200–1400 руб.",
                    FinalVariables = finalVars
                };
            }

            return new PracticeValidationResult
            {
                IsSuccess = true,
                ActualBranchOrResult = $"total = {total} руб. (>1000, 20% скидка), final_sum = {finalSum} руб. (<1200, Стандартная)",
                Explanation = $"Отличная работа! При price = {price} и qty = {qty}: total = {total} руб. (> 1000 → скидка 20% = {disc} руб.), итого к оплате final_sum = {finalSum} руб. (< 1200 → Стандартная доставка). Все условия выполнены!",
                FinalVariables = finalVars
            };
        }
    };

    public static readonly IReadOnlyList<FlowchartDefinition> AllFlowcharts = new List<FlowchartDefinition>
    {
        IntroFlowchart,
        MainFlowchart,
        SpecialFlowchart
    };

    public static FlowchartDefinition? GetById(string id) => AllFlowcharts.FirstOrDefault(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
}
