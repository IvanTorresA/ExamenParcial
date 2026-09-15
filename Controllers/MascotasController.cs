using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using PawCareMVC.Models;

namespace PawCareMVC.Controllers
{
    public class MascotasController : Controller
    {
        private readonly string _connectionString;

        public MascotasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PawCareDB")!;
        }

       public IActionResult Index()
        {
            var mascotas = ListarMascotas();
            return View(mascotas);
        }

      [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Mascota mascota)
        {
            if (!ModelState.IsValid)
            {
                var mascotasInvalidas = ListarMascotas();
                ViewBag.MascotaConError = mascota;
                ViewBag.Error = "Revisa los campos marcados en rojo.";
                return View("Index", mascotasInvalidas);
            }

            try
            {
                InsertarMascota(mascota);
                TempData["Mensaje"] = "Mascota registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var mascotasConError = ListarMascotas();
                ViewBag.MascotaConError = mascota;
                ViewBag.Error = "Error al guardar: " + ex.Message;
                return View("Index", mascotasConError);
            }
        }
        private List<Mascota> ListarMascotas()
        {
            var lista = new List<Mascota>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("spListarMascotas", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Mascota
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            NombreMascota = reader.GetString(reader.GetOrdinal("NombreMascota")),
                            NombreDueno = reader.GetString(reader.GetOrdinal("NombreDueno")),
                            Tipo = reader.GetString(reader.GetOrdinal("Tipo")),
                            Edad = reader.GetInt32(reader.GetOrdinal("Edad")),
                            Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                            Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Observaciones"))
                        });
                    }
                }
            }

            return lista;
        }

       private void InsertarMascota(Mascota mascota)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("spInsertarMascota", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NombreMascota", SqlDbType.VarChar, 100) { Value = mascota.NombreMascota });
                cmd.Parameters.Add(new SqlParameter("@NombreDueno", SqlDbType.VarChar, 100) { Value = mascota.NombreDueno });
                cmd.Parameters.Add(new SqlParameter("@Tipo", SqlDbType.VarChar, 20) { Value = mascota.Tipo });
                cmd.Parameters.Add(new SqlParameter("@Edad", SqlDbType.Int) { Value = mascota.Edad });
                cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 9) { Value = mascota.Telefono });

                object observaciones = string.IsNullOrWhiteSpace(mascota.Observaciones)
                    ? DBNull.Value
                    : mascota.Observaciones;
                cmd.Parameters.Add(new SqlParameter("@Observaciones", SqlDbType.VarChar, 500) { Value = observaciones });

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}