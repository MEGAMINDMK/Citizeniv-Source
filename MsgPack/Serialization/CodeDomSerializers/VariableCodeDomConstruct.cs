// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CodeDomSerializers.VariableCodeDomConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.CodeDom;
using System.Collections.Generic;

namespace MsgPack.Serialization.CodeDomSerializers
{
  internal class VariableCodeDomConstruct : CodeDomConstruct
  {
    private readonly CodeTypeReference _type;
    private readonly string _name;

    public CodeVariableDeclarationStatement Declaration
    {
      get
      {
        return new CodeVariableDeclarationStatement(this._type, this._name, (CodeExpression) new CodeDefaultValueExpression(this._type));
      }
    }

    public CodeVariableReferenceExpression Reference
    {
      get
      {
        return new CodeVariableReferenceExpression(this._name);
      }
    }

    public override bool IsStatement
    {
      get
      {
        return true;
      }
    }

    public override IEnumerable<CodeStatement> AsStatements()
    {
      yield return (CodeStatement) this.Declaration;
    }

    public override void AddStatements(CodeStatementCollection collection)
    {
      collection.Add((CodeStatement) this.Declaration);
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

    public VariableCodeDomConstruct(Type type, string name)
      : base(type)
    {
      this._type = new CodeTypeReference(type);
      this._name = name;
    }
  }
}
