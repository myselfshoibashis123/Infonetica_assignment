using InfoneticaWorkflow.Models;
using InfoneticaWorkflow.Storage;

namespace InfoneticaWorkflow.Services;

public static class WorkflowService
{
    public static WorkflowDefinition CreateWorkflow(WorkflowDefinition defn)
    {
        ValidateWorkflow(defn);

        if (InMemoryStore.WorkflowDefinitions.ContainsKey(defn.Id))
            throw new Exception("Workflow with this ID already exists.");

        InMemoryStore.WorkflowDefinitions[defn.Id] = defn;
        return defn;
    }

    public static WorkflowDefinition GetWorkflow(string id)
    {
        if (!InMemoryStore.WorkflowDefinitions.TryGetValue(id, out var defn))
            throw new Exception("Workflow not found.");

        return defn;
    }

    public static WorkflowInstance StartInstance(string workflowId)
    {
        var defn = GetWorkflow(workflowId);
        var initial = defn.States.FirstOrDefault(s => s.IsInitial);

        if (initial == null)
            throw new Exception("No initial state defined.");

        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid().ToString(),
            WorkflowId = workflowId,
            CurrentState = initial.Id
        };

        InMemoryStore.WorkflowInstances[instance.Id] = instance;
        return instance;
    }

    public static WorkflowInstance GetInstance(string id)
    {
        if (!InMemoryStore.WorkflowInstances.TryGetValue(id, out var inst))
            throw new Exception("Instance not found.");

        return inst;
    }

    public static WorkflowInstance ExecuteAction(string instanceId, string actionId)
    {
        var instance = GetInstance(instanceId);
        var defn = GetWorkflow(instance.WorkflowId);
        var currentState = instance.CurrentState;

        if (defn.States.First(s => s.Id == currentState).IsFinal)
            throw new Exception("Cannot transition from a final state.");

        var action = defn.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null) throw new Exception("Action not found.");
        if (!action.Enabled) throw new Exception("Action is disabled.");
        if (!action.FromStates.Contains(currentState))
            throw new Exception("Action not valid from current state.");

        instance.CurrentState = action.ToState;
        instance.History.Add(new ActionHistory
        {
            ActionId = action.Id,
            Timestamp = DateTime.UtcNow
        });

        return instance;
    }

    private static void ValidateWorkflow(WorkflowDefinition defn)
    {
        var stateIds = defn.States.Select(s => s.Id).ToHashSet();
        if (defn.States.Count(s => s.IsInitial) != 1)
            throw new Exception("Workflow must have exactly one initial state.");
        if (stateIds.Count != defn.States.Count)
            throw new Exception("Duplicate state IDs found.");
        if (defn.Actions.Select(a => a.Id).ToHashSet().Count != defn.Actions.Count)
            throw new Exception("Duplicate action IDs found.");

        foreach (var action in defn.Actions)
        {
            if (!stateIds.Contains(action.ToState))
                throw new Exception($"Invalid toState in action {action.Id}");
            foreach (var fs in action.FromStates)
                if (!stateIds.Contains(fs))
                    throw new Exception($"Invalid fromState '{fs}' in action {action.Id}");
        }
    }
}
