// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializationExceptions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using MsgPack.Serialization.Reflection;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace MsgPack.Serialization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class SerializationExceptions
  {
    internal static readonly MethodInfo NewValueTypeCannotBeNull3Method = FromExpression.ToMethod<string, Type, Type, Exception>((Expression<Func<string, Type, Type, Exception>>) ((name, memberType, declaringType) => SerializationExceptions.NewValueTypeCannotBeNull(name, memberType, declaringType)));
    internal static readonly MethodInfo NewMissingItemMethod = FromExpression.ToMethod<int, Exception>((Expression<Func<int, Exception>>) (index => SerializationExceptions.NewMissingItem(index)));
    internal static readonly MethodInfo NewIsNotArrayHeaderMethod = FromExpression.ToMethod<Exception>((Expression<Func<Exception>>) (() => SerializationExceptions.NewIsNotArrayHeader()));
    internal static readonly MethodInfo NewIsNotMapHeaderMethod = FromExpression.ToMethod<Exception>((Expression<Func<Exception>>) (() => SerializationExceptions.NewIsNotMapHeader()));
    internal static readonly MethodInfo NewNullIsProhibitedMethod = FromExpression.ToMethod<string, Exception>((Expression<Func<string, Exception>>) (memberName => SerializationExceptions.NewNullIsProhibited(memberName)));

    public static Exception NewValueTypeCannotBeNull(
      string name,
      Type memberType,
      Type declaringType)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Member '{0}' of type '{1}' cannot be null because it is value type('{2}').", (object) name, (object) declaringType, (object) memberType));
    }

    public static Exception NewValueTypeCannotBeNull(Type type)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot be null '{0}' type value.", (object) type));
    }

    public static Exception NewTypeCannotSerialize(Type type)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot serialize '{0}' type.", (object) type));
    }

    public static Exception NewTypeCannotDeserialize(Type type)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot deserialize '{0}' type.", (object) type));
    }

    public static Exception NewTypeCannotDeserialize(
      Type type,
      string memberName,
      Exception inner)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot deserialize member '{1}' of type '{0}'.", (object) type, (object) memberName), inner);
    }

    public static Exception NewMissingItem(int index)
    {
      return (Exception) new InvalidMessagePackStreamException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Items at index '{0}' is missing.", (object) index));
    }

    internal static Exception NewTargetDoesNotHavePublicDefaultConstructor(Type type)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' does not have default (parameterless) public constructor.", (object) type));
    }

    internal static Exception NewTargetDoesNotHavePublicDefaultConstructorNorInitialCapacity(
      Type type)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' does not have both of default (parameterless) public constructor and  public constructor with an Int32 parameter.", (object) type));
    }

    public static Exception NewMissingProperty(string name)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Property '{0}' is missing.", (object) name));
    }

    public static Exception NewUnexpectedEndOfStream()
    {
      return (Exception) new SerializationException("Stream unexpectedly ends.");
    }

    public static Exception NewMissingAddMethod(Type type)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' does not have appropriate Add method.", (object) type));
    }

    public static Exception NewIsNotArrayHeader()
    {
      return (Exception) new SerializationException("Unpacker is not in the array header. The stream may not be array.");
    }

    public static Exception NewIsNotMapHeader()
    {
      return (Exception) new SerializationException("Unpacker is not in the map header. The stream may not be map.");
    }

    public static Exception NewNotSupportedBecauseCannotInstanciateAbstractType(Type type)
    {
      return (Exception) new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "This operation is not supported because '{0}' cannot be instanciated.", (object) type));
    }

    public static Exception NewTupleCardinarityIsNotMatch(
      int expectedTupleCardinality,
      int actualArrayLength)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The length of array ({0}) does not match to tuple cardinality ({1}).", (object) actualArrayLength, (object) expectedTupleCardinality));
    }

    public static Exception NewIsIncorrectStream(Exception innerException)
    {
      return (Exception) new SerializationException("Failed to unpack items count of the collection.", innerException);
    }

    public static Exception NewIsTooLargeCollection()
    {
      return (Exception) new MessageNotSupportedException("The collection which has more than Int32.MaxValue items is not supported.");
    }

    public static Exception NewNullIsProhibited(string memberName)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The member '{0}' cannot be nil.", (object) memberName));
    }

    public static Exception NewReadOnlyMemberItemsMustNotBeNull(string memberName)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The member '{0}' cannot be nil because it is read only member.", (object) memberName));
    }

    public static Exception NewStreamDoesNotContainCollectionForMember(string memberName)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot deserialize member '{0}' because the underlying stream does not contain collection.", (object) memberName));
    }

    public static Exception NewUnexpectedArrayLength(int expectedLength, int actualLength)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The MessagePack stream is invalid. Expected array length is {0}, but actual is {1}.", (object) expectedLength, (object) actualLength));
    }

    public static Exception NewFailedToDeserializeMember(
      Type targetType,
      string memberName,
      Exception inner)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot deserialize member '{0}' of type '{1}'.", (object) memberName, (object) targetType), inner);
    }

    internal static Exception NewUnpackToIsNotSupported(Type type, Exception inner)
    {
      return (Exception) new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "This operation is not supported for '{0}' because it does not have accesible Add(T) method.", (object) type), inner);
    }

    internal static Exception NewValueTypeCannotBePolymorphic(Type type)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Value type '{0}' cannot be polymorphic.", (object) type));
    }

    internal static Exception NewUnknownTypeEmbedding()
    {
      return (Exception) new SerializationException("Cannot deserialize with type-embedding based serializer. Root object must be 3 element array.");
    }

    internal static Exception NewIncompatibleCollectionSerializer(
      Type targetType,
      Type incompatibleType,
      Type exampleClass)
    {
      return (Exception) new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot serialize type '{0}' because registered or generated serializer '{1}' does not implement '{2}', which is implemented by '{3}', for example.", (object) targetType.GetFullName(), (object) incompatibleType.GetFullName(), (object) typeof (ICollectionInstanceFactory), (object) exampleClass.GetFullName()));
    }
  }
}
