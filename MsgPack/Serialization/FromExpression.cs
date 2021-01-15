// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.FromExpression
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization
{
  internal static class FromExpression
  {
    public static PropertyInfo ToProperty<TSource, T>(
      Expression<Func<TSource, T>> source)
    {
      return FromExpression.ToPropertyCore<Func<TSource, T>>(source);
    }

    public static PropertyInfo ToProperty<T>(Expression<Func<T>> source)
    {
      return FromExpression.ToPropertyCore<Func<T>>(source);
    }

    private static PropertyInfo ToPropertyCore<T>(Expression<T> source)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (!(source.Body is MemberExpression body))
        FromExpression.ThrowNotValidExpressionTypeException((Expression) source);
      PropertyInfo member = body.Member as PropertyInfo;
      if (member == (PropertyInfo) null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Member '{0}' is not property.", (object) body.Member), nameof (source));
      return member;
    }

    private static MethodInfo ToMethodCore<T>(Expression<T> source)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (!(source.Body is MethodCallExpression body))
        FromExpression.ThrowNotValidExpressionTypeException((Expression) source);
      return body.Method;
    }

    public static MethodInfo ToOperator<T1, T2, TResult>(
      Expression<Func<T1, T2, TResult>> source)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (!(source.Body is BinaryExpression body))
        FromExpression.ThrowNotValidExpressionTypeException((Expression) source);
      return body.Method;
    }

    private static void ThrowNotValidExpressionTypeException(Expression source)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Specified expression '{0}' is too complex. Simple member reference expression is only supported. ", (object) source));
    }

    public static MethodInfo ToMethod<T>(Expression<Action<T>> source)
    {
      return FromExpression.ToMethodCore<Action<T>>(source);
    }

    public static MethodInfo ToMethod(Expression<Action> source)
    {
      return FromExpression.ToMethodCore<Action>(source);
    }

    public static MethodInfo ToMethod<T1, T2>(Expression<Action<T1, T2>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3>(Expression<Action<T1, T2, T3>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4>(
      Expression<Action<T1, T2, T3, T4>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5>(
      Expression<Action<T1, T2, T3, T4, T5>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6>(
      Expression<Action<T1, T2, T3, T4, T5, T6>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7, T8>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
      Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> source)
    {
      return FromExpression.ToMethodCore<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>(source);
    }

    public static MethodInfo ToMethod<TResult>(Expression<Func<TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<TResult>>(source);
    }

    public static MethodInfo ToMethod<T, TResult>(Expression<Func<T, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, TResult>(
      Expression<Func<T1, T2, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, TResult>(
      Expression<Func<T1, T2, T3, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, TResult>(
      Expression<Func<T1, T2, T3, T4, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>(source);
    }

    public static MethodInfo ToMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
      Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> source)
    {
      return FromExpression.ToMethodCore<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>(source);
    }
  }
}
