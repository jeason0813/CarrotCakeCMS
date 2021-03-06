﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarDisplay.ascx.cs" Inherits="Carrotware.CMS.UI.Plugins.CalendarModule.CalendarDisplay" %>
<div style="clear: both;">
</div>
<div>
	<center>
	<div><carrot:Calendar runat="server" ID="Calendar1" />
	<br />
	</div>
	<div>
	<asp:Button CssClass="calendarbutton" ID="btnLast" runat="server" Text="«««««" OnClick="btnLast_Click" />
	&nbsp;&nbsp;&nbsp;
	<asp:Button CssClass="calendarbutton" ID="btnNext" runat="server" Text="»»»»»" OnClick="btnNext_Click" />
	</div>
	<div style="width: 500px; padding: 25px;">
		<asp:DataGrid ID="dgEvents" runat="server" CellPadding="2" ShowHeader="False" GridLines="None" AutoGenerateColumns="False">
			<ItemStyle></ItemStyle>
			<Columns>
				<asp:TemplateColumn HeaderText="EventDate">
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Top"></ItemStyle>
					<ItemTemplate>
						<asp:Literal ID="litDate" runat="server" Text='<%# String.Format( "{0:d}", DataBinder.Eval(Container, "DataItem.EventDate") ) %>' />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn HeaderText="EventDate">
					<ItemTemplate>
						&nbsp;&nbsp;
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn HeaderText="EventDate">
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Top"></ItemStyle>
					<ItemTemplate>
						<b>
							<asp:Literal ID="litEvent" runat="server" Text='<%# String.Format( "{0}", DataBinder.Eval(Container, "DataItem.EventTitle") ) %>' />
							 </b><br />
						<asp:Literal ID="litDetail" runat="server" Text='<%# String.Format( "{0}<br /><br />", DataBinder.Eval(Container, "DataItem.EventDetail") ) %>' />
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>
	</div>
</center>
</div>
<div style="clear: both;">
</div>
<script type="text/javascript">
	function eventCalendarDateLaunch(val) {
		window.open("<%=LaunchURLWindow %>?calendardate=" + val);
	}
</script>