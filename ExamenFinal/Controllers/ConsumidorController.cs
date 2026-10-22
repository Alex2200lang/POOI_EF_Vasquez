
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ExamenFinal.Models;

namespace ExamenFinal.Controllers
{
    public class ConsumidorController : Controller
    {
        private readonly string conexion =
            ConfigurationManager.ConnectionStrings["ExamenFinal"].ConnectionString;

        public ActionResult Index()
        {
            List<Consumidor> lista = new List<Consumidor>();

            using (SqlConnection cn = new SqlConnection(conexion))
            using (SqlCommand cmd = new SqlCommand("usp_listar_consumidor", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Consumidor
                        {
                            dnicont = dr["dnicont"].ToString(),
                            nomcont = dr["nomcont"].ToString(),
                            apecont = dr["apecont"].ToString(),
                            emailcont = dr["emailcont"] == DBNull.Value
                                ? "" : dr["emailcont"].ToString(),
                            idpais = dr["idpais"] == DBNull.Value
                                ? "" : dr["idpais"].ToString()
                        });
                    }
                }
            }

            return View(lista);
        }

        public ActionResult Create()
        {
            return View(new Consumidor());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Consumidor consumidor)
        {
            if (!ModelState.IsValid)
                return View(consumidor);

            try
            {
                using (SqlConnection cn = new SqlConnection(conexion))
                using (SqlCommand cmd = new SqlCommand("usp_agrega_consumidor", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@dnicont", SqlDbType.Char, 8).Value =
                        consumidor.dnicont;
                    cmd.Parameters.Add("@nomcont", SqlDbType.VarChar, 40).Value =
                        consumidor.nomcont;
                    cmd.Parameters.Add("@apecont", SqlDbType.VarChar, 40).Value =
                        consumidor.apecont;
                    cmd.Parameters.Add("@emailcont", SqlDbType.VarChar, 250).Value =
                        string.IsNullOrWhiteSpace(consumidor.emailcont)
                        ? (object)DBNull.Value : consumidor.emailcont;
                    cmd.Parameters.Add("@idpais", SqlDbType.Char, 3).Value =
                        string.IsNullOrWhiteSpace(consumidor.idpais)
                        ? (object)DBNull.Value : consumidor.idpais;

                    SqlParameter mensaje = cmd.Parameters.Add(
                        "@mensaje", SqlDbType.VarChar, 200);
                    mensaje.Direction = ParameterDirection.Output;

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    TempData["Mensaje"] = mensaje.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Update(string id)
        {
            Consumidor consumidor = BuscarConsumidor(id);

            if (consumidor == null)
            {
                TempData["Mensaje"] = "Consumidor no encontrado.";
                return RedirectToAction("Index");
            }

            return View(consumidor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Consumidor consumidor)
        {
            if (!ModelState.IsValid)
                return View(consumidor);

            try
            {
                using (SqlConnection cn = new SqlConnection(conexion))
                using (SqlCommand cmd = new SqlCommand(
                    @"UPDATE tb_consumidor
                      SET nomcont = @nomcont,
                          apecont = @apecont,
                          emailcont = @emailcont,
                          idpais = @idpais
                      WHERE dnicont = @dnicont", cn))
                {
                    cmd.Parameters.Add("@dnicont", SqlDbType.Char, 8).Value =
                        consumidor.dnicont;
                    cmd.Parameters.Add("@nomcont", SqlDbType.VarChar, 40).Value =
                        consumidor.nomcont;
                    cmd.Parameters.Add("@apecont", SqlDbType.VarChar, 40).Value =
                        consumidor.apecont;
                    cmd.Parameters.Add("@emailcont", SqlDbType.VarChar, 250).Value =
                        string.IsNullOrWhiteSpace(consumidor.emailcont)
                        ? (object)DBNull.Value : consumidor.emailcont;
                    cmd.Parameters.Add("@idpais", SqlDbType.Char, 3).Value =
                        string.IsNullOrWhiteSpace(consumidor.idpais)
                        ? (object)DBNull.Value : consumidor.idpais;

                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();

                    TempData["Mensaje"] = filas > 0
                        ? "Consumidor actualizado correctamente."
                        : "No se encontró el consumidor.";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Details(string id)
        {
            Consumidor consumidor = BuscarConsumidor(id);

            if (consumidor == null)
            {
                TempData["Mensaje"] = "Consumidor no encontrado.";
                return RedirectToAction("Index");
            }

            return View(consumidor);
        }

        public ActionResult Delete(string id)
        {
            Consumidor consumidor = BuscarConsumidor(id);

            if (consumidor == null)
            {
                TempData["Mensaje"] = "Consumidor no encontrado.";
                return RedirectToAction("Index");
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(conexion))
                using (SqlCommand cmd = new SqlCommand(
                    "usp_eliminar_consumidor", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@dnicont", SqlDbType.Char, 8).Value = id;

                    SqlParameter mensaje = cmd.Parameters.Add(
                        "@mensaje", SqlDbType.VarChar, 200);
                    mensaje.Direction = ParameterDirection.Output;

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    TempData["Mensaje"] = mensaje.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        private Consumidor BuscarConsumidor(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            using (SqlConnection cn = new SqlConnection(conexion))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT dnicont, nomcont, apecont, emailcont, idpais
                  FROM tb_consumidor
                  WHERE dnicont = @dnicont", cn))
            {
                cmd.Parameters.Add("@dnicont", SqlDbType.Char, 8).Value = id;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return null;

                    return new Consumidor
                    {
                        dnicont = dr["dnicont"].ToString(),
                        nomcont = dr["nomcont"].ToString(),
                        apecont = dr["apecont"].ToString(),
                        emailcont = dr["emailcont"] == DBNull.Value
                            ? "" : dr["emailcont"].ToString(),
                        idpais = dr["idpais"] == DBNull.Value
                            ? "" : dr["idpais"].ToString()
                    };
                }
            }
        }
    }
}
