﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Components.Web.Bootstrap;

public enum OffcanvasResponsiveBreakpoint
{
	/// <summary>
	/// Offcanvas disabled, always render contents inside the viewport.
	/// </summary>
	None = 0,

	/// <summary>
	/// Render contents outside the viewport in an offcanvas when below the "small" breakpoint (<c>576px</c>, exclusive).
	/// </summary>
	Small = 1,

	/// <summary>
	/// Render contents outside the viewport in an offcanvas when below the "medium" breakpoint (<c>768px</c>, exclusive).
	/// </summary>
	Medium = 3,

	/// <summary>
	/// Render contents outside the viewport in an offcanvas when below the "large" breakpoint (<c>992px</c>, exclusive).
	/// </summary>
	Large = 4,

	/// <summary>
	/// Render contents outside the viewport in an offcanvas when bellow "extra-large" breakpoint (<c>1200px</c>, exclusive).
	/// </summary>
	ExtraLarge = 5,

	/// <summary>
	/// Render contents outside the viewport in an offcanvas when bellow "XXL" breakpoint (<c>1400px</c>, exclusive).
	/// </summary>
	Xxl = 6,
}
