using InfoneticaWorkflow.Models;

namespace InfoneticaWorkflow.Storage;

public static class InMemoryStore
{
    public static Dictionary<string, WorkflowDefinition> WorkflowDefinitions { get; } = new();
    public static Dictionary<string, WorkflowInstance> WorkflowInstances { get; } = new();
}
