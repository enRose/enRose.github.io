using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Barin.API.DelegateTypeFactory
{
    public class DelegateTypeFactory
    {
        private readonly ModuleBuilder moduleBuilder;

        public DelegateTypeFactory()
        {
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("DelegateTypeFactory"), 
                AssemblyBuilderAccess.RunAndCollect);

            moduleBuilder = assembly.DefineDynamicModule("DelegateTypeFactory");
        }

        public Type CreateDelegateType(MethodInfo method)
        {
            var nameBase = $"{method.DeclaringType.Name}{method.Name}";

            var name = GetUniqueName(nameBase);

            var typeBuilder = moduleBuilder.DefineType(
                name, TypeAttributes.Sealed | TypeAttributes.Public, 
                typeof(MulticastDelegate));

            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard, 
                new[] { typeof(object), typeof(IntPtr) });

            constructor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            var parameters = method.GetParameters();

            var invokeMethod = typeBuilder.DefineMethod(
                "Invoke", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
                method.ReturnType, parameters.Select(p => p.ParameterType)
                .ToArray());
            
            invokeMethod.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                invokeMethod.DefineParameter(i + 1, ParameterAttributes.None, parameter.Name);
            }

            return typeBuilder.CreateType();
        }

        private string GetUniqueName(string nameBase)
        {
            int number = 2;
            string name = nameBase;
            while (moduleBuilder.GetType(name) != null)
                name = nameBase + number++;
            return name;
        }
    }
}
