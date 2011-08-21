using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Objects;
using System.Collections;
using System.Diagnostics;
using System.Data.Metadata.Edm;
using EFExtension;

namespace EFExtensions
{
	public class ObjectQueryWrapper<T> : IOrderedQueryable<T>
	{
		private Expression expression;
		private ObjectQueryWrapperProvider<T> provider;

		public ObjectQueryWrapper(IQueryable source)
		{
			expression = Expression.Constant(this);
			provider = new ObjectQueryWrapperProvider<T>(source);
		}

		public ObjectQueryWrapper(IQueryable source, Expression e)
		{
			if (e == null) throw new ArgumentNullException("e");
			expression = e;
			provider = new ObjectQueryWrapperProvider<T>(source);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)provider.ExecuteEnumerable(this.expression)).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return provider.ExecuteEnumerable(this.expression).GetEnumerator();
		}

		public ObjectQueryWrapper<T> Include(String path)
		{
			ObjectQuery<T> possibleObjectQuery = provider.source as ObjectQuery<T>;
			if (possibleObjectQuery != null)
			{
				return new ObjectQueryWrapper<T>(possibleObjectQuery.Include(path));
			}
			else
			{
				throw new InvalidOperationException("The Include should only happen at the beginning of a LINQ expression");
			}
		}

		public Type ElementType
		{
			get { return typeof(T); }
		}

		public Expression Expression
		{
			get { return expression; }
		}

		public IQueryProvider Provider
		{
			get { return provider; }
		}
	}
}
