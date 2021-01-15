// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.AbstractSerializers.SerializerGenerationContext`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.AbstractSerializers
{
  internal abstract class SerializerGenerationContext<TConstruct>
  {
    public virtual TConstruct Context
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public SerializationContext SerializationContext { get; private set; }

    public TConstruct Packer { get; protected set; }

    public TConstruct PackToTarget { get; protected set; }

    public TConstruct Unpacker { get; protected set; }

    public TConstruct UnpackToTarget { get; protected set; }

    public TConstruct CollectionToBeAdded { get; protected set; }

    public TConstruct ItemToAdd { get; protected set; }

    public TConstruct KeyToAdd { get; protected set; }

    public TConstruct ValueToAdd { get; protected set; }

    public TConstruct InitialCapacity { get; protected set; }

    public NilImplication CollectionItemNilImplication { get; private set; }

    public NilImplication DictionaryKeyNilImplication { get; private set; }

    public NilImplication TupleItemNilImplication { get; private set; }

    protected SerializerGenerationContext(SerializationContext context)
    {
      this.SerializationContext = context;
      this.CollectionItemNilImplication = NilImplication.Null;
      this.DictionaryKeyNilImplication = NilImplication.Prohibit;
      this.TupleItemNilImplication = NilImplication.Null;
    }

    public void Reset(Type targetType, Type baseClass)
    {
      this.ResetCore(targetType, baseClass);
    }

    protected abstract void ResetCore(Type targetType, Type baseClass);

    public virtual string GetUniqueVariableName(string prefix)
    {
      return prefix;
    }
  }
}
