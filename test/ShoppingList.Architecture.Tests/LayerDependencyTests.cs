using NetArchTest.Rules;
using Shouldly;

namespace ShoppingList.Architecture.Tests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly DomainAssembly =
        typeof(ShoppingList.Domain.Entities.ShoppingList).Assembly;

    private static readonly System.Reflection.Assembly ApplicationAssembly =
        typeof(ShoppingList.Application.Handlers.CreateListHandler).Assembly;

    private static readonly System.Reflection.Assembly InfrastructureAssembly =
        typeof(ShoppingList.Infrastructure.Db.ApplicationContext).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_Application()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("ShoppingList.Application")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Infrastructure()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("ShoppingList.Infrastructure.Db")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Domain_ShouldNotDependOn_RestApi()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("ShoppingList.RestApi")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("ShoppingList.Infrastructure.Db")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOn_RestApi()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("ShoppingList.RestApi")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Application()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("ShoppingList.Application")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_RestApi()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("ShoppingList.RestApi")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }
}
