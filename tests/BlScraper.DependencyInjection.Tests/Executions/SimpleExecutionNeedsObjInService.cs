namespace BlScraper.DependencyInjection.Tests.Executions;

internal class SimpleExecutionNeedsObjInService : SimpleExecution
{
    public readonly IServiceNeedsObj Service;

    /// <summary>
    /// Instance with a service
    /// </summary>
    /// <param name="service">service</param>
    /// <exception cref="ArgumentNullException">argument null</exception>
    public SimpleExecutionNeedsObjInService(IServiceNeedsObj service)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(Service));
        Service = service;
    }
}