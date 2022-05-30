using System.Reflection;
using System.Reflection.Emit;
using Map = System.Collections.Generic.Dictionary<string, System.Delegate>;

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

    public Type Build(Type interfaceType)
    {
        var tb = _module.DefineType(interfaceType.Name + "_Proxy");

        var map = EmitDynamicInterface(tb);
        EmitInterface(interfaceType, tb, map);

        return tb.CreateType()!;
    }

    private static void EmitInterface(Type t, TypeBuilder tb, FieldBuilder map)
    {
        tb.AddInterfaceImplementation(t);

        foreach (var m in t.GetMethods())
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
        il.Emit(OpCodes.Callvirt, typeof(Map).GetMethod(nameof(Map.TryGetValue))!);

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

    private static FieldBuilder EmitDynamicInterface(TypeBuilder tb)
    {
        tb.AddInterfaceImplementation(typeof(IDelegateInterface));

        // private Map _map;
        var map = tb.DefineField("_map", typeof(Map), FieldAttributes.Private);

        // public T() => _map = new();
        EmitDynamicInterfaceCtor(tb, map);

        // public Map Methods => _map;
        EmitMethodsProperty(tb, map);

        return map;
    }

    private static void EmitDynamicInterfaceCtor(TypeBuilder tb, FieldBuilder map)
    {
        var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
        var il = ctor.GetILGenerator();

        // this._map = new();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Newobj, typeof(Map).GetConstructor(Array.Empty<Type>())!);
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
