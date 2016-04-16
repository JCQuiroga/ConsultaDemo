using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace ConsultaDemo.VisualWebPart1
{
    public partial class VisualWebPart1UserControl : UserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            SPQuery query = new SPQuery();
            query.Query = //"";
             //@"<Where><Eq><FieldRef Name=""Estado""></FieldRef><Value Type=""Choice"">Disponible</Value></Eq></Where>";
            @"<Where><Contains><FieldRef Name=""Title""></FieldRef><Value Type=""Text"">Samsung</Value></Contains></Where>";

            query.ViewFields = @"
                                <FieldRef Name=""ID"" />
                                <FieldRef Name=""Title"" />
                                <FieldRef Name=""Precio"" />
                                <FieldRef Name=""Estado"" />
                                ";
            var web = SPContext.Current.Web;
            var list = web.Lists["Stock"];
            var items = list.GetItems(query);
            lstExpenses.DataSource = items.GetDataTable();

            lstExpenses.DataBind();
        }

        private static bool IsChecked(ListViewDataItem item)
        {
            var checkBox = item.FindControl("chkUpdate") as CheckBox;
            return checkBox.Checked;
        }

        private void UpdateItems(bool b)
        {
            string status = b ? "Agotado" : "No Disponible";
            var selectedItems = from item in lstExpenses.Items
                                where IsChecked(item)
                                select item;
            var web = SPContext.Current.Web;
            var list = web.Lists["Stock"];

            foreach (var selectedItem in selectedItems)

            {

                var hiddenField = selectedItem.FindControl("hdCodigo") as HiddenField;
                int itemID;
                if (int.TryParse(hiddenField.Value, out itemID))
                {
                    SPListItem item = list.GetItemById(itemID);
                    item["Estado"] = status;
                    item.Update();
                }
            }
        }

        protected void btnAgotado_Click(object sender, EventArgs e)
        {
            UpdateItems(true);
        }

        protected void btnNoDisponible_Click(object sender, EventArgs e)
        {
            UpdateItems(false);
        }
    }
}
