# Code Requirement
- 这是Unity项目
- 只改动必要的部分，优先复用现有成熟代码，避免重复造轮子。
- 架构设计时让边界情况自然融入常规逻辑，而不是单独打补丁。
- 根据要求区分功能，创建新的代码文件时应当首先获得用户许可。
- 单个代码文件不超过 1000 行，否则应当进行功能拆分。
- 保持代码简单直观，不过度设计复杂架构方案。
- 代码应表达实际逻辑，结构清晰，不保留不再使用的代码，不留无用的混淆项，避免未来维护困惑。
- 在代码文件中撰写简明专业的代码注释。
- 新的代码文件写在`Assets/Scripts/` 文件夹中。
- 更改用户写或修改的代码需要先获得许可。
- 所有的script都会由我手动attach到object上，不要在代码里生成任何objects

# Documentation and Context Rules

## Context Files

This project uses four persistent markdown files under `Assets/Context/`:

- `Context/memory.md`
- `Context/plan.md`
- `Context/state.md`
- `Context/code_map.md`

You MUST read all four before starting any non-trivial task.

---

## Role of Each File

### 1. memory.md
Purpose:
- private working memory for the agent
- handoff notes
- temporary Context compression
- intermediate operational reminders

Rules:
- write for agent continuity, not for human readability
- may be concise and compact
- must not override `plan.md`, `state.md`, or `code_map.md`
- do not treat it as the authoritative source for goals or project truth

### 2. plan.md
Purpose:
- human-visible execution plan
- task priorities
- intended next steps
- scope decisions

Rules:
- treat `plan.md` as the authoritative source for what should be done next
- assume the human may edit this file directly
- do not silently replace human intent
- if implementation reality requires deviation, update `state.md` and propose a revision to `plan.md`

### 3. state.md
Purpose:
- human-visible current project state
- completed work
- current implementation status
- blockers and open issues

Rules:
- update `state.md` after any non-trivial implementation change
- keep it concise, accurate, and readable
- prefer facts over reasoning traces
- this file should reflect the current truth of the codebase

### 4. code_map.md
Purpose:
- human-visible explanation of the role of important code files
- module boundaries
- responsibilities of files and directories
- major relationships between components

Rules:
- update `code_map.md` whenever code structure changes materially
- write for human readability
- explain what each important file does, not just its name
- remove outdated entries when files are renamed, deleted, or repurposed

---

## Authority Rules

When conflicts exist, use this order of authority:

1. `Context/plan.md`
2. `Context/state.md`
3. `Context/code_map.md`
4. actual codebase
5. `Context/memory.md`

Additional rules:
- `memory.md` is never the final authority
- do not execute based on `memory.md` if it conflicts with `plan.md`
- if `plan.md` conflicts with the current codebase, update `state.md` to describe the mismatch and then align implementation to `plan.md` unless explicitly told otherwise

---

## Update Responsibilities

You MUST follow these update responsibilities:

- Human primarily edits: `Context/plan.md`
- Agent primarily edits: `Context/state.md`
- Agent primarily edits: `Context/code_map.md`
- Agent primarily edits: `Context/memory.md`

The human may edit any human-visible file at any time.

You MUST preserve human edits to:
- `Context/plan.md`
- `Context/state.md`
- `Context/code_map.md`

Do not overwrite human edits carelessly. Merge them when possible.

---

## Required Behavior Before Work

Before starting work, you MUST:

1. read `Context/plan.md`
2. read `Context/state.md`
3. read `Context/code_map.md`
4. read `Context/memory.md`
5. align the intended task with `plan.md`
6. use `state.md` and `code_map.md` to understand current implementation reality

If any file is missing, outdated, or contradictory, report it and repair the documentation before continuing when reasonable.

---

## Required Behavior After Work

After completing any non-trivial task, you MUST update:

- `Context/state.md`
- `Context/code_map.md` if file roles or structure changed
- `Context/memory.md`

Update `Context/plan.md` only when:
- a task has been completed and the next step should be marked clearly
- the human explicitly requested a plan change
- implementation reality forces a necessary plan adjustment

Do not rewrite the whole plan unnecessarily.

---

## Writing Style Rules

### For state.md
Use clear sections such as:
- Done
- In Progress
- Blockers
- Notes

### For plan.md
Use clear sections such as:
- Goal
- Priorities
- Next Steps
- Deferred

### For code_map.md
For each important file, describe:
- path
- purpose
- main dependencies
- important notes

Do not write vague summaries like "handles logic" or "main component".
Be specific.

### For memory.md
Keep it compact and operational.
It does not need to be polished for humans.

---

## File Change Tracking

When adding, deleting, renaming, or significantly repurposing code files, you MUST update `Context/code_map.md`.

When completing or changing implementation state, you MUST update `Context/state.md`.

When the human changes direction or priorities in `Context/plan.md`, you MUST follow the updated plan.

---

## Safety Against Drift

Do not let `memory.md` drift away from `state.md`.
Do not let `code_map.md` drift away from the codebase.
Do not let `state.md` claim work is done unless it is actually reflected in code.

# 任务处理指南
- 当需求不明确时，在继续之前向用户提出问题来澄清需求。
- 对于用户提出的设想和意见，结合项目情况进行全面客观的分析，给出建议，而不是一味遵循。
- 对于复杂任务，在分析和规划阶段先从宏观抽象层面厘清问题，再回到具体执行步骤。
- 对于复杂任务（例如涉及多个模块或跨领域变更） ，必须先制定分阶段实施计划并创建任务文
档， 然后按阶段使用 TodoWrite 规划并推进执行。 每完成一个阶段， 必须更新文档中的 TODO
状态，在`state.md`中更新。
- 当问题超过三次修改仍未解决时，应回到三次修改之前重新审视，分析并探索与失败尝试不
同的方案。

### 任务回顾

在任务完成呈现最终消息前，必须进行以下任务回顾：
- 并根据任务执行情况判断是否需要形成或更新文档/记忆：若产出新的可复用模式、跨文件约
束或关键决策，则写入对应文档；若仅影响局部实现且无长期价值，则无需写回。
- 如当前任务有对应文档，检查并更新对应文档内的 TODO 完成情况。如果TODO完成则将其删除。