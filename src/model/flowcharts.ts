import { FlowchartDefinition, PracticeTask, StepExecutionResult, Variables } from './types';

/**
 * 1. ВВОДНАЯ СХЕМА: «Чётность суммы двух чисел»
 * 1 проверка условия, простые арифметические операции
 */
export const introFlowchart: FlowchartDefinition = {
  id: 'intro',
  title: '1. Вводная: Чётность суммы двух чисел',
  typeCategory: 'intro',
  badge: '1 условие · Базовый уровень',
  goal: 'Научиться отслеживать вычисление суммы и проверку делимости на 2 (ветвление Да/Нет)',
  description: 'Программа принимает два целых числа a и b, находит их сумму sum = a + b, а затем проверяет, делится ли сумма на 2 без остатка. В зависимости от условия выбирается левая или правая ветвь.',
  inputs: [
    { name: 'a', label: 'Число a', min: 1, max: 20, defaultValue: 7, description: 'Первое целое слагаемое (1..20)' },
    { name: 'b', label: 'Число b', min: 1, max: 20, defaultValue: 4, description: 'Второе целое слагаемое (1..20)' },
  ],
  viewBox: { width: 560, height: 600 },
  nodes: [
    {
      id: 'n_start',
      type: 'start',
      text: 'Начало',
      x: 280,
      y: 40,
      width: 140,
      height: 48,
      next: 'n_input',
      execute: (vars) => ({
        explanation: 'Старт алгоритма. Инициализация переменных исходными значениями.',
        changes: [],
        nextNodeId: 'n_input',
      }),
    },
    {
      id: 'n_input',
      type: 'input',
      text: 'Ввод a, b',
      subtext: 'Считывание слагаемых',
      x: 280,
      y: 120,
      width: 160,
      height: 50,
      next: 'n_sum',
      execute: (vars) => ({
        explanation: `Получены входные данные: a = ${vars['a']}, b = ${vars['b']}. Переменные сохранены в памяти.`,
        changes: [
          { name: 'a', oldValue: undefined, newValue: vars['a'] },
          { name: 'b', oldValue: undefined, newValue: vars['b'] },
        ],
        nextNodeId: 'n_sum',
      }),
    },
    {
      id: 'n_sum',
      type: 'process',
      text: 'sum = a + b',
      subtext: 'Сложение чисел',
      x: 280,
      y: 210,
      width: 180,
      height: 54,
      next: 'n_cond',
      question: {
        id: 'q_intro_sum',
        prompt: 'Вопрос для самопроверки: чему равно значение sum = a + b?',
        type: 'number',
        getCorrectAnswer: (vars) => Number(vars['a']) + Number(vars['b']),
        explanationOnCorrect: 'Верно! Складываем значение переменной a со значением переменной b.',
        explanationOnWrong: 'Неверно. Сложите текущие значения a и b.',
      },
      execute: (vars) => {
        const a = Number(vars['a']);
        const b = Number(vars['b']);
        const sum = a + b;
        return {
          explanation: `Вычисляем сумму: sum = ${a} + ${b} = ${sum}. Значение ${sum} записывается в переменную sum.`,
          formulaApplied: `sum = ${a} + ${b} = ${sum}`,
          changes: [{ name: 'sum', oldValue: vars['sum'], newValue: sum, formula: `${a} + ${b} = ${sum}` }],
          nextNodeId: 'n_cond',
        };
      },
    },
    {
      id: 'n_cond',
      type: 'condition',
      text: 'sum mod 2 = 0 ?',
      subtext: 'Остаток от деления на 2',
      x: 280,
      y: 320,
      width: 180,
      height: 70,
      nextTrue: 'n_even',
      nextFalse: 'n_odd',
      question: {
        id: 'q_intro_branch',
        prompt: 'Проверка условия: делится ли sum на 2 без остатка? Какую ветвь выберем?',
        type: 'choice',
        options: ['Да (чётное)', 'Нет (нечётное)'],
        getCorrectAnswer: (vars) => (Number(vars['sum']) % 2 === 0 ? 'Да (чётное)' : 'Нет (нечётное)'),
        explanationOnCorrect: 'Точно! Проверяем делимость на 2: если остаток 0, идём по ветке «Да», иначе — «Нет».',
        explanationOnWrong: 'Внимательно посмотрите на остаток от деления sum на 2.',
      },
      execute: (vars) => {
        const sum = Number(vars['sum']);
        const rem = sum % 2;
        const isEven = rem === 0;
        return {
          explanation: `Проверяем: ${sum} mod 2 = ${rem}. ${
            isEven ? 'Остаток равен 0 — условие истинно (ветвь «Да»).' : 'Остаток не равен 0 — условие ложно (ветвь «Нет»).'
          }`,
          formulaApplied: `${sum} % 2 === ${rem} (${isEven ? 'True' : 'False'})`,
          changes: [],
          conditionResult: isEven,
          nextNodeId: isEven ? 'n_even' : 'n_odd',
        };
      },
    },
    {
      id: 'n_even',
      type: 'output',
      text: 'Вывод: «Сумма чётная»',
      x: 130,
      y: 430,
      width: 190,
      height: 52,
      next: 'n_end',
      execute: (vars) => ({
        explanation: `Выводим результат пользователю: сумма ${vars['sum']} является чётным числом.`,
        changes: [{ name: 'result', oldValue: vars['result'], newValue: 'Сумма чётная' }],
        nextNodeId: 'n_end',
      }),
    },
    {
      id: 'n_odd',
      type: 'output',
      text: 'Вывод: «Сумма нечётная»',
      x: 430,
      y: 430,
      width: 190,
      height: 52,
      next: 'n_end',
      execute: (vars) => ({
        explanation: `Выводим результат пользователю: сумма ${vars['sum']} является нечётным числом.`,
        changes: [{ name: 'result', oldValue: vars['result'], newValue: 'Сумма нечётная' }],
        nextNodeId: 'n_end',
      }),
    },
    {
      id: 'n_end',
      type: 'end',
      text: 'Конец',
      x: 280,
      y: 530,
      width: 140,
      height: 48,
      execute: (vars) => ({
        explanation: `Алгоритм завершил работу. Итоговый результат: «${vars['result']}».`,
        changes: [],
        nextNodeId: null,
      }),
    },
  ],
  edges: [
    { id: 'e1', from: 'n_start', to: 'n_input' },
    { id: 'e2', from: 'n_input', to: 'n_sum' },
    { id: 'e3', from: 'n_sum', to: 'n_cond' },
    {
      id: 'e_cond_true',
      from: 'n_cond',
      to: 'n_even',
      label: 'Да',
      waypoints: [
        { x: 190, y: 320 },
        { x: 130, y: 320 },
        { x: 130, y: 404 },
      ],
    },
    {
      id: 'e_cond_false',
      from: 'n_cond',
      to: 'n_odd',
      label: 'Нет',
      waypoints: [
        { x: 370, y: 320 },
        { x: 430, y: 320 },
        { x: 430, y: 404 },
      ],
    },
    {
      id: 'e_even_end',
      from: 'n_even',
      to: 'n_end',
      waypoints: [
        { x: 130, y: 490 },
        { x: 280, y: 490 },
        { x: 280, y: 506 },
      ],
    },
    {
      id: 'e_odd_end',
      from: 'n_odd',
      to: 'n_end',
      waypoints: [
        { x: 430, y: 490 },
        { x: 280, y: 490 },
        { x: 280, y: 506 },
      ],
    },
  ],
};

/**
 * 2. ОСНОВНАЯ СХЕМА: «Расчёт стоимости покупки со скидкой»
 * 2 проверки условий, умножение, процент, вычитание
 */
export const mainFlowchart: FlowchartDefinition = {
  id: 'main',
  title: '2. Основная: Расчёт покупки со скидкой',
  typeCategory: 'main',
  badge: '2 условия · Комплексный разбор',
  goal: 'Освоить чтение многоуровневого ветвления: расчёт процента скидки и присвоение статуса доставки',
  description: 'Программа вычисляет общую стоимость total = price × qty. Если total > 1000 руб., скидка 20%, иначе 5%. После вычитания скидки проверяется итоговая сумма: при final_sum ≥ 1200 руб. присваивается VIP-доставка, иначе — Стандартная.',
  inputs: [
    { name: 'price', label: 'Цена за шт. (руб)', min: 10, max: 300, defaultValue: 120, description: 'Стоимость единицы товара (10..300 руб.)' },
    { name: 'qty', label: 'Количество (шт)', min: 1, max: 20, defaultValue: 10, description: 'Количество единиц товара (1..20 шт.)' },
  ],
  viewBox: { width: 620, height: 780 },
  nodes: [
    {
      id: 'm_start',
      type: 'start',
      text: 'Начало',
      x: 310,
      y: 35,
      width: 140,
      height: 44,
      next: 'm_input',
      execute: () => ({
        explanation: 'Старт программы оформления заказа.',
        changes: [],
        nextNodeId: 'm_input',
      }),
    },
    {
      id: 'm_input',
      type: 'input',
      text: 'Ввод price, qty',
      subtext: 'Цена и количество',
      x: 310,
      y: 105,
      width: 180,
      height: 48,
      next: 'm_total',
      execute: (vars) => ({
        explanation: `Получены параметры покупки: price = ${vars['price']} руб., qty = ${vars['qty']} шт.`,
        changes: [
          { name: 'price', oldValue: undefined, newValue: vars['price'] },
          { name: 'qty', oldValue: undefined, newValue: vars['qty'] },
        ],
        nextNodeId: 'm_total',
      }),
    },
    {
      id: 'm_total',
      type: 'process',
      text: 'total = price * qty',
      subtext: 'Предварительная стоимость',
      x: 310,
      y: 185,
      width: 210,
      height: 52,
      next: 'm_cond_disc',
      question: {
        id: 'q_main_total',
        prompt: 'Вопрос 1: Вычислите предварительную стоимость покупки total = price * qty',
        type: 'number',
        getCorrectAnswer: (vars) => Number(vars['price']) * Number(vars['qty']),
        explanationOnCorrect: 'Верно! Умножаем цену на количество единиц товара.',
        explanationOnWrong: 'Перемножьте текущую цену price на количество qty.',
      },
      execute: (vars) => {
        const p = Number(vars['price']);
        const q = Number(vars['qty']);
        const total = p * q;
        return {
          explanation: `Умножаем цену на количество: total = ${p} × ${q} = ${total} руб.`,
          formulaApplied: `total = ${p} * ${q} = ${total}`,
          changes: [{ name: 'total', oldValue: vars['total'], newValue: total, formula: `${p} * ${q} = ${total}` }],
          nextNodeId: 'm_cond_disc',
        };
      },
    },
    {
      id: 'm_cond_disc',
      type: 'condition',
      text: 'total > 1000 ?',
      subtext: 'Порог крупного заказа',
      x: 310,
      y: 285,
      width: 180,
      height: 66,
      nextTrue: 'm_disc_20',
      nextFalse: 'm_disc_5',
      question: {
        id: 'q_main_disc_branch',
        prompt: 'Вопрос 2: Положена ли повышенная скидка (total > 1000)? Какую ветку выбираем?',
        type: 'choice',
        options: ['Да (скидка 20%)', 'Нет (скидка 5%)'],
        getCorrectAnswer: (vars) => (Number(vars['total']) > 1000 ? 'Да (скидка 20%)' : 'Нет (скидка 5%)'),
        explanationOnCorrect: 'Правильно! Сравниваем total с 1000: при строгом превышении положено 20%.',
        explanationOnWrong: 'Обратите внимание: условие строгое (total > 1000).',
      },
      execute: (vars) => {
        const total = Number(vars['total']);
        const isBig = total > 1000;
        return {
          explanation: `Сравниваем ${total} > 1000. Результат: ${isBig ? 'Да (True) → скидка 20%' : 'Нет (False) → базовая скидка 5%'}.`,
          formulaApplied: `${total} > 1000 is ${isBig}`,
          changes: [],
          conditionResult: isBig,
          nextNodeId: isBig ? 'm_disc_20' : 'm_disc_5',
        };
      },
    },
    {
      id: 'm_disc_20',
      type: 'process',
      text: 'disc = total * 0.20',
      subtext: 'Скидка 20%',
      x: 140,
      y: 385,
      width: 190,
      height: 52,
      next: 'm_final_calc',
      execute: (vars) => {
        const total = Number(vars['total']);
        const disc = Math.round(total * 0.2 * 100) / 100;
        return {
          explanation: `Вычисляем 20% скидки: disc = ${total} × 0.20 = ${disc} руб.`,
          formulaApplied: `disc = ${total} * 0.20 = ${disc}`,
          changes: [{ name: 'disc', oldValue: vars['disc'], newValue: disc, formula: `${total} * 0.20 = ${disc}` }],
          nextNodeId: 'm_final_calc',
        };
      },
    },
    {
      id: 'm_disc_5',
      type: 'process',
      text: 'disc = total * 0.05',
      subtext: 'Скидка 5%',
      x: 480,
      y: 385,
      width: 190,
      height: 52,
      next: 'm_final_calc',
      execute: (vars) => {
        const total = Number(vars['total']);
        const disc = Math.round(total * 0.05 * 100) / 100;
        return {
          explanation: `Вычисляем 5% скидки: disc = ${total} × 0.05 = ${disc} руб.`,
          formulaApplied: `disc = ${total} * 0.05 = ${disc}`,
          changes: [{ name: 'disc', oldValue: vars['disc'], newValue: disc, formula: `${total} * 0.05 = ${disc}` }],
          nextNodeId: 'm_final_calc',
        };
      },
    },
    {
      id: 'm_final_calc',
      type: 'process',
      text: 'final_sum = total - disc',
      subtext: 'Итог к оплате',
      x: 310,
      y: 475,
      width: 220,
      height: 52,
      next: 'm_cond_vip',
      execute: (vars) => {
        const total = Number(vars['total']);
        const disc = Number(vars['disc']);
        const finalSum = Math.round((total - disc) * 100) / 100;
        return {
          explanation: `Вычитаем скидку: final_sum = ${total} − ${disc} = ${finalSum} руб.`,
          formulaApplied: `final_sum = ${total} - ${disc} = ${finalSum}`,
          changes: [{ name: 'final_sum', oldValue: vars['final_sum'], newValue: finalSum, formula: `${total} - ${disc} = ${finalSum}` }],
          nextNodeId: 'm_cond_vip',
        };
      },
    },
    {
      id: 'm_cond_vip',
      type: 'condition',
      text: 'final_sum >= 1200 ?',
      subtext: 'Порог VIP-доставки',
      x: 310,
      y: 565,
      width: 200,
      height: 66,
      nextTrue: 'm_out_vip',
      nextFalse: 'm_out_std',
      execute: (vars) => {
        const fs = Number(vars['final_sum']);
        const isVip = fs >= 1200;
        return {
          explanation: `Проверяем: ${fs} >= 1200 ? ${isVip ? 'Да → VIP-доставка' : 'Нет → Стандартная доставка'}.`,
          formulaApplied: `${fs} >= 1200 is ${isVip}`,
          changes: [],
          conditionResult: isVip,
          nextNodeId: isVip ? 'm_out_vip' : 'm_out_std',
        };
      },
    },
    {
      id: 'm_out_vip',
      type: 'output',
      text: 'Вывод: «VIP (Бесплатно)»',
      x: 140,
      y: 665,
      width: 200,
      height: 48,
      next: 'm_end',
      execute: (vars) => ({
        explanation: `Назначаем статус: VIP клиент с бесплатной курьерской доставкой. Итог: ${vars['final_sum']} руб.`,
        changes: [{ name: 'delivery', oldValue: vars['delivery'], newValue: 'VIP (Бесплатно)' }],
        nextNodeId: 'm_end',
      }),
    },
    {
      id: 'm_out_std',
      type: 'output',
      text: 'Вывод: «Стандартная»',
      x: 480,
      y: 665,
      width: 190,
      height: 48,
      next: 'm_end',
      execute: (vars) => ({
        explanation: `Назначаем статус: Стандартная доставка. Итог к оплате: ${vars['final_sum']} руб.`,
        changes: [{ name: 'delivery', oldValue: vars['delivery'], newValue: 'Стандартная доставка' }],
        nextNodeId: 'm_end',
      }),
    },
    {
      id: 'm_end',
      type: 'end',
      text: 'Конец',
      x: 310,
      y: 740,
      width: 130,
      height: 42,
      execute: (vars) => ({
        explanation: `Расчёт завершён. К оплате: ${vars['final_sum']} руб., доставка: «${vars['delivery']}».`,
        changes: [],
        nextNodeId: null,
      }),
    },
  ],
  edges: [
    { id: 'me1', from: 'm_start', to: 'm_input' },
    { id: 'me2', from: 'm_input', to: 'm_total' },
    { id: 'me3', from: 'm_total', to: 'm_cond_disc' },
    {
      id: 'me_cond1_true',
      from: 'm_cond_disc',
      to: 'm_disc_20',
      label: 'Да',
      waypoints: [
        { x: 220, y: 285 },
        { x: 140, y: 285 },
        { x: 140, y: 359 },
      ],
    },
    {
      id: 'me_cond1_false',
      from: 'm_cond_disc',
      to: 'm_disc_5',
      label: 'Нет',
      waypoints: [
        { x: 400, y: 285 },
        { x: 480, y: 285 },
        { x: 480, y: 359 },
      ],
    },
    {
      id: 'me_disc20_merge',
      from: 'm_disc_20',
      to: 'm_final_calc',
      waypoints: [
        { x: 140, y: 430 },
        { x: 310, y: 430 },
        { x: 310, y: 449 },
      ],
    },
    {
      id: 'me_disc5_merge',
      from: 'm_disc_5',
      to: 'm_final_calc',
      waypoints: [
        { x: 480, y: 430 },
        { x: 310, y: 430 },
        { x: 310, y: 449 },
      ],
    },
    { id: 'me_final_to_cond2', from: 'm_final_calc', to: 'm_cond_vip' },
    {
      id: 'me_cond2_true',
      from: 'm_cond_vip',
      to: 'm_out_vip',
      label: 'Да',
      waypoints: [
        { x: 210, y: 565 },
        { x: 140, y: 565 },
        { x: 140, y: 641 },
      ],
    },
    {
      id: 'me_cond2_false',
      from: 'm_cond_vip',
      to: 'm_out_std',
      label: 'Нет',
      waypoints: [
        { x: 410, y: 565 },
        { x: 480, y: 565 },
        { x: 480, y: 641 },
      ],
    },
    {
      id: 'me_vip_to_end',
      from: 'm_out_vip',
      to: 'm_end',
      waypoints: [
        { x: 140, y: 705 },
        { x: 310, y: 705 },
        { x: 310, y: 719 },
      ],
    },
    {
      id: 'me_std_to_end',
      from: 'm_out_std',
      to: 'm_end',
      waypoints: [
        { x: 480, y: 705 },
        { x: 310, y: 705 },
        { x: 310, y: 719 },
      ],
    },
  ],
};

/**
 * 3. ОСОБЫЙ СЛУЧАЙ: «Деление с защитой от деления на 0»
 * Граничное условие, проверка на ноль, целочисленное частное и остаток
 */
export const specialFlowchart: FlowchartDefinition = {
  id: 'special',
  title: '3. Особый случай: Деление с защитой от деления на 0',
  typeCategory: 'special',
  badge: 'Особый случай · Защита от сбоя',
  goal: 'Понять роль защитных проверок (guard clauses) и обработку особых ситуаций до вычислений',
  description: 'При делении чисел a на b недопустимо значение b = 0. Алгоритм сначала проверяет: если b = 0, сразу выдаётся предупреждение и алгоритм прерывается. Иначе вычисляются частное q = ⌊a / b⌋ и остаток r = a mod b с проверкой на делимость нацело.',
  inputs: [
    { name: 'a', label: 'Делимое a', min: 1, max: 100, defaultValue: 25, description: 'Число, которое делим (1..100)' },
    { name: 'b', label: 'Делитель b', min: 0, max: 20, defaultValue: 0, description: 'Число, на которое делим (0..20, попробуйте 0!)' },
  ],
  viewBox: { width: 620, height: 680 },
  nodes: [
    {
      id: 's_start',
      type: 'start',
      text: 'Начало',
      x: 310,
      y: 35,
      width: 140,
      height: 44,
      next: 's_input',
      execute: () => ({
        explanation: 'Старт алгоритма деления.',
        changes: [],
        nextNodeId: 's_input',
      }),
    },
    {
      id: 's_input',
      type: 'input',
      text: 'Ввод a, b',
      subtext: 'Делимое и делитель',
      x: 310,
      y: 105,
      width: 180,
      height: 48,
      next: 's_check_zero',
      execute: (vars) => ({
        explanation: `Получены числа: делимое a = ${vars['a']}, делитель b = ${vars['b']}.`,
        changes: [
          { name: 'a', oldValue: undefined, newValue: vars['a'] },
          { name: 'b', oldValue: undefined, newValue: vars['b'] },
        ],
        nextNodeId: 's_check_zero',
      }),
    },
    {
      id: 's_check_zero',
      type: 'condition',
      text: 'b = 0 ?',
      subtext: 'Проверка деления на 0',
      x: 310,
      y: 200,
      width: 180,
      height: 66,
      nextTrue: 's_err_div_zero',
      nextFalse: 's_calc_div',
      question: {
        id: 'q_spec_zero',
        prompt: 'Вопрос 1: Равен ли делитель b нулю? Безопасно ли выполнять деление?',
        type: 'choice',
        options: ['Да (b = 0, делить нельзя)', 'Нет (b ≠ 0, можно делить)'],
        getCorrectAnswer: (vars) => (Number(vars['b']) === 0 ? 'Да (b = 0, делить нельзя)' : 'Нет (b ≠ 0, можно делить)'),
        explanationOnCorrect: 'Верно! Если b = 0, операция a / b не определена в математике.',
        explanationOnWrong: 'Посмотрите на текущее значение переменной b в таблице.',
      },
      execute: (vars) => {
        const b = Number(vars['b']);
        const isZero = b === 0;
        return {
          explanation: `Проверяем b = ${b}. ${
            isZero
              ? 'Делитель равен 0! Переходим по ветви «Да» на обработку ошибки.'
              : 'Делитель не равен 0. Переходим по ветви «Нет» к арифметическим операциям.'
          }`,
          formulaApplied: `b === 0 (${isZero})`,
          changes: [],
          conditionResult: isZero,
          nextNodeId: isZero ? 's_err_div_zero' : 's_calc_div',
        };
      },
    },
    {
      id: 's_err_div_zero',
      type: 'output',
      text: 'Ошибка: деление на 0!',
      subtext: 'Аварийное завершение',
      x: 130,
      y: 330,
      width: 200,
      height: 52,
      next: 's_end',
      execute: (vars) => ({
        explanation: 'Критическая ситуация предотвращена: выводим сообщение об ошибке деления на ноль.',
        changes: [{ name: 'result', oldValue: vars['result'], newValue: 'Ошибка: деление на ноль!' }],
        nextNodeId: 's_end',
      }),
    },
    {
      id: 's_calc_div',
      type: 'process',
      text: 'q = ⌊a / b⌋, r = a mod b',
      subtext: 'Частное и остаток',
      x: 470,
      y: 330,
      width: 220,
      height: 54,
      next: 's_cond_rem',
      question: {
        id: 'q_spec_rem',
        prompt: 'Вопрос 2: Чему равен остаток r = a mod b?',
        type: 'number',
        getCorrectAnswer: (vars) => Number(vars['a']) % Number(vars['b']),
        explanationOnCorrect: 'Верно! Остаток от деления вычисляется операцией mod (%) в программировании.',
        explanationOnWrong: 'Вычислите a % b (остаток от деления a на b).',
      },
      execute: (vars) => {
        const a = Number(vars['a']);
        const b = Number(vars['b']);
        const q = Math.floor(a / b);
        const r = a % b;
        return {
          explanation: `Делим ${a} на ${b}: целая часть q = ${q}, остаток r = ${r}.`,
          formulaApplied: `q = floor(${a} / ${b}) = ${q}; r = ${a} % ${b} = ${r}`,
          changes: [
            { name: 'q', oldValue: vars['q'], newValue: q, formula: `⌊${a} / ${b}⌋ = ${q}` },
            { name: 'r', oldValue: vars['r'], newValue: r, formula: `${a} mod ${b} = ${r}` },
          ],
          nextNodeId: 's_cond_rem',
        };
      },
    },
    {
      id: 's_cond_rem',
      type: 'condition',
      text: 'r = 0 ?',
      subtext: 'Делимость нацело',
      x: 470,
      y: 440,
      width: 170,
      height: 64,
      nextTrue: 's_out_exact',
      nextFalse: 's_out_rem',
      execute: (vars) => {
        const r = Number(vars['r']);
        const isExact = r === 0;
        return {
          explanation: `Проверяем остаток r = ${r}: ${isExact ? 'Делится нацело (ветвь «Да»)' : 'Есть остаток (ветвь «Нет»)'}.`,
          formulaApplied: `r === 0 (${isExact})`,
          changes: [],
          conditionResult: isExact,
          nextNodeId: isExact ? 's_out_exact' : 's_out_rem',
        };
      },
    },
    {
      id: 's_out_exact',
      type: 'output',
      text: 'Вывод: «Нацело: q»',
      x: 350,
      y: 550,
      width: 180,
      height: 48,
      next: 's_end',
      execute: (vars) => ({
        explanation: `Результат: делится нацело, частное = ${vars['q']}.`,
        changes: [{ name: 'result', oldValue: vars['result'], newValue: `Нацело: частное ${vars['q']}` }],
        nextNodeId: 's_end',
      }),
    },
    {
      id: 's_out_rem',
      type: 'output',
      text: 'Вывод: «С остатком: q (ост. r)»',
      x: 540,
      y: 550,
      width: 180,
      height: 48,
      next: 's_end',
      execute: (vars) => ({
        explanation: `Результат: частное = ${vars['q']}, остаток = ${vars['r']}.`,
        changes: [{ name: 'result', oldValue: vars['result'], newValue: `${vars['q']} (остаток ${vars['r']})` }],
        nextNodeId: 's_end',
      }),
    },
    {
      id: 's_end',
      type: 'end',
      text: 'Конец',
      x: 310,
      y: 635,
      width: 130,
      height: 42,
      execute: (vars) => ({
        explanation: `Работа алгоритма завершена. Статус: ${vars['result']}.`,
        changes: [],
        nextNodeId: null,
      }),
    },
  ],
  edges: [
    { id: 'se1', from: 's_start', to: 's_input' },
    { id: 'se2', from: 's_input', to: 's_check_zero' },
    {
      id: 'se_zero_true',
      from: 's_check_zero',
      to: 's_err_div_zero',
      label: 'Да',
      waypoints: [
        { x: 220, y: 200 },
        { x: 130, y: 200 },
        { x: 130, y: 304 },
      ],
    },
    {
      id: 'se_zero_false',
      from: 's_check_zero',
      to: 's_calc_div',
      label: 'Нет',
      waypoints: [
        { x: 400, y: 200 },
        { x: 470, y: 200 },
        { x: 470, y: 303 },
      ],
    },
    {
      id: 'se_err_to_end',
      from: 's_err_div_zero',
      to: 's_end',
      waypoints: [
        { x: 130, y: 382 },
        { x: 130, y: 635 },
        { x: 245, y: 635 },
      ],
    },
    { id: 'se_calc_to_rem', from: 's_calc_div', to: 's_cond_rem' },
    {
      id: 'se_rem_true',
      from: 's_cond_rem',
      to: 's_out_exact',
      label: 'Да',
      waypoints: [
        { x: 385, y: 440 },
        { x: 350, y: 440 },
        { x: 350, y: 526 },
      ],
    },
    {
      id: 'se_rem_false',
      from: 's_cond_rem',
      to: 's_out_rem',
      label: 'Нет',
      waypoints: [
        { x: 555, y: 440 },
        { x: 540, y: 440 },
        { x: 540, y: 526 },
      ],
    },
    {
      id: 'se_exact_to_end',
      from: 's_out_exact',
      to: 's_end',
      waypoints: [
        { x: 350, y: 598 },
        { x: 350, y: 615 },
        { x: 310, y: 615 },
      ],
    },
    {
      id: 'se_rem_to_end',
      from: 's_out_rem',
      to: 's_end',
      waypoints: [
        { x: 540, y: 598 },
        { x: 540, y: 615 },
        { x: 375, y: 615 },
      ],
    },
  ],
};

/**
 * 4. САМОСТОЯТЕЛЬНАЯ ЗАДАЧА (ТИП 2: ПОДБОР ВХОДНЫХ ДАННЫХ)
 * Задача: подобрать входные данные, при которых выполняется заданная ветвь или получается заданный результат
 */
export const practiceTask: PracticeTask = {
  id: 'practice_discount_standard',
  title: 'Самостоятельная задача: Подбор входных данных (Тип 2)',
  description:
    'В схеме «Расчёт покупки со скидкой» подберите такие цену (price) и количество (qty), чтобы выполнялась ветвь повышенной скидки 20% (total > 1000 руб.), но при этом итоговая сумма к оплате (final_sum) оказалась строго меньше 1200 руб., обеспечив назначение «Стандартной доставки».',
  targetCondition: 'Ветвь скидки 20% (total > 1000) И ветвь «Стандартная доставка» (final_sum < 1200)',
  inputs: [
    { name: 'price', label: 'Цена за шт. (руб)', min: 50, max: 250, defaultValue: 100, description: 'Диапазон: 50..250 руб.' },
    { name: 'qty', label: 'Количество (шт)', min: 5, max: 20, defaultValue: 8, description: 'Диапазон: 5..20 шт.' },
  ],
  flowchart: mainFlowchart,
  validateInputs: (inputs: Variables) => {
    const price = Number(inputs['price']);
    const qty = Number(inputs['qty']);

    if (isNaN(price) || isNaN(qty) || price <= 0 || qty <= 0) {
      return {
        isSuccess: false,
        finalVariables: {},
        actualBranchOrResult: 'Недопустимые данные',
        explanation: 'Цена и количество должны быть положительными числами.',
        hint: 'Введите числа из допустимого диапазона (цена 50..250, количество 5..20).',
      };
    }

    const total = price * qty;
    const isBig = total > 1000;
    const disc = isBig ? Math.round(total * 0.2 * 100) / 100 : Math.round(total * 0.05 * 100) / 100;
    const finalSum = Math.round((total - disc) * 100) / 100;
    const isVip = finalSum >= 1200;
    const delivery = isVip ? 'VIP (Бесплатно)' : 'Стандартная доставка';

    const finalVars: Variables = {
      price,
      qty,
      total,
      disc,
      final_sum: finalSum,
      delivery,
    };

    // Проверяем критерии задачи:
    // 1) total > 1000
    // 2) final_sum < 1200
    if (!isBig) {
      return {
        isSuccess: false,
        finalVariables: finalVars,
        actualBranchOrResult: `total = ${total} руб. (скидка 5%, а не 20%)`,
        explanation: `Сумма total = ${price} × ${qty} = ${total} руб. не превышает 1000 руб. Сработала ветвь скидки 5% вместо 20%.`,
        hint: 'Увеличьте цену или количество, чтобы предварительная сумма total = price × qty была строго больше 1000.',
      };
    }

    if (isVip) {
      return {
        isSuccess: false,
        finalVariables: finalVars,
        actualBranchOrResult: `final_sum = ${finalSum} руб. (VIP-доставка)`,
        explanation: `Скидка 20% сработала (total = ${total} руб.), но итоговая сумма к оплате final_sum = ${finalSum} руб. превысила 1200 руб. В результате назначена VIP-доставка, а требовалась Стандартная!`,
        hint: 'Итоговая сумма после скидки 20% равна total × 0.8. Нам нужно, чтобы 1000 < total < 1500 (так как 1500 × 0.8 = 1200). Попробуйте уменьшить параметры так, чтобы total был, например, 1200 руб. (1200 - 20% = 960 руб. < 1200).',
      };
    }

    // Если оба условия выполнены:
    return {
      isSuccess: true,
      finalVariables: finalVars,
      actualBranchOrResult: `total = ${total} руб. (>1000, 20% скидка), final_sum = ${finalSum} руб. (<1200, Стандартная)`,
      explanation: `Отличная работа! При price = ${price} и qty = ${qty}: total = ${total} руб. (>1000, скидка 20% = ${disc} руб.), итоговая сумма final_sum = ${finalSum} руб. (<1200), поэтому оформлена Стандартная доставка!`,
    };
  },
};

export const allFlowcharts: Record<string, FlowchartDefinition> = {
  intro: introFlowchart,
  main: mainFlowchart,
  special: specialFlowchart,
};
