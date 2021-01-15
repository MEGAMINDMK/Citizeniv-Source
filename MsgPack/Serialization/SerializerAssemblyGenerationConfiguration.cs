// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializerAssemblyGenerationConfiguration
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace MsgPack.Serialization
{
  public sealed class SerializerAssemblyGenerationConfiguration : ISerializerGeneratorConfiguration
  {
    private string _outputDirectory;
    private SerializationMethod _serializationMethod;
    private EnumSerializationMethod _enumSerializationMethod;

    public string OutputDirectory
    {
      get
      {
        return this._outputDirectory;
      }
      set
      {
        this._outputDirectory = Path.GetFullPath(value ?? ".");
      }
    }

    public SerializationMethod SerializationMethod
    {
      get
      {
        return this._serializationMethod;
      }
      set
      {
        switch (value)
        {
          case SerializationMethod.Array:
          case SerializationMethod.Map:
            this._serializationMethod = value;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (value));
        }
      }
    }

    public EnumSerializationMethod EnumSerializationMethod
    {
      get
      {
        return this._enumSerializationMethod;
      }
      set
      {
        switch (value)
        {
          case EnumSerializationMethod.ByName:
          case EnumSerializationMethod.ByUnderlyingValue:
            this._enumSerializationMethod = value;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (value));
        }
      }
    }

    public AssemblyName AssemblyName { get; set; }

    public bool IsRecursive { get; set; }

    public bool PreferReflectionBasedSerializer { get; set; }

    public bool WithNullableSerializers { get; set; }

    public SerializerAssemblyGenerationConfiguration()
    {
      this.OutputDirectory = (string) null;
      this._serializationMethod = SerializationMethod.Array;
    }

    void ISerializerGeneratorConfiguration.Validate()
    {
      try
      {
        Path.GetFullPath("." + (object) Path.DirectorySeparatorChar + this.AssemblyName.Name);
      }
      catch (ArgumentException ex)
      {
        throw SerializerAssemblyGenerationConfiguration.CreateValidationError((Exception) ex);
      }
      catch (NotSupportedException ex)
      {
        throw SerializerAssemblyGenerationConfiguration.CreateValidationError((Exception) ex);
      }
    }

    private static Exception CreateValidationError(Exception innerException)
    {
      return (Exception) new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "AssemblyName property is not set correctly. Detail: {0}", (object) innerException.Message), innerException);
    }
  }
}
