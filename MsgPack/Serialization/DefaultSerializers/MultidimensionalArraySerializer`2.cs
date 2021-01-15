// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.MultidimensionalArraySerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class MultidimensionalArraySerializer<TArray, TItem> : MessagePackSerializer<TArray>
  {
    private readonly MessagePackSerializer<TItem> _itemSerializer;
    private readonly MessagePackSerializer<int[]> _int32ArraySerializer;

    public MultidimensionalArraySerializer(
      SerializationContext ownerContext,
      PolymorphismSchema itemsSchema)
      : base(ownerContext)
    {
      this._itemSerializer = ownerContext.GetSerializer<TItem>((object) itemsSchema);
      this._int32ArraySerializer = ownerContext.GetSerializer<int[]>((object) itemsSchema);
    }

    protected internal override void PackToCore(Packer packer, TArray objectTree)
    {
      this.PackArrayCore(packer, (Array) (object) objectTree);
    }

    private void PackArrayCore(Packer packer, Array array)
    {
      long longLength = array.LongLength;
      if (longLength > (long) int.MaxValue)
        throw new NotSupportedException("Array length over 32bit is not supported.");
      int num = (int) longLength;
      int[] lengths;
      int[] lowerBounds;
      MultidimensionalArraySerializer<TArray, TItem>.GetArrayMetadata(array, out lengths, out lowerBounds);
      packer.PackArrayHeader(2);
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (Packer packer1 = Packer.Create((Stream) memoryStream, false))
        {
          packer1.PackArrayHeader(2);
          this._int32ArraySerializer.PackTo(packer1, lengths);
          this._int32ArraySerializer.PackTo(packer1, lowerBounds);
          packer.PackExtendedTypeValue(this.OwnerContext.ExtTypeCodeMapping[KnownExtTypeName.MultidimensionalArray], memoryStream.ToArray());
        }
      }
      packer.PackArrayHeader(num);
      MultidimensionalArraySerializer<TArray, TItem>.ForEach(array, num, lowerBounds, lengths, (Action<int[]>) (indices => this._itemSerializer.PackTo(packer, (TItem) array.GetValue(indices))));
    }

    private static void GetArrayMetadata(Array array, out int[] lengths, out int[] lowerBounds)
    {
      lowerBounds = new int[array.Rank];
      lengths = new int[array.Rank];
      for (int dimension = 0; dimension < array.Rank; ++dimension)
      {
        lowerBounds[dimension] = array.GetLowerBound(dimension);
        lengths[dimension] = array.GetLength(dimension);
      }
    }

    protected internal override TArray UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      if (UnpackHelpers.GetItemsCount(unpacker) != 2)
        throw new SerializationException("Multidimensional array must be encoded as 2 element array.");
      using (Unpacker unpacker1 = unpacker.ReadSubtree())
      {
        if (!unpacker1.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        MessagePackExtendedTypeObject extendedTypeObject;
        try
        {
          extendedTypeObject = unpacker1.LastReadData.AsMessagePackExtendedTypeObject();
        }
        catch (InvalidOperationException ex)
        {
          throw new SerializationException("Multidimensional array must be encoded as ext type.", (Exception) ex);
        }
        if ((int) extendedTypeObject.TypeCode != (int) this.OwnerContext.ExtTypeCodeMapping[KnownExtTypeName.MultidimensionalArray])
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Multidimensional array must be encoded as ext type 0x{0:X2}.", (object) this.OwnerContext.ExtTypeCodeMapping[KnownExtTypeName.MultidimensionalArray]));
        int[] lengths;
        int[] lowerBounds;
        using (MemoryStream memoryStream = new MemoryStream(extendedTypeObject.Body))
        {
          using (Unpacker unpacker2 = Unpacker.Create((Stream) memoryStream, false))
          {
            if (!unpacker2.Read())
              throw SerializationExceptions.NewUnexpectedEndOfStream();
            if (!unpacker2.IsArrayHeader)
              throw SerializationExceptions.NewIsNotArrayHeader();
            if (UnpackHelpers.GetItemsCount(unpacker2) != 2)
              throw new SerializationException("Multidimensional metadata array must be encoded as 2 element array.");
            this.ReadArrayMetadata(unpacker2, out lengths, out lowerBounds);
          }
        }
        if (!unpacker1.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        if (!unpacker1.IsArrayHeader)
          throw SerializationExceptions.NewIsNotArrayHeader();
        using (Unpacker arrayUnpacker = unpacker1.ReadSubtree())
        {
          Array result = Array.CreateInstance(typeof (TItem), lengths, lowerBounds);
          int itemsCount = UnpackHelpers.GetItemsCount(arrayUnpacker);
          if (itemsCount > 0)
            MultidimensionalArraySerializer<TArray, TItem>.ForEach(result, itemsCount, lowerBounds, lengths, (Action<int[]>) (indices =>
            {
              if (!arrayUnpacker.Read())
                throw SerializationExceptions.NewUnexpectedEndOfStream();
              result.SetValue((object) this._itemSerializer.UnpackFrom(arrayUnpacker), indices);
            }));
          return (TArray) result;
        }
      }
    }

    private void ReadArrayMetadata(
      Unpacker metadataUnpacker,
      out int[] lengths,
      out int[] lowerBounds)
    {
      if (!metadataUnpacker.Read())
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      if (!metadataUnpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      using (Unpacker unpacker = metadataUnpacker.ReadSubtree())
        lengths = this._int32ArraySerializer.UnpackFrom(unpacker);
      if (!metadataUnpacker.Read())
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      if (!metadataUnpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      using (Unpacker unpacker = metadataUnpacker.ReadSubtree())
        lowerBounds = this._int32ArraySerializer.UnpackFrom(unpacker);
    }

    private static void ForEach(
      Array array,
      int totalLength,
      int[] lowerBounds,
      int[] lengths,
      Action<int[]> action)
    {
      int[] numArray = new int[array.Rank];
      for (int index = 0; index < array.Rank; ++index)
        numArray[index] = lowerBounds[index];
      for (int index1 = 0; index1 < totalLength; ++index1)
      {
        action(numArray);
        for (int index2 = numArray.Length - 1; index2 >= 0; --index2)
        {
          if (numArray[index2] + 1 < lengths[index2] + lowerBounds[index2])
          {
            ++numArray[index2];
            break;
          }
          numArray[index2] = lowerBounds[index2];
        }
      }
    }
  }
}
