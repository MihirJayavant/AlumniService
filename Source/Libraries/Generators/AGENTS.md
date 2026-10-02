# Record View Generator

This Roslyn incremental generator targets netstandard2.0; do not apply the solution's net10.0 target to it. Consumers reference it as an analyzer with `ReferenceOutputAssembly=false`.

`Core/RecordView.cs` defines the attribute contract. `RecordViewGenerator.cs` derives public properties from source records and emits required init properties into partial records. Check source models, attribute exclusions, generated type names, and manual mappers when changing generation.

Keep ordinary builds unattended. Debugger launch is opt-in via `ALUMNI_GENERATOR_DEBUG=1` in Debug builds; never set this in default agent commands. Validate generator edits by building the whole solution so consumers compile. Generated files are diagnostic artifacts, not source to commit.
