﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
/*
* CarrotCake CMS
* http://carrotware.com/
*
* Copyright 2011, Samantha Copeland
* Dual licensed under the MIT or GPL Version 2 licenses.
*
* Date: October 2011
*/

namespace Carrotware.Web.UI.Controls {

	[DefaultProperty("Text"), ToolboxData("<{0}:CarrotGridView runat=server></{0}:CarrotGridView>")]

	public class CarrotGridView : GridView {

		public CarrotGridView()
			: base() {

			SortDownIndicator = "&nbsp;&#x25BC;";
			SortUpIndicator = "&nbsp;&#x25B2;";
		}


		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string DefaultSort {
			get {
				String s = ViewState["DefaultSort"] as String;
				return ((s == null) ? String.Empty : s);
			}
			set {
				ViewState["DefaultSort"] = value;
			}
		}


		private string SortField {
			get;
			set;
		}

		private string SortDir {
			get;
			set;
		}

		private string SortUpIndicator {
			get;
			set;
		}

		private string SortDownIndicator {
			get;
			set;
		}


		public void lblSort_Command(object sender, EventArgs e) {
			SortParm = DefaultSort;
			LinkButton lb = (LinkButton)sender;
			string sSortField = "";
			try { sSortField = lb.CommandName.ToString(); } catch { }
			sSortField = ResetSortToColumn(sSortField);
			DefaultSort = sSortField;

			base.DataBind();
		}


		public void SetHeaderClick(Control TheControl, EventHandler CmdFunc) {
			//add the command click event to the link buttons on the datagrid heading

			foreach (Control c in TheControl.Controls) {
				if (c is LinkButton) {
					LinkButton lb = (LinkButton)c;
					lb.Click += new EventHandler(CmdFunc);
				} else {
					SetHeaderClick(c, CmdFunc);
				}
			}
		}


		private void SetData() {
			int iCol = 0;
			foreach (DataControlField c in this.Columns) {
				if (c is CarrotHeaderSortTemplateField) {
					CarrotHeaderSortTemplateField ctf = (CarrotHeaderSortTemplateField)c;
					ctf.HeaderTemplate = new CarrotLinkButtonCmdTemplate("lnkHead", iCol, ctf.HeaderText, ctf.SortExpression);
					iCol++;
				}
			}

			if (!string.IsNullOrEmpty(DefaultSort)) {
				Type theType = DataSource.GetType();

				if (theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(List<>)) {
					IList lst = (IList)DataSource;
					SortParm = DefaultSort;
					var lstVals = SortDataListType(lst);

					DataSource = lstVals;
				}
			}
		}



		public IList SortDataListType(IList lst) {

			IList query = null;
			List<object> d = lst.Cast<object>().ToList();
			IEnumerable<object> enuQueryable = d.AsQueryable();
			if (lst != null) {
				if (lst.Count > 0) {
					SortField = GetProperties(d[0]).Where(x => x.ToLower() == SortField.ToLower()).FirstOrDefault();
				} else {
					SortField = string.Empty;
				}
			} else {
				SortField = string.Empty;
			}

			if (!string.IsNullOrEmpty(SortField)) {
				if (SortDir.ToUpper().Trim().IndexOf("ASC") < 0) {
					query = (from enu in enuQueryable
							 orderby GetPropertyValue(enu, SortField) descending
							 select enu).ToList();
				} else {
					query = (from enu in enuQueryable
							 orderby GetPropertyValue(enu, SortField) ascending
							 select enu).ToList();
				}
			} else {
				query = (from enu in enuQueryable
						 select enu).ToList();
			}

			return query;
		}

		private IList SortDataListType(IList lst, string sSort) {
			ResetSortToColumn(sSort);
			return SortDataListType(lst);
		}


		protected override void Render(HtmlTextWriter writer) {

			if (this.Rows.Count > 0) {
				WalkGridForHeadings(this.HeaderRow);
			}

			base.Render(writer);
		}


		protected override void PerformDataBinding(IEnumerable data) {

			SetData();

			Type theType = DataSource.GetType();
			if (theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(List<>)) {
				data = (IList)DataSource;
			}

			base.PerformDataBinding(data);

			if (string.IsNullOrEmpty(SortField) || string.IsNullOrEmpty(SortDir)) {
				SortParm = string.Empty;
			}

			if (this.Rows.Count > 0) {
				WalkGridSetClick(this.HeaderRow);
			}
		}

		private string SortParm {
			get {
				string sSort = "";
				try {
					sSort = SortField + "   " + SortDir;
				} catch {
					sSort = DefaultSort;
				}
				return sSort.Trim();
			}
			set {
				string sSort = DefaultSort;
				if (!string.IsNullOrEmpty(value)) {
					sSort = value;
				}
				string sSortFld = string.Empty;
				string sSortDir = string.Empty;

				if (!string.IsNullOrEmpty(sSort)) {
					int pos = sSort.LastIndexOf(" ");

					sSortFld = sSort.Substring(0, pos).Trim();
					sSortDir = sSort.Substring(pos).Trim();
				}

				SortField = sSortFld.Trim();
				SortDir = sSortDir.Trim().ToUpper();
			}
		}

		private void WalkGridSetClick(Control X) {
			foreach (Control c in X.Controls) {
				if (c is LinkButton) {
					LinkButton lb = (LinkButton)c;
					lb.Click += new EventHandler(this.lblSort_Command);
				} else {
					WalkGridSetClick(c);
				}
			}
		}


		private string ResetSortToColumn(string sSortField) {

			if (SortField.Length < 1) {
				SortField = sSortField;
				SortDir = string.Empty;
			} else {
				if (SortField.ToLower() != sSortField.ToLower()) {
					SortDir = string.Empty;  //previous sort not the same field, force ASC
				}
				SortField = sSortField;
			}

			if (SortDir.Trim().ToUpper().IndexOf("ASC") < 0) {
				SortDir = "ASC";
			} else {
				SortDir = "DESC";
			}
			sSortField = SortField + "   " + SortDir;
			return sSortField;
		}



		private object GetPropertyValue(object obj, string property) {
			PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
			return propertyInfo.GetValue(obj, null);
		}

		private bool TestIfPropExists(object obj, string property) {
			PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
			return propertyInfo == null ? false : true;
		}

		public List<string> GetProperties(object obj) {
			PropertyInfo[] info = obj.GetType().GetProperties();

			List<string> props = (from i in info.AsEnumerable()
								  select i.Name).ToList();
			return props;
		}


		private void WalkGridForHeadings(Control X) {

			if (string.IsNullOrEmpty(SortField) || string.IsNullOrEmpty(SortDir)) {
				SortParm = string.Empty;
			}

			WalkGridForHeadings(X, SortField, SortDir);
		}


		private void WalkGridForHeadings(Control X, string sSortFld, string sSortDir) {

			sSortFld = sSortFld.ToLower();
			sSortDir = sSortDir.ToLower();

			foreach (Control c in X.Controls) {
				if (c is LinkButton) {
					LinkButton lb = (LinkButton)c;
					if (sSortFld == lb.CommandName.ToLower()) {
						//don't add the arrows if alread sorted!
						if (lb.Text.IndexOf("#x25B") < 0) {
							if (sSortDir != "asc") {
								lb.Text += SortDownIndicator;
							} else {
								lb.Text += SortUpIndicator;
							}
						}
						break;
					}
				} else {
					WalkGridForHeadings(c, sSortFld, sSortDir);
				}
			}
		}


	}
}