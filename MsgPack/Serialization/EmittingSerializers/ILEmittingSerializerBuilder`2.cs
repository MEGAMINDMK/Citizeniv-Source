// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.ILEmittingSerializerBuilder`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Metadata;
using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal abstract class ILEmittingSerializerBuilder<TContext, TObject> : SerializerBuilder<TContext, ILConstruct, TObject>
    where TContext : ILEmittingContext
  {
    protected override void EmitMethodPrologue(TContext context, SerializerMethod method)
    {
      switch (method)
      {
        case SerializerMethod.PackToCore:
          context.IL = context.Emitter.GetPackToMethodILGenerator();
          break;
        case SerializerMethod.UnpackFromCore:
          context.IL = context.Emitter.GetUnpackFromMethodILGenerator();
          break;
        case SerializerMethod.UnpackToCore:
          context.IL = context.Emitter.GetUnpackToMethodILGenerator();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    protected override void EmitMethodPrologue(TContext context, EnumSerializerMethod method)
    {
      switch (method)
      {
        case EnumSerializerMethod.PackUnderlyingValueTo:
          context.IL = context.EnumEmitter.GetPackUnderyingValueToMethodILGenerator();
          break;
        case EnumSerializerMethod.UnpackFromUnderlyingValue:
          context.IL = context.EnumEmitter.GetUnpackFromUnderlyingValueMethodILGenerator();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    protected override void EmitMethodPrologue(
      TContext context,
      CollectionSerializerMethod method,
      MethodInfo declaration)
    {
      switch (method)
      {
        case CollectionSerializerMethod.AddItem:
          context.IL = context.Emitter.GetAddItemMethodILGenerator(declaration);
          break;
        case CollectionSerializerMethod.CreateInstance:
          context.IL = context.Emitter.GetCreateInstanceMethodILGenerator(declaration);
          break;
        case CollectionSerializerMethod.RestoreSchema:
          context.IL = context.Emitter.GetRestoreSchemaMethodILGenerator();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    protected override void EmitMethodEpilogue(
      TContext context,
      SerializerMethod method,
      ILConstruct construct)
    {
      ILEmittingSerializerBuilder<TContext, TObject>.EmitMethodEpilogue(context, construct);
    }

    protected override void EmitMethodEpilogue(
      TContext context,
      EnumSerializerMethod method,
      ILConstruct construct)
    {
      ILEmittingSerializerBuilder<TContext, TObject>.EmitMethodEpilogue(context, construct);
    }

    protected override void EmitMethodEpilogue(
      TContext context,
      CollectionSerializerMethod method,
      ILConstruct construct)
    {
      ILEmittingSerializerBuilder<TContext, TObject>.EmitMethodEpilogue(context, construct);
    }

    private static void EmitMethodEpilogue(TContext context, ILConstruct construct)
    {
      try
      {
        construct?.Evaluate(context.IL);
        context.IL.EmitRet();
      }
      finally
      {
        context.IL.FlushTrace();
        SerializerDebugging.FlushTraceData();
      }
    }

    protected override ILConstruct EmitSequentialStatements(
      TContext context,
      Type contextType,
      IEnumerable<ILConstruct> statements)
    {
      return ILConstruct.Sequence(contextType, statements);
    }

    protected override ILConstruct MakeNullLiteral(TContext context, Type contextType)
    {
      return ILConstruct.Literal<object>(contextType, (object) null, (Action<TracingILGenerator>) (il => il.EmitLdnull()));
    }

    protected override ILConstruct MakeByteLiteral(TContext context, byte constant)
    {
      return ILEmittingSerializerBuilder<TContext, TObject>.MakeIntegerLiteral(typeof (byte), (int) constant);
    }

    protected override ILConstruct MakeSByteLiteral(TContext context, sbyte constant)
    {
      return ILEmittingSerializerBuilder<TContext, TObject>.MakeIntegerLiteral(typeof (sbyte), (int) constant);
    }

    protected override ILConstruct MakeInt16Literal(TContext context, short constant)
    {
      return ILEmittingSerializerBuilder<TContext, TObject>.MakeIntegerLiteral(typeof (short), (int) constant);
    }

    protected override ILConstruct MakeUInt16Literal(TContext context, ushort constant)
    {
      return ILEmittingSerializerBuilder<TContext, TObject>.MakeIntegerLiteral(typeof (ushort), (int) constant);
    }

    protected override ILConstruct MakeInt32Literal(TContext context, int constant)
    {
      return ILEmittingSerializerBuilder<TContext, TObject>.MakeIntegerLiteral(typeof (int), constant);
    }

    protected override ILConstruct MakeUInt32Literal(TContext context, uint constant)
    {
      return ILEmittingSerializerBuilder<TContext, TObject>.MakeIntegerLiteral(typeof (uint), (int) constant);
    }

    private static ILConstruct MakeIntegerLiteral(Type contextType, int constant)
    {
      switch (constant)
      {
        case -1:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_M1()));
        case 0:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_0()));
        case 1:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_1()));
        case 2:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_2()));
        case 3:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_3()));
        case 4:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_4()));
        case 5:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_5()));
        case 6:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_6()));
        case 7:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_7()));
        case 8:
          return ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_8()));
        default:
          return (int) sbyte.MinValue <= constant && constant <= (int) sbyte.MaxValue ? ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4_S((byte) constant))) : ILConstruct.Literal<int>(contextType, constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I4(constant)));
      }
    }

    protected override ILConstruct MakeInt64Literal(TContext context, long constant)
    {
      return ILConstruct.Literal<long>(typeof (long), constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I8(constant)));
    }

    protected override ILConstruct MakeUInt64Literal(TContext context, ulong constant)
    {
      return ILConstruct.Literal<ulong>(typeof (ulong), constant, (Action<TracingILGenerator>) (il => il.EmitLdc_I8((long) constant)));
    }

    protected override ILConstruct MakeReal32Literal(TContext context, float constant)
    {
      return ILConstruct.Literal<float>(typeof (float), constant, (Action<TracingILGenerator>) (il => il.EmitLdc_R4(constant)));
    }

    protected override ILConstruct MakeReal64Literal(TContext context, double constant)
    {
      return ILConstruct.Literal<double>(typeof (double), constant, (Action<TracingILGenerator>) (il => il.EmitLdc_R8(constant)));
    }

    protected override ILConstruct MakeBooleanLiteral(TContext context, bool constant)
    {
      return ILEmittingSerializerBuilder<TContext, TObject>.MakeIntegerLiteral(typeof (bool), constant ? 1 : 0);
    }

    protected override ILConstruct MakeCharLiteral(TContext context, char constant)
    {
      return ILEmittingSerializerBuilder<TContext, TObject>.MakeIntegerLiteral(typeof (char), (int) constant);
    }

    protected override ILConstruct MakeStringLiteral(TContext context, string constant)
    {
      return ILConstruct.Literal<string>(typeof (string), constant, (Action<TracingILGenerator>) (il => il.EmitLdstr(constant)));
    }

    protected override ILConstruct MakeEnumLiteral(
      TContext context,
      Type type,
      object constant)
    {
      Type underlyingType = Enum.GetUnderlyingType(type);
      switch (Type.GetTypeCode(underlyingType))
      {
        case TypeCode.SByte:
          return this.MakeInt32Literal(context, (int) (sbyte) constant);
        case TypeCode.Byte:
          return this.MakeInt32Literal(context, (int) (byte) constant);
        case TypeCode.Int16:
          return this.MakeInt32Literal(context, (int) (short) constant);
        case TypeCode.UInt16:
          return this.MakeInt32Literal(context, (int) (ushort) constant);
        case TypeCode.Int32:
          return this.MakeInt32Literal(context, (int) constant);
        case TypeCode.UInt32:
          return this.MakeInt32Literal(context, (int) (uint) constant);
        case TypeCode.Int64:
          return this.MakeInt64Literal(context, (long) constant);
        case TypeCode.UInt64:
          return this.MakeInt64Literal(context, (long) (ulong) constant);
        default:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Underying type '{0}' is not supported.", (object) underlyingType));
      }
    }

    protected override ILConstruct MakeDefaultLiteral(TContext context, Type type)
    {
      return ILConstruct.Literal<string>(type, "default(" + (object) type + ")", (Action<TracingILGenerator>) (il =>
      {
        LocalBuilder local = il.DeclareLocal(type);
        il.EmitAnyLdloca(local);
        il.EmitInitobj(type);
        il.EmitAnyLdloc(local);
      }));
    }

    protected override ILConstruct EmitThisReferenceExpression(TContext context)
    {
      return ILConstruct.Literal<string>(context.GetSerializerType(typeof (TObject)), "(this)", (Action<TracingILGenerator>) (il => il.EmitLdarg_0()));
    }

    protected override ILConstruct EmitBoxExpression(
      TContext context,
      Type valueType,
      ILConstruct value)
    {
      return ILConstruct.UnaryOperator("box", value, (Action<TracingILGenerator, ILConstruct>) ((il, val) =>
      {
        val.LoadValue(il, false);
        il.EmitBox(valueType);
      }));
    }

    protected override ILConstruct EmitUnboxAnyExpression(
      TContext context,
      Type targetType,
      ILConstruct value)
    {
      return ILConstruct.UnaryOperator("unbox.any", value, (Action<TracingILGenerator, ILConstruct>) ((il, val) =>
      {
        val.LoadValue(il, false);
        il.EmitUnbox_Any(targetType);
      }));
    }

    protected override ILConstruct EmitNotExpression(
      TContext context,
      ILConstruct booleanExpression)
    {
      if (booleanExpression.ContextType != typeof (bool))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Not expression must be Boolean elementType, but actual is '{0}'.", (object) booleanExpression.ContextType), nameof (booleanExpression));
      return ILConstruct.UnaryOperator("!", booleanExpression, (Action<TracingILGenerator, ILConstruct>) ((il, val) =>
      {
        val.LoadValue(il, false);
        il.EmitLdc_I4_0();
        il.EmitCeq();
      }), (Action<TracingILGenerator, ILConstruct, Label>) ((il, val, @else) =>
      {
        val.LoadValue(il, false);
        il.EmitBrtrue(@else);
      }));
    }

    protected override ILConstruct EmitEqualsExpression(
      TContext context,
      ILConstruct left,
      ILConstruct right)
    {
      MethodInfo equality = left.ContextType.GetMethod("op_Equality");
      return ILConstruct.BinaryOperator("==", typeof (bool), left, right, (Action<TracingILGenerator, ILConstruct, ILConstruct>) ((il, l, r) =>
      {
        l.LoadValue(il, false);
        r.LoadValue(il, false);
        if (equality == (MethodInfo) null)
          il.EmitCeq();
        else
          il.EmitAnyCall(equality);
      }), (Action<TracingILGenerator, ILConstruct, ILConstruct, Label>) ((il, l, r, @else) =>
      {
        l.LoadValue(il, false);
        r.LoadValue(il, false);
        if (equality == (MethodInfo) null)
          il.EmitCeq();
        else
          il.EmitAnyCall(equality);
        il.EmitBrfalse(@else);
      }));
    }

    protected override ILConstruct EmitGreaterThanExpression(
      TContext context,
      ILConstruct left,
      ILConstruct right)
    {
      MethodInfo greaterThan = left.ContextType.GetMethod("op_GreaterThan");
      return ILConstruct.BinaryOperator(">", typeof (bool), left, right, (Action<TracingILGenerator, ILConstruct, ILConstruct>) ((il, l, r) =>
      {
        l.LoadValue(il, false);
        r.LoadValue(il, false);
        if (greaterThan == (MethodInfo) null)
          il.EmitCgt();
        else
          il.EmitAnyCall(greaterThan);
      }), (Action<TracingILGenerator, ILConstruct, ILConstruct, Label>) ((il, l, r, @else) =>
      {
        l.LoadValue(il, false);
        r.LoadValue(il, false);
        if (greaterThan == (MethodInfo) null)
          il.EmitCgt();
        else
          il.EmitAnyCall(greaterThan);
        il.EmitBrfalse(@else);
      }));
    }

    protected override ILConstruct EmitLessThanExpression(
      TContext context,
      ILConstruct left,
      ILConstruct right)
    {
      MethodInfo lessThan = left.ContextType.GetMethod("op_LessThan");
      return ILConstruct.BinaryOperator("<", typeof (bool), left, right, (Action<TracingILGenerator, ILConstruct, ILConstruct>) ((il, l, r) =>
      {
        l.LoadValue(il, false);
        r.LoadValue(il, false);
        if (lessThan == (MethodInfo) null)
          il.EmitClt();
        else
          il.EmitAnyCall(lessThan);
      }), (Action<TracingILGenerator, ILConstruct, ILConstruct, Label>) ((il, l, r, @else) =>
      {
        l.LoadValue(il, false);
        r.LoadValue(il, false);
        if (lessThan == (MethodInfo) null)
          il.EmitClt();
        else
          il.EmitAnyCall(lessThan);
        il.EmitBrfalse(@else);
      }));
    }

    protected override ILConstruct EmitIncrement(
      TContext context,
      ILConstruct int32Value)
    {
      return ILConstruct.UnaryOperator("++", int32Value, (Action<TracingILGenerator, ILConstruct>) ((il, variable) =>
      {
        variable.LoadValue(il, false);
        il.EmitLdc_I4_1();
        il.EmitAdd();
        variable.StoreValue(il);
      }));
    }

    protected override ILConstruct EmitTypeOfExpression(TContext context, Type type)
    {
      return ILConstruct.Literal<Type>(typeof (Type), type, (Action<TracingILGenerator>) (il => il.EmitTypeOf(type)));
    }

    protected override ILConstruct EmitMethodOfExpression(
      TContext context,
      MethodBase method)
    {
      return ILConstruct.Literal<MethodBase>(typeof (MethodInfo), method, (Action<TracingILGenerator>) (il =>
      {
        il.EmitLdtoken(method);
        il.EmitLdtoken(method.DeclaringType);
        il.EmitCall(_MethodBase.GetMethodFromHandle);
      }));
    }

    protected override ILConstruct EmitFieldOfExpression(
      TContext context,
      FieldInfo field)
    {
      return ILConstruct.Literal<FieldInfo>(typeof (MethodInfo), field, (Action<TracingILGenerator>) (il =>
      {
        il.EmitLdtoken(field);
        il.EmitLdtoken(field.DeclaringType);
        il.EmitCall(_FieldInfo.GetFieldFromHandle);
      }));
    }

    protected override ILConstruct DeclareLocal(
      TContext context,
      Type type,
      string name)
    {
      return ILConstruct.Variable(type, name);
    }

    protected override ILConstruct ReferArgument(
      TContext context,
      Type type,
      string name,
      int index)
    {
      return ILConstruct.Argument(index, type, name);
    }

    protected override ILConstruct EmitInvokeVoidMethod(
      TContext context,
      ILConstruct instance,
      MethodInfo method,
      params ILConstruct[] arguments)
    {
      if (method.ReturnType == typeof (void))
        return ILConstruct.Invoke(instance, method, (IEnumerable<ILConstruct>) arguments);
      return ILConstruct.Sequence(typeof (void), (IEnumerable<ILConstruct>) new ILConstruct[2]
      {
        ILConstruct.Invoke(instance, method, (IEnumerable<ILConstruct>) arguments),
        ILConstruct.Instruction("pop", typeof (void), false, (Action<TracingILGenerator>) (il => il.EmitPop()))
      });
    }

    protected override ILConstruct EmitCreateNewObjectExpression(
      TContext context,
      ILConstruct variable,
      ConstructorInfo constructor,
      params ILConstruct[] arguments)
    {
      return ILConstruct.NewObject(variable, constructor, (IEnumerable<ILConstruct>) arguments);
    }

    protected override ILConstruct EmitCreateNewArrayExpression(
      TContext context,
      Type elementType,
      int length)
    {
      ILConstruct array = ILConstruct.Variable(elementType.MakeArrayType(), "array");
      return ILConstruct.Composite(ILConstruct.Sequence(array.ContextType, (IEnumerable<ILConstruct>) new ILConstruct[2]
      {
        array,
        ILConstruct.Instruction("NewArray", array.ContextType, false, (Action<TracingILGenerator>) (il =>
        {
          il.EmitNewarr(elementType, (long) length);
          array.StoreValue(il);
        }))
      }), array);
    }

    protected override ILConstruct EmitCreateNewArrayExpression(
      TContext context,
      Type elementType,
      int length,
      IEnumerable<ILConstruct> initialElements)
    {
      ILConstruct array = ILConstruct.Variable(elementType.MakeArrayType(), "array");
      return ILConstruct.Composite(ILConstruct.Sequence(array.ContextType, (IEnumerable<ILConstruct>) new ILConstruct[2]
      {
        array,
        ILConstruct.Instruction("CreateArray", array.ContextType, false, (Action<TracingILGenerator>) (il =>
        {
          il.EmitNewarr(elementType, (long) length);
          array.StoreValue(il);
          int constant = 0;
          foreach (ILConstruct initialElement in initialElements)
          {
            array.LoadValue(il, false);
            this.MakeInt32Literal(context, constant).LoadValue(il, false);
            initialElement.LoadValue(il, false);
            il.EmitStelem(elementType);
            ++constant;
          }
        }))
      }), array);
    }

    protected override ILConstruct EmitSetArrayElementStatement(
      TContext context,
      ILConstruct array,
      ILConstruct index,
      ILConstruct value)
    {
      return ILConstruct.Instruction("SetArrayElement", array.ContextType, false, (Action<TracingILGenerator>) (il => il.EmitAnyStelem(value.ContextType, (Action<TracingILGenerator>) (il0 => array.LoadValue(il0, false)), (Action<TracingILGenerator>) (il0 => index.LoadValue(il0, false)), (Action<TracingILGenerator>) (il0 => value.LoadValue(il0, true)))));
    }

    protected override ILConstruct EmitInvokeMethodExpression(
      TContext context,
      ILConstruct instance,
      MethodInfo method,
      IEnumerable<ILConstruct> arguments)
    {
      return ILConstruct.Invoke(instance, method, arguments);
    }

    protected override ILConstruct EmitGetPropertyExpression(
      TContext context,
      ILConstruct instance,
      PropertyInfo property)
    {
      return ILConstruct.Invoke(instance, property.GetGetMethod(true), (IEnumerable<ILConstruct>) ILConstruct.NoArguments);
    }

    protected override ILConstruct EmitGetFieldExpression(
      TContext context,
      ILConstruct instance,
      FieldInfo field)
    {
      return ILConstruct.LoadField(instance, field);
    }

    protected override ILConstruct EmitSetProperty(
      TContext context,
      ILConstruct instance,
      PropertyInfo property,
      ILConstruct value)
    {
      return ILConstruct.Invoke(instance, property.GetSetMethod(true), (IEnumerable<ILConstruct>) new ILConstruct[1]
      {
        value
      });
    }

    protected override ILConstruct EmitSetField(
      TContext context,
      ILConstruct instance,
      FieldInfo field,
      ILConstruct value)
    {
      return ILConstruct.StoreField(instance, field, value);
    }

    protected override ILConstruct EmitLoadVariableExpression(
      TContext context,
      ILConstruct variable)
    {
      return ILConstruct.Instruction("load", variable.ContextType, false, (Action<TracingILGenerator>) (il => variable.LoadValue(il, false)));
    }

    protected override ILConstruct EmitStoreVariableStatement(
      TContext context,
      ILConstruct variable,
      ILConstruct value)
    {
      return ILConstruct.StoreLocal(variable, value);
    }

    protected override ILConstruct EmitThrowExpression(
      TContext context,
      Type expressionType,
      ILConstruct exceptionExpression)
    {
      return ILConstruct.Instruction("throw", expressionType, true, (Action<TracingILGenerator>) (il =>
      {
        exceptionExpression.LoadValue(il, false);
        il.EmitThrow();
      }));
    }

    protected override ILConstruct EmitTryFinally(
      TContext context,
      ILConstruct tryStatement,
      ILConstruct finallyStatement)
    {
      return ILConstruct.Instruction("try-finally", tryStatement.ContextType, false, (Action<TracingILGenerator>) (il =>
      {
        il.BeginExceptionBlock();
        tryStatement.Evaluate(il);
        il.BeginFinallyBlock();
        finallyStatement.Evaluate(il);
        il.EndExceptionBlock();
      }));
    }

    protected override ILConstruct EmitConditionalExpression(
      TContext context,
      ILConstruct conditionExpression,
      ILConstruct thenExpression,
      ILConstruct elseExpression)
    {
      return ILConstruct.IfThenElse(conditionExpression, thenExpression, elseExpression);
    }

    protected override ILConstruct EmitAndConditionalExpression(
      TContext context,
      IList<ILConstruct> conditionExpressions,
      ILConstruct thenExpression,
      ILConstruct elseExpression)
    {
      return ILConstruct.IfThenElse(ILConstruct.AndCondition(conditionExpressions), thenExpression, elseExpression);
    }

    protected override ILConstruct EmitStringSwitchStatement(
      TContext context,
      ILConstruct target,
      IDictionary<string, ILConstruct> cases,
      ILConstruct defaultCase)
    {
      ILConstruct elseExpression = defaultCase;
      foreach (KeyValuePair<string, ILConstruct> keyValuePair in (IEnumerable<KeyValuePair<string, ILConstruct>>) cases)
        elseExpression = this.EmitConditionalExpression(context, this.EmitInvokeMethodExpression(context, (ILConstruct) null, _String.op_Equality, target, this.MakeStringLiteral(context, keyValuePair.Key)), keyValuePair.Value, elseExpression);
      return elseExpression;
    }

    protected override ILConstruct EmitForLoop(
      TContext context,
      ILConstruct count,
      Func<SerializerBuilder<TContext, ILConstruct, TObject>.ForLoopContext, ILConstruct> loopBodyEmitter)
    {
      ILConstruct i = this.DeclareLocal(context, typeof (int), "i");
      SerializerBuilder<TContext, ILConstruct, TObject>.ForLoopContext loopContext = new SerializerBuilder<TContext, ILConstruct, TObject>.ForLoopContext(i);
      return this.EmitSequentialStatements(context, i.ContextType, i, ILConstruct.Instruction("for", typeof (void), false, (Action<TracingILGenerator>) (il =>
      {
        Label label1 = il.DefineLabel("FOR_COND");
        il.EmitBr(label1);
        Label label2 = il.DefineLabel("BODY");
        il.MarkLabel(label2);
        loopBodyEmitter(loopContext).Evaluate(il);
        i.LoadValue(il, false);
        il.EmitLdc_I4_1();
        il.EmitAdd();
        i.StoreValue(il);
        il.MarkLabel(label1);
        i.LoadValue(il, false);
        count.LoadValue(il, false);
        il.EmitBlt(label2);
      })));
    }

    protected override ILConstruct EmitForEachLoop(
      TContext context,
      CollectionTraits traits,
      ILConstruct collection,
      Func<ILConstruct, ILConstruct> loopBodyEmitter)
    {
      return ILConstruct.Instruction("foreach", typeof (void), false, (Action<TracingILGenerator>) (il =>
      {
        LocalBuilder local = il.DeclareLocal(traits.GetEnumeratorMethod.ReturnType, "enumerator");
        ILConstruct ilConstruct = this.DeclareLocal(context, traits.ElementType, "item");
        collection.LoadValue(il, true);
        il.EmitAnyCall(traits.GetEnumeratorMethod);
        il.EmitAnyStloc(local);
        if (typeof (IDisposable).IsAssignableFrom(traits.GetEnumeratorMethod.ReturnType))
          il.BeginExceptionBlock();
        Label label1 = il.DefineLabel("START_LOOP");
        il.MarkLabel(label1);
        ilConstruct.Evaluate(il);
        Label label2 = il.DefineLabel("END_LOOP");
        Type returnType = traits.GetEnumeratorMethod.ReturnType;
        MethodInfo enumeratorMoveNextMethod = _IEnumerator.FindEnumeratorMoveNextMethod(returnType);
        PropertyInfo enumeratorCurrentProperty = _IEnumerator.FindEnumeratorCurrentProperty(returnType, traits);
        if (traits.GetEnumeratorMethod.ReturnType.IsValueType)
          il.EmitAnyLdloca(local);
        else
          il.EmitAnyLdloc(local);
        il.EmitAnyCall(enumeratorMoveNextMethod);
        il.EmitBrfalse(label2);
        if (traits.GetEnumeratorMethod.ReturnType.IsValueType)
          il.EmitAnyLdloca(local);
        else
          il.EmitAnyLdloc(local);
        il.EmitGetProperty(enumeratorCurrentProperty);
        ilConstruct.StoreValue(il);
        loopBodyEmitter(ilConstruct).Evaluate(il);
        il.EmitBr(label1);
        il.MarkLabel(label2);
        if (!typeof (IDisposable).IsAssignableFrom(traits.GetEnumeratorMethod.ReturnType))
          return;
        il.BeginFinallyBlock();
        if (traits.GetEnumeratorMethod.ReturnType.IsValueType)
        {
          MethodInfo method = traits.GetEnumeratorMethod.ReturnType.GetMethod("Dispose");
          if (method != (MethodInfo) null && method.GetParameters().Length == 0 && method.ReturnType == typeof (void))
          {
            il.EmitAnyLdloca(local);
            il.EmitAnyCall(method);
          }
          else
          {
            il.EmitAnyLdloc(local);
            il.EmitBox(traits.GetEnumeratorMethod.ReturnType);
            il.EmitAnyCall(_IDisposable.Dispose);
          }
        }
        else
        {
          il.EmitAnyLdloc(local);
          il.EmitAnyCall(_IDisposable.Dispose);
        }
        il.EndExceptionBlock();
      }));
    }

    protected override ILConstruct EmitEnumFromUnderlyingCastExpression(
      TContext context,
      Type enumType,
      ILConstruct underlyingValue)
    {
      return underlyingValue;
    }

    protected override ILConstruct EmitEnumToUnderlyingCastExpression(
      TContext context,
      Type underlyingType,
      ILConstruct enumValue)
    {
      return enumValue;
    }

    protected override Func<SerializationContext, MessagePackSerializer<TObject>> CreateSerializerConstructor(
      TContext codeGenerationContext,
      PolymorphismSchema schema)
    {
      return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => codeGenerationContext.Emitter.CreateInstance<TObject>(context, schema));
    }

    protected override Func<SerializationContext, MessagePackSerializer<TObject>> CreateEnumSerializerConstructor(
      TContext codeGenerationContext)
    {
      return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => codeGenerationContext.EnumEmitter.CreateInstance<TObject>(context, EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethod(context, typeof (TObject), EnumMemberSerializationMethod.Default)));
    }
  }
}
