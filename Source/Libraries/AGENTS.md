# Domain and Shared Libraries

Keep handlers, validators, request/response records, mappings, and EF configuration within the owning feature. `Alumni.Student/Company` is a representative slice; faculty has its own domain library.

Read `Core/Handler.cs` before changing handler execution: callers use `Execute` to invoke validation and translate exceptions into `ErrorType`. Preserve cancellation propagation and existing `OneOf<TResponse, ErrorType>` contracts. `Alumni.Student/AddStudentHandler.cs` demonstrates validation, persistence, and mapping; do not copy its behavior without considering the requested feature.

`[RecordView]` request/response properties come from the referenced source record. Model changes can affect generated contracts and manual mappers together. Never edit generated output in `obj/`.

Read the more specific instructions for Infrastructure or Generators when working there. Build the whole solution after cross-project changes.
