using CSCache.Controlador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSCache.Cache
{
    public partial class CacheProcessor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // AC - 21/07/2024
            try
            {
                CacheController.InitCache();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}