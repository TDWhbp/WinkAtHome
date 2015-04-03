﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Groups.ascx.cs" Inherits="WinkAtHome.Controls.Groups" %>

<asp:Table ID="Table1" runat="server" BorderColor="LightGray" BorderWidth="1" BorderStyle="Ridge" Width="100%">
    <asp:TableHeaderRow>
        <asp:TableHeaderCell BackColor="#22b9ec" HorizontalAlign="Left" style="padding:10px;">
            <asp:Label ID="lblHeader" runat="server" Text="Groups" ForeColor="White" />
        </asp:TableHeaderCell>
    </asp:TableHeaderRow>
    <asp:TableRow>
        <asp:TableCell style="padding:10px;">
            <asp:DataList ID="dlGroups" runat="server" RepeatColumns="8" RepeatDirection="Horizontal" Width="100%">
                <ItemStyle Width="100" />
                <ItemTemplate>
                    <table>
                        <tr>
                            <td align="center">
                                <asp:ImageButton ID="imgIcon" runat="server" ImageUrl="~/Images/Lights/false.png" Height="75" OnClick="imgIcon_Click" 
                                    CommandArgument='<%# ((Wink.Shortcut)Container.DataItem).id %>' ToolTip='<%# ((Wink.Shortcut)Container.DataItem).name %>' />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" nowrap="true">
                                <asp:Label ID="lblName" runat="server" Text='<%# ((Wink.Shortcut)Container.DataItem).name %>' Font-Size="small" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:DataList>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>