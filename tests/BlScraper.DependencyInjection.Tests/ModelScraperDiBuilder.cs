using BlScraper.DependencyInjection.Model;
using BlScraper.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BlScraper.DependencyInjection.Tests;

//
// Tests sintaxe: MethodName_ExpectedBehavior_StateUnderTest
// Example: isAdult_AgeLessThan18_False
//


public class ModelScraperDiBuilder
{
    [Fact]
    public async Task Test()
    {
        await Task.CompletedTask;
    }
}