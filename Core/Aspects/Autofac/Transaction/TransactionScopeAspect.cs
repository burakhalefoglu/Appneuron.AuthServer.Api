using System.Transactions;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;

namespace Core.Aspects.Autofac.Transaction;

/// <summary>
///     TransactionScopeAspect
/// </summary>
public class TransactionScopeAspect : MethodInterceptionAttribute
{
    public override void Intercept(IInvocation invocation)
    {
        using var transactionScope = new TransactionScope();
        try
        {
            invocation.Proceed();
            transactionScope.Complete();
        }
        catch (System.Exception ex)
        {
            _ = ex.ToString();
            throw;
        }
    }
}