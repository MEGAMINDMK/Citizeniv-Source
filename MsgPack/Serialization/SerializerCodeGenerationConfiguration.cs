// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializerCodeGenerationConfiguration
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.IO;

namespace MsgPack.Serialization
{
  public sealed class SerializerCodeGenerationConfiguration : ISerializerGeneratorConfiguration
  {
    private const string DefaultNamespace = "MsgPack.Serialization.GeneratedSerializers";
    private const string DefaultLanguage = "C#";
    private const string DefaultIndentString = "    ";
    private string _namespace;
    private string _outputDirectory;
    private string _language;
    private string _indentString;
    private SerializationMethod _serializationMethod;
    private EnumSerializationMethod _enumSerializationMethod;

    public string Namespace
    {
      get
      {
        return this._namespace;
      }
      set
      {
        if (value == null)
        {
          this._namespace = "MsgPack.Serialization.GeneratedSerializers";
        }
        else
        {
          Validation.ValidateNamespace(value, nameof (value));
          this._namespace = value;
        }
      }
    }

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

    public string Language
    {
      get
      {
        return this._language;
      }
      set
      {
        this._language = value ?? "C#";
      }
    }

    public string CodeIndentString
    {
      get
      {
        return this._indentString;
      }
      set
      {
        this._indentString = value ?? "    ";
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

    public bool IsRecursive { get; set; }

    public bool PreferReflectionBasedSerializer { get; set; }

    public bool WithNullableSerializers { get; set; }

    public bool IsInternalToMsgPackLibrary { get; set; }

    public SerializerCodeGenerationConfiguration()
    {
      this.OutputDirectory = (string) null;
      this.Language = (string) null;
      this.Namespace = (string) null;
      this.CodeIndentString = (string) null;
      this._serializationMethod = SerializationMethod.Array;
    }

    void ISerializerGeneratorConfiguration.Validate()
    {
    }
  }
}
