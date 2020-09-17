using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TREMEC_EtiquetaInventario_WS.Negocios;
using Telerik.Web.UI;
using System.Configuration;

namespace TREMEC_EtiquetaInventario_WS.Presentacion
{
    public partial class Principal : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["Menu"] == null)
                {
                    Session.Clear();
                    NavMenu.Visible = false;
                }
                if (Session["Menu"] != null)
                {
                    navBarCollapseIndar.Controls.Add(new LiteralControl(Session["Menu"].ToString()));
                    NavMenu.Visible = Session["Menu"] != null; // Si hay menú se muestra, de lo conotrario se oculta
                    if (Session["Tab"].Equals("Salir"))
                    {
                        Session.Clear();
                        Response.Redirect("Login.aspx");
                    }
                

#if DEBUG
                txtDebug.Visible = true;
                System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();
                builder.ConnectionString = ConfigurationManager.ConnectionStrings["BD"].ConnectionString;
                string server = builder["Data Source"] as string;
                string database = builder["Initial Catalog"] as string;

                Label myLabel = this.FindControl("txtBdName") as Label;
                myLabel.Text = ("Debug en: " + server + ". Base de Datos: " + database);
#endif
            }

            Control contentPh = FindControl("ContentSection");
            foreach (Control controlCph in contentPh.Controls)
                if (controlCph is MultiView)
                {
                    foreach (Control controlMv in controlCph.Controls)
                        if (controlMv is View)
                        {
                            foreach (Control controlV in controlMv.Controls)
                                if (controlV is Panel)
                                {
                                    foreach (Control controlP in controlV.Controls)
                                        if (controlP is RadGrid)
                                            ChangeLanguageRadFilterMenu((RadGrid)controlP);
                                }
                                else if (controlV is RadGrid)
                                    ChangeLanguageRadFilterMenu((RadGrid)controlV);
                        }
                        else if (controlMv is RadGrid)
                            ChangeLanguageRadFilterMenu((RadGrid)controlMv);
                }
                else if (controlCph is RadGrid)
                    ChangeLanguageRadFilterMenu((RadGrid)controlCph);
        }

        private void ChangeLanguageRadFilterMenu(RadGrid generalRadGrid)
        {
            //Oculta opciones del menú
            GridFilterMenu menu = generalRadGrid.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "IsEmpty" ||
                    menu.Items[i].Text == "NotIsEmpty" ||
                    menu.Items[i].Text == "IsNull" ||
                    menu.Items[i].Text == "NotIsNull")
                    menu.Items.RemoveAt(i);
                else
                    i++;
            }
            //Traduce tooltip del botón Filtro
            foreach (GridColumn column in generalRadGrid.Columns)
                generalRadGrid.MasterTableView.GetColumn(column.UniqueName).FilterImageToolTip = "Menú filtros";
            //Traduce filtros
            foreach (RadMenuItem item in menu.Items)
            {
                switch (item.Value)
                {
                    case "NoFilter":
                        item.Text = "Sin filtro";
                        break;
                    case "Contains":
                        item.Text = "Contiene";
                        break;
                    case "DoesNotContain":
                        item.Text = "No contiene";
                        break;
                    case "StartsWith":
                        item.Text = "Empieza con";
                        break;
                    case "EndsWith":
                        item.Text = "Termina con";
                        break;
                    case "EqualTo":
                        item.Text = "Es igual a";
                        break;
                    case "NotEqualTo":
                        item.Text = "No es igual a";
                        break;
                    case "GreaterThan":
                        item.Text = "Mayor que";
                        break;
                    case "LessThan":
                        item.Text = "Menor que";
                        break;
                    case "GreaterThanOrEqualTo":
                        item.Text = "Mayor o igual que";
                        break;
                    case "LessThanOrEqualTo":
                        item.Text = "Menor o igual que";
                        break;
                    case "Between":
                        item.Text = "Está entre";
                        break;
                    case "NotBetween":
                        item.Text = "No está entre";
                        break;
                    case "IsEmpty":
                        item.Text = "Es espacio en blanco";
                        item.Remove();
                        break;
                    case "NotIsEmpty":
                        item.Text = "Es es espacio en blanco";
                        item.Remove();
                        break;
                    case "IsNull":
                        item.Text = "Es valor nulo";
                        item.Remove();
                        break;
                    case "NotIsNull":
                        item.Text = "No es valor nulo";
                        item.Remove();
                        break;
                }
            }
            //Traduce toltips accesibles
            generalRadGrid.FilterMenu.ToolTip = "Filtro";
            generalRadGrid.SortingSettings.SortedAscToolTip = "Orden ascendente";
            generalRadGrid.SortingSettings.SortedDescToolTip = "Orden descendente";
            generalRadGrid.SortingSettings.SortToolTip = "Clic aquí para ordenar";
        }

        public void SetAdvancedPagerStyle(GridPagerItem item)
        {
            //Traduce el Pager del RadGrid y asigna tooltips
            Label goToPageLabel = item.FindControl("GoToPageLabel") as Label;
            Label pageOfLabel = item.FindControl("PageOfLabel") as Label;
            RadNumericTextBox changePageSizeTextBox = item.FindControl("ChangePageSizeTextBox") as RadNumericTextBox;
            RadNumericTextBox goToPageTextBox = item.FindControl("GoToPageTextBox") as RadNumericTextBox;
            Button goToPageLinkButton = item.FindControl("GoToPageLinkButton") as Button;
            Button changePageSizeLinkButton = item.FindControl("ChangePageSizeLinkButton") as Button;
            if (goToPageLabel != null)
                goToPageLabel.Text = "Página:";
            if (pageOfLabel != null)
                pageOfLabel.Text = "de " + item.Paging.PageCount.ToString(string.Format("n0"));
            if (changePageSizeTextBox != null)
            {
                changePageSizeTextBox.MaxValue = 1000;
                changePageSizeTextBox.Attributes.Add("data-toggle", "tooltip");
            }
            if (goToPageTextBox != null)
                goToPageTextBox.Attributes.Add("data-toggle", "tooltip");
            if (goToPageLinkButton != null)
            {
                goToPageLinkButton.Text = "Ver";
                goToPageLinkButton.CssClass = changePageSizeLinkButton.CssClass = "btn btn-primary btn-sm";
                goToPageLinkButton.Attributes.Add("data-toggle", "tooltip");
            }
            if (changePageSizeLinkButton != null)
            {
                changePageSizeLinkButton.Text = "Cambiar";
                changePageSizeLinkButton.Attributes.Add("data-toggle", "tooltip");
            }
        }

        public void SetTooltipFilterMenu(GridFilteringItem filterItem, RadGrid radgrid)
        {
            //Traduce el tooltip del botón Calendario en las columnas DateTime
            foreach (GridColumn column in radgrid.Columns)
            {
                if (column is GridDateTimeColumn)
                {
                    RadDatePicker picker = (RadDatePicker)filterItem[column.UniqueName].Controls[0];
                    picker.DatePopupButton.ToolTip = "Abrir calendario";
                    picker.DatePopupButton.Attributes.Add("data-toggle", "tooltip");
                    //Agrega el atributo data-toggle para mostrar el tooltip en los botones de filtro
                    Button btn = (Button)filterItem[column.UniqueName].Controls[2];
                    btn.Attributes.Add("data-toggle", "tooltip");
                }
                else if (column is GridBoundColumn)
                {
                    //Agrega el atributo data-toggle para mostrar el tooltip en los botones de filtro
                    Button btn = (Button)filterItem[column.UniqueName].Controls[1];
                    btn.Attributes.Add("data-toggle", "tooltip");
                }
            }
        }

        public void SetTooltipDataItem(GridDataItem dataItem, RadGrid grid)
        {
            foreach (GridColumn column in grid.MasterTableView.RenderColumns)
                if (column is GridBoundColumn)
                    if (column.Display)
                        dataItem[column.UniqueName].ToolTip = dataItem[column.UniqueName].Text;
            //dataItem[column.UniqueName].Attributes.Add("data-toggle", "tooltip");
        }

        public void SetTooltipGridHeader(GridHeaderItem gridHeader)
        {
            TableRow table = gridHeader as TableRow;
            foreach (TableCell cell in table.Cells)
                foreach (Control itemCtrl in cell.Controls)
                    if (itemCtrl is LinkButton)//Agrega tooltip al encabezado de la columna
                    {
                        LinkButton btn = itemCtrl as LinkButton;
                        btn.Attributes.Add("data-toggle", "tooltip");
                    }
                    else if (itemCtrl is CheckBox)//Agrega tooltip al checkbox seleccionar todo
                    {
                        CheckBox cbx = itemCtrl as CheckBox;
                        cbx.InputAttributes.Add("data-toggle", "tooltip");
                    }
                    else if (itemCtrl is Button)//Agrega tooltip al boton orden asc/desc
                    {
                        Button btn = itemCtrl as Button;
                        btn.Attributes.Add("data-toggle", "tooltip");
                    }
        }

        public void SetTooltips(object sender, GridItemEventArgs e)
        {
            //Traduce y agrega tooltips al pager
            if (e.Item is GridPagerItem)
                SetAdvancedPagerStyle((GridPagerItem)e.Item);
            //Agrega tooltips al encabezado
            else if (e.Item is GridHeaderItem)
                SetTooltipGridHeader((GridHeaderItem)e.Item);
            //Agrega tooltips a los filtros
            else if (e.Item is GridFilteringItem)
                SetTooltipFilterMenu((GridFilteringItem)e.Item, (RadGrid)sender);
            //Asigna un tooltip a los Items del RadGrid
            else if (e.Item is GridDataItem)
                SetTooltipDataItem((GridDataItem)e.Item, (RadGrid)sender);
        }

        public void AlertSuccess(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "", "<script> document.getElementById('btnSuccess').setAttribute('data-whatever', '" + mensaje + "'); document.getElementById('btnSuccess').click(); </script> ", false);
        }

        public void AlertError(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "", "<script> document.getElementById('btnError').setAttribute('data-whatever', '" + mensaje + "'); document.getElementById('btnError').click(); </script> ", false);
        }

        public void AlertQuestion(string mensaje, string btnUniqueId)
        {
            //btnConfirm es el id del botón que se va a presionar si el usuario da clic al botón Aceptar de la alerta de confirmación
            //string href = btnConfirm.Replace("_", "$");
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "",
                "<script> document.getElementById('btnOk').setAttribute('onclick', \"javascript:__doPostBack('" + btnUniqueId + "', '')\"); document.getElementById('btnQuestion').setAttribute('data-whatever', '" + mensaje + "'); document.getElementById('btnQuestion').click(); </script> ", false);
        }
    }
}