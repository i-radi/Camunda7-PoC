
using Microsoft.Extensions.DependencyInjection;
using muatamer_camunda_poc.Constants;
using muatamer_camunda_poc.Context;
using muatamer_camunda_poc.Services.GenericWorkflow;
using muatamer_camunda_poc.Services.MuatamerProcess;
using muatamer_camunda_poc.Services.PaymentProcess;

namespace muatamer_camunda_poc.HostedServices;

public class DeployWorkflows : IHostedService
{
    private readonly IWorkflowService _workflowService;
    private readonly IPaymentProcessService _paymentProcessService;
    private readonly IMuatamerProcessService _muatamerProcessService;

    public DeployWorkflows(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        _workflowService = scope.ServiceProvider.GetService<IWorkflowService>();
        _paymentProcessService = scope.ServiceProvider.GetService<IPaymentProcessService>();
        _muatamerProcessService = scope.ServiceProvider.GetService<IMuatamerProcessService>();
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _workflowService.Deploy(BPMNFile.PaymentProcessFile);
        //_paymentProcessService.StartWorkers();

        //await _workflowService.Deploy(BPMNFile.MuatamerProcessFile);
        //_muatamerProcessService.StartWorkers();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
