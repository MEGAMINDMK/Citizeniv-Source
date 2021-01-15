// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionTreeContext
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal sealed class ExpressionTreeContext : SerializerGenerationContext<ExpressionConstruct>
  {
    private readonly ExpressionConstruct _context;
    private ParameterExpression[] _currentParamters;
    private Delegate _packToCore;
    private Delegate _unpackFromCore;
    private Delegate _unpackToCore;
    private Delegate _packUnderyingValueTo;
    private Delegate _unpackFromUnderlyingValue;
    private Delegate _addItem;
    private Delegate _createInstance;

    public override ExpressionConstruct Context
    {
      get
      {
        return this._context;
      }
    }

    public ExpressionConstruct This { get; private set; }

    public ExpressionTreeContext(SerializationContext context)
      : base(context)
    {
      this._context = (ExpressionConstruct) (Expression) Expression.Parameter(typeof (SerializationContext), nameof (context));
    }

    protected override void ResetCore(Type targetType, Type baseClass)
    {
      this.This = (ExpressionConstruct) (Expression) Expression.Parameter(baseClass, "this");
      this.Packer = (ExpressionConstruct) (Expression) Expression.Parameter(typeof (Packer), "packer");
      this.PackToTarget = (ExpressionConstruct) (Expression) Expression.Parameter(targetType, "objectTree");
      this.Unpacker = (ExpressionConstruct) (Expression) Expression.Parameter(typeof (Unpacker), "unpacker");
      this.UnpackToTarget = (ExpressionConstruct) (Expression) Expression.Parameter(targetType, "collection");
      CollectionTraits collectionTraits = targetType.GetCollectionTraits();
      if (!(collectionTraits.ElementType != (Type) null))
        return;
      this.CollectionToBeAdded = (ExpressionConstruct) (Expression) Expression.Parameter(targetType, "collection");
      this.ItemToAdd = (ExpressionConstruct) (Expression) Expression.Parameter(collectionTraits.ElementType, "item");
      if (collectionTraits.DetailedCollectionType == CollectionDetailedKind.GenericDictionary || collectionTraits.DetailedCollectionType == CollectionDetailedKind.GenericReadOnlyDictionary)
      {
        this.KeyToAdd = (ExpressionConstruct) (Expression) Expression.Parameter(collectionTraits.ElementType.GetGenericArguments()[0], "key");
        this.ValueToAdd = (ExpressionConstruct) (Expression) Expression.Parameter(collectionTraits.ElementType.GetGenericArguments()[1], "value");
      }
      this.InitialCapacity = (ExpressionConstruct) (Expression) Expression.Parameter(typeof (int), "initialCapacity");
    }

    public IList<ParameterExpression> GetCurrentParameters()
    {
      return (IList<ParameterExpression>) this._currentParamters;
    }

    public void SetCurrentMethod(SerializerMethod method)
    {
      this._currentParamters = this.GetParameters(method).ToArray<ParameterExpression>();
    }

    public void SetCurrentMethod(Type targetType, EnumSerializerMethod method)
    {
      this._currentParamters = this.GetParameters(targetType, method).ToArray<ParameterExpression>();
    }

    public void SetCurrentMethod(CollectionSerializerMethod method, CollectionTraits traits)
    {
      this._currentParamters = this.GetParameters(method, traits).ToArray<ParameterExpression>();
    }

    public static Type CreateDelegateType<TObject>(SerializerMethod method, Type serializerType)
    {
      switch (method)
      {
        case SerializerMethod.PackToCore:
          return typeof (Action<,,,>).MakeGenericType(serializerType, typeof (SerializationContext), typeof (Packer), typeof (TObject));
        case SerializerMethod.UnpackFromCore:
          return typeof (Func<,,,>).MakeGenericType(serializerType, typeof (SerializationContext), typeof (Unpacker), typeof (TObject));
        case SerializerMethod.UnpackToCore:
          return typeof (Action<,,,>).MakeGenericType(serializerType, typeof (SerializationContext), typeof (Unpacker), typeof (TObject));
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    public static Type CreateDelegateType<TObject>(EnumSerializerMethod method)
    {
      switch (method)
      {
        case EnumSerializerMethod.PackUnderlyingValueTo:
          return typeof (Action<,,>).MakeGenericType(typeof (ExpressionCallbackEnumMessagePackSerializer<>).MakeGenericType(typeof (TObject)), typeof (Packer), typeof (TObject));
        case EnumSerializerMethod.UnpackFromUnderlyingValue:
          return typeof (Func<,,>).MakeGenericType(typeof (ExpressionCallbackEnumMessagePackSerializer<>).MakeGenericType(typeof (TObject)), typeof (MessagePackObject), typeof (TObject));
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    public static Type CreateDelegateType<TObject>(
      CollectionSerializerMethod method,
      Type serializerType,
      CollectionTraits traits)
    {
      switch (method)
      {
        case CollectionSerializerMethod.AddItem:
          return traits.DetailedCollectionType == CollectionDetailedKind.GenericDictionary || traits.DetailedCollectionType == CollectionDetailedKind.GenericReadOnlyDictionary ? typeof (Action<,,,,>).MakeGenericType(serializerType, typeof (SerializationContext), typeof (TObject), traits.ElementType.GetGenericArguments()[0], traits.ElementType.GetGenericArguments()[1]) : typeof (Action<,,,>).MakeGenericType(serializerType, typeof (SerializationContext), typeof (TObject), traits.ElementType);
        case CollectionSerializerMethod.CreateInstance:
          return typeof (Func<,,,>).MakeGenericType(serializerType, typeof (SerializationContext), typeof (int), typeof (TObject));
        case CollectionSerializerMethod.RestoreSchema:
          return typeof (Func<>).MakeGenericType(typeof (PolymorphismSchema));
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    private IEnumerable<ParameterExpression> GetParameters(
      SerializerMethod method)
    {
      yield return this.This.Expression as ParameterExpression;
      yield return this._context.Expression as ParameterExpression;
      switch (method)
      {
        case SerializerMethod.PackToCore:
          yield return this.Packer.Expression as ParameterExpression;
          yield return this.PackToTarget.Expression as ParameterExpression;
          break;
        case SerializerMethod.UnpackFromCore:
          yield return this.Unpacker.Expression as ParameterExpression;
          break;
        case SerializerMethod.UnpackToCore:
          yield return this.Unpacker.Expression as ParameterExpression;
          yield return this.UnpackToTarget.Expression as ParameterExpression;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    private IEnumerable<ParameterExpression> GetParameters(
      Type targetType,
      EnumSerializerMethod method)
    {
      yield return this.This.Expression as ParameterExpression;
      switch (method)
      {
        case EnumSerializerMethod.PackUnderlyingValueTo:
          yield return this.Packer.Expression as ParameterExpression;
          yield return Expression.Parameter(targetType, "enumValue");
          break;
        case EnumSerializerMethod.UnpackFromUnderlyingValue:
          yield return Expression.Parameter(typeof (MessagePackObject), "messagePackObject");
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    private IEnumerable<ParameterExpression> GetParameters(
      CollectionSerializerMethod method,
      CollectionTraits traits)
    {
      switch (method)
      {
        case CollectionSerializerMethod.AddItem:
          yield return this.This.Expression as ParameterExpression;
          yield return this._context.Expression as ParameterExpression;
          yield return this.CollectionToBeAdded.Expression as ParameterExpression;
          if (traits.DetailedCollectionType == CollectionDetailedKind.GenericDictionary || traits.DetailedCollectionType == CollectionDetailedKind.GenericReadOnlyDictionary)
          {
            yield return this.KeyToAdd.Expression as ParameterExpression;
            yield return this.ValueToAdd.Expression as ParameterExpression;
            break;
          }
          yield return this.ItemToAdd.Expression as ParameterExpression;
          break;
        case CollectionSerializerMethod.CreateInstance:
          yield return this.This.Expression as ParameterExpression;
          yield return this._context.Expression as ParameterExpression;
          yield return this.InitialCapacity.Expression as ParameterExpression;
          break;
        case CollectionSerializerMethod.RestoreSchema:
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    public void SetDelegate(SerializerMethod method, Delegate @delegate)
    {
      switch (method)
      {
        case SerializerMethod.PackToCore:
          this._packToCore = @delegate;
          break;
        case SerializerMethod.UnpackFromCore:
          this._unpackFromCore = @delegate;
          break;
        case SerializerMethod.UnpackToCore:
          this._unpackToCore = @delegate;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    public void SetDelegate(EnumSerializerMethod method, Delegate @delegate)
    {
      switch (method)
      {
        case EnumSerializerMethod.PackUnderlyingValueTo:
          this._packUnderyingValueTo = @delegate;
          break;
        case EnumSerializerMethod.UnpackFromUnderlyingValue:
          this._unpackFromUnderlyingValue = @delegate;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    public void SetDelegate(CollectionSerializerMethod method, Delegate @delegate)
    {
      switch (method)
      {
        case CollectionSerializerMethod.AddItem:
          this._addItem = @delegate;
          break;
        case CollectionSerializerMethod.CreateInstance:
          this._createInstance = @delegate;
          break;
        case CollectionSerializerMethod.RestoreSchema:
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method), method.ToString());
      }
    }

    public Delegate GetPackToCore()
    {
      return this._packToCore;
    }

    public Delegate GetUnpackFromCore()
    {
      return this._unpackFromCore;
    }

    public Delegate GetUnpackToCore()
    {
      return this._unpackToCore;
    }

    public Delegate GetPackUnderyingValueTo()
    {
      return this._packUnderyingValueTo;
    }

    public Delegate GetUnpackFromUnderlyingValue()
    {
      return this._unpackFromUnderlyingValue;
    }

    public Delegate GetCreateInstance()
    {
      return this._createInstance;
    }

    public Delegate GetAddItem()
    {
      return this._addItem;
    }
  }
}
