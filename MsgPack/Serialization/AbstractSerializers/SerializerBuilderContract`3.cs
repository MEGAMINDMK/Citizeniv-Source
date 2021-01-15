// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.AbstractSerializers.SerializerBuilderContract`3
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MsgPack.Serialization.AbstractSerializers
{
  internal class SerializerBuilderContract<TContext, TConstruct, TObject> : SerializerBuilder<TContext, TConstruct, TObject>
    where TContext : SerializerGenerationContext<TConstruct>
    where TConstruct : class, ICodeConstruct
  {
    protected override TContext CreateCodeGenerationContextForSerializerCreation(
      SerializationContext context)
    {
      return default (TContext);
    }

    protected override void BuildSerializerCodeCore(
      ISerializerCodeGenerationContext context,
      Type concreteType,
      PolymorphismSchema itemSchema)
    {
    }

    protected override Func<SerializationContext, MessagePackSerializer<TObject>> CreateSerializerConstructor(
      TContext codeGenerationContext,
      PolymorphismSchema schema)
    {
      return (Func<SerializationContext, MessagePackSerializer<TObject>>) null;
    }

    protected override Func<SerializationContext, MessagePackSerializer<TObject>> CreateEnumSerializerConstructor(
      TContext codeGenerationContext)
    {
      return (Func<SerializationContext, MessagePackSerializer<TObject>>) null;
    }

    protected override void EmitMethodPrologue(TContext context, SerializerMethod method)
    {
    }

    protected override void EmitMethodPrologue(TContext context, EnumSerializerMethod method)
    {
    }

    protected override void EmitMethodPrologue(
      TContext context,
      CollectionSerializerMethod method,
      MethodInfo declaration)
    {
    }

    protected override void EmitMethodEpilogue(
      TContext context,
      SerializerMethod method,
      TConstruct construct)
    {
    }

    protected override void EmitMethodEpilogue(
      TContext context,
      EnumSerializerMethod method,
      TConstruct construct)
    {
    }

    protected override void EmitMethodEpilogue(
      TContext context,
      CollectionSerializerMethod method,
      TConstruct construct)
    {
    }

    protected override TConstruct EmitSequentialStatements(
      TContext context,
      Type contextType,
      IEnumerable<TConstruct> statements)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeNullLiteral(TContext context, Type contextType)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeByteLiteral(TContext context, byte constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeSByteLiteral(TContext context, sbyte constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeInt16Literal(TContext context, short constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeUInt16Literal(TContext context, ushort constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeInt32Literal(TContext context, int constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeUInt32Literal(TContext context, uint constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeInt64Literal(TContext context, long constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeUInt64Literal(TContext context, ulong constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeReal32Literal(TContext context, float constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeReal64Literal(TContext context, double constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeBooleanLiteral(TContext context, bool constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeCharLiteral(TContext context, char constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeStringLiteral(TContext context, string constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeEnumLiteral(TContext context, Type type, object constant)
    {
      return default (TConstruct);
    }

    protected override TConstruct MakeDefaultLiteral(TContext context, Type type)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitThisReferenceExpression(TContext context)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitBoxExpression(
      TContext context,
      Type valueType,
      TConstruct value)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitUnboxAnyExpression(
      TContext context,
      Type targetType,
      TConstruct value)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitNotExpression(TContext context, TConstruct booleanExpression)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitEqualsExpression(
      TContext context,
      TConstruct left,
      TConstruct right)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitNotEqualsExpression(
      TContext context,
      TConstruct left,
      TConstruct right)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitGreaterThanExpression(
      TContext context,
      TConstruct left,
      TConstruct right)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitLessThanExpression(
      TContext context,
      TConstruct left,
      TConstruct right)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitIncrement(TContext context, TConstruct int32Value)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitTypeOfExpression(TContext context, Type type)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitFieldOfExpression(TContext context, FieldInfo field)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitMethodOfExpression(TContext context, MethodBase method)
    {
      return default (TConstruct);
    }

    protected override TConstruct DeclareLocal(TContext context, Type type, string name)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitInvokeVoidMethod(
      TContext context,
      TConstruct instance,
      MethodInfo method,
      params TConstruct[] arguments)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitCreateNewObjectExpression(
      TContext context,
      TConstruct variable,
      ConstructorInfo constructor,
      params TConstruct[] arguments)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitCreateNewArrayExpression(
      TContext context,
      Type elementType,
      int length)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitCreateNewArrayExpression(
      TContext context,
      Type elementType,
      int length,
      IEnumerable<TConstruct> initialElements)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitSetArrayElementStatement(
      TContext context,
      TConstruct array,
      TConstruct index,
      TConstruct value)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitInvokeMethodExpression(
      TContext context,
      TConstruct instance,
      MethodInfo method,
      IEnumerable<TConstruct> arguments)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitGetPropertyExpression(
      TContext context,
      TConstruct instance,
      PropertyInfo property)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitGetFieldExpression(
      TContext context,
      TConstruct instance,
      FieldInfo field)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitSetProperty(
      TContext context,
      TConstruct instance,
      PropertyInfo property,
      TConstruct value)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitSetField(
      TContext context,
      TConstruct instance,
      FieldInfo field,
      TConstruct value)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitLoadVariableExpression(TContext context, TConstruct variable)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitStoreVariableStatement(
      TContext context,
      TConstruct variable,
      TConstruct value)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitThrowExpression(
      TContext context,
      Type expressionType,
      TConstruct exceptionExpression)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitTryFinally(
      TContext context,
      TConstruct tryStatement,
      TConstruct finallyStatement)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitConditionalExpression(
      TContext context,
      TConstruct conditionExpression,
      TConstruct thenExpression,
      TConstruct elseExpression)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitAndConditionalExpression(
      TContext context,
      IList<TConstruct> conditionExpressions,
      TConstruct thenExpression,
      TConstruct elseExpression)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitStringSwitchStatement(
      TContext context,
      TConstruct target,
      IDictionary<string, TConstruct> cases,
      TConstruct defaultCase)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitForLoop(
      TContext context,
      TConstruct count,
      Func<SerializerBuilder<TContext, TConstruct, TObject>.ForLoopContext, TConstruct> loopBodyEmitter)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitForEachLoop(
      TContext context,
      CollectionTraits collectionTraits,
      TConstruct collection,
      Func<TConstruct, TConstruct> loopBodyEmitter)
    {
      return default (TConstruct);
    }

    protected override TConstruct ReferArgument(
      TContext context,
      Type type,
      string name,
      int index)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitEnumFromUnderlyingCastExpression(
      TContext context,
      Type enumType,
      TConstruct underlyingValue)
    {
      return default (TConstruct);
    }

    protected override TConstruct EmitEnumToUnderlyingCastExpression(
      TContext context,
      Type underlyingType,
      TConstruct enumValue)
    {
      return default (TConstruct);
    }
  }
}
