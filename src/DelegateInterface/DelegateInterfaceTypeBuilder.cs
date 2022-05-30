using System.Reflection;
using System.Reflection.Emit;

namespace DelegateInterface;

/// <summary>
/// Builds proxy types.
/// </summary>
/// <example>
/// If you have e.g.
/// <code><![CDATA[
/// interface IA
/// {
///     void M1();
///     string M2();
///     void M3(TimeSpan x);
///     int M4(int x, int y);
/// }
/// ]]></code>
///
/// <see cref="Build(Type)"/> method create a proxy type:
///
/// <code><![CDATA[
/// class IA_Proxy : IDynamicInterface, IA
/// {
///     public IDictionary<string, Delegate> Methods { get; }
///     public void M1() => Methods["M1"].DynamicInvoke();
///     public string M2() => (string)Methods["M2"].DynamicInvoke();
///     public void M3(TimeSpan x) => Methods["M3"].DynamicInvoke(x);
///     public int M4(int x, int y) => (int)Methods["M4"].DynamicInvoke(x, y);
/// }
/// ]]></code>
/// </example>
public class DelegateInterfaceTypeBuilder
{
    private readonly AssemblyBuilder _assembly;
    private readonly ModuleBuilder _module;

    public DelegateInterfaceTypeBuilder()
    {
        _assembly = AssemblyBuilder.DefineDynamicAssembly(new("DynamicAsm"), AssemblyBuilderAccess.Run);
        _module = _assembly.DefineDynamicModule("DynamicModule");
    }

    private static void Check(Type interfaceType)
    {
        if (!interfaceType.IsInterface) throw new InvalidOperationException("must be interface");

        foreach (var m in interfaceType.GetMethods())
        {
            if (m.ReturnType.IsByRef) throw new InvalidOperationException("ref return not supported");
            if (m.ReturnType.IsByRefLike) throw new InvalidOperationException("ref-like return not supported");

            foreach (var p in m.GetParameters())
            {
                if (p.IsIn) throw new InvalidOperationException("in parameter not supported");
                if (p.IsOut) throw new InvalidOperationException("out parameter not supported");
                if (p.ParameterType.IsByRef) throw new InvalidOperationException("ref parameter not supported");
                if (p.ParameterType.IsByRefLike) throw new InvalidOperationException("ref-like parameter not supported");
            }
        }
    }

    public Type Build(Type interfaceType)
    {
        Check(interfaceType);

        var typeName = interfaceType.Namespace + "." + interfaceType.Name + "_Proxy";

        // already built
        var t = _module.GetType(typeName);
        if (t is not null) return t;

        // build new
        var tb = _module.DefineType(typeName);

        var mapType = typeof(InterfaceToDelegateMap<>).MakeGenericType(interfaceType);
        var map = EmitDynamicInterface(tb, mapType);
        EmitInterface(tb, interfaceType, map);

        return tb.CreateType()!;
    }

    private static void EmitInterface(TypeBuilder tb, Type interfaceType, FieldBuilder map)
    {
        tb.AddInterfaceImplementation(interfaceType);

        foreach (var m in interfaceType.GetMethods())
        {
            EmitInterfaceMethod(tb, map, m);
        }
    }

    private static void EmitInterfaceMethod(TypeBuilder tb, FieldBuilder map, MethodInfo m)
    {
        var mb = tb.DefineMethod(
            m.Name,
            MethodAttributes.Public | MethodAttributes.Virtual,
            m.ReturnType,
            m.GetParameters().Select(p => p.ParameterType).ToArray());

        var il = mb.GetILGenerator();
        var r = m.ReturnType;

        var hasValueLabel = il.DefineLabel();
        var value = il.DeclareLocal(typeof(Delegate));

        // var local = this._map;
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, map);

        // var d = local.GetOrDefault(nameof(Method))
        il.Emit(OpCodes.Ldstr, m.Name);
        il.Emit(OpCodes.Ldloca, value);
        il.Emit(OpCodes.Callvirt, typeof(IDictionary<string, Delegate>).GetMethod("TryGetValue")!);

        // if (d is null) return;
        il.Emit(OpCodes.Brtrue_S, hasValueLabel);
        if (r != typeof(void)) il.Emit(OpCodes.Ldnull);
        il.Emit(OpCodes.Ret);

        il.MarkLabel(hasValueLabel);

        il.Emit(OpCodes.Ldloc, value);

        // var args = new object[] { arg1, arg2, ... };
        var parameters = m.GetParameters();
        il.Emit(OpCodes.Ldc_I4, parameters.Length);
        il.Emit(OpCodes.Newarr, typeof(object));
        for (int i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i].ParameterType;
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldc_I4, i);
            il.Emit(OpCodes.Ldarg, i + 1);
            if (p.IsValueType) il.Emit(OpCodes.Box, p);
            il.Emit(OpCodes.Stelem_Ref);
        }

        // d.DynamicInvoke(args);
        il.Emit(OpCodes.Callvirt, typeof(Delegate).GetMethod("DynamicInvoke")!);

        if (r == typeof(void)) il.Emit(OpCodes.Pop);
        else if (r.IsValueType) il.Emit(OpCodes.Unbox_Any, r);
        else il.Emit(OpCodes.Castclass, r);

        il.Emit(OpCodes.Ret);
    }

    private static FieldBuilder EmitDynamicInterface(TypeBuilder tb, Type mapType)
    {
        tb.AddInterfaceImplementation(typeof(IDelegateInterface));

        // private Map _map;
        var map = tb.DefineField("_map", mapType, FieldAttributes.Private);

        // public T() => _map = new();
        EmitDynamicInterfaceCtor(tb, mapType, map);

        // public Map Methods => _map;
        EmitMethodsProperty(tb, map);

        return map;
    }

    private static void EmitDynamicInterfaceCtor(TypeBuilder tb, Type mapType, FieldBuilder map)
    {
        var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
        var il = ctor.GetILGenerator();

        // this._map = new();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Newobj, mapType.GetConstructor(Array.Empty<Type>())!);
        il.Emit(OpCodes.Stfld, map);

        // : base()
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, typeof(object).GetConstructor(Array.Empty<Type>())!);

        il.Emit(OpCodes.Ret);
    }

    private static void EmitMethodsProperty(TypeBuilder tb, FieldBuilder map)
    {
        var m = tb.DefineMethod("get_Methods", MethodAttributes.Public | MethodAttributes.Virtual, typeof(IDictionary<string, Delegate>), Array.Empty<Type>());
        var il = m.GetILGenerator();

        // return this._map;
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, map);
        il.Emit(OpCodes.Ret);

        var p = tb.DefineProperty("Methods", PropertyAttributes.None, typeof(IDictionary<string, Delegate>), Array.Empty<Type>());
        p.SetGetMethod(m);
    }
}
