using System;
using AriesContador.Core.Services;
using Xunit;

namespace Aries.Desktop.Tests
{
    public class FinancialServiceContractTests
    {
        [Fact]
        public void IFinancialService_avoids_ValueTuple_so_net48_can_call_it()
        {
            foreach (var method in typeof(IFinancialService).GetMethods())
            {
                Assert.False(UsesValueTuple(method.ReturnType), method.Name);
                foreach (var parameter in method.GetParameters())
                    Assert.False(UsesValueTuple(parameter.ParameterType), method.Name);
            }
        }

        private static bool UsesValueTuple(Type type)
        {
            if (type == null)
                return false;
            if (type.Name.StartsWith("ValueTuple", StringComparison.Ordinal))
                return true;
            if (!type.IsGenericType)
                return false;
            foreach (var arg in type.GetGenericArguments())
            {
                if (UsesValueTuple(arg))
                    return true;
            }
            return false;
        }
    }
}
