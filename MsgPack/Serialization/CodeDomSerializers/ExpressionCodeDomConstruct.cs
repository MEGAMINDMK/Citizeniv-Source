// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CodeDomSerializers.ExpressionCodeDomConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.CodeDom;
using System.Collections.Generic;

namespace MsgPack.Serialization.CodeDomSerializers
{
  internal class ExpressionCodeDomConstruct : CodeDomConstruct
  {
    private readonly CodeExpression _dom;

    public override bool IsExpression
    {
      get
      {
        return true;
      }
    }

    public override bool IsStatement
    {
      get
      {
        return true;
      }
    }

    public override CodeExpression AsExpression()
    {
      return this._dom;
    }

    public override IEnumerable<CodeStatement> AsStatements()
    {
      yield return (CodeStatement) new CodeExpressionStatement(this._dom);
    }

    public override void AddStatements(CodeStatementCollection collection)
    {
      collection.Add((CodeStatement) new CodeExpressionStatement(this._dom));
    }

    public ExpressionCodeDomConstruct(Type contextType, CodeExpression dom)
      : base(contextType)
    {
      this._dom = dom;
    }
  }
}
