# AsyncPatterns

C# examples of asynchronous and concurrent programming patterns.

## Patterns Included

### Locks
- Standard lock synchronization
- Reader-writer lock optimization
- Thread-safe collections

### IO
- Task-based async programming
- Async iterators (IAsyncEnumerable)
- Custom async enumeration

### Signals
- ManualResetEventSlim for thread coordination 
- Task.WhenAny for task completion

### Events
- Asynchronous event publishing
- Event subscription patterns

### Compute
- Parallel.For data processing
- PLINQ for parallel queries

## Requirements

- .NET 9.0
- C# Latest

## Usage

Run tests to see patterns in action:

```bash
dotnet test
```

Run specific pattern demo:

```bash
dotnet test --filter "FullyQualifiedName~ThreadBound.Locks.RegularLockTests"
```