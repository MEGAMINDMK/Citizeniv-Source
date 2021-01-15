// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.ImmutableCollectionSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class ImmutableCollectionSerializer<T, TItem> : MessagePackSerializer<T>
    where T : IEnumerable<TItem>
  {
    protected static readonly Func<TItem[], T> factory = ImmutableCollectionSerializer<T, TItem>.FindFactory();
    private readonly MessagePackSerializer<TItem> _itemSerializer;

    private static Func<TItem[], T> FindFactory()
    {
      Type factoryType = typeof (T).GetAssembly().GetType(typeof (T).FullName.Remove(typeof (T).FullName.IndexOf('`')));
      if (factoryType == (Type) null)
        return (Func<TItem[], T>) (_ =>
        {
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' may not be an immutable collection.", (object) typeof (T).AssemblyQualifiedName));
        });
      MethodInfo methodInfo = ((IEnumerable<MethodInfo>) factoryType.GetMethods()).SingleOrDefault<MethodInfo>((Func<MethodInfo, bool>) (m => m.IsStatic && m.IsPublic && (m.IsGenericMethod && m.Name == "Create") && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.IsArray));
      if (methodInfo == (MethodInfo) null)
        return (Func<TItem[], T>) (_ =>
        {
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' does not have Create({1}) public static method.", (object) factoryType.AssemblyQualifiedName, (object) typeof (TItem[])));
        });
      return methodInfo.MakeGenericMethod(typeof (TItem)).CreateDelegate(typeof (Func<TItem[], T>)) as Func<TItem[], T>;
    }

    public ImmutableCollectionSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema itemsSchema)
      : base(ownerContext)
    {
      this._itemSerializer = ownerContext.GetSerializer<TItem>((object) itemsSchema);
    }

    protected internal override void PackToCore(Packer packer, T objectTree)
    {
      packer.PackArrayHeader(objectTree.Count<TItem>());
      using (IEnumerator<TItem> enumerator = objectTree.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          TItem current = enumerator.Current;
          this._itemSerializer.PackTo(packer, current);
        }
      }
    }

    protected internal override T UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      TItem[] objArray = new TItem[UnpackHelpers.GetItemsCount(unpacker)];
      using (Unpacker unpacker1 = unpacker.ReadSubtree())
      {
        for (int index = 0; index < objArray.Length; ++index)
        {
          if (!unpacker1.Read())
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          objArray[index] = this._itemSerializer.UnpackFrom(unpacker1);
        }
      }
      return ImmutableCollectionSerializer<T, TItem>.factory(objArray);
    }

    protected internal override void UnpackToCore(Unpacker unpacker, T collection)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unable to unpack items to existing immutable collection '{0}'.", (object) typeof (T)));
    }
  }
}
