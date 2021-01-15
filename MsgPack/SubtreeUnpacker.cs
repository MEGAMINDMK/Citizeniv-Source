// Decompiled with JetBrains decompiler
// Type: MsgPack.SubtreeUnpacker
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;

namespace MsgPack
{
  internal sealed class SubtreeUnpacker : Unpacker
  {
    private readonly ItemsUnpacker _root;
    private readonly SubtreeUnpacker _parent;
    private readonly Stack<bool> _isMap;
    private readonly Stack<long> _unpacked;
    private readonly Stack<long> _itemsCount;
    private SubtreeUnpacker.State _state;

    public override long ItemsCount
    {
      get
      {
        return this._itemsCount.Count != 0 ? this._itemsCount.Peek() / (this._isMap.Peek() ? 2L : 1L) : 0L;
      }
    }

    public override bool IsArrayHeader
    {
      get
      {
        return this._root.InternalCollectionType == ItemsUnpacker.CollectionType.Array;
      }
    }

    public override bool IsMapHeader
    {
      get
      {
        return this._root.InternalCollectionType == ItemsUnpacker.CollectionType.Map;
      }
    }

    public override bool IsCollectionHeader
    {
      get
      {
        return this._root.InternalCollectionType != ItemsUnpacker.CollectionType.None;
      }
    }

    [Obsolete("Consumer should not use this property. Query LastReadData instead.")]
    public override MessagePackObject? Data
    {
      get
      {
        return new MessagePackObject?(this._root.InternalData);
      }
      protected set
      {
        this._root.InternalData = value.GetValueOrDefault();
      }
    }

    public override MessagePackObject LastReadData
    {
      get
      {
        return this._root.InternalData;
      }
      protected set
      {
        this._root.InternalData = value;
      }
    }

    public SubtreeUnpacker(ItemsUnpacker parent)
      : this(parent, (SubtreeUnpacker) null)
    {
    }

    private SubtreeUnpacker(ItemsUnpacker root, SubtreeUnpacker parent)
    {
      this._root = root;
      this._parent = parent;
      this._unpacked = new Stack<long>(2);
      this._itemsCount = new Stack<long>(2);
      this._isMap = new Stack<bool>(2);
      if (root.ItemsCount > 0L)
      {
        this._itemsCount.Push(root.InternalItemsCount * (long) root.InternalCollectionType);
        this._unpacked.Push(0L);
        this._isMap.Push(root.InternalCollectionType == ItemsUnpacker.CollectionType.Map);
      }
      this._state = SubtreeUnpacker.State.InHead;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this._state != SubtreeUnpacker.State.Disposed)
      {
        do
          ;
        while (this.ReadCore());
        if (this._parent != null)
          this._parent.EndReadSubtree();
        else
          this._root.EndReadSubtree();
        this._state = SubtreeUnpacker.State.Disposed;
      }
      base.Dispose(disposing);
    }

    protected internal override void EndReadSubtree()
    {
      base.EndReadSubtree();
      this._unpacked.Pop();
      this._unpacked.Push(this._itemsCount.Peek());
      this.DiscardCompletedStacks();
    }

    protected override Unpacker ReadSubtreeCore()
    {
      if (this._state == SubtreeUnpacker.State.InHead)
        return (Unpacker) this;
      if (this._unpacked.Count == 0)
        SubtreeUnpacker.ThrowInTailException();
      if (this._root.InternalCollectionType == ItemsUnpacker.CollectionType.None)
        SubtreeUnpacker.ThrowNotInHeadOfCollectionException();
      return (Unpacker) new SubtreeUnpacker(this._root, this);
    }

    private static void ThrowInTailException()
    {
      throw new InvalidOperationException("This unpacker is located in the tail.");
    }

    private static void ThrowNotInHeadOfCollectionException()
    {
      throw new InvalidOperationException("This unpacker is not located in the head of collection.");
    }

    protected override bool ReadCore()
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0 || !this._root.ReadSubtreeItem())
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      this._state = SubtreeUnpacker.State.InProgress;
      return true;
    }

    protected override long? SkipCore()
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
        return new long?(0L);
      long? nullable = this._root.SkipSubtreeItem();
      if (nullable.HasValue)
        this._unpacked.Push(this._unpacked.Pop() + 1L);
      return nullable;
    }

    private void DiscardCompletedStacks()
    {
      if (this._itemsCount.Count == 0)
        return;
      while (this._unpacked.Peek() == this._itemsCount.Peek())
      {
        this._itemsCount.Pop();
        this._unpacked.Pop();
        this._isMap.Pop();
        if (this._itemsCount.Count == 0)
          break;
        this._unpacked.Push(this._unpacked.Pop() + 1L);
      }
    }

    public override bool ReadBoolean(out bool result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = false;
        return false;
      }
      if (!this._root.ReadSubtreeBoolean(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableBoolean(out bool? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new bool?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableBoolean(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadByte(out byte result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = (byte) 0;
        return false;
      }
      if (!this._root.ReadSubtreeByte(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableByte(out byte? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new byte?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableByte(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadSByte(out sbyte result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = (sbyte) 0;
        return false;
      }
      if (!this._root.ReadSubtreeSByte(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableSByte(out sbyte? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new sbyte?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableSByte(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadInt16(out short result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = (short) 0;
        return false;
      }
      if (!this._root.ReadSubtreeInt16(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableInt16(out short? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new short?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableInt16(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadUInt16(out ushort result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = (ushort) 0;
        return false;
      }
      if (!this._root.ReadSubtreeUInt16(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableUInt16(out ushort? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new ushort?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableUInt16(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadInt32(out int result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = 0;
        return false;
      }
      if (!this._root.ReadSubtreeInt32(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableInt32(out int? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new int?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableInt32(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadUInt32(out uint result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = 0U;
        return false;
      }
      if (!this._root.ReadSubtreeUInt32(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableUInt32(out uint? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new uint?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableUInt32(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadInt64(out long result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = 0L;
        return false;
      }
      if (!this._root.ReadSubtreeInt64(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableInt64(out long? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new long?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableInt64(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadUInt64(out ulong result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = 0UL;
        return false;
      }
      if (!this._root.ReadSubtreeUInt64(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableUInt64(out ulong? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new ulong?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableUInt64(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadSingle(out float result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = 0.0f;
        return false;
      }
      if (!this._root.ReadSubtreeSingle(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableSingle(out float? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new float?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableSingle(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadDouble(out double result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = 0.0;
        return false;
      }
      if (!this._root.ReadSubtreeDouble(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadNullableDouble(out double? result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new double?();
        return false;
      }
      if (!this._root.ReadSubtreeNullableDouble(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadArrayLength(out long result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = 0L;
        return false;
      }
      if (!this._root.ReadSubtreeArrayLength(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadMapLength(out long result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = 0L;
        return false;
      }
      if (!this._root.ReadSubtreeMapLength(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadBinary(out byte[] result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = (byte[]) null;
        return false;
      }
      if (!this._root.ReadSubtreeBinary(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadString(out string result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = (string) null;
        return false;
      }
      if (!this._root.ReadSubtreeString(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadMessagePackExtendedTypeObject(out MessagePackExtendedTypeObject result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new MessagePackExtendedTypeObject();
        return false;
      }
      if (!this._root.ReadSubtreeMessagePackExtendedTypeObject(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    public override bool ReadObject(out MessagePackObject result)
    {
      this.DiscardCompletedStacks();
      if (this._itemsCount.Count == 0)
      {
        result = new MessagePackObject();
        return false;
      }
      if (!this._root.ReadSubtreeObject(out result))
        return false;
      switch (this._root.InternalCollectionType)
      {
        case ItemsUnpacker.CollectionType.Array:
          this._itemsCount.Push(this._root.InternalItemsCount);
          this._unpacked.Push(0L);
          this._isMap.Push(false);
          break;
        case ItemsUnpacker.CollectionType.Map:
          this._itemsCount.Push(this._root.InternalItemsCount * 2L);
          this._unpacked.Push(0L);
          this._isMap.Push(true);
          break;
        default:
          this._unpacked.Push(this._unpacked.Pop() + 1L);
          break;
      }
      return true;
    }

    private enum State
    {
      InHead,
      InProgress,
      Disposed,
    }
  }
}
