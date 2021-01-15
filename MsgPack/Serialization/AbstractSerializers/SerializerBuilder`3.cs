// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.AbstractSerializers.SerializerBuilder`3
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using MsgPack.Serialization.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.AbstractSerializers
{
  internal abstract class SerializerBuilder<TContext, TConstruct, TObject> : ISerializerCodeGenerator, ISerializerBuilder<TObject>
    where TContext : SerializerGenerationContext<TConstruct>
    where TConstruct : class, ICodeConstruct
  {
    private readonly SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderNilImplicationHandler _nilImplicationHandler = new SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderNilImplicationHandler();
    private static readonly TConstruct[] NoConstructs = new TConstruct[0];
    protected static readonly CollectionTraits CollectionTraitsOfThis;
    protected static readonly Type BaseClass;

    private void BuildCollectionSerializer(
      TContext context,
      Type concreteType,
      PolymorphismSchema schema)
    {
      bool isUnpackFromRequired;
      bool isAddItemRequired;
      Type declaringType;
      SerializerBuilder<TContext, TConstruct, TObject>.DetermineSerializationStrategy(out isUnpackFromRequired, out isAddItemRequired, out declaringType);
      if (isAddItemRequired)
      {
        if (SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.AddMethod != (MethodInfo) null)
          this.BuildCollectionAddItem(context, declaringType, SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis);
        else
          this.BuildCollectionAddItem(context, declaringType, concreteType.GetCollectionTraits());
      }
      if (isUnpackFromRequired)
        this.BuildCollectionUnpackFromCore(context, concreteType, schema);
      this.BuildCollectionCreateInstance(context, concreteType, declaringType);
      this.BuildRestoreSchema(context, schema);
    }

    private static void DetermineSerializationStrategy(
      out bool isUnpackFromRequired,
      out bool isAddItemRequired,
      out Type declaringType)
    {
      switch (SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.DetailedCollectionType)
      {
        case CollectionDetailedKind.NonGenericList:
          isUnpackFromRequired = false;
          isAddItemRequired = false;
          declaringType = typeof (NonGenericEnumerableMessagePackSerializerBase<>).MakeGenericType(typeof (TObject));
          break;
        case CollectionDetailedKind.GenericDictionary:
          isUnpackFromRequired = false;
          isAddItemRequired = false;
          Type[] genericArguments1 = SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.ElementType.GetGenericArguments();
          declaringType = typeof (DictionaryMessagePackSerializer<,,>).MakeGenericType(typeof (TObject), genericArguments1[0], genericArguments1[1]);
          break;
        case CollectionDetailedKind.NonGenericDictionary:
          isUnpackFromRequired = false;
          isAddItemRequired = false;
          declaringType = typeof (NonGenericDictionaryMessagePackSerializer<>).MakeGenericType(typeof (TObject));
          break;
        case CollectionDetailedKind.NonGenericCollection:
        case CollectionDetailedKind.NonGenericEnumerable:
          isUnpackFromRequired = true;
          isAddItemRequired = true;
          declaringType = typeof (NonGenericEnumerableMessagePackSerializerBase<>).MakeGenericType(typeof (TObject));
          break;
        case CollectionDetailedKind.GenericEnumerable:
          isUnpackFromRequired = true;
          isAddItemRequired = true;
          declaringType = typeof (EnumerableMessagePackSerializerBase<,>).MakeGenericType(typeof (TObject), SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.ElementType);
          break;
        case CollectionDetailedKind.GenericReadOnlyList:
        case CollectionDetailedKind.GenericReadOnlyCollection:
          isUnpackFromRequired = false;
          isAddItemRequired = true;
          declaringType = typeof (ReadOnlyCollectionMessagePackSerializer<,>).MakeGenericType(typeof (TObject), SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.ElementType);
          break;
        case CollectionDetailedKind.GenericReadOnlyDictionary:
          isUnpackFromRequired = false;
          isAddItemRequired = true;
          Type[] genericArguments2 = SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.ElementType.GetGenericArguments();
          declaringType = typeof (ReadOnlyDictionaryMessagePackSerializer<,,>).MakeGenericType(typeof (TObject), genericArguments2[0], genericArguments2[1]);
          break;
        default:
          isUnpackFromRequired = false;
          isAddItemRequired = false;
          declaringType = typeof (EnumerableMessagePackSerializerBase<,>).MakeGenericType(typeof (TObject), SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.ElementType);
          break;
      }
    }

    private void BuildCollectionAddItem(
      TContext context,
      Type declaringType,
      CollectionTraits traits)
    {
      MethodInfo serializerMethod = SerializerBuilder<TContext, TConstruct, TObject>.GetCollectionSerializerMethod("AddItem", declaringType);
      this.EmitMethodPrologue(context, CollectionSerializerMethod.AddItem, serializerMethod);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = traits.CollectionType == CollectionKind.Map ? this.EmitAppendDictionaryItem(context, traits, context.CollectionToBeAdded, serializerMethod.GetParameters()[0].ParameterType, context.KeyToAdd, serializerMethod.GetParameters()[1].ParameterType, context.ValueToAdd, false) : this.EmitAppendCollectionItem(context, (MemberInfo) null, traits, context.CollectionToBeAdded, context.ItemToAdd);
      }
      finally
      {
        this.EmitMethodEpilogue(context, CollectionSerializerMethod.AddItem, construct);
      }
    }

    private void BuildCollectionUnpackFromCore(
      TContext context,
      Type concreteType,
      PolymorphismSchema schema)
    {
      this.EmitMethodPrologue(context, SerializerMethod.UnpackFromCore);
      TConstruct construct = default (TConstruct);
      try
      {
        Type type1 = concreteType;
        if ((object) type1 == null)
          type1 = typeof (TObject);
        Type type2 = type1;
        TConstruct collection = this.DeclareLocal(context, type2, "collection");
        construct = this.EmitSequentialStatements(context, collection.ContextType, this.EmitCollectionUnpackFromStatements(context, type2, schema, collection));
      }
      finally
      {
        this.EmitMethodEpilogue(context, SerializerMethod.UnpackFromCore, construct);
      }
    }

    private IEnumerable<TConstruct> EmitCollectionUnpackFromStatements(
      TContext context,
      Type instanceType,
      PolymorphismSchema schema,
      TConstruct collection)
    {
      ConstructorInfo ctor = UnpackHelpers.GetCollectionConstructor(instanceType);
      yield return SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.CollectionType == CollectionKind.Array ? this.EmitCheckIsArrayHeaderExpression(context, context.Unpacker) : this.EmitCheckIsMapHeaderExpression(context, context.Unpacker);
      TConstruct itemsCount = this.DeclareLocal(context, typeof (int), "itemsCount");
      yield return itemsCount;
      yield return this.EmitStoreVariableStatement(context, itemsCount, this.EmitGetItemsCountExpression(context, context.Unpacker));
      yield return collection;
      if (SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.CollectionType == CollectionKind.Array && SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.AddMethod == (MethodInfo) null)
      {
        TContext context1 = context;
        TConstruct variable1 = collection;
        TContext context2 = context;
        TConstruct variable2 = collection;
        ConstructorInfo constructor = ctor;
        TConstruct[] constructArray;
        if (ctor.GetParameters().Length != 0)
          constructArray = new TConstruct[1]{ itemsCount };
        else
          constructArray = SerializerBuilder<TContext, TConstruct, TObject>.NoConstructs;
        TConstruct objectExpression = this.EmitCreateNewObjectExpression(context2, variable2, constructor, constructArray);
        yield return this.EmitStoreVariableStatement(context1, variable1, objectExpression);
        // ISSUE: reference to a compiler-generated field
        yield return this.EmitForLoop(context, itemsCount, (Func<SerializerBuilder<TContext, TConstruct, TObject>.ForLoopContext, TConstruct>) (flc => this.\u003C\u003E4__this.EmitUnpackToCollectionLoopBody(context, flc, instanceType.GetCollectionTraits(), context.Unpacker, collection, (schema ?? PolymorphismSchema.Default).ItemSchema)));
      }
      else
        yield return this.EmitUnpackCollectionWithUnpackToExpression(context, ctor, itemsCount, context.Unpacker, collection);
      yield return this.EmitRetrunStatement(context, this.EmitLoadVariableExpression(context, collection));
    }

    private void BuildCollectionCreateInstance(
      TContext context,
      Type concreteType,
      Type declaringType)
    {
      MethodInfo serializerMethod = SerializerBuilder<TContext, TConstruct, TObject>.GetCollectionSerializerMethod("CreateInstance", declaringType);
      this.EmitMethodPrologue(context, CollectionSerializerMethod.CreateInstance, serializerMethod);
      TConstruct construct = default (TConstruct);
      try
      {
        Type type = concreteType;
        if ((object) type == null)
          type = typeof (TObject);
        Type instanceType = type;
        TConstruct variable = this.DeclareLocal(context, typeof (TObject), "collection");
        ConstructorInfo collectionConstructor = UnpackHelpers.GetCollectionConstructor(instanceType);
        TConstruct[] constructorArguments = this.DetermineCollectionConstructorArguments(context, collectionConstructor);
        construct = this.EmitSequentialStatements(context, typeof (TObject), variable, this.EmitStoreVariableStatement(context, variable, this.EmitCreateNewObjectExpression(context, variable, collectionConstructor, constructorArguments)), this.EmitRetrunStatement(context, this.EmitLoadVariableExpression(context, variable)));
      }
      finally
      {
        this.EmitMethodEpilogue(context, CollectionSerializerMethod.CreateInstance, construct);
      }
    }

    private void BuildRestoreSchema(TContext context, PolymorphismSchema schema)
    {
      this.EmitMethodPrologue(context, CollectionSerializerMethod.RestoreSchema, (MethodInfo) null);
      TConstruct construct1 = default (TConstruct);
      try
      {
        TConstruct construct2 = this.DeclareLocal(context, typeof (PolymorphismSchema), nameof (schema));
        construct1 = this.EmitSequentialStatements(context, typeof (PolymorphismSchema), ((IEnumerable<TConstruct>) new TConstruct[1]
        {
          construct2
        }).Concat<TConstruct>(this.EmitConstructPolymorphismSchema(context, construct2, schema)).Concat<TConstruct>((IEnumerable<TConstruct>) new TConstruct[1]
        {
          this.EmitRetrunStatement(context, this.EmitLoadVariableExpression(context, construct2))
        }));
      }
      finally
      {
        this.EmitMethodEpilogue(context, CollectionSerializerMethod.RestoreSchema, construct1);
      }
    }

    private static MethodInfo GetCollectionSerializerMethod(
      string name,
      Type declaringType)
    {
      return declaringType.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
    }

    private TConstruct EmitPutArrayHeaderExpression(TContext context, TConstruct length)
    {
      return this.EmitInvokeVoidMethod(context, context.Packer, _Packer.PackArrayHeader, length);
    }

    private TConstruct EmitPutMapHeaderExpression(TContext context, TConstruct collectionCount)
    {
      return this.EmitInvokeVoidMethod(context, context.Packer, _Packer.PackMapHeader, collectionCount);
    }

    private TConstruct EmitCheckIsArrayHeaderExpression(TContext context, TConstruct unpacker)
    {
      return this.EmitConditionalExpression(context, this.EmitNotExpression(context, this.EmitGetPropertyExpression(context, unpacker, _Unpacker.IsArrayHeader)), this.EmitThrowExpression(context, typeof (Unpacker), SerializationExceptions.NewIsNotArrayHeaderMethod), default (TConstruct));
    }

    private TConstruct EmitCheckIsMapHeaderExpression(TContext context, TConstruct unpacker)
    {
      return this.EmitConditionalExpression(context, this.EmitNotExpression(context, this.EmitGetPropertyExpression(context, unpacker, _Unpacker.IsMapHeader)), this.EmitThrowExpression(context, typeof (Unpacker), SerializationExceptions.NewIsNotMapHeaderMethod), default (TConstruct));
    }

    private TConstruct EmitUnpackToCollectionLoopBody(
      TContext context,
      SerializerBuilder<TContext, TConstruct, TObject>.ForLoopContext forLoopContext,
      CollectionTraits traitsOfTheCollection,
      TConstruct unpacker,
      TConstruct collection,
      PolymorphismSchema itemsSchema)
    {
      return this.EmitUnpackItemValueExpression(context, traitsOfTheCollection.ElementType, context.CollectionItemNilImplication, unpacker, forLoopContext.Counter, this.EmitInvariantStringFormat(context, "item{0}", forLoopContext.Counter), default (TConstruct), default (TConstruct), new SerializingMember?(), itemsSchema, (Func<TConstruct, TConstruct>) (unpackedItem => this.EmitAppendCollectionItem(context, (MemberInfo) null, traitsOfTheCollection, collection, unpackedItem)));
    }

    private void BuildNullableSerializer(TContext context, Type underlyingType)
    {
      this.BuildNullablePackTo(context, underlyingType);
      this.BuildNullableUnpackFrom(context, underlyingType);
    }

    private void BuildNullablePackTo(TContext context, Type underlyingType)
    {
      this.EmitMethodPrologue(context, SerializerMethod.PackToCore);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = this.EmitSerializeItemExpressionCore(context, context.Packer, underlyingType, this.EmitGetProperty(context, context.PackToTarget, typeof (TObject).GetProperty("Value"), false), new SerializingMember?(), (PolymorphismSchema) null);
      }
      finally
      {
        this.EmitMethodEpilogue(context, SerializerMethod.PackToCore, construct);
      }
    }

    private void BuildNullableUnpackFrom(TContext context, Type underlyingType)
    {
      this.EmitMethodPrologue(context, SerializerMethod.UnpackFromCore);
      TConstruct construct = default (TConstruct);
      try
      {
        TConstruct variable = this.DeclareLocal(context, typeof (TObject), "result");
        construct = this.EmitRetrunStatement(context, this.EmitCreateNewObjectExpression(context, variable, ((IEnumerable<ConstructorInfo>) typeof (TObject).GetConstructors()).Single<ConstructorInfo>((Func<ConstructorInfo, bool>) (c => c.GetParameters().Length == 1)), this.EmitDeserializeItemExpressionCore(context, context.Unpacker, underlyingType, new SerializingMember?(), (PolymorphismSchema) null)));
      }
      finally
      {
        this.EmitMethodEpilogue(context, SerializerMethod.UnpackFromCore, construct);
      }
    }

    protected void BuildEnumSerializer(TContext context)
    {
      Type underlyingType = Enum.GetUnderlyingType(typeof (TObject));
      this.BuildPackUnderlyingValueTo(context, underlyingType);
      this.BuildUnpackFromUnderlyingValue(context, underlyingType);
    }

    private void BuildPackUnderlyingValueTo(TContext context, Type underlyingType)
    {
      this.EmitMethodPrologue(context, EnumSerializerMethod.PackUnderlyingValueTo);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = this.EmitInvokeVoidMethod(context, this.ReferArgument(context, typeof (Packer), "packer", 1), typeof (Packer).GetMethod("Pack", new Type[1]
        {
          underlyingType
        }), this.EmitEnumToUnderlyingCastExpression(context, underlyingType, this.ReferArgument(context, typeof (TObject), "enumValue", 2)));
      }
      finally
      {
        this.EmitMethodEpilogue(context, EnumSerializerMethod.PackUnderlyingValueTo, construct);
      }
    }

    private void BuildUnpackFromUnderlyingValue(TContext context, Type underlyingType)
    {
      this.EmitMethodPrologue(context, EnumSerializerMethod.UnpackFromUnderlyingValue);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = this.EmitRetrunStatement(context, this.EmitEnumFromUnderlyingCastExpression(context, typeof (TObject), this.EmitInvokeMethodExpression(context, this.ReferArgument(context, typeof (MessagePackObject), "messagePackObject", 1), typeof (MessagePackObject).GetMethod("As" + underlyingType.Name, ReflectionAbstractions.EmptyTypes))));
      }
      finally
      {
        this.EmitMethodEpilogue(context, EnumSerializerMethod.UnpackFromUnderlyingValue, construct);
      }
    }

    protected abstract TConstruct EmitEnumToUnderlyingCastExpression(
      TContext context,
      Type underlyingType,
      TConstruct enumValue);

    protected abstract TConstruct EmitEnumFromUnderlyingCastExpression(
      TContext context,
      Type enumType,
      TConstruct underlyingValue);

    protected abstract void EmitMethodPrologue(TContext context, SerializerMethod method);

    protected abstract void EmitMethodPrologue(TContext context, EnumSerializerMethod method);

    protected abstract void EmitMethodPrologue(
      TContext context,
      CollectionSerializerMethod method,
      MethodInfo declaration);

    protected abstract void EmitMethodEpilogue(
      TContext context,
      SerializerMethod method,
      TConstruct construct);

    protected abstract void EmitMethodEpilogue(
      TContext context,
      EnumSerializerMethod method,
      TConstruct construct);

    protected abstract void EmitMethodEpilogue(
      TContext context,
      CollectionSerializerMethod method,
      TConstruct construct);

    private TConstruct MakeDefaultParameterValueLiteral(
      TContext context,
      TConstruct targetVariable,
      Type literalType,
      object literal,
      bool hasDefault)
    {
      if (literalType == typeof (byte))
        return this.MakeByteLiteral(context, !hasDefault ? (byte) 0 : (byte) literal);
      if (literalType == typeof (sbyte))
        return this.MakeSByteLiteral(context, !hasDefault ? (sbyte) 0 : (sbyte) literal);
      if (literalType == typeof (short))
        return this.MakeInt16Literal(context, !hasDefault ? (short) 0 : (short) literal);
      if (literalType == typeof (ushort))
        return this.MakeUInt16Literal(context, !hasDefault ? (ushort) 0 : (ushort) literal);
      if (literalType == typeof (int))
        return this.MakeInt32Literal(context, !hasDefault ? 0 : (int) literal);
      if (literalType == typeof (uint))
        return this.MakeUInt32Literal(context, !hasDefault ? 0U : (uint) literal);
      if (literalType == typeof (long))
        return this.MakeInt64Literal(context, !hasDefault ? 0L : (long) literal);
      if (literalType == typeof (ulong))
        return this.MakeUInt64Literal(context, !hasDefault ? 0UL : (ulong) literal);
      if (literalType == typeof (float))
        return this.MakeReal32Literal(context, !hasDefault ? 0.0f : (float) literal);
      if (literalType == typeof (double))
        return this.MakeReal64Literal(context, !hasDefault ? 0.0 : (double) literal);
      if (literalType == typeof (Decimal))
        return this.MakeDecimalLiteral(context, targetVariable, !hasDefault ? new Decimal(0) : (Decimal) literal);
      if (literalType == typeof (bool))
        return this.MakeBooleanLiteral(context, hasDefault && (bool) literal);
      if (literalType == typeof (char))
        return this.MakeCharLiteral(context, !hasDefault ? char.MinValue : (char) literal);
      if (literalType.GetIsEnum())
        return this.MakeEnumLiteral(context, literalType, !hasDefault ? Enum.ToObject(literalType, 0) : literal);
      if (literal != null && hasDefault)
      {
        if (literalType == typeof (string))
          return this.MakeStringLiteral(context, literal as string);
        if (literalType.GetIsValueType())
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Literal for value type '{0}' is not supported.", (object) literalType));
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Literal for reference type '{0}' is not supported except null reference.", (object) literalType));
      }
      return literalType.GetIsValueType() ? this.MakeDefaultLiteral(context, literalType) : this.MakeNullLiteral(context, literalType);
    }

    private TConstruct MakeDecimalLiteral(
      TContext context,
      TConstruct targetVariable,
      Decimal constant)
    {
      int[] bits = Decimal.GetBits(constant);
      return this.EmitCreateNewObjectExpression(context, targetVariable, _Decimal.Constructor, this.MakeInt32Literal(context, bits[0]), this.MakeInt32Literal(context, bits[1]), this.MakeInt32Literal(context, bits[2]), this.MakeBooleanLiteral(context, ((long) bits[3] & 2147483648L) != 0L), this.MakeByteLiteral(context, (byte) (bits[3] >> 16 & (int) byte.MaxValue)));
    }

    protected abstract TConstruct MakeNullLiteral(TContext context, Type contextType);

    protected abstract TConstruct MakeByteLiteral(TContext context, byte constant);

    protected abstract TConstruct MakeSByteLiteral(TContext context, sbyte constant);

    protected abstract TConstruct MakeInt16Literal(TContext context, short constant);

    protected abstract TConstruct MakeUInt16Literal(TContext context, ushort constant);

    protected abstract TConstruct MakeInt32Literal(TContext context, int constant);

    protected abstract TConstruct MakeUInt32Literal(TContext context, uint constant);

    protected abstract TConstruct MakeInt64Literal(TContext context, long constant);

    protected abstract TConstruct MakeUInt64Literal(TContext context, ulong constant);

    protected abstract TConstruct MakeReal32Literal(TContext context, float constant);

    protected abstract TConstruct MakeReal64Literal(TContext context, double constant);

    protected abstract TConstruct MakeBooleanLiteral(TContext context, bool constant);

    protected abstract TConstruct MakeCharLiteral(TContext context, char constant);

    protected abstract TConstruct MakeStringLiteral(TContext context, string constant);

    protected abstract TConstruct MakeEnumLiteral(TContext context, Type type, object constant);

    protected abstract TConstruct MakeDefaultLiteral(TContext context, Type type);

    protected abstract TConstruct EmitThisReferenceExpression(TContext context);

    protected abstract TConstruct EmitBoxExpression(
      TContext context,
      Type valueType,
      TConstruct value);

    protected abstract TConstruct EmitUnboxAnyExpression(
      TContext context,
      Type targetType,
      TConstruct value);

    protected abstract TConstruct EmitNotExpression(TContext context, TConstruct booleanExpression);

    protected abstract TConstruct EmitEqualsExpression(
      TContext context,
      TConstruct left,
      TConstruct right);

    protected virtual TConstruct EmitNotEqualsExpression(
      TContext context,
      TConstruct left,
      TConstruct right)
    {
      return this.EmitNotExpression(context, this.EmitEqualsExpression(context, left, right));
    }

    protected abstract TConstruct EmitGreaterThanExpression(
      TContext context,
      TConstruct left,
      TConstruct right);

    protected abstract TConstruct EmitLessThanExpression(
      TContext context,
      TConstruct left,
      TConstruct right);

    protected abstract TConstruct EmitIncrement(TContext context, TConstruct int32Value);

    protected abstract TConstruct EmitTypeOfExpression(TContext context, Type type);

    protected TConstruct EmitSequentialStatements(
      TContext context,
      Type contextType,
      params TConstruct[] statements)
    {
      return this.EmitSequentialStatements(context, contextType, (IEnumerable<TConstruct>) statements);
    }

    protected abstract TConstruct EmitSequentialStatements(
      TContext context,
      Type contextType,
      IEnumerable<TConstruct> statements);

    protected abstract TConstruct ReferArgument(
      TContext context,
      Type type,
      string name,
      int index);

    protected abstract TConstruct DeclareLocal(TContext context, Type type, string name);

    protected abstract TConstruct EmitCreateNewObjectExpression(
      TContext context,
      TConstruct variable,
      ConstructorInfo constructor,
      params TConstruct[] arguments);

    protected abstract TConstruct EmitInvokeVoidMethod(
      TContext context,
      TConstruct instance,
      MethodInfo method,
      params TConstruct[] arguments);

    protected TConstruct EmitInvokeMethodExpression(
      TContext context,
      TConstruct instance,
      MethodInfo method,
      params TConstruct[] arguments)
    {
      return this.EmitInvokeMethodExpression(context, instance, method, (IEnumerable<TConstruct>) (arguments ?? new TConstruct[0]));
    }

    protected abstract TConstruct EmitInvokeMethodExpression(
      TContext context,
      TConstruct instance,
      MethodInfo method,
      IEnumerable<TConstruct> arguments);

    private TConstruct EmitGetMemberValueExpression(
      TContext context,
      TConstruct instance,
      MemberInfo member)
    {
      FieldInfo fieldInfo;
      if ((fieldInfo = member as FieldInfo) != (FieldInfo) null)
        return this.EmitGetField(context, instance, fieldInfo, !fieldInfo.GetHasPublicGetter());
      PropertyInfo propertyInfo = member as PropertyInfo;
      return this.EmitGetProperty(context, instance, propertyInfo, !propertyInfo.GetHasPublicGetter());
    }

    private TConstruct EmitGetProperty(
      TContext context,
      TConstruct instance,
      PropertyInfo property,
      bool withReflection)
    {
      if (!withReflection)
        return this.EmitGetPropertyExpression(context, instance, property);
      return this.EmitUnboxAnyExpression(context, property.PropertyType, this.EmitInvokeMethodExpression(context, this.EmitMethodOfExpression(context, (MethodBase) property.GetGetMethod(true)), _MethodBase.Invoke_2, instance, this.MakeNullLiteral(context, typeof (object[]))));
    }

    protected abstract TConstruct EmitGetPropertyExpression(
      TContext context,
      TConstruct instance,
      PropertyInfo property);

    private TConstruct EmitGetField(
      TContext context,
      TConstruct instance,
      FieldInfo field,
      bool withReflection)
    {
      if (!withReflection)
        return this.EmitGetFieldExpression(context, instance, field);
      return this.EmitUnboxAnyExpression(context, field.FieldType, this.EmitInvokeMethodExpression(context, this.EmitFieldOfExpression(context, field), _FieldInfo.GetValue, instance));
    }

    protected abstract TConstruct EmitGetFieldExpression(
      TContext context,
      TConstruct instance,
      FieldInfo field);

    private TConstruct EmitSetMemberValueStatement(
      TContext context,
      TConstruct instance,
      MemberInfo member,
      TConstruct value)
    {
      PropertyInfo propertyInfo = (PropertyInfo) null;
      FieldInfo fieldInfo;
      TConstruct getCollection;
      CollectionTraits traits;
      if ((fieldInfo = member as FieldInfo) != (FieldInfo) null)
      {
        if (!fieldInfo.IsInitOnly && fieldInfo.GetIsPublic())
          return this.EmitSetField(context, instance, fieldInfo, value, false);
        getCollection = this.EmitGetField(context, instance, fieldInfo, !fieldInfo.GetHasPublicGetter());
        traits = fieldInfo.FieldType.GetCollectionTraits();
      }
      else
      {
        propertyInfo = member as PropertyInfo;
        MethodInfo setMethod = propertyInfo.GetSetMethod(true);
        if (setMethod != (MethodInfo) null && setMethod.GetIsPublic())
          return this.EmitSetProperty(context, instance, propertyInfo, value, false);
        getCollection = this.EmitGetProperty(context, instance, propertyInfo, !propertyInfo.GetHasPublicGetter());
        traits = propertyInfo.PropertyType.GetCollectionTraits();
      }
      switch (traits.CollectionType)
      {
        case CollectionKind.Array:
          return this.EmitStoreCollectionItemsEmitSetCollectionMemberIfNullAndSettable(context, instance, value, member.GetMemberValueType(), fieldInfo, propertyInfo, traits.AddMethod == (MethodInfo) null ? default (TConstruct) : this.EmitForEachLoop(context, traits, value, (Func<TConstruct, TConstruct>) (current => this.EmitAppendCollectionItem(context, member, traits, getCollection, current))));
        case CollectionKind.Map:
          Type keyType;
          Type valueType;
          SerializerBuilder<TContext, TConstruct, TObject>.GetDictionaryKeyValueType(traits.ElementType, out keyType, out valueType);
          return this.EmitStoreCollectionItemsEmitSetCollectionMemberIfNullAndSettable(context, instance, value, member.GetMemberValueType(), fieldInfo, propertyInfo, traits.AddMethod == (MethodInfo) null ? default (TConstruct) : this.EmitForEachLoop(context, traits, value, (Func<TConstruct, TConstruct>) (current => this.EmitAppendDictionaryItem(context, traits, getCollection, keyType, this.EmitGetPropertyExpression(context, current, traits.ElementType == typeof (DictionaryEntry) ? _DictionaryEntry.Key : traits.ElementType.GetProperty("Key")), valueType, this.EmitGetPropertyExpression(context, current, traits.ElementType == typeof (DictionaryEntry) ? _DictionaryEntry.Value : traits.ElementType.GetProperty("Value")), false))));
        default:
          if (fieldInfo != (FieldInfo) null)
            return this.EmitSetField(context, instance, fieldInfo, value, true);
          if (propertyInfo.GetSetMethod(true) != (MethodInfo) null)
            return this.EmitSetProperty(context, instance, propertyInfo, value, true);
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Member '{0}' is read only and its elementType ('{1}') is not an appendable collection", (object) member.Name, (object) member.GetMemberValueType()));
      }
    }

    private TConstruct EmitStoreCollectionItemsEmitSetCollectionMemberIfNullAndSettable(
      TContext context,
      TConstruct instance,
      TConstruct collection,
      Type collectionType,
      FieldInfo asField,
      PropertyInfo asProperty,
      TConstruct storeCollectionItems)
    {
      if ((object) storeCollectionItems != null && asField != (FieldInfo) null && asField.IsInitOnly || asProperty != (PropertyInfo) null && asProperty.GetSetMethod(true) == (MethodInfo) null)
        return storeCollectionItems;
      TConstruct thenExpression = asField != (FieldInfo) null ? this.EmitSetField(context, instance, asField, collection, !asField.GetHasPublicSetter()) : this.EmitSetProperty(context, instance, asProperty, collection, !asProperty.GetHasPublicSetter());
      if ((object) storeCollectionItems == null)
        return thenExpression;
      return this.EmitSequentialStatements(context, storeCollectionItems.ContextType, this.EmitConditionalExpression(context, this.EmitEqualsExpression(context, asField != (FieldInfo) null ? this.EmitGetFieldExpression(context, instance, asField) : this.EmitGetPropertyExpression(context, instance, asProperty), this.MakeNullLiteral(context, collectionType)), thenExpression, storeCollectionItems));
    }

    protected abstract TConstruct EmitMethodOfExpression(TContext context, MethodBase method);

    private TConstruct EmitSetProperty(
      TContext context,
      TConstruct instance,
      PropertyInfo property,
      TConstruct value,
      bool withReflection)
    {
      if (!withReflection)
        return this.EmitSetProperty(context, instance, property, value);
      return this.EmitInvokeVoidMethod(context, this.EmitMethodOfExpression(context, (MethodBase) property.GetSetMethod(true)), _MethodBase.Invoke_2, instance, this.EmitCreateNewArrayExpression(context, typeof (object), 1, (IEnumerable<TConstruct>) new TConstruct[1]
      {
        value.ContextType.GetIsValueType() ? this.EmitBoxExpression(context, value.ContextType, value) : value
      }));
    }

    protected abstract TConstruct EmitSetProperty(
      TContext context,
      TConstruct instance,
      PropertyInfo property,
      TConstruct value);

    protected abstract TConstruct EmitFieldOfExpression(TContext context, FieldInfo field);

    private TConstruct EmitSetField(
      TContext context,
      TConstruct instance,
      FieldInfo field,
      TConstruct value,
      bool withReflection)
    {
      if (!withReflection)
        return this.EmitSetField(context, instance, field, value);
      return this.EmitInvokeVoidMethod(context, this.EmitFieldOfExpression(context, field), _FieldInfo.SetValue, instance, value.ContextType.GetIsValueType() ? this.EmitBoxExpression(context, value.ContextType, value) : value);
    }

    protected abstract TConstruct EmitSetField(
      TContext context,
      TConstruct instance,
      FieldInfo field,
      TConstruct value);

    protected abstract TConstruct EmitLoadVariableExpression(TContext context, TConstruct variable);

    protected virtual TConstruct EmitStoreVariableStatement(TContext context, TConstruct variable)
    {
      return this.EmitStoreVariableStatement(context, variable, default (TConstruct));
    }

    protected abstract TConstruct EmitStoreVariableStatement(
      TContext context,
      TConstruct variable,
      TConstruct value);

    private TConstruct EmitThrowExpression(
      TContext context,
      Type expressionType,
      MethodInfo factoryMethod,
      params TConstruct[] arguments)
    {
      return this.EmitThrowExpression(context, expressionType, this.EmitInvokeMethodExpression(context, default (TConstruct), factoryMethod, arguments));
    }

    protected abstract TConstruct EmitThrowExpression(
      TContext context,
      Type expressionType,
      TConstruct exceptionExpression);

    protected abstract TConstruct EmitTryFinally(
      TContext context,
      TConstruct tryStatement,
      TConstruct finallyStatement);

    private TConstruct EmitInvariantStringFormat(
      TContext context,
      string format,
      params TConstruct[] arguments)
    {
      return this.EmitInvokeMethodExpression(context, default (TConstruct), _String.Format_P, this.EmitGetPropertyExpression(context, default (TConstruct), _CultureInfo.InvariantCulture), this.MakeStringLiteral(context, format), this.EmitCreateNewArrayExpression(context, typeof (object), arguments.Length, ((IEnumerable<TConstruct>) arguments).Select<TConstruct, TConstruct>((Func<TConstruct, TConstruct>) (a => !a.ContextType.GetIsValueType() ? a : this.EmitBoxExpression(context, a.ContextType, a)))));
    }

    protected abstract TConstruct EmitCreateNewArrayExpression(
      TContext context,
      Type elementType,
      int length);

    protected abstract TConstruct EmitCreateNewArrayExpression(
      TContext context,
      Type elementType,
      int length,
      IEnumerable<TConstruct> initialElements);

    protected abstract TConstruct EmitSetArrayElementStatement(
      TContext context,
      TConstruct array,
      TConstruct index,
      TConstruct value);

    protected virtual TConstruct EmitGetSerializerExpression(
      TContext context,
      Type targetType,
      SerializingMember? memberInfo,
      PolymorphismSchema itemsSchema)
    {
      if (memberInfo.HasValue && targetType.GetIsEnum())
        return this.EmitInvokeMethodExpression(context, context.Context, _SerializationContext.GetSerializer1_Parameter_Method.MakeGenericMethod(targetType), this.EmitBoxExpression(context, typeof (EnumSerializationMethod), this.EmitInvokeMethodExpression(context, default (TConstruct), _EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethodMethod, context.Context, this.EmitTypeOfExpression(context, targetType), this.MakeEnumLiteral(context, typeof (EnumMemberSerializationMethod), (object) memberInfo.Value.GetEnumMemberSerializationMethod()))));
      if (memberInfo.HasValue && DateTimeMessagePackSerializerHelpers.IsDateTime(targetType))
        return this.EmitInvokeMethodExpression(context, context.Context, _SerializationContext.GetSerializer1_Parameter_Method.MakeGenericMethod(targetType), this.EmitBoxExpression(context, typeof (DateTimeConversionMethod), this.EmitInvokeMethodExpression(context, default (TConstruct), _DateTimeMessagePackSerializerHelpers.DetermineDateTimeConversionMethodMethod, context.Context, this.MakeEnumLiteral(context, typeof (DateTimeMemberConversionMethod), (object) memberInfo.Value.GetDateTimeMemberConversionMethod()))));
      PolymorphismSchema schema = itemsSchema ?? (memberInfo.HasValue ? PolymorphismSchema.Create(targetType, memberInfo) : PolymorphismSchema.Default);
      context.SerializationContext.GetSerializer(targetType, (object) schema);
      TConstruct storage = this.DeclareLocal(context, typeof (PolymorphismSchema), "__schema");
      return this.EmitSequentialStatements(context, typeof (MessagePackSerializer<>).MakeGenericType(targetType), ((IEnumerable<TConstruct>) new TConstruct[1]
      {
        storage
      }).Concat<TConstruct>(this.EmitConstructPolymorphismSchema(context, storage, schema)).Concat<TConstruct>((IEnumerable<TConstruct>) new TConstruct[1]
      {
        this.EmitInvokeMethodExpression(context, context.Context, _SerializationContext.GetSerializer1_Parameter_Method.MakeGenericMethod(targetType), storage)
      }));
    }

    private IEnumerable<TConstruct> EmitPackItemStatements(
      TContext context,
      TConstruct packer,
      Type itemType,
      NilImplication nilImplication,
      string memberName,
      TConstruct item,
      SerializingMember? memberInfo,
      PolymorphismSchema itemsSchema)
    {
      TConstruct nilImplicationConstruct = this._nilImplicationHandler.OnPacking(new SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnPackingParameter(this, context, item, itemType, memberName), nilImplication);
      if ((object) nilImplicationConstruct != null)
        yield return nilImplicationConstruct;
      yield return this.EmitSerializeItemExpressionCore(context, packer, itemType, item, memberInfo, itemsSchema);
    }

    private TConstruct EmitSerializeItemExpressionCore(
      TContext context,
      TConstruct packer,
      Type itemType,
      TConstruct item,
      SerializingMember? memberInfo,
      PolymorphismSchema itemsSchema)
    {
      return this.EmitInvokeVoidMethod(context, this.EmitGetSerializerExpression(context, itemType, memberInfo, itemsSchema), ((IEnumerable<MethodInfo>) typeof (MessagePackSerializer<>).MakeGenericType(itemType).GetMethods()).Single<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "PackTo" && !m.IsStatic && m.IsPublic)), packer, item);
    }

    private TConstruct EmitGetItemsCountExpression(TContext context, TConstruct unpacker)
    {
      return this.EmitInvokeMethodExpression(context, default (TConstruct), _UnpackHelpers.GetItemsCount, unpacker);
    }

    private TConstruct EmitUnpackCollectionWithUnpackToExpression(
      TContext context,
      ConstructorInfo ctor,
      TConstruct collectionCapacity,
      TConstruct unpacker,
      TConstruct collection)
    {
      TContext context1 = context;
      Type contextType = typeof (void);
      TConstruct[] constructArray1 = new TConstruct[2];
      TConstruct[] constructArray2 = constructArray1;
      TContext context2 = context;
      TConstruct variable1 = collection;
      TContext context3 = context;
      TConstruct variable2 = collection;
      ConstructorInfo constructor = ctor;
      TConstruct[] constructArray3;
      if (ctor.GetParameters().Length != 0)
        constructArray3 = new TConstruct[1]
        {
          collectionCapacity
        };
      else
        constructArray3 = SerializerBuilder<TContext, TConstruct, TObject>.NoConstructs;
      TConstruct objectExpression = this.EmitCreateNewObjectExpression(context3, variable2, constructor, constructArray3);
      TConstruct construct = this.EmitStoreVariableStatement(context2, variable1, objectExpression);
      constructArray2[0] = construct;
      constructArray1[1] = this.EmitInvokeUnpackTo(context, unpacker, collection);
      TConstruct[] constructArray4 = constructArray1;
      return this.EmitSequentialStatements(context1, contextType, constructArray4);
    }

    protected virtual TConstruct EmitInvokeUnpackTo(
      TContext context,
      TConstruct unpacker,
      TConstruct collection)
    {
      return this.EmitInvokeVoidMethod(context, this.EmitThisReferenceExpression(context), MessagePackSerializer<TObject>.UnpackToCoreMethod, unpacker, collection);
    }

    protected TConstruct EmitUnpackItemValueExpression(
      TContext context,
      Type itemType,
      NilImplication nilImplication,
      TConstruct unpacker,
      TConstruct itemIndex,
      TConstruct memberName,
      TConstruct itemsCount,
      TConstruct unpacked,
      SerializingMember? memberInfo,
      PolymorphismSchema itemsSchema,
      Func<TConstruct, TConstruct> storeValueStatementEmitter)
    {
      return this.EmitSequentialStatements(context, typeof (void), this.EmitUnpackItemValueExpressionCore(context, itemType, nilImplication, unpacker, itemIndex, memberName, itemsCount, unpacked, memberInfo, itemsSchema, storeValueStatementEmitter));
    }

    private IEnumerable<TConstruct> EmitUnpackItemValueExpressionCore(
      TContext context,
      Type itemType,
      NilImplication nilImplication,
      TConstruct unpacker,
      TConstruct itemIndex,
      TConstruct memberName,
      TConstruct itemsCount,
      TConstruct unpacked,
      SerializingMember? memberInfo,
      PolymorphismSchema itemsSchema,
      Func<TConstruct, TConstruct> storeValueStatementEmitter)
    {
      bool isNativelyNullable = itemType == typeof (MessagePackObject) || !itemType.GetIsValueType() || Nullable.GetUnderlyingType(itemType) != (Type) null;
      Type nullableType = itemType;
      if (!isNativelyNullable)
        nullableType = typeof (Nullable<>).MakeGenericType(itemType);
      TConstruct nullable = this.DeclareLocal(context, nullableType, "nullable");
      MethodInfo directRead = _UnpackHelpers.GetDirectUnpackMethod(nullableType);
      TConstruct[] isNotInCollectionCondition = new TConstruct[2]
      {
        this.EmitNotExpression(context, this.EmitGetPropertyExpression(context, unpacker, _Unpacker.IsArrayHeader)),
        this.EmitNotExpression(context, this.EmitGetPropertyExpression(context, unpacker, _Unpacker.IsMapHeader))
      };
      TConstruct construct;
      if (!(directRead != (MethodInfo) null))
      {
        // ISSUE: reference to a compiler-generated field
        construct = this.EmitSequentialStatements(context, typeof (void), this.EmitConditionalExpression(context, this.EmitNotExpression(context, this.EmitInvokeMethodExpression(context, unpacker, _Unpacker.Read)), this.EmitThrowExpression(context, typeof (Unpacker), SerializationExceptions.NewMissingItemMethod, itemIndex), default (TConstruct)), itemType == typeof (MessagePackObject) ? this.EmitAndConditionalExpression(context, (IList<TConstruct>) isNotInCollectionCondition, this.EmitStoreVariableStatement(context, nullable, this.EmitGetPropertyExpression(context, unpacker, _Unpacker.LastReadData)), this.EmitStoreVariableStatement(context, nullable, this.EmitInvokeMethodExpression(context, unpacker, _Unpacker.UnpackSubtreeData))) : this.EmitAndConditionalExpression(context, (IList<TConstruct>) isNotInCollectionCondition, this.EmitDeserializeItemExpression(context, unpacker, nullableType, nullable, memberInfo, itemsSchema), this.EmitUsingStatement(context, typeof (Unpacker), this.EmitInvokeMethodExpression(context, unpacker, _Unpacker.ReadSubtree), (Func<TConstruct, TConstruct>) (subtreeUnpacker => this.\u003C\u003E4__this.EmitDeserializeItemExpression(context, subtreeUnpacker, nullableType, nullable, memberInfo, itemsSchema)))));
      }
      else
        construct = this.EmitStoreVariableStatement(context, nullable, this.EmitInvokeMethodExpression(context, default (TConstruct), directRead, unpacker, this.EmitTypeOfExpression(context, typeof (TObject)), memberName));
      TConstruct readAndUnpack = construct;
      TConstruct unpackedItem = !(Nullable.GetUnderlyingType(nullable.ContextType) != (Type) null) || !(Nullable.GetUnderlyingType(itemType) == (Type) null) ? nullable : this.EmitGetPropertyExpression(context, nullable, nullable.ContextType.GetProperty("Value"));
      TConstruct store = storeValueStatementEmitter(unpackedItem);
      TConstruct expressionWhenNil = this._nilImplicationHandler.OnUnpacked(new SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnUnpacedParameter(this, context, itemType, memberName, store), nilImplication);
      yield return nullable;
      if ((object) unpacked != null)
        yield return this.EmitConditionalExpression(context, this.EmitLessThanExpression(context, unpacked, itemsCount), readAndUnpack, default (TConstruct));
      else
        yield return readAndUnpack;
      yield return this.EmitConditionalExpression(context, !isNativelyNullable || Nullable.GetUnderlyingType(itemType) != (Type) null ? this.EmitGetPropertyExpression(context, nullable, nullableType.GetProperty("HasValue")) : (itemType == typeof (MessagePackObject) ? this.EmitNotExpression(context, this.EmitGetPropertyExpression(context, nullable, _MessagePackObject.IsNil)) : this.EmitNotEqualsExpression(context, nullable, this.MakeNullLiteral(context, itemType))), store, expressionWhenNil);
      if ((object) unpacked != null)
        yield return this.EmitIncrement(context, unpacked);
    }

    private TConstruct EmitUsingStatement(
      TContext context,
      Type disposableType,
      TConstruct instantiateIDisposableExpression,
      Func<TConstruct, TConstruct> usingBodyEmitter)
    {
      TConstruct construct = this.DeclareLocal(context, disposableType, "disposable");
      return this.EmitSequentialStatements(context, typeof (void), construct, this.EmitStoreVariableStatement(context, construct, instantiateIDisposableExpression), this.EmitTryFinally(context, usingBodyEmitter(construct), this.EmitConditionalExpression(context, this.EmitNotEqualsExpression(context, construct, this.MakeNullLiteral(context, disposableType)), this.EmitInvokeMethodExpression(context, construct, disposableType.GetMethod("Dispose", ReflectionAbstractions.EmptyTypes)), default (TConstruct))));
    }

    private TConstruct EmitDeserializeItemExpression(
      TContext context,
      TConstruct unpacker,
      Type itemType,
      TConstruct unpacked,
      SerializingMember? memberInfo,
      PolymorphismSchema itemsSchema)
    {
      return this.EmitStoreVariableStatement(context, unpacked, this.EmitDeserializeItemExpressionCore(context, unpacker, itemType, memberInfo, itemsSchema));
    }

    private TConstruct EmitDeserializeItemExpressionCore(
      TContext context,
      TConstruct unpacker,
      Type itemType,
      SerializingMember? memberInfo,
      PolymorphismSchema itemsSchema)
    {
      return this.EmitInvokeMethodExpression(context, this.EmitGetSerializerExpression(context, itemType, memberInfo, itemsSchema), typeof (MessagePackSerializer<>).MakeGenericType(itemType).GetMethod("UnpackFrom"), unpacker);
    }

    protected abstract TConstruct EmitConditionalExpression(
      TContext context,
      TConstruct conditionExpression,
      TConstruct thenExpression,
      TConstruct elseExpression);

    protected abstract TConstruct EmitAndConditionalExpression(
      TContext context,
      IList<TConstruct> conditionExpressions,
      TConstruct thenExpression,
      TConstruct elseExpression);

    protected abstract TConstruct EmitStringSwitchStatement(
      TContext context,
      TConstruct target,
      IDictionary<string, TConstruct> cases,
      TConstruct defaultCase);

    protected virtual TConstruct EmitRetrunStatement(TContext context, TConstruct expression)
    {
      return expression;
    }

    protected abstract TConstruct EmitForLoop(
      TContext context,
      TConstruct count,
      Func<SerializerBuilder<TContext, TConstruct, TObject>.ForLoopContext, TConstruct> loopBodyEmitter);

    protected abstract TConstruct EmitForEachLoop(
      TContext context,
      CollectionTraits collectionTraits,
      TConstruct collection,
      Func<TConstruct, TConstruct> loopBodyEmitter);

    private TConstruct EmitAppendCollectionItem(
      TContext context,
      MemberInfo member,
      CollectionTraits traits,
      TConstruct collection,
      TConstruct unpackedItem)
    {
      if (traits.AddMethod == (MethodInfo) null)
      {
        if (member != (MemberInfo) null)
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' of read only member '{1}' does not have public 'Add' method.", (object) member.GetMemberValueType(), (object) member));
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' does not have public 'Add' method.", (object) collection.ContextType));
      }
      return this.EmitInvokeVoidMethod(context, traits.AddMethod.DeclaringType.IsAssignableFrom(collection.ContextType) ? collection : this.EmitUnboxAnyExpression(context, traits.AddMethod.DeclaringType, collection), traits.AddMethod, unpackedItem);
    }

    private TConstruct EmitAppendDictionaryItem(
      TContext context,
      CollectionTraits traits,
      TConstruct dictionary,
      Type keyType,
      TConstruct key,
      Type valueType,
      TConstruct value,
      bool withBoxing)
    {
      return this.EmitInvokeVoidMethod(context, traits.AddMethod.DeclaringType.IsAssignableFrom(dictionary.ContextType) ? dictionary : this.EmitUnboxAnyExpression(context, traits.AddMethod.DeclaringType, dictionary), traits.AddMethod, withBoxing ? this.EmitBoxExpression(context, keyType, key) : key, withBoxing ? this.EmitBoxExpression(context, valueType, value) : value);
    }

    private static void GetDictionaryKeyValueType(
      Type elementType,
      out Type keyType,
      out Type valueType)
    {
      if (elementType == typeof (DictionaryEntry))
      {
        keyType = typeof (object);
        valueType = typeof (object);
      }
      else
      {
        keyType = elementType.GetGenericArguments()[0];
        valueType = elementType.GetGenericArguments()[1];
      }
    }

    private static ConstructorInfo GetDefaultConstructor(Type instanceType)
    {
      ConstructorInfo constructor = typeof (TObject).GetConstructor(ReflectionAbstractions.EmptyTypes);
      if (constructor == (ConstructorInfo) null)
        throw SerializationExceptions.NewTargetDoesNotHavePublicDefaultConstructor(instanceType);
      return constructor;
    }

    private TConstruct[] DetermineCollectionConstructorArguments(
      TContext context,
      ConstructorInfo constructor)
    {
      ParameterInfo[] parameters = constructor.GetParameters();
      switch (parameters.Length)
      {
        case 0:
          return SerializerBuilder<TContext, TConstruct, TObject>.NoConstructs;
        case 1:
          return new TConstruct[1]
          {
            this.GetConstructorArgument(context, parameters[0])
          };
        case 2:
          return new TConstruct[2]
          {
            this.GetConstructorArgument(context, parameters[0]),
            this.GetConstructorArgument(context, parameters[1])
          };
        default:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructor signature '{0}' is not supported.", (object) constructor));
      }
    }

    private TConstruct GetConstructorArgument(TContext context, ParameterInfo parameter)
    {
      return !(parameter.ParameterType == typeof (int)) ? this.EmitGetEqualityComparer(context) : context.InitialCapacity;
    }

    private TConstruct EmitGetEqualityComparer(TContext context)
    {
      Type type;
      switch (SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.DetailedCollectionType)
      {
        case CollectionDetailedKind.Array:
        case CollectionDetailedKind.GenericList:
        case CollectionDetailedKind.GenericSet:
        case CollectionDetailedKind.GenericCollection:
        case CollectionDetailedKind.GenericEnumerable:
        case CollectionDetailedKind.GenericReadOnlyList:
        case CollectionDetailedKind.GenericReadOnlyCollection:
          type = SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.ElementType;
          break;
        case CollectionDetailedKind.GenericDictionary:
        case CollectionDetailedKind.GenericReadOnlyDictionary:
          type = SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.ElementType.GetGenericArguments()[0];
          break;
        default:
          type = typeof (object);
          break;
      }
      return this.EmitInvokeMethodExpression(context, default (TConstruct), _UnpackHelpers.GetEqualityComparer_1Method.MakeGenericMethod(type));
    }

    protected IEnumerable<TConstruct> EmitConstructPolymorphismSchema(
      TContext context,
      TConstruct storage,
      PolymorphismSchema schema)
    {
      if (schema == null)
      {
        yield return this.EmitStoreVariableStatement(context, storage, this.MakeNullLiteral(context, typeof (PolymorphismSchema)));
      }
      else
      {
        switch (schema.ChildrenType)
        {
          case PolymorphismSchemaChildrenType.CollectionItems:
            string itemsSchemaVariableName = context.GetUniqueVariableName("itemsSchema");
            TConstruct itemsSchema = schema.ItemSchema.UseDefault ? this.MakeNullLiteral(context, typeof (PolymorphismSchema)) : this.DeclareLocal(context, typeof (PolymorphismSchema), itemsSchemaVariableName);
            if (!schema.ItemSchema.UseDefault)
            {
              yield return itemsSchema;
              foreach (TConstruct construct in this.EmitConstructLeafPolymorphismSchema(context, itemsSchema, schema.ItemSchema, itemsSchemaVariableName))
                yield return construct;
            }
            if (schema.UseDefault)
            {
              yield return this.EmitStoreVariableStatement(context, storage, this.EmitInvokeMethodExpression(context, default (TConstruct), PolymorphismSchema.ForContextSpecifiedCollectionMethod, this.EmitTypeOfExpression(context, schema.TargetType), itemsSchema));
              break;
            }
            if (schema.UseTypeEmbedding)
            {
              yield return this.EmitStoreVariableStatement(context, storage, this.EmitInvokeMethodExpression(context, default (TConstruct), PolymorphismSchema.ForPolymorphicCollectionTypeEmbeddingMethod, this.EmitTypeOfExpression(context, schema.TargetType), itemsSchema));
              break;
            }
            TConstruct typeMap1 = this.DeclareLocal(context, typeof (Dictionary<string, Type>), context.GetUniqueVariableName("typeMap"));
            yield return typeMap1;
            yield return this.EmitStoreVariableStatement(context, typeMap1, this.EmitCreateNewObjectExpression(context, typeMap1, PolymorphismSchema.CodeTypeMapConstructor, this.MakeInt32Literal(context, schema.CodeTypeMapping.Count)));
            foreach (TConstruct construct in this.EmitConstructTypeCodeMappingForPolymorphismSchema(context, schema, typeMap1))
              yield return construct;
            yield return this.EmitStoreVariableStatement(context, storage, this.EmitInvokeMethodExpression(context, default (TConstruct), PolymorphismSchema.ForPolymorphicCollectionCodeTypeMappingMethod, this.EmitTypeOfExpression(context, schema.TargetType), typeMap1, itemsSchema));
            break;
          case PolymorphismSchemaChildrenType.DictionaryKeyValues:
            string keysSchemaVariableName = context.GetUniqueVariableName("keysSchema");
            TConstruct keysSchema = schema.KeySchema.UseDefault ? this.MakeNullLiteral(context, typeof (PolymorphismSchema)) : this.DeclareLocal(context, typeof (PolymorphismSchema), keysSchemaVariableName);
            if (!schema.KeySchema.UseDefault)
            {
              yield return keysSchema;
              foreach (TConstruct construct in this.EmitConstructLeafPolymorphismSchema(context, keysSchema, schema.KeySchema, keysSchemaVariableName))
                yield return construct;
            }
            string valuesSchemaVariableName = context.GetUniqueVariableName("valuesSchema");
            TConstruct valuesSchema = schema.ItemSchema.UseDefault ? this.MakeNullLiteral(context, typeof (PolymorphismSchema)) : this.DeclareLocal(context, typeof (PolymorphismSchema), valuesSchemaVariableName);
            if (!schema.ItemSchema.UseDefault)
            {
              yield return valuesSchema;
              foreach (TConstruct construct in this.EmitConstructLeafPolymorphismSchema(context, valuesSchema, schema.ItemSchema, valuesSchemaVariableName))
                yield return construct;
            }
            if (schema.UseDefault)
            {
              yield return this.EmitStoreVariableStatement(context, storage, this.EmitInvokeMethodExpression(context, default (TConstruct), PolymorphismSchema.ForContextSpecifiedDictionaryMethod, this.EmitTypeOfExpression(context, schema.TargetType), keysSchema, valuesSchema));
              break;
            }
            if (schema.UseTypeEmbedding)
            {
              yield return this.EmitStoreVariableStatement(context, storage, this.EmitInvokeMethodExpression(context, default (TConstruct), PolymorphismSchema.ForPolymorphicDictionaryTypeEmbeddingMethod, this.EmitTypeOfExpression(context, schema.TargetType), keysSchema, valuesSchema));
              break;
            }
            TConstruct typeMap2 = this.DeclareLocal(context, typeof (Dictionary<string, Type>), context.GetUniqueVariableName("typeMap"));
            yield return typeMap2;
            yield return this.EmitStoreVariableStatement(context, typeMap2, this.EmitCreateNewObjectExpression(context, typeMap2, PolymorphismSchema.CodeTypeMapConstructor, this.MakeInt32Literal(context, schema.CodeTypeMapping.Count)));
            foreach (TConstruct construct in this.EmitConstructTypeCodeMappingForPolymorphismSchema(context, schema, typeMap2))
              yield return construct;
            yield return this.EmitStoreVariableStatement(context, storage, this.EmitInvokeMethodExpression(context, default (TConstruct), PolymorphismSchema.ForPolymorphicDictionaryCodeTypeMappingMethod, this.EmitTypeOfExpression(context, schema.TargetType), typeMap2, keysSchema, valuesSchema));
            break;
          case PolymorphismSchemaChildrenType.TupleItems:
            if (schema.ChildSchemaList.Count == 0)
              yield return this.EmitStoreVariableStatement(context, storage, this.MakeNullLiteral(context, typeof (PolymorphismSchema)));
            IList<Type> tupleItems = TupleItems.GetTupleItemTypes(schema.TargetType);
            TConstruct tupleItemsSchema = this.DeclareLocal(context, typeof (PolymorphismSchema[]), context.GetUniqueVariableName("tupleItemsSchema"));
            yield return tupleItemsSchema;
            yield return this.EmitStoreVariableStatement(context, tupleItemsSchema, this.EmitCreateNewArrayExpression(context, typeof (PolymorphismSchema), tupleItems.Count));
            for (int i = 0; i < tupleItems.Count; ++i)
            {
              string variableName = context.GetUniqueVariableName("tupleItemSchema");
              TConstruct itemSchema = this.DeclareLocal(context, typeof (PolymorphismSchema), variableName);
              yield return itemSchema;
              foreach (TConstruct construct in this.EmitConstructLeafPolymorphismSchema(context, itemSchema, schema.ChildSchemaList[i], variableName))
                yield return construct;
              yield return this.EmitSetArrayElementStatement(context, tupleItemsSchema, this.MakeInt32Literal(context, i), itemSchema);
            }
            yield return this.EmitStoreVariableStatement(context, storage, this.EmitInvokeMethodExpression(context, default (TConstruct), PolymorphismSchema.ForPolymorphicTupleMethod, this.EmitTypeOfExpression(context, schema.TargetType), tupleItemsSchema));
            break;
          default:
            foreach (TConstruct construct in this.EmitConstructLeafPolymorphismSchema(context, storage, schema, string.Empty))
              yield return construct;
            break;
        }
      }
    }

    private IEnumerable<TConstruct> EmitConstructLeafPolymorphismSchema(
      TContext context,
      TConstruct storage,
      PolymorphismSchema currentSchema,
      string prefix)
    {
      if (currentSchema.UseDefault)
        yield return this.EmitStoreVariableStatement(context, storage, this.MakeNullLiteral(context, typeof (PolymorphismSchema)));
      else if (currentSchema.UseTypeEmbedding)
      {
        yield return this.EmitStoreVariableStatement(context, storage, this.EmitInvokeMethodExpression(context, default (TConstruct), PolymorphismSchema.ForPolymorphicObjectTypeEmbeddingMethod, this.EmitTypeOfExpression(context, currentSchema.TargetType)));
      }
      else
      {
        TConstruct typeMap = this.DeclareLocal(context, typeof (Dictionary<string, Type>), context.GetUniqueVariableName(string.IsNullOrEmpty(prefix) ? "typeMap" : prefix + "TypeMap"));
        yield return typeMap;
        foreach (TConstruct construct in this.EmitConstructTypeCodeMappingForPolymorphismSchema(context, currentSchema, typeMap))
          yield return construct;
        yield return this.EmitStoreVariableStatement(context, storage, this.EmitInvokeMethodExpression(context, default (TConstruct), PolymorphismSchema.ForPolymorphicObjectCodeTypeMappingMethod, this.EmitTypeOfExpression(context, currentSchema.TargetType), typeMap));
      }
    }

    private IEnumerable<TConstruct> EmitConstructTypeCodeMappingForPolymorphismSchema(
      TContext context,
      PolymorphismSchema currentSchema,
      TConstruct typeMap)
    {
      yield return this.EmitStoreVariableStatement(context, typeMap, this.EmitCreateNewObjectExpression(context, typeMap, PolymorphismSchema.CodeTypeMapConstructor, this.MakeInt32Literal(context, currentSchema.CodeTypeMapping.Count)));
      foreach (KeyValuePair<string, Type> keyValuePair in (IEnumerable<KeyValuePair<string, Type>>) currentSchema.CodeTypeMapping)
        yield return this.EmitInvokeMethodExpression(context, typeMap, PolymorphismSchema.AddToCodeTypeMapMethod, this.MakeStringLiteral(context, keyValuePair.Key), this.EmitTypeOfExpression(context, keyValuePair.Value));
    }

    static SerializerBuilder()
    {
      CollectionTraits collectionTraits = typeof (TObject).GetCollectionTraits();
      SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis = collectionTraits;
      switch (collectionTraits.DetailedCollectionType)
      {
        case CollectionDetailedKind.Array:
          Type type1;
          if (!typeof (TObject).GetIsEnum())
            type1 = typeof (MessagePackSerializer<TObject>);
          else
            type1 = typeof (EnumMessagePackSerializer<>).MakeGenericType(typeof (TObject));
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = type1;
          break;
        case CollectionDetailedKind.GenericList:
        case CollectionDetailedKind.GenericSet:
        case CollectionDetailedKind.GenericCollection:
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = typeof (CollectionMessagePackSerializer<,>).MakeGenericType(typeof (TObject), collectionTraits.ElementType);
          break;
        case CollectionDetailedKind.NonGenericList:
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = typeof (NonGenericListMessagePackSerializer<>).MakeGenericType(typeof (TObject));
          break;
        case CollectionDetailedKind.GenericDictionary:
          Type[] genericArguments1 = collectionTraits.ElementType.GetGenericArguments();
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = typeof (DictionaryMessagePackSerializer<,,>).MakeGenericType(typeof (TObject), genericArguments1[0], genericArguments1[1]);
          break;
        case CollectionDetailedKind.NonGenericDictionary:
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = typeof (NonGenericDictionaryMessagePackSerializer<>).MakeGenericType(typeof (TObject));
          break;
        case CollectionDetailedKind.NonGenericCollection:
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = typeof (NonGenericCollectionMessagePackSerializer<>).MakeGenericType(typeof (TObject));
          break;
        case CollectionDetailedKind.GenericEnumerable:
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = typeof (EnumerableMessagePackSerializer<,>).MakeGenericType(typeof (TObject), collectionTraits.ElementType);
          break;
        case CollectionDetailedKind.NonGenericEnumerable:
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = typeof (NonGenericEnumerableMessagePackSerializer<>).MakeGenericType(typeof (TObject));
          break;
        case CollectionDetailedKind.GenericReadOnlyList:
        case CollectionDetailedKind.GenericReadOnlyCollection:
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = typeof (ReadOnlyCollectionMessagePackSerializer<,>).MakeGenericType(typeof (TObject), collectionTraits.ElementType);
          break;
        case CollectionDetailedKind.GenericReadOnlyDictionary:
          Type[] genericArguments2 = collectionTraits.ElementType.GetGenericArguments();
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = typeof (ReadOnlyDictionaryMessagePackSerializer<,,>).MakeGenericType(typeof (TObject), genericArguments2[0], genericArguments2[1]);
          break;
        default:
          Type type2;
          if (!typeof (TObject).GetIsEnum())
            type2 = typeof (MessagePackSerializer<TObject>);
          else
            type2 = typeof (EnumMessagePackSerializer<>).MakeGenericType(typeof (TObject));
          SerializerBuilder<TContext, TConstruct, TObject>.BaseClass = type2;
          break;
      }
    }

    public MessagePackSerializer<TObject> BuildSerializerInstance(
      SerializationContext context,
      Type concreteType,
      PolymorphismSchema schema)
    {
      TContext serializerCreation = this.CreateCodeGenerationContextForSerializerCreation(context);
      Func<SerializationContext, MessagePackSerializer<TObject>> serializerConstructor;
      if (typeof (TObject).GetIsEnum())
      {
        this.BuildEnumSerializer(serializerCreation);
        serializerConstructor = this.CreateEnumSerializerConstructor(serializerCreation);
      }
      else
      {
        this.BuildSerializer(serializerCreation, concreteType, schema);
        serializerConstructor = this.CreateSerializerConstructor(serializerCreation, schema);
      }
      if (serializerConstructor != null)
        return serializerConstructor(context);
      throw SerializationExceptions.NewTypeCannotSerialize(typeof (TObject));
    }

    protected abstract TContext CreateCodeGenerationContextForSerializerCreation(
      SerializationContext context);

    protected void BuildSerializer(TContext context, Type concreteType, PolymorphismSchema schema)
    {
      switch (SerializerBuilder<TContext, TConstruct, TObject>.CollectionTraitsOfThis.CollectionType)
      {
        case CollectionKind.NotCollection:
          Type underlyingType = Nullable.GetUnderlyingType(typeof (TObject));
          if (underlyingType != (Type) null)
          {
            this.BuildNullableSerializer(context, underlyingType);
            break;
          }
          if (TupleItems.IsTuple(typeof (TObject)))
          {
            this.BuildTupleSerializer(context, (schema ?? PolymorphismSchema.Default).ChildSchemaList);
            break;
          }
          this.BuildObjectSerializer(context);
          break;
        case CollectionKind.Array:
        case CollectionKind.Map:
          this.BuildCollectionSerializer(context, concreteType, schema);
          break;
      }
    }

    protected abstract Func<SerializationContext, MessagePackSerializer<TObject>> CreateSerializerConstructor(
      TContext codeGenerationContext,
      PolymorphismSchema schema);

    protected abstract Func<SerializationContext, MessagePackSerializer<TObject>> CreateEnumSerializerConstructor(
      TContext codeGenerationContext);

    public void BuildSerializerCode(
      ISerializerCodeGenerationContext context,
      Type concreteType,
      PolymorphismSchema itemSchema)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      this.BuildSerializerCodeCore(context, concreteType, itemSchema);
    }

    protected virtual void BuildSerializerCodeCore(
      ISerializerCodeGenerationContext context,
      Type concreteType,
      PolymorphismSchema itemSchema)
    {
      throw new NotSupportedException();
    }

    private void BuildObjectSerializer(TContext context)
    {
      SerializationTarget.VerifyType(typeof (TObject));
      SerializationTarget target = SerializationTarget.Prepare(context.SerializationContext, typeof (TObject));
      if (typeof (IPackable).IsAssignableFrom(typeof (TObject)))
        this.BuildIPackablePackTo(context);
      else
        this.BuildObjectPackTo(context, target);
      if (typeof (IUnpackable).IsAssignableFrom(typeof (TObject)))
        this.BuildIUnpackableUnpackFrom(context);
      else
        this.BuildObjectUnpackFrom(context, target);
    }

    private void BuildIPackablePackTo(TContext context)
    {
      this.EmitMethodPrologue(context, SerializerMethod.PackToCore);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = this.EmitInvokeVoidMethod(context, context.PackToTarget, ((IEnumerable<MethodInfo>) typeof (TObject).GetInterfaceMap(typeof (IPackable)).TargetMethods).Single<MethodInfo>(), context.Packer, this.MakeNullLiteral(context, typeof (PackingOptions)));
      }
      finally
      {
        this.EmitMethodEpilogue(context, SerializerMethod.PackToCore, construct);
      }
    }

    private void BuildObjectPackTo(TContext context, SerializationTarget target)
    {
      this.EmitMethodPrologue(context, SerializerMethod.PackToCore);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = this.EmitSequentialStatements(context, typeof (void), context.SerializationContext.SerializationMethod == SerializationMethod.Array ? this.BuildObjectPackToWithArray(context, target.Members) : this.BuildObjectPackToWithMap(context, target.Members));
      }
      finally
      {
        this.EmitMethodEpilogue(context, SerializerMethod.PackToCore, construct);
      }
    }

    private IEnumerable<TConstruct> BuildObjectPackToWithArray(
      TContext context,
      IList<SerializingMember> entries)
    {
      yield return this.EmitPutArrayHeaderExpression(context, this.MakeInt32Literal(context, entries.Count));
      for (int i = 0; i < entries.Count; ++i)
      {
        if (entries[i].Member == (MemberInfo) null)
        {
          yield return this.EmitInvokeVoidMethod(context, context.Packer, _Packer.PackNull);
        }
        else
        {
          foreach (TConstruct packItemStatement in this.EmitPackItemStatements(context, context.Packer, entries[i].Member.GetMemberValueType(), entries[i].Contract.NilImplication, entries[i].Member.ToString(), this.EmitGetMemberValueExpression(context, context.PackToTarget, entries[i].Member), new SerializingMember?(entries[i]), (PolymorphismSchema) null))
            yield return packItemStatement;
        }
      }
    }

    private IEnumerable<TConstruct> BuildObjectPackToWithMap(
      TContext context,
      IList<SerializingMember> entries)
    {
      yield return this.EmitPutMapHeaderExpression(context, this.MakeInt32Literal(context, entries.Count<SerializingMember>((Func<SerializingMember, bool>) (e => e.Member != (MemberInfo) null))));
      for (int i = 0; i < entries.Count; ++i)
      {
        if (!(entries[i].Member == (MemberInfo) null))
        {
          foreach (TConstruct packItemStatement in this.EmitPackItemStatements(context, context.Packer, typeof (string), NilImplication.Null, "MemberName", this.MakeStringLiteral(context, entries[i].Contract.Name), new SerializingMember?(entries[i]), (PolymorphismSchema) null))
            yield return packItemStatement;
          foreach (TConstruct packItemStatement in this.EmitPackItemStatements(context, context.Packer, entries[i].Member.GetMemberValueType(), entries[i].Contract.NilImplication, entries[i].Member.ToString(), this.EmitGetMemberValueExpression(context, context.PackToTarget, entries[i].Member), new SerializingMember?(entries[i]), (PolymorphismSchema) null))
            yield return packItemStatement;
        }
      }
    }

    private void BuildIUnpackableUnpackFrom(TContext context)
    {
      this.EmitMethodPrologue(context, SerializerMethod.UnpackFromCore);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = this.EmitSequentialStatements(context, typeof (TObject), this.BuildIUnpackableUnpackFromCore(context));
      }
      finally
      {
        this.EmitMethodEpilogue(context, SerializerMethod.UnpackFromCore, construct);
      }
    }

    private IEnumerable<TConstruct> BuildIUnpackableUnpackFromCore(TContext context)
    {
      TConstruct result = this.DeclareLocal(context, typeof (TObject), "result");
      yield return result;
      if (!typeof (TObject).GetIsValueType())
        yield return this.EmitStoreVariableStatement(context, result, this.EmitCreateNewObjectExpression(context, default (TConstruct), SerializerBuilder<TContext, TConstruct, TObject>.GetDefaultConstructor(typeof (TObject))));
      yield return this.EmitInvokeVoidMethod(context, result, ((IEnumerable<MethodInfo>) typeof (TObject).GetInterfaceMap(typeof (IUnpackable)).TargetMethods).Single<MethodInfo>(), context.Unpacker);
      yield return this.EmitRetrunStatement(context, this.EmitLoadVariableExpression(context, result));
    }

    private void BuildObjectUnpackFrom(TContext context, SerializationTarget target)
    {
      this.EmitMethodPrologue(context, SerializerMethod.UnpackFromCore);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = this.EmitSequentialStatements(context, typeof (TObject), this.BuildObjectUnpackFromCore(context, target));
      }
      finally
      {
        this.EmitMethodEpilogue(context, SerializerMethod.UnpackFromCore, construct);
      }
    }

    private IEnumerable<TConstruct> BuildObjectUnpackFromCore(
      TContext context,
      SerializationTarget target)
    {
      TConstruct result = this.DeclareLocal(context, typeof (TObject), "result");
      yield return result;
      if (!typeof (TObject).GetIsValueType() && !target.IsConstructorDeserialization)
        yield return this.EmitStoreVariableStatement(context, result, this.EmitCreateNewObjectExpression(context, default (TConstruct), SerializerBuilder<TContext, TConstruct, TObject>.GetDefaultConstructor(typeof (TObject))));
      yield return this.EmitConditionalExpression(context, this.EmitGetPropertyExpression(context, context.Unpacker, _Unpacker.IsArrayHeader), this.EmitObjectUnpackFromArray(context, result, target), this.EmitObjectUnpackFromMap(context, result, target));
      yield return this.EmitRetrunStatement(context, this.EmitLoadVariableExpression(context, result));
    }

    private TConstruct EmitObjectUnpackFromArray(
      TContext context,
      TConstruct result,
      SerializationTarget target)
    {
      return this.EmitSequentialStatements(context, typeof (void), this.EmitObjectUnpackFromArrayCore(context, result, target));
    }

    private IEnumerable<TConstruct> EmitObjectUnpackFromArrayCore(
      TContext context,
      TConstruct result,
      SerializationTarget target)
    {
      TConstruct unpacked = this.DeclareLocal(context, typeof (int), "unpacked");
      yield return unpacked;
      TConstruct itemsCount = this.DeclareLocal(context, typeof (int), "itemsCount");
      yield return itemsCount;
      yield return this.EmitStoreVariableStatement(context, itemsCount, this.EmitGetItemsCountExpression(context, context.Unpacker));
      ParameterInfo[] constructorParameters = target.IsConstructorDeserialization ? target.DeserializationConstructor.GetParameters() : (ParameterInfo[]) null;
      List<TConstruct> constructorArguments = target.IsConstructorDeserialization ? new List<TConstruct>(constructorParameters.Length) : (List<TConstruct>) null;
      Dictionary<string, TConstruct> constructorArgumentsIndex = target.IsConstructorDeserialization ? new Dictionary<string, TConstruct>(constructorParameters.Length) : (Dictionary<string, TConstruct>) null;
      foreach (TConstruct initializationStatement in this.InitializeConstructorArgumentInitializationStatements(context, target, constructorParameters, constructorArguments, constructorArgumentsIndex))
        yield return initializationStatement;
      for (int i = 0; i < target.Members.Count; ++i)
      {
        int count = i;
        if (target.Members[i].Member == (MemberInfo) null || target.IsConstructorDeserialization && !constructorArgumentsIndex.ContainsKey(target.Members[i].Contract.Name))
        {
          yield return this.EmitInvokeVoidMethod(context, context.Unpacker, _Unpacker.Read);
        }
        else
        {
          Func<TConstruct, TConstruct> storeValueStatementEmitter;
          if (target.IsConstructorDeserialization)
          {
            SerializingMember member = target.Members[i];
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            storeValueStatementEmitter = (Func<TConstruct, TConstruct>) (unpackedItem => this.CS\u0024\u003C\u003E8__locals7b.\u003C\u003E4__this.EmitStoreVariableStatement(context, constructorArgumentsIndex[member.Contract.Name], unpackedItem));
          }
          else
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            storeValueStatementEmitter = (Func<TConstruct, TConstruct>) (unpackedItem => this.CS\u0024\u003C\u003E8__locals7b.\u003C\u003E4__this.EmitSetMemberValueStatement(context, result, target.Members[count].Member, unpackedItem));
          }
          yield return this.EmitUnpackItemValueExpression(context, target.Members[count].Member.GetMemberValueType(), target.Members[count].Contract.NilImplication, context.Unpacker, this.MakeInt32Literal(context, count), this.MakeStringLiteral(context, target.Members[count].Member.ToString()), itemsCount, unpacked, new SerializingMember?(target.Members[i]), (PolymorphismSchema) null, storeValueStatementEmitter);
        }
      }
      if (target.IsConstructorDeserialization)
        yield return this.EmitInvokeDeserializationConstructorStatement(context, target.DeserializationConstructor, (IList<TConstruct>) constructorArguments, result);
    }

    private TConstruct EmitObjectUnpackFromMap(
      TContext context,
      TConstruct result,
      SerializationTarget target)
    {
      return this.EmitSequentialStatements(context, typeof (void), this.EmitObjectUnpackFromMapCore(context, result, target));
    }

    private IEnumerable<TConstruct> EmitObjectUnpackFromMapCore(
      TContext context,
      TConstruct result,
      SerializationTarget target)
    {
      TConstruct itemsCount = this.DeclareLocal(context, typeof (int), "itemsCount");
      yield return itemsCount;
      yield return this.EmitStoreVariableStatement(context, itemsCount, this.EmitGetItemsCountExpression(context, context.Unpacker));
      ParameterInfo[] constructorParameters = target.IsConstructorDeserialization ? target.DeserializationConstructor.GetParameters() : (ParameterInfo[]) null;
      List<TConstruct> constructorArguments = target.IsConstructorDeserialization ? new List<TConstruct>(constructorParameters.Length) : (List<TConstruct>) null;
      Dictionary<string, TConstruct> constructorArgumentsIndex = target.IsConstructorDeserialization ? new Dictionary<string, TConstruct>(constructorParameters.Length) : (Dictionary<string, TConstruct>) null;
      foreach (TConstruct initializationStatement in this.InitializeConstructorArgumentInitializationStatements(context, target, constructorParameters, constructorArguments, constructorArgumentsIndex))
        yield return initializationStatement;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Func<TConstruct, SerializingMember, TConstruct> storeValueStatementEmitter = !target.IsConstructorDeserialization ? (Func<TConstruct, SerializingMember, TConstruct>) ((unpackedValue, entry) => this.\u003C\u003E4__this.EmitSetMemberValueStatement(context, result, entry.Member, unpackedValue)) : (Func<TConstruct, SerializingMember, TConstruct>) ((unpackedValue, entry) => this.\u003C\u003E4__this.EmitStoreVariableStatement(context, constructorArgumentsIndex[entry.Contract.Name], unpackedValue));
      yield return this.EmitForLoop(context, itemsCount, (Func<SerializerBuilder<TContext, TConstruct, TObject>.ForLoopContext, TConstruct>) (loopContext =>
      {
        // ISSUE: reference to a compiler-generated field
        TConstruct key = this.\u003C\u003E4__this.DeclareLocal(context, typeof (string), "key");
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        TConstruct construct1 = this.\u003C\u003E4__this.EmitUnpackItemValueExpression(context, typeof (string), context.DictionaryKeyNilImplication, context.Unpacker, loopContext.Counter, this.\u003C\u003E4__this.MakeStringLiteral(context, "MemberName"), default (TConstruct), default (TConstruct), new SerializingMember?(), (PolymorphismSchema) null, (Func<TConstruct, TConstruct>) (unpackedKey => this.CS\u0024\u003C\u003E8__locals98.\u003C\u003E4__this.EmitStoreVariableStatement(context, key, unpackedKey)));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        TConstruct construct2 = this.\u003C\u003E4__this.EmitStringSwitchStatement(context, key, (IDictionary<string, TConstruct>) target.Members.Where<SerializingMember>((Func<SerializingMember, bool>) (e =>
        {
          if (!(e.Member != (MemberInfo) null))
            return false;
          return !target.IsConstructorDeserialization || constructorArgumentsIndex.ContainsKey(e.Contract.Name);
        })).ToDictionary<SerializingMember, string, TConstruct>((Func<SerializingMember, string>) (entry => entry.Contract.Name), (Func<SerializingMember, TConstruct>) (entry => this.CS\u0024\u003C\u003E8__locals98.\u003C\u003E4__this.EmitUnpackItemValueExpression(context, entry.Member.GetMemberValueType(), entry.Contract.NilImplication, context.Unpacker, loopContext.Counter, this.CS\u0024\u003C\u003E8__locals98.\u003C\u003E4__this.MakeStringLiteral(context, entry.Member.ToString()), default (TConstruct), default (TConstruct), new SerializingMember?(entry), (PolymorphismSchema) null, (Func<TConstruct, TConstruct>) (unpackedValue => storeValueStatementEmitter(unpackedValue, entry))))), this.\u003C\u003E4__this.EmitInvokeVoidMethod(context, context.Unpacker, typeof (Unpacker).GetMethod("Skip")));
        // ISSUE: reference to a compiler-generated field
        return this.\u003C\u003E4__this.EmitSequentialStatements(context, typeof (void), key, construct1, construct2);
      }));
      if (target.IsConstructorDeserialization)
        yield return this.EmitInvokeDeserializationConstructorStatement(context, target.DeserializationConstructor, (IList<TConstruct>) constructorArguments, result);
    }

    private IEnumerable<TConstruct> InitializeConstructorArgumentInitializationStatements(
      TContext context,
      SerializationTarget target,
      ParameterInfo[] constructorParameters,
      List<TConstruct> constructorArguments,
      Dictionary<string, TConstruct> constructorArgumentsIndex)
    {
      if (target.IsConstructorDeserialization)
      {
        for (int i = 0; i < constructorParameters.Length; ++i)
        {
          TConstruct argument = this.DeclareLocal(context, constructorParameters[i].ParameterType, "ctorArg" + (object) i);
          yield return argument;
          constructorArguments.Add(argument);
          string correspondingMemberName = target.FindCorrespondingMemberName(constructorParameters[i]);
          if (correspondingMemberName != null)
            constructorArgumentsIndex.Add(correspondingMemberName, argument);
          yield return this.EmitStoreVariableStatement(context, argument, this.MakeDefaultParameterValueLiteral(context, argument, constructorParameters[i].ParameterType, constructorParameters[i].DefaultValue, constructorParameters[i].GetHasDefaultValue()));
        }
      }
    }

    private TConstruct EmitInvokeDeserializationConstructorStatement(
      TContext context,
      ConstructorInfo constructor,
      IList<TConstruct> constructorArguments,
      TConstruct resultVariable)
    {
      return this.EmitStoreVariableStatement(context, resultVariable, this.EmitCreateNewObjectExpression(context, resultVariable, constructor, constructorArguments.ToArray<TConstruct>()));
    }

    private void BuildTupleSerializer(TContext context, IList<PolymorphismSchema> itemSchemaList)
    {
      IList<Type> tupleItemTypes = TupleItems.GetTupleItemTypes(typeof (TObject));
      this.BuildTuplePackTo(context, tupleItemTypes, itemSchemaList);
      this.BuildTupleUnpackFrom(context, tupleItemTypes, itemSchemaList);
    }

    private void BuildTuplePackTo(
      TContext context,
      IList<Type> itemTypes,
      IList<PolymorphismSchema> itemSchemaList)
    {
      this.EmitMethodPrologue(context, SerializerMethod.PackToCore);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = this.EmitSequentialStatements(context, typeof (void), this.BuildTuplePackToCore(context, itemTypes, itemSchemaList));
      }
      finally
      {
        this.EmitMethodEpilogue(context, SerializerMethod.PackToCore, construct);
      }
    }

    private IEnumerable<TConstruct> BuildTuplePackToCore(
      TContext context,
      IList<Type> itemTypes,
      IList<PolymorphismSchema> itemSchemaList)
    {
      yield return this.EmitPutArrayHeaderExpression(context, this.MakeInt32Literal(context, itemTypes.Count));
      int depth = -1;
      List<Type> tupleTypeList = TupleItems.CreateTupleTypeList(itemTypes);
      List<PropertyInfo> propertyInvocationChain = new List<PropertyInfo>(itemTypes.Count % 7 + 1);
      for (int i = 0; i < itemTypes.Count; ++i)
      {
        if (i % 7 == 0)
          ++depth;
        for (int index = 0; index < depth; ++index)
          propertyInvocationChain.Add(tupleTypeList[index].GetProperty("Rest"));
        PropertyInfo itemNProperty = tupleTypeList[depth].GetProperty("Item" + (object) (i % 7 + 1));
        propertyInvocationChain.Add(itemNProperty);
        foreach (TConstruct tupleItemStatement in this.EmitPackTupleItemStatements(context, itemTypes[i], context.Packer, context.PackToTarget, (IEnumerable<PropertyInfo>) propertyInvocationChain, itemSchemaList.Count == 0 ? (PolymorphismSchema) null : itemSchemaList[i]))
          yield return tupleItemStatement;
        propertyInvocationChain.Clear();
      }
    }

    private IEnumerable<TConstruct> EmitPackTupleItemStatements(
      TContext context,
      Type itemType,
      TConstruct currentPacker,
      TConstruct tuple,
      IEnumerable<PropertyInfo> propertyInvocationChain,
      PolymorphismSchema itemsSchema)
    {
      return this.EmitPackItemStatements(context, currentPacker, itemType, NilImplication.Null, (string) null, propertyInvocationChain.Aggregate<PropertyInfo, TConstruct>(tuple, (Func<TConstruct, PropertyInfo, TConstruct>) ((propertySource, property) => this.EmitGetPropertyExpression(context, propertySource, property))), new SerializingMember?(), itemsSchema);
    }

    private void BuildTupleUnpackFrom(
      TContext context,
      IList<Type> itemTypes,
      IList<PolymorphismSchema> itemSchemaList)
    {
      this.EmitMethodPrologue(context, SerializerMethod.UnpackFromCore);
      TConstruct construct = default (TConstruct);
      try
      {
        construct = this.EmitSequentialStatements(context, typeof (TObject), this.BuildTupleUnpackFromCore(context, itemTypes, itemSchemaList));
      }
      finally
      {
        this.EmitMethodEpilogue(context, SerializerMethod.UnpackFromCore, construct);
      }
    }

    private IEnumerable<TConstruct> BuildTupleUnpackFromCore(
      TContext context,
      IList<Type> itemTypes,
      IList<PolymorphismSchema> itemSchemaList)
    {
      List<Type> tupleTypeList = TupleItems.CreateTupleTypeList(itemTypes);
      yield return this.EmitCheckIsArrayHeaderExpression(context, context.Unpacker);
      yield return this.EmitCheckTupleCardinarityExpression(context, context.Unpacker, itemTypes.Count);
      // ISSUE: reference to a compiler-generated field
      TConstruct[] unpackedItems = itemTypes.Select<Type, TConstruct>((Func<Type, int, TConstruct>) ((type, i) => this.\u003C\u003E4__this.DeclareLocal(context, type, "item" + (object) i))).ToArray<TConstruct>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<TConstruct> unpackItems = itemTypes.Select<Type, TConstruct>((Func<Type, int, TConstruct>) ((unpackedNullableItemType, i) => this.\u003C\u003E4__this.EmitUnpackItemValueExpression(context, unpackedNullableItemType, context.TupleItemNilImplication, context.Unpacker, this.\u003C\u003E4__this.MakeInt32Literal(context, i), this.\u003C\u003E4__this.MakeStringLiteral(context, "Item" + i.ToString((IFormatProvider) CultureInfo.InvariantCulture)), default (TConstruct), default (TConstruct), new SerializingMember?(), itemSchemaList.Count == 0 ? (PolymorphismSchema) null : itemSchemaList[i], (Func<TConstruct, TConstruct>) (unpackedItem => this.CS\u0024\u003C\u003E8__localsbf.\u003C\u003E4__this.EmitStoreVariableStatement(context, unpackedItems[i], unpackedItem)))));
      TConstruct currentTuple = default (TConstruct);
      for (int index = tupleTypeList.Count - 1; index >= 0; --index)
      {
        IEnumerable<TConstruct> source = ((IEnumerable<TConstruct>) unpackedItems).Skip<TConstruct>(index * 7).Take<TConstruct>(Math.Min(unpackedItems.Length, 7));
        TContext context1 = context;
        TConstruct variable = default (TConstruct);
        ConstructorInfo constructor = ((IEnumerable<ConstructorInfo>) tupleTypeList[index].GetConstructors()).Single<ConstructorInfo>();
        TConstruct[] array;
        if ((object) currentTuple != null)
          array = ((IEnumerable<TConstruct>) source.ToArray<TConstruct>()).Concat<TConstruct>((IEnumerable<TConstruct>) new TConstruct[1]
          {
            currentTuple
          }).ToArray<TConstruct>();
        else
          array = source.ToArray<TConstruct>();
        currentTuple = this.EmitCreateNewObjectExpression(context1, variable, constructor, array);
      }
      yield return this.EmitSequentialStatements(context, typeof (TObject), ((IEnumerable<TConstruct>) unpackedItems).Concat<TConstruct>(unpackItems).Concat<TConstruct>((IEnumerable<TConstruct>) new TConstruct[1]
      {
        this.EmitRetrunStatement(context, currentTuple)
      }));
    }

    private TConstruct EmitCheckTupleCardinarityExpression(
      TContext context,
      TConstruct unpacker,
      int cardinarity)
    {
      return this.EmitConditionalExpression(context, this.EmitNotEqualsExpression(context, this.EmitGetPropertyExpression(context, unpacker, _Unpacker.ItemsCount), this.MakeInt64Literal(context, (long) cardinarity)), this.EmitThrowExpression(context, typeof (Unpacker), SerializationExceptions.NewIsNotArrayHeaderMethod), default (TConstruct));
    }

    protected class ForLoopContext
    {
      public TConstruct Counter { get; private set; }

      public ForLoopContext(TConstruct counter)
      {
        this.Counter = counter;
      }
    }

    internal class SerializerBuilderNilImplicationHandler : NilImplicationHandler<TConstruct, TConstruct, SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnPackingParameter, SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnUnpacedParameter>
    {
      protected override TConstruct OnPackingMessagePackObject(
        SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnPackingParameter parameter)
      {
        return parameter.Builder.EmitGetPropertyExpression(parameter.Context, parameter.Item, _MessagePackObject.IsNil);
      }

      protected override TConstruct OnPackingReferenceTypeObject(
        SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnPackingParameter parameter)
      {
        return parameter.Builder.EmitEqualsExpression(parameter.Context, parameter.Item, parameter.Builder.MakeNullLiteral(parameter.Context, parameter.ItemType));
      }

      protected override TConstruct OnPackingNullableValueTypeObject(
        SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnPackingParameter parameter)
      {
        return parameter.Builder.EmitNotExpression(parameter.Context, parameter.Builder.EmitGetPropertyExpression(parameter.Context, parameter.Item, parameter.ItemType.GetProperty("HasValue")));
      }

      protected override TConstruct OnPackingCore(
        SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnPackingParameter parameter,
        TConstruct condition)
      {
        return parameter.Builder.EmitConditionalExpression(parameter.Context, condition, parameter.Builder.EmitThrowExpression(parameter.Context, parameter.ItemType, SerializationExceptions.NewNullIsProhibitedMethod, parameter.Builder.MakeStringLiteral(parameter.Context, parameter.MemberName)), default (TConstruct));
      }

      protected override TConstruct OnNopOnUnpacked(
        SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnUnpacedParameter parameter)
      {
        return default (TConstruct);
      }

      protected override TConstruct OnThrowNullIsProhibitedExceptionOnUnpacked(
        SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnUnpacedParameter parameter)
      {
        return parameter.Builder.EmitThrowExpression(parameter.Context, parameter.Store.ContextType, SerializationExceptions.NewNullIsProhibitedMethod, parameter.MemberName);
      }

      protected override TConstruct OnThrowValueTypeCannotBeNull3OnUnpacked(
        SerializerBuilder<TContext, TConstruct, TObject>.SerializerBuilderOnUnpacedParameter parameter)
      {
        return parameter.Builder.EmitThrowExpression(parameter.Context, parameter.Store.ContextType, SerializationExceptions.NewValueTypeCannotBeNull3Method, parameter.MemberName, parameter.Builder.EmitTypeOfExpression(parameter.Context, parameter.ItemType), parameter.Builder.EmitTypeOfExpression(parameter.Context, typeof (TObject)));
      }
    }

    internal struct SerializerBuilderOnPackingParameter : INilImplicationHandlerParameter
    {
      public readonly SerializerBuilder<TContext, TConstruct, TObject> Builder;
      public readonly TContext Context;
      public readonly TConstruct Item;
      private readonly Type _itemType;
      public readonly string MemberName;

      public Type ItemType
      {
        get
        {
          return this._itemType;
        }
      }

      public SerializerBuilderOnPackingParameter(
        SerializerBuilder<TContext, TConstruct, TObject> builder,
        TContext context,
        TConstruct item,
        Type itemType,
        string memberName)
      {
        this.Builder = builder;
        this.Context = context;
        this.Item = item;
        this._itemType = itemType;
        this.MemberName = memberName;
      }
    }

    internal struct SerializerBuilderOnUnpacedParameter : INilImplicationHandlerOnUnpackedParameter<TConstruct>, INilImplicationHandlerParameter
    {
      public readonly SerializerBuilder<TContext, TConstruct, TObject> Builder;
      public readonly TContext Context;
      private readonly Type _itemType;
      public readonly TConstruct MemberName;
      private readonly TConstruct _store;

      public Type ItemType
      {
        get
        {
          return this._itemType;
        }
      }

      public TConstruct Store
      {
        get
        {
          return this._store;
        }
      }

      public SerializerBuilderOnUnpacedParameter(
        SerializerBuilder<TContext, TConstruct, TObject> builder,
        TContext context,
        Type itemType,
        TConstruct memberName,
        TConstruct store)
      {
        this.Builder = builder;
        this.Context = context;
        this._itemType = itemType;
        this.MemberName = memberName;
        this._store = store;
      }
    }
  }
}
