
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace ExamenFinal.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly string conexion =
            "Data Source=LAPTOP-VN12RC0S\\SQLEXPRESS;Initial Catalog=BDNorthwind;Integrated Security=True";

        public ActionResult Index(int? anio)
        {
            if (anio == null)
                anio = 1997;

            DataTable tabla = new DataTable();

            using (SqlConnection cn = new SqlConnection(conexion))
            using (SqlCommand cmd = new SqlCommand(
                "dbo.usp_pedidos_por_empleado", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ANIO", SqlDbType.Int).Value = anio.Value;

                cn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(tabla);
                }
            }

            ViewBag.Anio = anio;
            return View(tabla);
        }
    }
}
