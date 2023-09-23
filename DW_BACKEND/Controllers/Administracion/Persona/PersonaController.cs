using dw_backend.Functions;
using dw_backend.Helpers;
using dw_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace dw_backend.Controllers.Administracion.Persona
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly string _connectionString;
        private readonly string _nameProcedure;
        private string _pathImagePersona;
        private string _root;
        private readonly string _replaceFile;
        public PersonaController(IConfiguration _configuration)
        {
            Configuration = _configuration;
            _connectionString = Configuration.GetConnectionString("MainConnection");
            _nameProcedure = "adm.crudPersona";
            _pathImagePersona = Configuration.GetValue<string>("Files:pathImageFotoPersona");
            _root = Configuration.GetValue<string>("Root");
            _replaceFile = Configuration.GetValue<string>("ReplaceFiles:pathProfile");
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("store")]
        public IActionResult Store([FromForm] PersonaModel request)
        {
            dynamic resultado;
            Responses result;

            var file = request.foto;
            string imageName = file.FileName;
            string extension = Path.GetExtension(imageName);
            long sizeImage = file.Length;

            imageName = CleanText.RemoveSpaces(imageName);
            imageName = CleanText.RemoveAccents(imageName);

            Regex reg = new Regex(@"^.*\.(pdf) || \.(pdf)$ || \.(jpg|png|jpeg)$");

            if (!reg.IsMatch(extension))
            {
                resultado = new JObject();
                resultado.message = "El formato del archivo no es soportado.";
                resultado.response = 2;
                resultado.value = 2;
                return BadRequest(resultado);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1001, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    string thisTime = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                    string nameUnique = Guid.NewGuid().ToString();
                    string newNameDB = thisTime + "-" + nameUnique + "-" + imageName;

                    using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlTransaction transaction; //Hacemos esto para poder deshacer la transacción, en caso de que algo no se haya guardado
                        transaction = conn.BeginTransaction("imagenTransaction");
                        cmd.Transaction = transaction;

                        string fotoDB = Path.Combine(_pathImagePersona, newNameDB);
                        string fotoServ = Path.Combine(_root, fotoDB);

                        cmd.Parameters.AddWithValue("@opcion", 1);
                        cmd.Parameters.AddWithValue("@nombre1", request.nombre1);
                        cmd.Parameters.AddWithValue("@nombre2", request.nombre2);
                        cmd.Parameters.AddWithValue("@nombre3", request.nombre3);
                        cmd.Parameters.AddWithValue("@nombre4", request.nombre4);
                        cmd.Parameters.AddWithValue("@nombre5", request.nombre5);
                        cmd.Parameters.AddWithValue("@nombre6", request.nombre6);
                        cmd.Parameters.AddWithValue("@apellido1", request.apellido1);
                        cmd.Parameters.AddWithValue("@apellido2", request.apellido2);
                        cmd.Parameters.AddWithValue("@apellido3", request.apellido3);
                        cmd.Parameters.AddWithValue("@dpi", request.dpi);                        
                        cmd.Parameters.AddWithValue("@genero", request.genero);                        
                        cmd.Parameters.AddWithValue("@email", request.email);                        
                        cmd.Parameters.AddWithValue("@telefono", request.telefono);
                        cmd.Parameters.AddWithValue("@tipoPersona", request.tipoPersona);                                          
                        cmd.Parameters.AddWithValue("@nit", request.nit);                        
                        cmd.Parameters.AddWithValue("@foto", fotoDB);
                        cmd.Parameters.AddWithValue("@extension", extension);
                        cmd.Parameters.AddWithValue("@usuario", request.usuario);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet setter = new DataSet();

                        try
                        {
                            adapter.Fill(setter, "tabla");

                            if (setter.Tables["tabla"] == null)
                            {
                                resultado = new JObject();
                                resultado.message = "No se ha podido almacenar el registro.";
                                resultado.response = 0;
                                resultado.value = 0;
                                transaction.Rollback();
                                return BadRequest(resultado);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = new Responses(1003, ex.ToString());
                            transaction.Rollback();
                            return BadRequest(result.Payback());
                        }

                        if (setter.Tables["tabla"].Rows.Count <= 0)
                        {
                            result = new Responses(2009, null);
                            transaction.Rollback();
                            return BadRequest(result.Payback());
                        }

                        using (FileStream fileStream = new FileStream(fotoServ, FileMode.Create))
                        {
                            try
                            {
                                file.CopyTo(fileStream);
                                result = new Responses(1, null);
                                transaction.Commit();
                                return Ok(result.Payback());
                            }
                            catch (Exception ex)
                            {
                                resultado = new JObject();
                                resultado.message = "Problema encontrado al guardar el registro.";
                                resultado.response = 7001;
                                resultado.value = ex.ToString();
                                transaction.Rollback();
                                return BadRequest(resultado);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result = new Responses(1003, ex.ToString());
                return BadRequest(result.Payback());
            }
        }

        [HttpPost]
        [Route("store/simple")]
        //[Authorize]
        public IActionResult Store(JObject request)
        {
            Responses result;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    result = new Responses(1001, ex.ToString());
                    return BadRequest(result.Payback());
                }

                string nombre1 = request.GetValue("nombre1").ToString();
                string apellido1 = request.GetValue("apellido1").ToString();
                string tipoPersona = request.GetValue("tipoPersona").ToString();
                string usuario = request.GetValue("usuario").ToString();

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 12);
                    cmd.Parameters.AddWithValue("@nombre1", nombre1);
                    cmd.Parameters.AddWithValue("@apellido1", apellido1);
                    cmd.Parameters.AddWithValue("@tipoPersona", tipoPersona);
                    cmd.Parameters.AddWithValue("@genero", 3);
                    cmd.Parameters.AddWithValue("@usuario", usuario);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {
                        adapter.Fill(setter, "tabla");
                        if (setter.Tables["tabla"] == null)
                        {
                            result = new Responses(7001, null);
                            return BadRequest(result.Payback());
                        }
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1002, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    if (setter.Tables["tabla"].Rows.Count <= 0)
                    {
                        result = new Responses(2009, null);
                        return BadRequest(result.Payback());
                    }

                    dynamic resultado = new JObject();
                    resultado.response = 1;
                    resultado.message = "Persona agregada con éxito.";
                    resultado.value = 1;

                    return Ok(resultado);
                }
            }
        }

        [HttpPost]
        [Route("update-data/simple")]
        //[Authorize]
        public IActionResult updateData(JObject request)
        {
            Responses result;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    result = new Responses(1001, ex.ToString());
                    return BadRequest(result.Payback());
                }

                int id = Int32.Parse(request.GetValue("id").ToString());
                string nombre1 = request.GetValue("nombre1").ToString();
                string apellido1 = request.GetValue("apellido1").ToString();
                int tipoPersona = Int32.Parse(request.GetValue("tipoPersona").ToString());
                string usuario = request.GetValue("usuario").ToString();

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 13);
                    cmd.Parameters.AddWithValue("@nombre1", nombre1);
                    cmd.Parameters.AddWithValue("@apellido1", apellido1);
                    cmd.Parameters.AddWithValue("@tipoPersona", tipoPersona);
                    cmd.Parameters.AddWithValue("@genero", 3);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@usuario", usuario);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {
                        adapter.Fill(setter, "tabla");
                        if (setter.Tables["tabla"] == null)
                        {
                            result = new Responses(7001, null);
                            return BadRequest(result.Payback());
                        }
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1002, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    if (setter.Tables["tabla"].Rows.Count <= 0)
                    {
                        result = new Responses(2009, null);
                        return BadRequest(result.Payback());
                    }

                    dynamic resultado = new JObject();
                    resultado.response = 1;
                    resultado.message = "PErsona actualizada con éxito.";
                    resultado.value = 1;

                    return Ok(resultado);
                }
            }
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("all")]
        //[Authorize]
        public IActionResult GetAll()
        {
            Responses result;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    result = new Responses(1001, ex.ToString());
                    return BadRequest(result.Payback());
                }

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 4);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {
                        adapter.Fill(setter, "tabla");
                        if (setter.Tables == null)
                        {
                            result = new Responses(7001, null);
                            return BadRequest(result.Payback());
                        }
                    }
                    catch (SqlException ex)
                    {
                        result = new Responses(1002, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    if (setter.Tables["tabla"].Rows.Count <= 0)
                    {
                        result = new Responses(2009, null);
                        return BadRequest(result.Payback());
                    }

                    JArray personas = new JArray();
                    foreach (DataRow row in setter.Tables["tabla"].Rows)
                    {
                        string path = string.Empty;
                        string imageBase64 = string.Empty;
                        byte[] doc64;
                        if (!string.IsNullOrEmpty(row["foto"].ToString()))
                        {
                            path = Path.Combine(_root, row["foto"].ToString());

                            try
                            {
                                doc64 = System.IO.File.ReadAllBytes(path);
                            }
                            catch (Exception)
                            {

                                path = Path.Combine(_root, _replaceFile);
                                doc64 = System.IO.File.ReadAllBytes(path);
                            }

                            imageBase64 = Convert.ToBase64String(doc64);
                        } 
                            else
                        {
                            doc64 = System.IO.File.ReadAllBytes(_replaceFile);
                            imageBase64 = Convert.ToBase64String(doc64);
                        }


                        try
                        {
                            personas.Add(new JObject(
                                             new JProperty("id", row["id"].ToString()),
                                             new JProperty("nombreCompleto", row["nombreCompleto"].ToString()),
                                             new JProperty("dpi", row["dpi"].ToString()),
                                             new JProperty("genero", row["genero"].ToString()),
                                             new JProperty("email", row["email"].ToString()),
                                             new JProperty("telefono", row["telefono"].ToString()),
                                             new JProperty("tipoPersona", row["tipoPersona"].ToString()),
                                             new JProperty("nombreTipoPersona", row["nombreTipoPersona"].ToString()),                                             
                                             new JProperty("nit", row["nit"].ToString()),
                                             new JProperty("estado", row["estado"].ToString()),
                                             new JProperty("extension", row["extension"].ToString()),                                                                                                                                      
                                             new JProperty("foto", imageBase64),
                                             new JProperty("response", 1)
                                            ));
                        }
                        catch (Exception ex)
                        {
                            personas.Add(new JObject(
                                             new JProperty("message", ex.Message),
                                             new JProperty("id", "NA"),
                                             new JProperty("nombreCompleto", "NA"),
                                             new JProperty("dpi", "NA"),
                                             new JProperty("genero", "NA"),
                                             new JProperty("email", "NA"),
                                             new JProperty("telefono", "NA"),
                                             new JProperty("tipoPersona", "NA"),
                                             new JProperty("nombreTipoPersona", "NA"),
                                             new JProperty("nit", "NA"),                                             
                                             new JProperty("estado", "NA"),
                                             new JProperty("extension", "NA"),
                                             new JProperty("foto", "NA"),
                                             new JProperty("response", 0)
                                            ));
                        }
                    }

                    return Ok(personas);
                }
            }
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("one")]
        //[Authorize]
        public IActionResult GetOne(JObject request)
        {
            Responses result;


            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1001, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("opcion", 5);
                        cmd.Parameters.AddWithValue("@id", request.GetValue("id").ToString());

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet setter = new DataSet();

                        try
                        {
                            adapter.Fill(setter, "tabla");

                            if (setter.Tables["tabla"] == null)
                            {
                                result = new Responses(7001, null);
                                return BadRequest(result.Payback());
                            }
                        }
                        catch (Exception ex)
                        {
                            result = new Responses(1002, ex.ToString());
                            return BadRequest(result.Payback());
                        }

                        if (setter.Tables["tabla"].Rows.Count <= 0)
                        {
                            result = new Responses(2009, null);
                            return BadRequest(result.Payback());
                        }

                        JArray onePersona = new JArray();
                        string path = string.Empty;
                        string imageBase64 = string.Empty;
                        if (!string.IsNullOrEmpty(setter.Tables["tabla"].Rows[0]["foto"].ToString()))
                        {
                            path = Path.Combine(_root, setter.Tables["tabla"].Rows[0]["foto"].ToString());
                            byte[] doc64;

                            try
                            {
                                doc64 = System.IO.File.ReadAllBytes(path);
                            }
                            catch (Exception)
                            {

                                path = Path.Combine(_root, _replaceFile);
                                doc64 = System.IO.File.ReadAllBytes(path);
                            }

                            imageBase64 = Convert.ToBase64String(doc64);

                        }

                        try
                        {
                            onePersona.Add(new JObject(
                                             new JProperty("id", setter.Tables["tabla"].Rows[0]["id"].ToString()),
                                             new JProperty("nombre1", setter.Tables["tabla"].Rows[0]["nombre1"].ToString()),
                                             new JProperty("nombre2", setter.Tables["tabla"].Rows[0]["nombre2"].ToString()),
                                             new JProperty("nombre3", setter.Tables["tabla"].Rows[0]["nombre3"].ToString()),
                                             new JProperty("nombre4", setter.Tables["tabla"].Rows[0]["nombre4"].ToString()),
                                             new JProperty("nombre5", setter.Tables["tabla"].Rows[0]["nombre5"].ToString()),
                                             new JProperty("nombre6", setter.Tables["tabla"].Rows[0]["nombre6"].ToString()),
                                             new JProperty("apellido1", setter.Tables["tabla"].Rows[0]["apellido1"].ToString()),
                                             new JProperty("apellido2", setter.Tables["tabla"].Rows[0]["apellido2"].ToString()),
                                             new JProperty("apellido3", setter.Tables["tabla"].Rows[0]["apellido3"].ToString()),
                                             new JProperty("dpi", setter.Tables["tabla"].Rows[0]["dpi"].ToString()),
                                             new JProperty("idGenero", setter.Tables["tabla"].Rows[0]["idGenero"].ToString()),
                                             new JProperty("genero", setter.Tables["tabla"].Rows[0]["genero"].ToString()),                                             
                                             new JProperty("email", setter.Tables["tabla"].Rows[0]["email"].ToString()),
                                             new JProperty("telefono", setter.Tables["tabla"].Rows[0]["telefono"].ToString()),
                                             new JProperty("tipoPersona", setter.Tables["tabla"].Rows[0]["tipoPersona"].ToString()),
                                             new JProperty("nombreTipoPersona", setter.Tables["tabla"].Rows[0]["nombreTipoPersona"].ToString()),
                                             new JProperty("nit", setter.Tables["tabla"].Rows[0]["nit"].ToString()),                                                                                          
                                             new JProperty("estado", setter.Tables["tabla"].Rows[0]["estado"].ToString()),
                                             new JProperty("extension", setter.Tables["tabla"].Rows[0]["extension"].ToString()),
                                             new JProperty("foto", imageBase64),
                                             new JProperty("response", 1)
                                            ));
                        }
                        catch (Exception ex)
                        {
                            onePersona.Add(new JObject(
                                             new JProperty("message", ex.Message),
                                             new JProperty("id", "NA"),
                                             new JProperty("nombre1", "NA"),
                                             new JProperty("nombre2", "NA"),
                                             new JProperty("nombre3", "NA"),
                                             new JProperty("nombre4", "NA"),
                                             new JProperty("nombre5", "NA"),
                                             new JProperty("nombre6", "NA"),
                                             new JProperty("apellido1", "NA"),
                                             new JProperty("apellido2", "NA"),
                                             new JProperty("apellido3", "NA"),
                                             new JProperty("dpi", "NA"),
                                             new JProperty("genero", "NA"),
                                             new JProperty("idGenero", "NA"),
                                             new JProperty("email", "NA"),
                                             new JProperty("telefono", "NA"),
                                             new JProperty("tipoPersona", "NA"),
                                             new JProperty("nombreTipoPersona", "NA"),
                                             new JProperty("nit", "NA"),
                                             new JProperty("estado", "NA"),                                                                                          
                                             new JProperty("extension", "NA"),
                                             new JProperty("foto", "NA"),
                                             new JProperty("response", 0)
                                            ));
                        }

                        return Ok(onePersona);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Responses(1003, ex.ToString());
                return BadRequest(result.Payback());
            }
        }

        [HttpPost]
        [Route("update-data")]
        //[Authorize]
        public IActionResult UpdateData([FromForm] PersonaModel request)
        {
            Responses result;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    result = new Responses(1001, ex.ToString());
                    return BadRequest(result.Payback());
                }                                

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 2);
                    cmd.Parameters.AddWithValue("@id", request.id);
                    cmd.Parameters.AddWithValue("@nombre1", request.nombre1);
                    cmd.Parameters.AddWithValue("@nombre2", request.nombre2);
                    cmd.Parameters.AddWithValue("@nombre3", request.nombre3);
                    cmd.Parameters.AddWithValue("@nombre4", request.nombre4);
                    cmd.Parameters.AddWithValue("@nombre5", request.nombre5);
                    cmd.Parameters.AddWithValue("@nombre6", request.nombre6);
                    cmd.Parameters.AddWithValue("@apellido1", request.apellido1);
                    cmd.Parameters.AddWithValue("@apellido2", request.apellido2);
                    cmd.Parameters.AddWithValue("@apellido3", request.apellido3);
                    cmd.Parameters.AddWithValue("@dpi", request.dpi);
                    cmd.Parameters.AddWithValue("@genero", request.genero);
                    cmd.Parameters.AddWithValue("@email", request.email);
                    cmd.Parameters.AddWithValue("@telefono", request.telefono);
                    cmd.Parameters.AddWithValue("@tipoPersona", request.tipoPersona);
                    cmd.Parameters.AddWithValue("@nit", request.nit);
                    cmd.Parameters.AddWithValue("@usuario", request.usuario);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {
                        adapter.Fill(setter, "tabla");
                        if (setter.Tables["tabla"] == null)
                        {
                            result = new Responses(7001, null);
                            return BadRequest(result.Payback());
                        }
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1002, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    if (setter.Tables["tabla"].Rows.Count <= 0)
                    {
                        result = new Responses(2009, null);
                        return BadRequest(result.Payback());
                    }

                    dynamic resultado = new JObject();
                    resultado.response = 1;
                    resultado.message = "Datos actualizados con éxito.";
                    resultado.value = 1;

                    return Ok(resultado);
                }
            }
        }


        [HttpPost]
        [Produces("application/json")]
        [Route("update-image")]
        public IActionResult UpdateImage([FromForm] PersonaModel request)
        {
            dynamic resultado;
            Responses result;

            var file = request.foto;
            string imageName = file.FileName;
            string extension = Path.GetExtension(imageName);
            long sizeImage = file.Length;

            imageName = CleanText.RemoveSpaces(imageName);
            imageName = CleanText.RemoveAccents(imageName);

            Regex reg = new Regex(@"^.*\.(pdf) || \.(pdf)$ || \.(jpg|png|jpeg)$");

            if (!reg.IsMatch(extension))
            {
                resultado = new JObject();
                resultado.message = "El formato del archivo no es soportado.";
                resultado.response = 2;
                resultado.value = 2;
                return BadRequest(resultado);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1001, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    string thisTime = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                    string nameUnique = Guid.NewGuid().ToString();
                    string newNameDB = thisTime + "-" + nameUnique + "-" + imageName;

                    using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlTransaction transaction; //Hacemos esto para poder deshacer la transacción, en caso de que algo no se haya guardado
                        transaction = conn.BeginTransaction("imagenTransaction");
                        cmd.Transaction = transaction;

                        string fotoDB = Path.Combine(_pathImagePersona, newNameDB);
                        string fotoServ = Path.Combine(_root, fotoDB);

                        cmd.Parameters.AddWithValue("@opcion", 10);
                        cmd.Parameters.AddWithValue("@id", request.id);
                        cmd.Parameters.AddWithValue("@foto", fotoDB);
                        cmd.Parameters.AddWithValue("@extension", extension);
                        cmd.Parameters.AddWithValue("@usuario", request.usuario);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet setter = new DataSet();

                        try
                        {
                            adapter.Fill(setter, "tabla");

                            if (setter.Tables["tabla"] == null)
                            {
                                resultado = new JObject();
                                resultado.message = "No se ha podido almacenar el registro.";
                                resultado.response = 0;
                                resultado.value = 0;
                                transaction.Rollback();
                                return BadRequest(resultado);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = new Responses(1003, ex.ToString());
                            transaction.Rollback();
                            return BadRequest(result.Payback());
                        }

                        if (setter.Tables["tabla"].Rows.Count <= 0)
                        {
                            result = new Responses(2009, null);
                            transaction.Rollback();
                            return BadRequest(result.Payback());
                        }

                        using (FileStream fileStream = new FileStream(fotoServ, FileMode.Create))
                        {
                            try
                            {
                                file.CopyTo(fileStream);
                                resultado = new JObject();
                                resultado.message = "Fotografía actualizada con éxito.";
                                resultado.response = 1;
                                resultado.value = 1;
                                transaction.Commit();

                                return Ok(resultado);
                            }
                            catch (Exception ex)
                            {
                                resultado = new JObject();
                                resultado.message = "Problema encontrado al guardar el registro.";
                                resultado.response = 7001;
                                resultado.value = ex.ToString();
                                transaction.Rollback();
                                return BadRequest(resultado);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result = new Responses(1003, ex.ToString());
                return BadRequest(result.Payback());
            }
        }

        [HttpPost]
        [Route("destroy")]
        //[Authorize]
        public IActionResult Destroy(JObject request)
        {
            Responses result;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    result = new Responses(1001, ex.ToString());
                    return BadRequest(result.Payback());
                }

                int id = Int32.Parse(request.GetValue("id").ToString());
                string usuario = request.GetValue("usuario").ToString();

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 3);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@estado", 0);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {
                        adapter.Fill(setter, "tabla");
                        if (setter.Tables["tabla"] == null)
                        {
                            result = new Responses(7001, null);
                            return BadRequest(result.Payback());
                        }
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1002, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    if (setter.Tables["tabla"].Rows.Count <= 0)
                    {
                        result = new Responses(2009, null);
                        return BadRequest(result.Payback());
                    }

                    dynamic resultado = new JObject();
                    resultado.response = 1;
                    resultado.message = "Persona se ha dado de baja con éxito.";
                    resultado.value = 1;

                    return Ok(resultado);
                }
            }
        }

        [HttpPost]
        [Route("active")]
        //[Authorize]
        public IActionResult Active(JObject request)
        {
            Responses result;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    result = new Responses(1001, ex.ToString());
                    return BadRequest(result.Payback());
                }

                int id = Int32.Parse(request.GetValue("id").ToString());
                string usuario = request.GetValue("usuario").ToString();

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 3);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@estado", 1);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {
                        adapter.Fill(setter, "tabla");
                        if (setter.Tables["tabla"] == null)
                        {
                            result = new Responses(7001, null);
                            return BadRequest(result.Payback());
                        }
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1002, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    if (setter.Tables["tabla"].Rows.Count <= 0)
                    {
                        result = new Responses(2009, null);
                        return BadRequest(result.Payback());
                    }

                    dynamic resultado = new JObject();
                    resultado.response = 1;
                    resultado.message = "Persona se ha dado de alta con éxito.";
                    resultado.value = 1;

                    return Ok(resultado);
                }
            }
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("label")]
        //[Authorize]
        public IActionResult GetAllLabel()
        {
            Responses result;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1001, ex.ToString());
                        return BadRequest(result.Payback());
                    }
                    using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("opcion", 6);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet setter = new DataSet();
                        try
                        {
                            adapter.Fill(setter, "tabla");
                            if (setter.Tables["tabla"] == null)
                            {
                                result = new Responses(7001, null);
                                return BadRequest(result.Payback());
                            }
                        }
                        catch (Exception ex)
                        {
                            result = new Responses(1003, ex.ToString());
                            return BadRequest(result.Payback());
                        }
                        if (setter.Tables["tabla"].Rows.Count <= 0)
                        {
                            result = new Responses(2009, null);
                            return BadRequest(result.Payback());
                        }
                        return Ok(setter.Tables["tabla"]);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Responses(1003, ex.ToString());
                return BadRequest(result.Payback());
            }
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("get-perfil")]
        //[Authorize]
        public IActionResult GetDataPersona(JObject request)
        {
            Responses result;


            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(1001, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("opcion", 9);
                        cmd.Parameters.AddWithValue("@usuario", request.GetValue("usuario").ToString());

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet setter = new DataSet();

                        try
                        {
                            adapter.Fill(setter, "tabla");

                            if (setter.Tables["tabla"] == null)
                            {
                                result = new Responses(7001, null);
                                return BadRequest(result.Payback());
                            }
                        }
                        catch (Exception ex)
                        {
                            result = new Responses(1002, ex.ToString());
                            return BadRequest(result.Payback());
                        }

                        if (setter.Tables["tabla"].Rows.Count <= 0)
                        {
                            result = new Responses(2009, null);
                            return BadRequest(result.Payback());
                        }

                        JArray onePersona = new JArray();
                        string path = string.Empty;
                        string imageBase64 = string.Empty;
                        if (!string.IsNullOrEmpty(setter.Tables["tabla"].Rows[0]["foto"].ToString()))
                        {
                            path = Path.Combine(_root, setter.Tables["tabla"].Rows[0]["foto"].ToString());
                            byte[] doc64 = System.IO.File.ReadAllBytes(path);
                            imageBase64 = Convert.ToBase64String(doc64);

                        }

                        try
                        {
                            onePersona.Add(new JObject(
                                             new JProperty("id", setter.Tables["tabla"].Rows[0]["id"].ToString()),
                                             new JProperty("nombreCompleto", setter.Tables["tabla"].Rows[0]["nombreCompleto"].ToString()),
                                             new JProperty("username", setter.Tables["tabla"].Rows[0]["username"].ToString()),
                                             new JProperty("dpi", setter.Tables["tabla"].Rows[0]["dpi"].ToString()),
                                             new JProperty("genero", setter.Tables["tabla"].Rows[0]["genero"].ToString()),
                                             new JProperty("email", setter.Tables["tabla"].Rows[0]["email"].ToString()),
                                             new JProperty("telefono", setter.Tables["tabla"].Rows[0]["telefono"].ToString()),                                                                                          
                                             new JProperty("nit", setter.Tables["tabla"].Rows[0]["nit"].ToString()),
                                             new JProperty("estado", setter.Tables["tabla"].Rows[0]["estado"].ToString()),
                                             new JProperty("extension", setter.Tables["tabla"].Rows[0]["extension"].ToString()),
                                             new JProperty("foto", imageBase64),
                                             new JProperty("response", 1)
                                            ));
                        }
                        catch (Exception ex)
                        {
                            onePersona.Add(new JObject(
                                             new JProperty("message", ex.Message),
                                             new JProperty("id", "NA"),
                                             new JProperty("nombreCompleto", "NA"),
                                             new JProperty("dpi", "NA"),
                                             new JProperty("genero", "NA"),
                                             new JProperty("email", "NA"),
                                             new JProperty("telefono", "NA"),
                                             new JProperty("nit", "NA"),
                                             new JProperty("estado", "NA"),
                                             new JProperty("extension", "NA"),
                                             new JProperty("foto", "NA"),
                                             new JProperty("response", 0)
                                            ));
                        }

                        return Ok(onePersona);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Responses(1003, ex.ToString());
                return BadRequest(result.Payback());
            }
        }
    }
}
