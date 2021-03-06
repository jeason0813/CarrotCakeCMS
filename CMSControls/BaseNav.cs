﻿using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Carrotware.CMS.Core;

/*
* CarrotCake CMS
* http://www.carrotware.com/
*
* Copyright 2011, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: October 2011
*/

namespace Carrotware.CMS.UI.Controls {

	public abstract class BaseNav : BaseNavCommon {

		protected override void RenderContents(HtmlTextWriter output) {
			LoadAndTweakData();

			int indent = output.Indent;

			List<SiteNav> lstNav = this.NavigationData;

			output.Indent = indent + 3;
			output.WriteLine();

			WriteListPrefix(output);

			if (lstNav != null && lstNav.Any()) {
				output.Indent++;

				foreach (SiteNav c in lstNav) {
					if (c.NavOrder >= 0) {
						output.WriteLine("<li class=\"child-nav\"><a href=\"" + c.FileName + "\">" + c.NavMenuText + "</a></li> ");
					} else {
						output.WriteLine("<li class=\"parent-nav\"><a href=\"" + c.FileName + "\">" + c.NavMenuText + "</a></li> ");
					}
				}
				output.Indent--;
			} else {
#if DEBUG
				output.WriteLine("<span style=\"display: none;\" id=\"" + this.HtmlClientID + "\"></span>");
#endif
			}

			WriteListSuffix(output);

			output.Indent = indent;
		}
	}
}