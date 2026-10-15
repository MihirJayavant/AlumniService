# AlumniService agent instructions

Read and follow [AGENTS.md](../AGENTS.md) and [README.md](../README.md) before repository work. Explicitly read applicable scoped AGENTS.md files before editing; do not assume nested instructions have been loaded.

Use the relevant workflow in [.agents/skills](../.agents/skills) for feature changes, migrations, local debugging, and tests. Keep these shared files as the source of repository conventions.

Custom VS Code roles are in [.github/agents](agents). Assign disjoint ownership when delegating and serialize restore, build, test, and EF commands in a shared checkout. Preserve unrelated local changes and never edit generated files under obj/.

Stay within the configured sandbox. If an action needs access outside it, provide the exact manual command or replacement text and wait for the user's result before dependent work. Never request elevation or bypass restrictions.
