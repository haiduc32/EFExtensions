using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using EFExtensions;
using System.Collections;

namespace EFExtension
{
	class ObjectQueryWrapperProvider<T> : ExpressionVisitor, IQueryProvider
	{
		internal IQueryable source;

		public ObjectQueryWrapperProvider(IQueryable source)
		{
			if (source == null) throw new ArgumentNullException("source");
			this.source = source;
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException("expression");

			return new ObjectQueryWrapper<TElement>(source, expression) as IQueryable<TElement>;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			Type elementType = expression.Type.GetGenericArguments().First();
			IQueryable result = (IQueryable)Activator.CreateInstance(typeof(ObjectQueryWrapper<>).MakeGenericType(elementType),
				new object[] { source, expression });
			return result;
		}

		public TResult Execute<TResult>(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			object result = (this as IQueryProvider).Execute(expression);
			return (TResult)result;
		}

		public object Execute(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException("expression");

			Expression translated = ExpressionTranslator.ForExpression(expression);
			translated = this.Visit(translated);
			return source.Provider.Execute(translated);
		}

		internal IEnumerable ExecuteEnumerable(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException("expression");

			Expression translated = ExpressionTranslator.ForExpression(expression);
			translated = this.Visit(translated);
			return source.Provider.CreateQuery(translated);
		}

		#region Visitors
		protected override Expression VisitConstant(ConstantExpression c)
		{
			// remove our wrapper from the Expression Tree.
			if (c.Type.IsGenericType && (
				c.Type.GetGenericTypeDefinition() == typeof(ObjectQueryWrapper<>) ||
				c.Type.GetGenericTypeDefinition() == typeof(ObjectSetWrapper<>)))
			{
				return source.Expression;
			}
			else
			{
				return base.VisitConstant(c);
			}
		}
		#endregion
	}
}
