namespace InfoneticaWorkflow.Models;

public class WorkflowInstance
{
    public string Id { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentState { get; set; }
    public List<ActionHistory> History { get; set; } = new();
}
