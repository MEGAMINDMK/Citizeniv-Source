// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CodeDomSerializers.ParameterCodeDomConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.CodeDom;

namespace MsgPack.Serialization.CodeDomSerializers
{
  internal class ParameterCodeDomConstruct : CodeDomConstruct
  {
    private readonly CodeTypeReference _type;
    private readonly string _name;

    public CodeParameterDeclarationExpression Declaration
    {
      get
      {
        return new CodeParameterDeclarationExpression(this._type, this._name);
      }
    }

    public CodeArgumentReferenceExpression Reference
    {
      get
      {
        return new CodeArgumentReferenceExpression(this._name);
      }
    }

    public override bool IsArgument
    {
      get
      {
        return true;
      }
    }

    public override CodeParameterDeclarationExpression AsParameter()
    {
      return this.Declaration;
    }

    public override CodeArgumentReferenceExpression AsArgument()
    {
      return this.Reference;
    }

    public override bool IsExpression
    {
      get
      {
        return true;
      }
    }

    public override CodeExpression AsExpression()
    {
      return (CodeExpression) this.Reference;
    }

    public ParameterCodeDomConstruct(Type type, string name)
      : base(type)
    {
      this._type = new CodeTypeReference(type);
      this._name = name;
    }
  }
}
