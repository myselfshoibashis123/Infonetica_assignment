using InfoneticaWorkflow.Models;
using InfoneticaWorkflow.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/workflows", (WorkflowDefinition defn) =>
{
    try { return Results.Ok(WorkflowService.CreateWorkflow(defn)); }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
});

app.MapGet("/workflows/{id}", (string id) =>
{
    try { return Results.Ok(WorkflowService.GetWorkflow(id)); }
    catch (Exception ex) { return Results.NotFound(ex.Message); }
});

app.MapPost("/instances", (string workflowId) =>
{
    try { return Results.Ok(WorkflowService.StartInstance(workflowId)); }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
});

app.MapGet("/instances/{id}", (string id) =>
{
    try { return Results.Ok(WorkflowService.GetInstance(id)); }
    catch (Exception ex) { return Results.NotFound(ex.Message); }
});

app.MapPost("/instances/{id}/actions", (string id, string actionId) =>
{
    try { return Results.Ok(WorkflowService.ExecuteAction(id, actionId)); }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
});

app.Run();
