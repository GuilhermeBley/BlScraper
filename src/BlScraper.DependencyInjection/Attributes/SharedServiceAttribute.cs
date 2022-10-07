namespace BlScraper.DependencyInjection.Attributes;

/// <summary>
/// References shared service to model
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class SharedServiceAttribute : Attribute
{
}