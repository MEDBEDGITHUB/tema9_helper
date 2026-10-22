/**
 * Модель типов для обучающего приложения «Объясни задачу — Блок-схемы»
 */

export type NodeType = 'start' | 'end' | 'input' | 'process' | 'condition' | 'output';

export type VariableValue = number | string | boolean;
export type Variables = Record<string, VariableValue>;

/** Изменение переменной на шаге выполнения */
export interface VariableChange {
  name: string;
  oldValue: VariableValue | undefined;
  newValue: VariableValue;
  formula?: string; // Например: "a + b = 7 + 14 = 21"
}

/** Вопрос для пользователя внутри шага */
export interface StepQuestion {
  id: string;
  prompt: string;
  type: 'choice' | 'number';
  options?: string[]; // Варианты для выбора
  getCorrectAnswer: (vars: Variables) => string | number;
  explanationOnCorrect: string;
  explanationOnWrong: string;
}

/** Результат вычисления шага узла */
export interface StepExecutionResult {
  explanation: string;
  formulaApplied?: string;
  changes: VariableChange[];
  nextNodeId: string | null;
  conditionResult?: boolean;
}

/** Узел блок-схемы */
export interface FlowchartNode {
  id: string;
  type: NodeType;
  text: string;
  subtext?: string;
  x: number;
  y: number;
  width: number;
  height: number;
  // Действие при попадании на узел
  execute?: (vars: Variables) => StepExecutionResult;
  // Условие (для NodeType === 'condition')
  conditionExpr?: (vars: Variables) => boolean;
  nextTrue?: string;
  nextFalse?: string;
  next?: string;
  // Интерактивный вопрос перед выполнением или в процессе
  question?: StepQuestion;
}

/** Ребро (стрелка) блок-схемы */
export interface FlowchartEdge {
  id: string;
  from: string;
  to: string;
  label?: 'Да' | 'Нет' | '';
  // Опциональные промежуточные точки для ломаных линий
  waypoints?: Array<{ x: number; y: number }>;
}

/** Описание входного параметра */
export interface InputDefinition {
  name: string;
  label: string;
  min: number;
  max: number;
  step?: number;
  defaultValue: number;
  description: string;
}

/** Блок-схема с полным набором данных */
export interface FlowchartDefinition {
  id: string;
  title: string;
  typeCategory: 'intro' | 'main' | 'special';
  badge: string;
  description: string;
  goal: string;
  inputs: InputDefinition[];
  nodes: FlowchartNode[];
  edges: FlowchartEdge[];
  viewBox: { width: number; height: number };
}

/** Вопрос в текущем состоянии пошагового выполнения */
export interface ActiveQuestionState {
  nodeId: string;
  question: StepQuestion;
  correctAnswer: string | number;
  userAnswer?: string | number;
  isResolved: boolean;
  attempts: number;
  feedback?: {
    isCorrect: boolean;
    message: string;
  };
}

/** Полное состояние пошагового выполнения */
export interface ExecutionState {
  flowchartId: string;
  currentNodeId: string;
  variables: Variables;
  initialInputs: Variables;
  history: Array<{
    stepNumber: number;
    nodeId: string;
    nodeText: string;
    explanation: string;
    changes: VariableChange[];
    variablesSnapshot: Variables;
  }>;
  visitedNodeIds: string[];
  visitedEdgeIds: string[];
  stepIndex: number;
  isComplete: boolean;
  activeQuestion: ActiveQuestionState | null;
}

/** Самостоятельная задача (Тип 2: подбор входных данных) */
export interface PracticeTask {
  id: string;
  title: string;
  description: string;
  targetCondition: string;
  inputs: InputDefinition[];
  flowchart: FlowchartDefinition;
  validateInputs: (inputs: Variables) => {
    isSuccess: boolean;
    finalVariables: Variables;
    actualBranchOrResult: string;
    explanation: string;
    hint?: string;
  };
}
