﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFExtensions
{
	public class EnumValueAttribute : Attribute
	{
		public string Value { get; set; }
	}
}
