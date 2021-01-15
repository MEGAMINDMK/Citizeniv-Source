// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionTreeSerializerBuilder`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal sealed class ExpressionTreeSerializerBuilder<TObject> : SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>
  {
    private static readonly Type SerializerClass = ExpressionTreeSerializerBuilderHelpers.GetSerializerClass(typeof (TObject), SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.CollectionTraitsOfThis);
    private readonly TypeBuilder _typeBuilder;

    public ExpressionTreeSerializerBuilder()
    {
      if (!SerializerDebugging.DumpEnabled)
        return;
      SerializerDebugging.PrepareDump();
      this._typeBuilder = SerializerDebugging.NewTypeBuilder(typeof (TObject));
    }

    protected override void EmitMethodPrologue(
      ExpressionTreeContext context,
      SerializerMethod method)
    {
      context.Reset(typeof (TObject), SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.BaseClass);
      context.SetCurrentMethod(method);
    }

    protected override void EmitMethodPrologue(
      ExpressionTreeContext context,
      EnumSerializerMethod method)
    {
      context.Reset(typeof (TObject), SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.BaseClass);
      context.SetCurrentMethod(typeof (TObject), method);
    }

    protected override void EmitMethodPrologue(
      ExpressionTreeContext context,
      CollectionSerializerMethod method,
      MethodInfo declaration)
    {
      context.Reset(typeof (TObject), SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.BaseClass);
      context.SetCurrentMethod(method, SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.CollectionTraitsOfThis);
    }

    protected override void EmitMethodEpilogue(
      ExpressionTreeContext context,
      SerializerMethod method,
      ExpressionConstruct construct)
    {
      if (construct == null)
        return;
      context.SetDelegate(method, this.EmitMethodEpilogue<SerializerMethod>(context, ExpressionTreeContext.CreateDelegateType<TObject>(method, ExpressionTreeSerializerBuilder<TObject>.SerializerClass), method, construct));
    }

    protected override void EmitMethodEpilogue(
      ExpressionTreeContext context,
      EnumSerializerMethod method,
      ExpressionConstruct construct)
    {
      if (construct == null)
        return;
      context.SetDelegate(method, this.EmitMethodEpilogue<EnumSerializerMethod>(context, ExpressionTreeContext.CreateDelegateType<TObject>(method), method, construct));
    }

    protected override void EmitMethodEpilogue(
      ExpressionTreeContext context,
      CollectionSerializerMethod method,
      ExpressionConstruct construct)
    {
      if (construct == null)
        return;
      context.SetDelegate(method, this.EmitMethodEpilogue<CollectionSerializerMethod>(context, ExpressionTreeContext.CreateDelegateType<TObject>(method, ExpressionTreeSerializerBuilder<TObject>.SerializerClass, SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.CollectionTraitsOfThis), method, construct));
    }

    private Delegate EmitMethodEpilogue<T>(
      ExpressionTreeContext context,
      Type delegateType,
      T method,
      ExpressionConstruct construct)
    {
      if (SerializerDebugging.TraceEnabled)
      {
        SerializerDebugging.TraceEvent("----{0}----", (object) method);
        construct.ToString(SerializerDebugging.ILTraceWriter);
        SerializerDebugging.FlushTraceData();
      }
      LambdaExpression lambdaExpression = Expression.Lambda(delegateType, construct.Expression, method.ToString(), false, (IEnumerable<ParameterExpression>) context.GetCurrentParameters());
      if (SerializerDebugging.DumpEnabled)
      {
        MethodBuilder method1 = this._typeBuilder.DefineMethod(method.ToString(), MethodAttributes.Public | MethodAttributes.Static, lambdaExpression.Type, lambdaExpression.Parameters.Select<ParameterExpression, Type>((Func<ParameterExpression, Type>) (e => e.Type)).ToArray<Type>());
        lambdaExpression.CompileToMethod(method1);
      }
      return lambdaExpression.Compile();
    }

    protected override ExpressionConstruct MakeNullLiteral(
      ExpressionTreeContext context,
      Type contextType)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) null, contextType);
    }

    protected override ExpressionConstruct MakeByteLiteral(
      ExpressionTreeContext context,
      byte constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeSByteLiteral(
      ExpressionTreeContext context,
      sbyte constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeInt16Literal(
      ExpressionTreeContext context,
      short constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeUInt16Literal(
      ExpressionTreeContext context,
      ushort constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeInt32Literal(
      ExpressionTreeContext context,
      int constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeUInt32Literal(
      ExpressionTreeContext context,
      uint constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeInt64Literal(
      ExpressionTreeContext context,
      long constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeUInt64Literal(
      ExpressionTreeContext context,
      ulong constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeReal32Literal(
      ExpressionTreeContext context,
      float constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeReal64Literal(
      ExpressionTreeContext context,
      double constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeBooleanLiteral(
      ExpressionTreeContext context,
      bool constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeCharLiteral(
      ExpressionTreeContext context,
      char constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeStringLiteral(
      ExpressionTreeContext context,
      string constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant((object) constant);
    }

    protected override ExpressionConstruct MakeEnumLiteral(
      ExpressionTreeContext context,
      Type type,
      object constant)
    {
      return (ExpressionConstruct) (Expression) Expression.Constant(constant, type);
    }

    protected override ExpressionConstruct MakeDefaultLiteral(
      ExpressionTreeContext context,
      Type type)
    {
      return (ExpressionConstruct) (Expression) Expression.Default(type);
    }

    protected override ExpressionConstruct EmitThisReferenceExpression(
      ExpressionTreeContext context)
    {
      return context.This;
    }

    protected override ExpressionConstruct EmitBoxExpression(
      ExpressionTreeContext context,
      Type valueType,
      ExpressionConstruct value)
    {
      return (ExpressionConstruct) (Expression) Expression.Convert((Expression) value, typeof (object));
    }

    protected override ExpressionConstruct EmitUnboxAnyExpression(
      ExpressionTreeContext context,
      Type targetType,
      ExpressionConstruct value)
    {
      return (ExpressionConstruct) (Expression) Expression.Convert((Expression) value, targetType);
    }

    protected override ExpressionConstruct EmitNotExpression(
      ExpressionTreeContext context,
      ExpressionConstruct booleanExpression)
    {
      return (ExpressionConstruct) (Expression) Expression.Not((Expression) booleanExpression);
    }

    protected override ExpressionConstruct EmitEqualsExpression(
      ExpressionTreeContext context,
      ExpressionConstruct left,
      ExpressionConstruct right)
    {
      return (ExpressionConstruct) (Expression) Expression.Equal((Expression) left, (Expression) right);
    }

    protected override ExpressionConstruct EmitNotEqualsExpression(
      ExpressionTreeContext context,
      ExpressionConstruct left,
      ExpressionConstruct right)
    {
      return (ExpressionConstruct) (Expression) Expression.NotEqual((Expression) left, (Expression) right);
    }

    protected override ExpressionConstruct EmitGreaterThanExpression(
      ExpressionTreeContext context,
      ExpressionConstruct left,
      ExpressionConstruct right)
    {
      return (ExpressionConstruct) (Expression) Expression.GreaterThan((Expression) left, (Expression) right);
    }

    protected override ExpressionConstruct EmitLessThanExpression(
      ExpressionTreeContext context,
      ExpressionConstruct left,
      ExpressionConstruct right)
    {
      return (ExpressionConstruct) (Expression) Expression.LessThan((Expression) left, (Expression) right);
    }

    protected override ExpressionConstruct EmitIncrement(
      ExpressionTreeContext context,
      ExpressionConstruct int32Value)
    {
      return (ExpressionConstruct) (Expression) Expression.Assign((Expression) int32Value, (Expression) Expression.Increment((Expression) int32Value));
    }

    protected override ExpressionConstruct EmitTypeOfExpression(
      ExpressionTreeContext context,
      Type type)
    {
      return SerializerDebugging.DumpEnabled ? (ExpressionConstruct) (Expression) Expression.Constant((object) type) : (ExpressionConstruct) (Expression) Expression.Call(_Type.GetTypeFromHandle, (Expression) Expression.Constant((object) type.TypeHandle));
    }

    protected override ExpressionConstruct EmitMethodOfExpression(
      ExpressionTreeContext context,
      MethodBase method)
    {
      return SerializerDebugging.DumpEnabled ? (ExpressionConstruct) (Expression) Expression.Constant((object) method) : (ExpressionConstruct) (Expression) Expression.Call(_MethodBase.GetMethodFromHandle, (Expression) Expression.Constant((object) method.MethodHandle), (Expression) Expression.Constant((object) method.DeclaringType.TypeHandle));
    }

    protected override ExpressionConstruct EmitFieldOfExpression(
      ExpressionTreeContext context,
      FieldInfo field)
    {
      return SerializerDebugging.DumpEnabled ? (ExpressionConstruct) (Expression) Expression.Constant((object) field) : (ExpressionConstruct) (Expression) Expression.Call(_FieldInfo.GetFieldFromHandle, (Expression) Expression.Constant((object) field.FieldHandle), (Expression) Expression.Constant((object) field.DeclaringType.TypeHandle));
    }

    protected override ExpressionConstruct EmitSequentialStatements(
      ExpressionTreeContext context,
      Type contextType,
      IEnumerable<ExpressionConstruct> statements)
    {
      ExpressionConstruct[] array = statements.Where<ExpressionConstruct>((Func<ExpressionConstruct, bool>) (s => s != null)).ToArray<ExpressionConstruct>();
      return (ExpressionConstruct) (Expression) Expression.Block(contextType, ((IEnumerable<ExpressionConstruct>) array).Select<ExpressionConstruct, Expression>((Func<ExpressionConstruct, Expression>) (c => c.Expression)).OfType<ParameterExpression>().Distinct<ParameterExpression>(), ((IEnumerable<ExpressionConstruct>) array).Where<ExpressionConstruct>((Func<ExpressionConstruct, bool>) (c => c.IsSignificantReference || !(c.Expression is ParameterExpression))).Select<ExpressionConstruct, Expression>((Func<ExpressionConstruct, Expression>) (c => c.Expression)));
    }

    protected override ExpressionConstruct DeclareLocal(
      ExpressionTreeContext context,
      Type type,
      string name)
    {
      return (ExpressionConstruct) (Expression) Expression.Variable(type, name);
    }

    protected override ExpressionConstruct ReferArgument(
      ExpressionTreeContext context,
      Type type,
      string name,
      int index)
    {
      return (ExpressionConstruct) (Expression) context.GetCurrentParameters()[index];
    }

    protected override ExpressionConstruct EmitCreateNewObjectExpression(
      ExpressionTreeContext context,
      ExpressionConstruct variable,
      ConstructorInfo constructor,
      params ExpressionConstruct[] arguments)
    {
      return (ExpressionConstruct) (Expression) Expression.New(constructor, ((IEnumerable<ExpressionConstruct>) arguments).Select<ExpressionConstruct, Expression>((Func<ExpressionConstruct, Expression>) (c => c.Expression)));
    }

    protected override ExpressionConstruct EmitInvokeVoidMethod(
      ExpressionTreeContext context,
      ExpressionConstruct instance,
      MethodInfo method,
      params ExpressionConstruct[] arguments)
    {
      return this.EmitInvokeMethodExpression(context, instance, method, arguments);
    }

    protected override ExpressionConstruct EmitInvokeMethodExpression(
      ExpressionTreeContext context,
      ExpressionConstruct instance,
      MethodInfo method,
      IEnumerable<ExpressionConstruct> arguments)
    {
      return (ExpressionConstruct) (instance == null ? (Expression) Expression.Call(method, arguments.Select<ExpressionConstruct, Expression>((Func<ExpressionConstruct, Expression>) (c => c.Expression))) : (Expression) Expression.Call((Expression) instance, method, arguments.Select<ExpressionConstruct, Expression>((Func<ExpressionConstruct, Expression>) (c => c.Expression))));
    }

    protected override ExpressionConstruct EmitGetPropertyExpression(
      ExpressionTreeContext context,
      ExpressionConstruct instance,
      PropertyInfo property)
    {
      return (ExpressionConstruct) (Expression) Expression.Property((Expression) instance, property);
    }

    protected override ExpressionConstruct EmitGetFieldExpression(
      ExpressionTreeContext context,
      ExpressionConstruct instance,
      FieldInfo field)
    {
      return (ExpressionConstruct) (Expression) Expression.Field((Expression) instance, field);
    }

    protected override ExpressionConstruct EmitSetProperty(
      ExpressionTreeContext context,
      ExpressionConstruct instance,
      PropertyInfo property,
      ExpressionConstruct value)
    {
      return (ExpressionConstruct) (Expression) Expression.Assign((Expression) Expression.Property((Expression) instance, property), (Expression) value);
    }

    protected override ExpressionConstruct EmitSetField(
      ExpressionTreeContext context,
      ExpressionConstruct instance,
      FieldInfo field,
      ExpressionConstruct value)
    {
      return (ExpressionConstruct) (Expression) Expression.Assign((Expression) Expression.Field((Expression) instance, field), (Expression) value);
    }

    protected override ExpressionConstruct EmitLoadVariableExpression(
      ExpressionTreeContext context,
      ExpressionConstruct variable)
    {
      return new ExpressionConstruct((Expression) variable, true);
    }

    protected override ExpressionConstruct EmitStoreVariableStatement(
      ExpressionTreeContext context,
      ExpressionConstruct variable,
      ExpressionConstruct value)
    {
      return (ExpressionConstruct) (Expression) Expression.Assign((Expression) variable, (Expression) value);
    }

    protected override ExpressionConstruct EmitStoreVariableStatement(
      ExpressionTreeContext context,
      ExpressionConstruct variable)
    {
      return (ExpressionConstruct) null;
    }

    protected override ExpressionConstruct EmitThrowExpression(
      ExpressionTreeContext context,
      Type expressionType,
      ExpressionConstruct exceptionExpression)
    {
      return (ExpressionConstruct) (Expression) Expression.Throw((Expression) exceptionExpression, expressionType);
    }

    protected override ExpressionConstruct EmitTryFinally(
      ExpressionTreeContext context,
      ExpressionConstruct tryStatement,
      ExpressionConstruct finallyStatement)
    {
      return (ExpressionConstruct) (Expression) Expression.TryFinally((Expression) tryStatement, (Expression) finallyStatement);
    }

    protected override ExpressionConstruct EmitCreateNewArrayExpression(
      ExpressionTreeContext context,
      Type elementType,
      int length)
    {
      return (ExpressionConstruct) (Expression) Expression.NewArrayBounds(elementType, (Expression) Expression.Constant((object) length));
    }

    protected override ExpressionConstruct EmitCreateNewArrayExpression(
      ExpressionTreeContext context,
      Type elementType,
      int length,
      IEnumerable<ExpressionConstruct> initialElements)
    {
      return (ExpressionConstruct) (Expression) Expression.NewArrayInit(elementType, initialElements.Select<ExpressionConstruct, Expression>((Func<ExpressionConstruct, Expression>) (c => c.Expression)));
    }

    protected override ExpressionConstruct EmitSetArrayElementStatement(
      ExpressionTreeContext context,
      ExpressionConstruct array,
      ExpressionConstruct index,
      ExpressionConstruct value)
    {
      return (ExpressionConstruct) (Expression) Expression.Assign((Expression) Expression.ArrayAccess((Expression) array, (Expression) index), (Expression) value);
    }

    protected override ExpressionConstruct EmitConditionalExpression(
      ExpressionTreeContext context,
      ExpressionConstruct conditionExpression,
      ExpressionConstruct thenExpression,
      ExpressionConstruct elseExpression)
    {
      return (ExpressionConstruct) (elseExpression == null ? (Expression) Expression.IfThen((Expression) conditionExpression, (Expression) thenExpression) : (thenExpression.ContextType == typeof (void) || elseExpression.ContextType == typeof (void) ? (Expression) Expression.IfThenElse((Expression) conditionExpression, (Expression) thenExpression, (Expression) elseExpression) : (Expression) Expression.Condition((Expression) conditionExpression, (Expression) thenExpression, (Expression) elseExpression)));
    }

    protected override ExpressionConstruct EmitAndConditionalExpression(
      ExpressionTreeContext context,
      IList<ExpressionConstruct> conditionExpressions,
      ExpressionConstruct thenExpression,
      ExpressionConstruct elseExpression)
    {
      return (ExpressionConstruct) (Expression) Expression.IfThenElse((Expression) conditionExpressions.Aggregate<ExpressionConstruct>((Func<ExpressionConstruct, ExpressionConstruct, ExpressionConstruct>) ((l, r) => (ExpressionConstruct) (Expression) Expression.AndAlso((Expression) l, (Expression) r))), (Expression) thenExpression, (Expression) elseExpression);
    }

    protected override ExpressionConstruct EmitStringSwitchStatement(
      ExpressionTreeContext context,
      ExpressionConstruct target,
      IDictionary<string, ExpressionConstruct> cases,
      ExpressionConstruct defaultCase)
    {
      return (ExpressionConstruct) (Expression) Expression.Switch(typeof (void), (Expression) target, (Expression) defaultCase, _String.op_Equality, cases.Select<KeyValuePair<string, ExpressionConstruct>, SwitchCase>((Func<KeyValuePair<string, ExpressionConstruct>, SwitchCase>) (kv => Expression.SwitchCase((Expression) kv.Value, (Expression) Expression.Constant((object) kv.Key)))).ToArray<SwitchCase>());
    }

    protected override ExpressionConstruct EmitForLoop(
      ExpressionTreeContext context,
      ExpressionConstruct count,
      Func<SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.ForLoopContext, ExpressionConstruct> loopBodyEmitter)
    {
      ParameterExpression parameterExpression = Expression.Variable(typeof (int), "i");
      SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.ForLoopContext forLoopContext = new SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.ForLoopContext((ExpressionConstruct) (Expression) parameterExpression);
      LabelTarget labelTarget = Expression.Label("END_FOR");
      return (ExpressionConstruct) (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        parameterExpression
      }, (Expression) Expression.Loop((Expression) Expression.IfThenElse((Expression) Expression.LessThan((Expression) parameterExpression, (Expression) count), (Expression) Expression.Block((Expression) loopBodyEmitter(forLoopContext), (Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Increment((Expression) parameterExpression))), (Expression) Expression.Break(labelTarget)), labelTarget));
    }

    protected override ExpressionConstruct EmitForEachLoop(
      ExpressionTreeContext context,
      CollectionTraits collectionTraits,
      ExpressionConstruct collection,
      Func<ExpressionConstruct, ExpressionConstruct> loopBodyEmitter)
    {
      ParameterExpression parameterExpression1 = Expression.Variable(collectionTraits.GetEnumeratorMethod.ReturnType, "enumerator");
      ParameterExpression parameterExpression2 = Expression.Variable(collectionTraits.ElementType, "current");
      MethodInfo enumeratorMoveNextMethod = _IEnumerator.FindEnumeratorMoveNextMethod(parameterExpression1.Type);
      PropertyInfo enumeratorCurrentProperty = _IEnumerator.FindEnumeratorCurrentProperty(parameterExpression1.Type, collectionTraits);
      LabelTarget labelTarget = Expression.Label("END_FOREACH");
      return (ExpressionConstruct) (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
      {
        parameterExpression1,
        parameterExpression2
      }, (Expression) Expression.Assign((Expression) parameterExpression1, (Expression) Expression.Call((Expression) collection, collectionTraits.GetEnumeratorMethod)), (Expression) Expression.Loop((Expression) Expression.IfThenElse((Expression) Expression.Call((Expression) parameterExpression1, enumeratorMoveNextMethod), (Expression) Expression.Block((Expression) Expression.Assign((Expression) parameterExpression2, (Expression) Expression.Property((Expression) parameterExpression1, enumeratorCurrentProperty)), (Expression) loopBodyEmitter((ExpressionConstruct) (Expression) parameterExpression2)), (Expression) Expression.Break(labelTarget)), labelTarget));
    }

    protected override ExpressionConstruct EmitEnumFromUnderlyingCastExpression(
      ExpressionTreeContext context,
      Type enumType,
      ExpressionConstruct underlyingValue)
    {
      return (ExpressionConstruct) (Expression) Expression.Convert((Expression) underlyingValue, enumType);
    }

    protected override ExpressionConstruct EmitEnumToUnderlyingCastExpression(
      ExpressionTreeContext context,
      Type underlyingType,
      ExpressionConstruct enumValue)
    {
      return (ExpressionConstruct) (Expression) Expression.Convert((Expression) enumValue, underlyingType);
    }

    protected override Func<SerializationContext, MessagePackSerializer<TObject>> CreateSerializerConstructor(
      ExpressionTreeContext codeGenerationContext,
      PolymorphismSchema schema)
    {
      if (SerializerDebugging.DumpEnabled)
        this._typeBuilder.CreateType();
      return ExpressionTreeSerializerBuilderHelpers.CreateFactory<TObject>(codeGenerationContext, SerializerBuilder<ExpressionTreeContext, ExpressionConstruct, TObject>.CollectionTraitsOfThis, schema);
    }

    protected override Func<SerializationContext, MessagePackSerializer<TObject>> CreateEnumSerializerConstructor(
      ExpressionTreeContext codeGenerationContext)
    {
      if (SerializerDebugging.DumpEnabled)
        this._typeBuilder.CreateType();
      Delegate packUnderyingValueTo = codeGenerationContext.GetPackUnderyingValueTo();
      Delegate unpackFromUnderlyingValue = codeGenerationContext.GetUnpackFromUnderlyingValue();
      Type targetType = typeof (ExpressionCallbackEnumMessagePackSerializer<>).MakeGenericType(typeof (TObject));
      return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => ReflectionExtensions.CreateInstancePreservingExceptionType<MessagePackSerializer<TObject>>(targetType, (object) context, (object) EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethod(context, typeof (TObject), EnumMemberSerializationMethod.Default), (object) packUnderyingValueTo, (object) unpackFromUnderlyingValue));
    }

    protected override ExpressionTreeContext CreateCodeGenerationContextForSerializerCreation(
      SerializationContext context)
    {
      return new ExpressionTreeContext(context);
    }
  }
}
