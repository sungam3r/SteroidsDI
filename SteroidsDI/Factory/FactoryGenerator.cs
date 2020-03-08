using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SteroidsDI
{
    /// <summary>
    /// The class generates a factory using the specified factory interface. The factory implementation delegates
    /// resolve of objects to the appropriate <see cref="IServiceProvider" />.
    /// </summary>
    /// <remarks> See the manually written IMegaFactory_Generated example class in the test assembly. </remarks>
    internal static class FactoryGenerator
    {
        private static readonly AssemblyBuilder _asmBuilder;
        private static readonly ModuleBuilder _moduleBuilder;

        static FactoryGenerator()
        {
            var asmName = new AssemblyName("DynamicAssembly_Factory_Projections");
            _asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _asmBuilder.DefineDynamicModule(asmName.Name);
        }

        private static void AssertType(Type type)
        {
            if (!type.IsInterface || !type.IsPublic)
                throw new InvalidOperationException($"Type '{type}' must be a public interface in order to be able to build a projection.");

            foreach (var member in type.GetMembers())
            {
                if (member.MemberType != MemberTypes.Method)
                    throw new InvalidOperationException($"A member {member.MemberType} was found in the interface '{type}': '{member.Name}'. Only methods are supported.");

                var parameters = (member as MethodInfo)!.GetParameters();
                if (parameters.Length > 1)
                    throw new InvalidOperationException($"The {member.Name} method with an invalid signature was detected in the interface '{type}'. Methods without parameters and methods with a single parameter are supported.");
            }
        }

        /// <summary> Generate a type that implements the specified factory. </summary>
        /// <param name="factoryType"> Factory type. </param>
        /// <remarks> Works both for .NET Framework and .NET Core. </remarks>
        /// <returns> A type that can be used as an implementation type for <paramref name="factoryType" /> in DI. </returns>
        public static Type Generate(Type factoryType)
        {
            if (factoryType == null)
                throw new ArgumentNullException(nameof(factoryType));

            var alreadyGeneratedType = _moduleBuilder.Assembly.DefinedTypes.FirstOrDefault(t => t.GetInterfaces().Contains(factoryType));
            if (alreadyGeneratedType != null)
                return alreadyGeneratedType;

            AssertType(factoryType);

            var typeBuilder = _moduleBuilder
                .DefineType($"{factoryType.Name.Replace('`', '_')}_DynamicFactory_{Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)}", TypeAttributes.NotPublic | TypeAttributes.Sealed, typeof(object), new[] { factoryType })
                .Generate_Ctor(out var providerField, out var bindingsField, out var optionsField);

            foreach (var method in factoryType.GetMethods())
                typeBuilder.Generate_Factory_Method(method, providerField, bindingsField, optionsField);

            return typeBuilder.CreateTypeInfo()!;
        }

        private static TypeBuilder Generate_Ctor(this TypeBuilder typeBuilder, out FieldBuilder providerField, out FieldBuilder bindingsField, out FieldBuilder optionsField)
        {
            providerField = typeBuilder.DefineField("_provider", typeof(IServiceProvider), FieldAttributes.Private);
            bindingsField = typeBuilder.DefineField("_bindings", typeof(List<NamedBinding>), FieldAttributes.Private);
            optionsField = typeBuilder.DefineField("_options", typeof(ServiceProviderAdvancedOptions), FieldAttributes.Private);

            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IServiceProvider), typeof(IEnumerable<NamedBinding>), typeof(ServiceProviderAdvancedOptions) });

            var ctorIL = ctorBuilder.GetILGenerator();

            ctorIL.Emit(OpCodes.Ldarg_0); // this
            ctorIL.Emit(OpCodes.Ldarg_1); // provider arg
            ctorIL.Emit(OpCodes.Stfld, providerField);

            ctorIL.Emit(OpCodes.Ldarg_0); // this
            ctorIL.Emit(OpCodes.Ldarg_2); // bindings arg
            ctorIL.Emit(OpCodes.Call, typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(typeof(NamedBinding)));
            ctorIL.Emit(OpCodes.Stfld, bindingsField);

            ctorIL.Emit(OpCodes.Ldarg_0); // this
            ctorIL.Emit(OpCodes.Ldarg_3); // options arg
            ctorIL.Emit(OpCodes.Stfld, optionsField);

            ctorIL.Emit(OpCodes.Ret);

            return typeBuilder;
        }

        private static TypeBuilder Generate_Factory_Method(this TypeBuilder typeBuilder, MethodInfo method, FieldBuilder providerField, FieldBuilder bindingsField, FieldBuilder optionsField)
        {
            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot,
                method.ReturnType,
                Type.EmptyTypes);

            var parameter = method.GetParameters().FirstOrDefault();

            var ilGenerator = methodBuilder.GetILGenerator();

            if (parameter == null)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, providerField);
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, optionsField);
                ilGenerator.Emit(OpCodes.Call, typeof(Resolver).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(m => m.Name == nameof(Resolver.Resolve) && m.IsGenericMethod == true).MakeGenericMethod(method.ReturnType));
                ilGenerator.Emit(OpCodes.Ret);
            }
            else
            {
                methodBuilder.SetParameters(parameter.ParameterType);

                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, providerField);
                ilGenerator.Emit(OpCodes.Ldarg_1); // name arg
                if (parameter.ParameterType.IsValueType)
                    ilGenerator.Emit(OpCodes.Box, parameter.ParameterType);
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, bindingsField);
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, optionsField);
                ilGenerator.Emit(OpCodes.Call, typeof(Resolver).GetMethod(nameof(Resolver.ResolveByNamedBinding), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(method.ReturnType));
                ilGenerator.Emit(OpCodes.Ret);
            }

            return typeBuilder;
        }
    }
}
