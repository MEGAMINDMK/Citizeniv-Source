// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Reflection.TracingILGenerator
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MsgPack.Serialization.Reflection
{
  internal sealed class TracingILGenerator : IDisposable
  {
    private static readonly MethodInfo _type_GetTypeFromHandle = typeof (Type).GetMethod("GetTypeFromHandle", new Type[1]
    {
      typeof (RuntimeTypeHandle)
    });
    private readonly Dictionary<LocalBuilder, string> _localDeclarations = new Dictionary<LocalBuilder, string>();
    private readonly Dictionary<Label, string> _labels = new Dictionary<Label, string>();
    private readonly Stack<Label> _endOfExceptionBlocks = new Stack<Label>();
    private string _indentChars = "  ";
    private readonly ILGenerator _underlying;
    private readonly TextWriter _realTrace;
    private readonly TextWriter _trace;
    private readonly StringBuilder _traceBuffer;
    private readonly Label _endOfMethod;
    private bool _isInDynamicMethod;
    private int _indentLevel;
    private int _lineNumber;
    private bool _isEnded;
    private readonly bool _isDebuggable;

    public void EmitAnyCall(MethodInfo target)
    {
      if (target.IsStatic || target.DeclaringType.IsValueType)
        this.EmitCall(target);
      else
        this.EmitCallvirt(target);
    }

    public void EmitGetProperty(PropertyInfo property)
    {
      this.EmitAnyCall(property.GetGetMethod(true));
    }

    public void EmitCallConstructor(ConstructorInfo constructor)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Call);
      this.TraceWrite(" ");
      this.TraceOperand((MethodBase) constructor);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Call, constructor);
    }

    public void EmitAnyLdarg(int argumentIndex)
    {
      switch (argumentIndex)
      {
        case 0:
          this.EmitLdarg_0();
          break;
        case 1:
          this.EmitLdarg_1();
          break;
        case 2:
          this.EmitLdarg_2();
          break;
        case 3:
          this.EmitLdarg_3();
          break;
        default:
          if ((int) sbyte.MinValue <= argumentIndex && argumentIndex <= (int) sbyte.MaxValue)
          {
            this.EmitLdarg_S((byte) argumentIndex);
            break;
          }
          this.EmitLdarg(argumentIndex);
          break;
      }
    }

    public void EmitAnyLdarga(int argumentIndex)
    {
      if ((int) sbyte.MinValue <= argumentIndex && argumentIndex <= (int) sbyte.MaxValue)
        this.EmitLdarga_S((byte) argumentIndex);
      else
        this.EmitLdarga(argumentIndex);
    }

    public void EmitAnyLdloc(int localIndex)
    {
      switch (localIndex)
      {
        case 0:
          this.EmitLdloc_0();
          break;
        case 1:
          this.EmitLdloc_1();
          break;
        case 2:
          this.EmitLdloc_2();
          break;
        case 3:
          this.EmitLdloc_3();
          break;
        default:
          if ((int) sbyte.MinValue <= localIndex && localIndex <= (int) sbyte.MaxValue)
          {
            this.EmitLdloc_S((byte) localIndex);
            break;
          }
          this.EmitLdloc(localIndex);
          break;
      }
    }

    public void EmitAnyLdloc(LocalBuilder local)
    {
      this.EmitAnyLdloc(local.LocalIndex);
    }

    public void EmitAnyLdloca(int localIndex)
    {
      if ((int) sbyte.MinValue <= localIndex && localIndex <= (int) sbyte.MaxValue)
        this.EmitLdloca_S((byte) localIndex);
      else
        this.EmitLdloca(localIndex);
    }

    public void EmitAnyLdloca(LocalBuilder local)
    {
      this.EmitAnyLdloca(local.LocalIndex);
    }

    public void EmitAnyLdc_I4(int value)
    {
      switch (value)
      {
        case -1:
          this.EmitLdc_I4_M1();
          break;
        case 0:
          this.EmitLdc_I4_0();
          break;
        case 1:
          this.EmitLdc_I4_1();
          break;
        case 2:
          this.EmitLdc_I4_2();
          break;
        case 3:
          this.EmitLdc_I4_3();
          break;
        case 4:
          this.EmitLdc_I4_4();
          break;
        case 5:
          this.EmitLdc_I4_5();
          break;
        case 6:
          this.EmitLdc_I4_6();
          break;
        case 7:
          this.EmitLdc_I4_7();
          break;
        default:
          if ((int) sbyte.MinValue <= value && value <= (int) sbyte.MaxValue)
          {
            this.EmitLdc_I4_S((byte) value);
            break;
          }
          this.EmitLdc_I4(value);
          break;
      }
    }

    public void EmitAnyStloc(int localIndex)
    {
      switch (localIndex)
      {
        case 0:
          this.EmitStloc_0();
          break;
        case 1:
          this.EmitStloc_1();
          break;
        case 2:
          this.EmitStloc_2();
          break;
        case 3:
          this.EmitStloc_3();
          break;
        default:
          if ((int) sbyte.MinValue <= localIndex && localIndex <= (int) sbyte.MaxValue)
          {
            this.EmitStloc_S((byte) localIndex);
            break;
          }
          this.EmitStloc(localIndex);
          break;
      }
    }

    public void EmitAnyStloc(LocalBuilder local)
    {
      this.EmitAnyStloc(local.LocalIndex);
    }

    public void EmitNewarr(Type elementType, long length)
    {
      this.EmitNewarrCore(elementType, length);
    }

    private void EmitNewarrCore(Type elementType, long length)
    {
      this.EmitLiteralInteger(length);
      this.EmitNewarr(elementType);
    }

    public void EmitAnyStelem(
      Type elementType,
      Action<TracingILGenerator> arrayLoadingEmitter,
      Action<TracingILGenerator> indexEmitter,
      Action<TracingILGenerator> elementLoadingEmitter)
    {
      arrayLoadingEmitter(this);
      indexEmitter(this);
      elementLoadingEmitter(this);
      if (elementType.IsGenericParameter)
        this.EmitStelem(elementType);
      else if (!elementType.IsValueType)
      {
        this.EmitStelem_Ref();
      }
      else
      {
        switch (Type.GetTypeCode(elementType))
        {
          case TypeCode.Boolean:
          case TypeCode.SByte:
          case TypeCode.Byte:
            this.EmitStelem_I1();
            break;
          case TypeCode.Char:
          case TypeCode.Int16:
          case TypeCode.UInt16:
            this.EmitStelem_I2();
            break;
          case TypeCode.Int32:
          case TypeCode.UInt32:
            this.EmitStelem_I4();
            break;
          case TypeCode.Int64:
          case TypeCode.UInt64:
            this.EmitStelem_I8();
            break;
          case TypeCode.Single:
            this.EmitStelem_R4();
            break;
          case TypeCode.Double:
            this.EmitStelem_R8();
            break;
          default:
            this.EmitLdelema(elementType);
            elementLoadingEmitter(this);
            this.EmitStobj(elementType);
            break;
        }
      }
    }

    public void EmitLiteralInteger(long value)
    {
      switch (value)
      {
        case -1:
          this.EmitLdc_I4_M1();
          break;
        case 0:
          this.EmitLdc_I4_0();
          break;
        case 1:
          this.EmitLdc_I4_1();
          break;
        case 2:
          this.EmitLdc_I4_2();
          break;
        case 3:
          this.EmitLdc_I4_3();
          break;
        case 4:
          this.EmitLdc_I4_4();
          break;
        case 5:
          this.EmitLdc_I4_5();
          break;
        case 6:
          this.EmitLdc_I4_6();
          break;
        case 7:
          this.EmitLdc_I4_7();
          break;
        case 8:
          this.EmitLdc_I4_8();
          break;
        default:
          if ((long) sbyte.MinValue <= value && value <= (long) sbyte.MaxValue)
          {
            this.EmitLdc_I4_S((byte) value);
            break;
          }
          if ((long) int.MinValue <= value && value <= (long) int.MaxValue)
          {
            this.EmitLdc_I4((int) value);
            break;
          }
          this.EmitLdc_I8(value);
          break;
      }
    }

    public void EmitTypeOf(Type type)
    {
      this.EmitLdtoken(type);
      this.EmitCall(TracingILGenerator._type_GetTypeFromHandle);
    }

    public Label EndOfMethod
    {
      get
      {
        return this._endOfMethod;
      }
    }

    public bool IsInExceptionBlock
    {
      get
      {
        return 0 < this._endOfExceptionBlocks.Count;
      }
    }

    public bool IsEnded
    {
      get
      {
        return this._isEnded;
      }
    }

    public TracingILGenerator(DynamicMethod dynamicMethod, TextWriter traceWriter)
      : this((MethodInfo) dynamicMethod != (MethodInfo) null ? dynamicMethod.GetILGenerator() : (ILGenerator) null, true, traceWriter, false)
    {
    }

    public TracingILGenerator(
      MethodBuilder methodBuilder,
      TextWriter traceWriter,
      bool isDebuggable)
      : this((MethodInfo) methodBuilder != (MethodInfo) null ? methodBuilder.GetILGenerator() : (ILGenerator) null, false, traceWriter, isDebuggable)
    {
    }

    public TracingILGenerator(
      ConstructorBuilder constructorBuilder,
      TextWriter traceWriter,
      bool isDebuggable)
      : this((ConstructorInfo) constructorBuilder != (ConstructorInfo) null ? constructorBuilder.GetILGenerator() : (ILGenerator) null, false, traceWriter, isDebuggable)
    {
    }

    private TracingILGenerator(
      ILGenerator underlying,
      bool isInDynamicMethod,
      TextWriter traceWriter,
      bool isDebuggable)
    {
      this._underlying = underlying;
      this._realTrace = traceWriter ?? TextWriter.Null;
      this._traceBuffer = traceWriter != null ? new StringBuilder() : (StringBuilder) null;
      this._trace = traceWriter != null ? (TextWriter) new StringWriter(this._traceBuffer, (IFormatProvider) CultureInfo.InvariantCulture) : TextWriter.Null;
      this._isInDynamicMethod = isInDynamicMethod;
      this._endOfMethod = underlying == null ? new Label() : underlying.DefineLabel();
      this._isDebuggable = isDebuggable;
    }

    public void Dispose()
    {
      this._trace.Dispose();
    }

    public void EmitRet()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ret);
      this._underlying.Emit(OpCodes.Ret);
      this.FlushTrace();
      this._isEnded = true;
    }

    public void FlushTrace()
    {
      if (this._traceBuffer == null || this._traceBuffer.Length <= 0)
        return;
      this.TraceLocals();
      this._trace.Flush();
      this._realTrace.Write((object) this._traceBuffer);
      this._traceBuffer.Clear();
    }

    public LocalBuilder DeclareLocal(Type localType)
    {
      return this.DeclareLocalCore(localType, (string) null);
    }

    public LocalBuilder DeclareLocal(Type localType, string name)
    {
      return this.DeclareLocalCore(localType, name);
    }

    private LocalBuilder DeclareLocalCore(Type localType, string name)
    {
      LocalBuilder key = this._underlying.DeclareLocal(localType);
      this._localDeclarations.Add(key, name);
      if (!this._isInDynamicMethod)
      {
        if (this._isDebuggable)
        {
          try
          {
            key.SetLocalSymInfo(name);
          }
          catch (NotSupportedException ex)
          {
            this._isInDynamicMethod = true;
          }
        }
      }
      return key;
    }

    private void TraceLocals()
    {
      this._realTrace.WriteLine(".locals init (");
      foreach (KeyValuePair<LocalBuilder, string> localDeclaration in this._localDeclarations)
      {
        this.WriteIndent(this._realTrace, 1);
        this._realTrace.Write("[");
        this._realTrace.Write(localDeclaration.Key.LocalIndex);
        this._realTrace.Write("] ");
        TracingILGenerator.WriteType(this._realTrace, localDeclaration.Key.LocalType);
        if (localDeclaration.Key.IsPinned)
          this._realTrace.Write("(pinned)");
        if (localDeclaration.Value != null)
        {
          this._realTrace.Write(" ");
          this._realTrace.Write(localDeclaration.Value);
        }
        this._realTrace.WriteLine();
      }
      this._realTrace.WriteLine(")");
    }

    public Label BeginExceptionBlock()
    {
      this.TraceStart();
      this.TraceWriteLine(".try");
      this.Indent();
      Label index = this._underlying.BeginExceptionBlock();
      this._endOfExceptionBlocks.Push(index);
      this._labels[index] = "END_TRY_" + this._labels.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return index;
    }

    public void BeginFinallyBlock()
    {
      this.Unindent();
      this.TraceStart();
      this.TraceWriteLine(".finally");
      this.Indent();
      this._underlying.BeginFinallyBlock();
    }

    public void EndExceptionBlock()
    {
      this.Unindent();
      this._underlying.EndExceptionBlock();
      this._endOfExceptionBlocks.Pop();
    }

    public Label DefineLabel(string name)
    {
      Label key = this._underlying.DefineLabel();
      this._labels.Add(key, name);
      return key;
    }

    public void MarkLabel(Label label)
    {
      this.TraceStart();
      this.TraceWriteLine(this._labels[label]);
      this._underlying.MarkLabel(label);
    }

    public void TraceWrite(string value)
    {
      this._trace.Write(value);
    }

    public void TraceWriteLine()
    {
      this._trace.WriteLine();
    }

    public void TraceWriteLine(string value)
    {
      this._trace.WriteLine(value);
    }

    public void TraceWriteLine(string format, object arg0)
    {
      this._trace.WriteLine(format, arg0);
    }

    private void TraceType(Type type)
    {
      TracingILGenerator.WriteType(this._trace, type);
    }

    private static void WriteType(TextWriter writer, Type type)
    {
      if (type == (Type) null || type == typeof (void))
        writer.Write("void");
      else if (type.IsGenericParameter)
        writer.Write("{0}{1}", type.DeclaringMethod == (MethodBase) null ? (object) "!" : (object) "!!", (object) type.Name);
      else
        writer.Write("[{0}]{1}", (object) type.Assembly.GetName().Name, (object) type.FullName);
    }

    private void TraceField(FieldInfo field)
    {
      TracingILGenerator.WriteType(this._trace, field.FieldType);
      this._trace.Write(" ");
      FieldBuilder fieldBuilder = field as FieldBuilder;
      if ((FieldInfo) fieldBuilder == (FieldInfo) null)
      {
        Type[] requiredCustomModifiers = field.GetRequiredCustomModifiers();
        if (0 < requiredCustomModifiers.Length)
        {
          this._trace.Write("modreq(");
          foreach (Type type in requiredCustomModifiers)
            TracingILGenerator.WriteType(this._trace, type);
          this._trace.Write(") ");
        }
        Type[] optionalCustomModifiers = field.GetOptionalCustomModifiers();
        if (0 < optionalCustomModifiers.Length)
        {
          this._trace.Write("modopt(");
          foreach (Type type in optionalCustomModifiers)
            TracingILGenerator.WriteType(this._trace, type);
          this._trace.Write(") ");
        }
      }
      if (this._isInDynamicMethod || (FieldInfo) fieldBuilder == (FieldInfo) null)
        TracingILGenerator.WriteType(this._trace, field.DeclaringType);
      this._trace.Write("::");
      this._trace.Write(field.Name);
    }

    private void TraceMethod(MethodBase method)
    {
      bool flag = method is MethodBuilder || method is ConstructorBuilder;
      if (!method.IsStatic)
        this._trace.Write("instance ");
      CallingConvention? unamangedCallingConvention = new CallingConvention?();
      if (!flag && Attribute.GetCustomAttribute((MemberInfo) method, typeof (DllImportAttribute)) is DllImportAttribute customAttribute)
        unamangedCallingConvention = new CallingConvention?(customAttribute.CallingConvention);
      TracingILGenerator.WriteCallingConventions(this._trace, new CallingConventions?(method.CallingConvention), unamangedCallingConvention);
      MethodInfo methodInfo = method as MethodInfo;
      if (methodInfo != (MethodInfo) null)
      {
        TracingILGenerator.WriteType(this._trace, methodInfo.ReturnType);
        this._trace.Write(" ");
      }
      if (method.DeclaringType == (Type) null)
      {
        this._trace.Write("[.module");
        this._trace.Write(methodInfo.Module.Name);
        this._trace.Write("]::");
      }
      else if (this._isInDynamicMethod || !flag)
      {
        TracingILGenerator.WriteType(this._trace, method.DeclaringType);
        this._trace.Write("::");
      }
      this._trace.Write(method.Name);
      this._trace.Write("(");
      if (!flag)
      {
        ParameterInfo[] parameters = method.GetParameters();
        for (int index = 0; index < parameters.Length; ++index)
        {
          if (index == 0)
            this._trace.Write(" ");
          else
            this._trace.Write(", ");
          if (parameters[index].IsOut)
            this._trace.Write("out ");
          else if (parameters[index].ParameterType.IsByRef)
            this._trace.Write("ref ");
          TracingILGenerator.WriteType(this._trace, parameters[index].ParameterType.IsByRef ? parameters[index].ParameterType.GetElementType() : parameters[index].ParameterType);
          this._trace.Write(" ");
          this._trace.Write(parameters[index].Name);
        }
        if (0 < parameters.Length)
          this._trace.Write(" ");
      }
      this._trace.Write(")");
    }

    private static void WriteCallingConventions(
      TextWriter writer,
      CallingConventions? managedCallingConverntions,
      CallingConvention? unamangedCallingConvention)
    {
      bool flag = false;
      if (managedCallingConverntions.HasValue)
      {
        CallingConventions? nullable1 = managedCallingConverntions;
        CallingConventions? nullable2 = nullable1.HasValue ? new CallingConventions?(nullable1.GetValueOrDefault() & CallingConventions.ExplicitThis) : new CallingConventions?();
        if ((nullable2.GetValueOrDefault() != CallingConventions.ExplicitThis ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
        {
          writer.Write("explicit");
          flag = true;
        }
      }
      if (!unamangedCallingConvention.HasValue)
      {
        if (!managedCallingConverntions.HasValue)
          return;
        CallingConventions? nullable1 = managedCallingConverntions;
        CallingConventions? nullable2 = nullable1.HasValue ? new CallingConventions?(nullable1.GetValueOrDefault() & CallingConventions.VarArgs) : new CallingConventions?();
        if ((nullable2.GetValueOrDefault() != CallingConventions.VarArgs ? 0 : (nullable2.HasValue ? 1 : 0)) == 0)
          return;
        if (flag)
          writer.Write(" ");
        writer.Write("varargs");
      }
      else
      {
        switch (unamangedCallingConvention.Value)
        {
          case CallingConvention.Winapi:
          case CallingConvention.StdCall:
            if (flag)
              writer.Write(" ");
            writer.Write("unmanaged stdcall");
            break;
          default:
            if (flag)
              writer.Write(" ");
            writer.Write("unmanaged ");
            writer.Write(unamangedCallingConvention.Value.ToString().ToLowerInvariant());
            break;
        }
      }
    }

    private void TraceOpCode(OpCode opCode)
    {
      this._trace.Write(opCode.Name);
    }

    private void TraceOperand(int value)
    {
      this._trace.Write(value);
    }

    private void TraceOperand(long value)
    {
      this._trace.Write(value);
    }

    private void TraceOperand(double value)
    {
      this._trace.Write(value);
    }

    private void TraceOperand(string value)
    {
      this._trace.Write(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0:L}\"", (object) value));
    }

    private void TraceOperand(Label value)
    {
      this._trace.Write(this._labels[value]);
    }

    private void TraceOperand(Type value)
    {
      this.TraceType(value);
    }

    private void TraceOperand(FieldInfo value)
    {
      this.TraceField(value);
    }

    private void TraceOperand(MethodBase value)
    {
      this.TraceMethod(value);
    }

    private void TraceOperandToken(Type target)
    {
      this.TraceType(target);
      this.TraceOperandTokenValue(target.MetadataToken);
    }

    private void TraceOperandToken(FieldInfo target)
    {
      this._trace.Write("field ");
      this.TraceField(target);
      this.TraceOperandTokenValue(target.MetadataToken);
    }

    private void TraceOperandToken(MethodBase target)
    {
      this._trace.Write("method ");
      this.TraceMethod(target);
      this.TraceOperandTokenValue(target.MetadataToken);
    }

    private void TraceOperandTokenValue(int value)
    {
      this._trace.Write("<");
      this._trace.Write(value.ToString("x8", (IFormatProvider) CultureInfo.InvariantCulture));
      this._trace.Write(">");
    }

    private void TraceStart()
    {
      this.WriteLineNumber();
      this.WriteIndent();
    }

    private void WriteLineNumber()
    {
      this._trace.Write("L_{0:d4}\t", (object) this._lineNumber);
      ++this._lineNumber;
    }

    private void WriteIndent()
    {
      this.WriteIndent(this._trace, this._indentLevel);
    }

    private void WriteIndent(TextWriter writer, int indentLevel)
    {
      for (int index = 0; index < indentLevel; ++index)
        writer.Write(this._indentChars);
    }

    private void Indent()
    {
      ++this._indentLevel;
    }

    private void Unindent()
    {
      --this._indentLevel;
    }

    public void EmitLdarg_0()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldarg_0);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldarg_0);
    }

    public void EmitLdarg_1()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldarg_1);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldarg_1);
    }

    public void EmitLdarg_2()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldarg_2);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldarg_2);
    }

    public void EmitLdarg_3()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldarg_3);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldarg_3);
    }

    public void EmitLdloc_0()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldloc_0);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldloc_0);
    }

    public void EmitLdloc_1()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldloc_1);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldloc_1);
    }

    public void EmitLdloc_2()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldloc_2);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldloc_2);
    }

    public void EmitLdloc_3()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldloc_3);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldloc_3);
    }

    public void EmitStloc_0()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stloc_0);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stloc_0);
    }

    public void EmitStloc_1()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stloc_1);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stloc_1);
    }

    public void EmitStloc_2()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stloc_2);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stloc_2);
    }

    public void EmitStloc_3()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stloc_3);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stloc_3);
    }

    public void EmitLdarg_S(byte value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldarg_S);
      this.TraceWrite(" ");
      this.TraceOperand((int) value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldarg_S, value);
    }

    public void EmitLdarga_S(byte value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldarga_S);
      this.TraceWrite(" ");
      this.TraceOperand((int) value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldarga_S, value);
    }

    public void EmitLdloc_S(byte value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldloc_S);
      this.TraceWrite(" ");
      this.TraceOperand((int) value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldloc_S, value);
    }

    public void EmitLdloca_S(byte value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldloca_S);
      this.TraceWrite(" ");
      this.TraceOperand((int) value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldloca_S, value);
    }

    public void EmitStloc_S(byte value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stloc_S);
      this.TraceWrite(" ");
      this.TraceOperand((int) value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stloc_S, value);
    }

    public void EmitLdnull()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldnull);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldnull);
    }

    public void EmitLdc_I4_M1()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_M1);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_M1);
    }

    public void EmitLdc_I4_0()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_0);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_0);
    }

    public void EmitLdc_I4_1()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_1);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_1);
    }

    public void EmitLdc_I4_2()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_2);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_2);
    }

    public void EmitLdc_I4_3()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_3);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_3);
    }

    public void EmitLdc_I4_4()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_4);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_4);
    }

    public void EmitLdc_I4_5()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_5);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_5);
    }

    public void EmitLdc_I4_6()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_6);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_6);
    }

    public void EmitLdc_I4_7()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_7);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_7);
    }

    public void EmitLdc_I4_8()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_8);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_8);
    }

    public void EmitLdc_I4_S(byte value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4_S);
      this.TraceWrite(" ");
      this.TraceOperand((int) value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4_S, value);
    }

    public void EmitLdc_I4(int value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I4);
      this.TraceWrite(" ");
      this.TraceOperand(value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I4, value);
    }

    public void EmitLdc_I8(long value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_I8);
      this.TraceWrite(" ");
      this.TraceOperand(value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_I8, value);
    }

    public void EmitLdc_R4(float value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_R4);
      this.TraceWrite(" ");
      this.TraceOperand((double) value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_R4, value);
    }

    public void EmitLdc_R8(double value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldc_R8);
      this.TraceWrite(" ");
      this.TraceOperand(value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldc_R8, value);
    }

    public void EmitPop()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Pop);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Pop);
    }

    public void EmitCall(MethodInfo target)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Call);
      this.TraceWrite(" ");
      this.TraceOperand((MethodBase) target);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Call, target);
    }

    public void EmitBr(Label target)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Br);
      this.TraceWrite(" ");
      this.TraceOperand(target);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Br, target);
    }

    public void EmitBrfalse(Label target)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Brfalse);
      this.TraceWrite(" ");
      this.TraceOperand(target);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Brfalse, target);
    }

    public void EmitBrtrue(Label target)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Brtrue);
      this.TraceWrite(" ");
      this.TraceOperand(target);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Brtrue, target);
    }

    public void EmitBlt(Label target)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Blt);
      this.TraceWrite(" ");
      this.TraceOperand(target);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Blt, target);
    }

    public void EmitAdd()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Add);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Add);
    }

    public void EmitAnd()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.And);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.And);
    }

    public void EmitCallvirt(MethodInfo target)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Callvirt);
      this.TraceWrite(" ");
      this.TraceOperand((MethodBase) target);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Callvirt, target);
    }

    public void EmitLdstr(string value)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldstr);
      this.TraceWrite(" ");
      this.TraceOperand(value);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldstr, value);
    }

    public void EmitNewobj(ConstructorInfo constructor)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Newobj);
      this.TraceWrite(" ");
      this.TraceOperand((MethodBase) constructor);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Newobj, constructor);
    }

    public void EmitThrow()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Throw);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Throw);
    }

    public void EmitLdfld(FieldInfo field)
    {
      if (!(field is FieldBuilder) && ((IEnumerable<Type>) field.GetRequiredCustomModifiers()).Any<Type>((Func<Type, bool>) (item => typeof (IsVolatile).Equals(item))))
      {
        this.TraceStart();
        this.TraceOpCode(OpCodes.Volatile);
        this.TraceWriteLine();
      }
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldfld);
      this.TraceWrite(" ");
      this.TraceOperand(field);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldfld, field);
    }

    public void EmitLdflda(FieldInfo field)
    {
      if (!(field is FieldBuilder) && ((IEnumerable<Type>) field.GetRequiredCustomModifiers()).Any<Type>((Func<Type, bool>) (item => typeof (IsVolatile).Equals(item))))
      {
        this.TraceStart();
        this.TraceOpCode(OpCodes.Volatile);
        this.TraceWriteLine();
      }
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldflda);
      this.TraceWrite(" ");
      this.TraceOperand(field);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldflda, field);
    }

    public void EmitStfld(FieldInfo field)
    {
      if (!(field is FieldBuilder) && ((IEnumerable<Type>) field.GetRequiredCustomModifiers()).Any<Type>((Func<Type, bool>) (item => typeof (IsVolatile).Equals(item))))
      {
        this.TraceStart();
        this.TraceOpCode(OpCodes.Volatile);
        this.TraceWriteLine();
      }
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stfld);
      this.TraceWrite(" ");
      this.TraceOperand(field);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stfld, field);
    }

    public void EmitStobj(Type type)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stobj);
      this.TraceWrite(" ");
      this.TraceOperand(type);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stobj, type);
    }

    public void EmitBox(Type type)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Box);
      this.TraceWrite(" ");
      this.TraceOperand(type);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Box, type);
    }

    public void EmitNewarr(Type type)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Newarr);
      this.TraceWrite(" ");
      this.TraceOperand(type);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Newarr, type);
    }

    public void EmitLdelema(Type type)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldelema);
      this.TraceWrite(" ");
      this.TraceOperand(type);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldelema, type);
    }

    public void EmitStelem_I1()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stelem_I1);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stelem_I1);
    }

    public void EmitStelem_I2()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stelem_I2);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stelem_I2);
    }

    public void EmitStelem_I4()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stelem_I4);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stelem_I4);
    }

    public void EmitStelem_I8()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stelem_I8);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stelem_I8);
    }

    public void EmitStelem_R4()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stelem_R4);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stelem_R4);
    }

    public void EmitStelem_R8()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stelem_R8);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stelem_R8);
    }

    public void EmitStelem_Ref()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stelem_Ref);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stelem_Ref);
    }

    public void EmitStelem(Type type)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stelem);
      this.TraceWrite(" ");
      this.TraceOperand(type);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stelem, type);
    }

    public void EmitUnbox_Any(Type type)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Unbox_Any);
      this.TraceWrite(" ");
      this.TraceOperand(type);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Unbox_Any, type);
    }

    public void EmitLdtoken(Type target)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldtoken);
      this.TraceWrite(" ");
      this.TraceOperandToken(target);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldtoken, target);
    }

    public void EmitLdtoken(MethodBase target)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldtoken);
      this.TraceWrite(" ");
      this.TraceOperandToken(target);
      this.TraceWriteLine();
      MethodInfo meth;
      if ((meth = target as MethodInfo) != (MethodInfo) null)
        this._underlying.Emit(OpCodes.Ldtoken, meth);
      else
        this._underlying.Emit(OpCodes.Ldtoken, target as ConstructorInfo);
    }

    public void EmitLdtoken(FieldInfo target)
    {
      if (!(target is FieldBuilder) && ((IEnumerable<Type>) target.GetRequiredCustomModifiers()).Any<Type>((Func<Type, bool>) (item => typeof (IsVolatile).Equals(item))))
      {
        this.TraceStart();
        this.TraceOpCode(OpCodes.Volatile);
        this.TraceWriteLine();
      }
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldtoken);
      this.TraceWrite(" ");
      this.TraceOperandToken(target);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldtoken, target);
    }

    public void EmitCeq()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ceq);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ceq);
    }

    public void EmitCgt()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Cgt);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Cgt);
    }

    public void EmitClt()
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Clt);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Clt);
    }

    public void EmitLdarg(int index)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldarg);
      this.TraceWrite(" ");
      this.TraceOperand(index);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldarg, index);
    }

    public void EmitLdarga(int index)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldarga);
      this.TraceWrite(" ");
      this.TraceOperand(index);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldarga, index);
    }

    public void EmitLdloc(int index)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldloc);
      this.TraceWrite(" ");
      this.TraceOperand(index);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldloc, index);
    }

    public void EmitLdloca(int index)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Ldloca);
      this.TraceWrite(" ");
      this.TraceOperand(index);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Ldloca, index);
    }

    public void EmitStloc(int index)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Stloc);
      this.TraceWrite(" ");
      this.TraceOperand(index);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Stloc, index);
    }

    public void EmitInitobj(Type type)
    {
      this.TraceStart();
      this.TraceOpCode(OpCodes.Initobj);
      this.TraceWrite(" ");
      this.TraceOperand(type);
      this.TraceWriteLine();
      this._underlying.Emit(OpCodes.Initobj, type);
    }
  }
}
