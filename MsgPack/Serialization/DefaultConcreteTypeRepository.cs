// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultConcreteTypeRepository
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MsgPack.Serialization
{
  public sealed class DefaultConcreteTypeRepository
  {
    private readonly TypeKeyRepository _defaultCollectionTypes;

    internal DefaultConcreteTypeRepository()
    {
      this._defaultCollectionTypes = new TypeKeyRepository(new Dictionary<RuntimeTypeHandle, object>(12)
      {
        {
          typeof (IEnumerable<>).TypeHandle,
          (object) typeof (List<>)
        },
        {
          typeof (ICollection<>).TypeHandle,
          (object) typeof (List<>)
        },
        {
          typeof (IList<>).TypeHandle,
          (object) typeof (List<>)
        },
        {
          typeof (IDictionary<,>).TypeHandle,
          (object) typeof (Dictionary<,>)
        },
        {
          typeof (IEnumerable).TypeHandle,
          (object) typeof (List<MessagePackObject>)
        },
        {
          typeof (ICollection).TypeHandle,
          (object) typeof (List<MessagePackObject>)
        },
        {
          typeof (IList).TypeHandle,
          (object) typeof (List<MessagePackObject>)
        },
        {
          typeof (IDictionary).TypeHandle,
          (object) typeof (MessagePackObjectDictionary)
        },
        {
          typeof (ISet<>).TypeHandle,
          (object) typeof (HashSet<>)
        },
        {
          typeof (IReadOnlyCollection<>).TypeHandle,
          (object) typeof (List<>)
        },
        {
          typeof (IReadOnlyList<>).TypeHandle,
          (object) typeof (List<>)
        },
        {
          typeof (IReadOnlyDictionary<,>).TypeHandle,
          (object) typeof (Dictionary<,>)
        }
      });
    }

    public Type Get(Type abstractCollectionType)
    {
      if (abstractCollectionType == (Type) null)
        throw new ArgumentNullException(nameof (abstractCollectionType));
      object matched;
      object genericDefinitionMatched;
      this._defaultCollectionTypes.Get(abstractCollectionType, out matched, out genericDefinitionMatched);
      Type type = matched as Type;
      return (object) type != null ? type : genericDefinitionMatched as Type;
    }

    internal Type GetConcreteType(Type abstractCollectionType)
    {
      Type source = this.Get(abstractCollectionType);
      return source == (Type) null || !source.GetIsGenericTypeDefinition() || !abstractCollectionType.GetIsGenericType() ? source : source.MakeGenericType(abstractCollectionType.GetGenericArguments());
    }

    public void Register(Type abstractCollectionType, Type defaultCollectionType)
    {
      if (abstractCollectionType == (Type) null)
        throw new ArgumentNullException(nameof (abstractCollectionType));
      if (defaultCollectionType == (Type) null)
        throw new ArgumentNullException(nameof (defaultCollectionType));
      if (!((IEnumerable<Type>) abstractCollectionType.GetInterfaces()).Contains<Type>(typeof (IEnumerable)))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The type '{0}' is not collection.", (object) abstractCollectionType), nameof (abstractCollectionType));
      if (abstractCollectionType.GetIsGenericTypeDefinition())
      {
        if (!defaultCollectionType.GetIsGenericTypeDefinition())
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The closed generic type '{1}' cannot be assigned to open generic type '{0}'.", (object) abstractCollectionType, (object) defaultCollectionType), nameof (defaultCollectionType));
        if (abstractCollectionType.GetGenericTypeParameters().Length != defaultCollectionType.GetGenericTypeParameters().Length)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The default generic collection type '{1}' does not have same arity for abstract generic collection type '{0}'.", (object) abstractCollectionType, (object) defaultCollectionType), nameof (defaultCollectionType));
      }
      else if (defaultCollectionType.GetIsGenericTypeDefinition())
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The open generic type '{1}' cannot be assigned to closed generic type '{0}'.", (object) abstractCollectionType, (object) defaultCollectionType), nameof (defaultCollectionType));
      if (defaultCollectionType.GetIsAbstract() || defaultCollectionType.GetIsInterface())
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The defaultCollectionType cannot be abstract class nor interface. The type '{0}' is abstract type.", (object) defaultCollectionType), nameof (defaultCollectionType));
      if (!abstractCollectionType.IsAssignableFrom(defaultCollectionType) && abstractCollectionType.GetIsGenericTypeDefinition() && (!((IEnumerable<Type>) defaultCollectionType.GetInterfaces()).Select<Type, Type>((Func<Type, Type>) (t => !t.GetIsGenericType() || t.GetIsGenericTypeDefinition() ? t : t.GetGenericTypeDefinition())).Contains<Type>(abstractCollectionType) && !DefaultConcreteTypeRepository.IsAnscestorType(abstractCollectionType, defaultCollectionType)))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The type '{1}' is not assignable to '{0}'.", (object) abstractCollectionType, (object) defaultCollectionType), nameof (defaultCollectionType));
      this._defaultCollectionTypes.Register(abstractCollectionType, (object) defaultCollectionType, (Type) null, (object) null, SerializerRegistrationOptions.AllowOverride);
    }

    private static bool IsAnscestorType(Type mayBeAncestor, Type mayBeDescendant)
    {
      for (Type source = mayBeDescendant; source != (Type) null; source = source.GetBaseType())
      {
        if (source == mayBeAncestor || source.GetIsGenericType() && source.GetGenericTypeDefinition() == mayBeAncestor)
          return true;
      }
      return false;
    }

    public bool Unregister(Type abstractCollectionType)
    {
      return !(abstractCollectionType == (Type) null) && this._defaultCollectionTypes.Unregister(abstractCollectionType);
    }
  }
}
