using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFExtensions
{
	public class DbEnumAttribute: Attribute
	{
		/// <summary>
		/// Full name of the type that should be used as target enum.
		/// </summary>
		public string TargetEnumType { get; set; }
	}
}
