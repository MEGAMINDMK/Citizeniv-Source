// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CodeDomSerializers.StatementCodeDomConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace MsgPack.Serialization.CodeDomSerializers
{
  internal class StatementCodeDomConstruct : CodeDomConstruct
  {
    private readonly CodeStatement[] _statements;

    public override bool IsStatement
    {
      get
      {
        return true;
      }
    }

    public override IEnumerable<CodeStatement> AsStatements()
    {
      return (IEnumerable<CodeStatement>) this._statements;
    }

    public override void AddStatements(CodeStatementCollection collection)
    {
      collection.AddRange(this._statements);
    }

    public StatementCodeDomConstruct(IEnumerable<CodeStatement> statements)
      : base(typeof (void))
    {
      this._statements = statements.ToArray<CodeStatement>();
    }
  }
}
