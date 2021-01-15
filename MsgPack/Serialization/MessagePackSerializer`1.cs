// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.MessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization
{
  public abstract class MessagePackSerializer<T> : IMessagePackSingleObjectSerializer, IMessagePackSerializer
  {
    private static readonly bool _isNullable = MessagePackSerializer<T>.JudgeNullable();
    internal static readonly MethodInfo UnpackToCoreMethod = FromExpression.ToMethod<MessagePackSerializer<T>, Unpacker, T>((Expression<Action<MessagePackSerializer<T>, Unpacker, T>>) ((@this, unpacker, collection) => @this.UnpackToCore(unpacker, collection)));
    private readonly PackerCompatibilityOptions? _packerCompatibilityOptionsForCompatibility;
    private readonly SerializationContext _ownerContext;

    protected internal PackerCompatibilityOptions PackerCompatibilityOptions
    {
      get
      {
        return this._packerCompatibilityOptionsForCompatibility.GetValueOrDefault(this.OwnerContext.CompatibilityOptions.PackerCompatibilityOptions);
      }
    }

    protected internal SerializationContext OwnerContext
    {
      get
      {
        return this._ownerContext;
      }
    }

    [Obsolete("Use MessagePackSerializer (SerlaizationContext) instead.")]
    protected MessagePackSerializer()
      : this(PackerCompatibilityOptions.Classic)
    {
    }

    [Obsolete("Use MessagePackSerializer (SerlaizationContext, PackerCompatibilityOptions) instead.")]
    protected MessagePackSerializer(
      PackerCompatibilityOptions packerCompatibilityOptions)
      : this((SerializationContext) null, packerCompatibilityOptions)
    {
    }

    protected MessagePackSerializer(SerializationContext ownerContext)
      : this(ownerContext, new PackerCompatibilityOptions?())
    {
    }

    protected MessagePackSerializer(
      SerializationContext ownerContext,
      PackerCompatibilityOptions packerCompatibilityOptions)
      : this(ownerContext, new PackerCompatibilityOptions?(packerCompatibilityOptions))
    {
    }

    private MessagePackSerializer(
      SerializationContext ownerContext,
      PackerCompatibilityOptions? packerCompatibilityOptions)
    {
      if (ownerContext == null)
        throw new ArgumentNullException(nameof (ownerContext));
      this._packerCompatibilityOptionsForCompatibility = packerCompatibilityOptions;
      this._ownerContext = ownerContext;
    }

    private static bool JudgeNullable()
    {
      return !typeof (T).GetIsValueType() || typeof (T) == typeof (MessagePackObject) || typeof (T).GetIsGenericType() && typeof (T).GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    public void Pack(Stream stream, T objectTree)
    {
      this.PackTo(Packer.Create(stream, this.PackerCompatibilityOptions), objectTree);
    }

    public T Unpack(Stream stream)
    {
      Unpacker unpacker = Unpacker.Create(stream);
      if (!unpacker.Read())
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      return this.UnpackFrom(unpacker);
    }

    public void PackTo(Packer packer, T objectTree)
    {
      if (packer == null)
        throw new ArgumentNullException(nameof (packer));
      if ((object) objectTree == null)
        packer.PackNull();
      else
        this.PackToCore(packer, objectTree);
    }

    protected internal abstract void PackToCore(Packer packer, T objectTree);

    public T UnpackFrom(Unpacker unpacker)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      return unpacker.LastReadData.IsNil ? this.UnpackNil() : this.UnpackFromCore(unpacker);
    }

    protected internal virtual T UnpackNil()
    {
      if (MessagePackSerializer<T>._isNullable)
        return default (T);
      throw SerializationExceptions.NewValueTypeCannotBeNull(typeof (T));
    }

    protected internal abstract T UnpackFromCore(Unpacker unpacker);

    public void UnpackTo(Unpacker unpacker, T collection)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      if ((object) collection == null)
        throw new ArgumentNullException(nameof (unpacker));
      if (unpacker.LastReadData.IsNil)
        return;
      this.UnpackToCore(unpacker, collection);
    }

    protected internal virtual void UnpackToCore(Unpacker unpacker, T collection)
    {
      throw SerializationExceptions.NewUnpackToIsNotSupported(typeof (T), (Exception) null);
    }

    public byte[] PackSingleObject(T objectTree)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        this.Pack((Stream) memoryStream, objectTree);
        return memoryStream.ToArray();
      }
    }

    public T UnpackSingleObject(byte[] buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        return this.Unpack((Stream) memoryStream);
    }

    void IMessagePackSerializer.PackTo(Packer packer, object objectTree)
    {
      if (packer == null)
        throw new ArgumentNullException(nameof (packer));
      if (objectTree == null)
      {
        if (typeof (T).GetIsValueType() && (!typeof (T).GetIsGenericType() || !(typeof (T).GetGenericTypeDefinition() == typeof (Nullable<>))))
          throw SerializationExceptions.NewValueTypeCannotBeNull(typeof (T));
        packer.PackNull();
      }
      else
      {
        if (!(objectTree is T objectTree1))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' is not compatible for '{1}'.", (object) objectTree.GetType(), (object) typeof (T)), nameof (objectTree));
        this.PackToCore(packer, objectTree1);
      }
    }

    object IMessagePackSerializer.UnpackFrom(Unpacker unpacker)
    {
      return (object) this.UnpackFrom(unpacker);
    }

    void IMessagePackSerializer.UnpackTo(Unpacker unpacker, object collection)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      if (!(collection is T collection1))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' is not compatible for '{1}'.", (object) collection.GetType(), (object) typeof (T)), nameof (collection));
      this.UnpackTo(unpacker, collection1);
    }

    byte[] IMessagePackSingleObjectSerializer.PackSingleObject(
      object objectTree)
    {
      bool flag = objectTree is T;
      if (typeof (T).GetIsValueType() && !flag || objectTree != null && !flag)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' is not compatible for '{1}'.", objectTree == null ? (object) "(null)" : (object) objectTree.GetType().FullName, (object) typeof (T)), nameof (objectTree));
      return this.PackSingleObject((T) objectTree);
    }

    object IMessagePackSingleObjectSerializer.UnpackSingleObject(
      byte[] buffer)
    {
      return (object) this.UnpackSingleObject(buffer);
    }
  }
}
