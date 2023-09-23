using dw_backend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace dw_backend.Controllers.Administracion.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly string _connectionString;
        private string _root;
        private readonly string _replaceFile;

        public LoginController(IConfiguration _configuration)
        {
            Configuration = _configuration;
            _connectionString = Configuration.GetConnectionString("MainConnection");
            _root = Configuration.GetValue<string>("Root");
            _replaceFile = Configuration.GetValue<string>("ReplaceFiles:pathProfile");
        }

        [HttpPost]
        [Route("registrar")]
        public IActionResult Registrar(JObject request)
        {
            Responses result;

            string username = request.GetValue("username").ToString();
            string password = request.GetValue("password").ToString();
            string roles = request.GetValue("roles").ToString();
            int persona = Int32.Parse(request.GetValue("persona").ToString());
            int usuario = Int32.Parse(request.GetValue("usuario").ToString());

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

                using (SqlCommand cmd = new SqlCommand("adm.crudUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 1);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(password));
                    cmd.Parameters.AddWithValue("@persona", persona);
                    cmd.Parameters.AddWithValue("@roles", roles);
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

                    dynamic data = new JObject();
                    data.response = 1;
                    data.message = "Se ha registrado con éxito.";
                    data.value = 1;

                    return Ok(data);
                }
            }
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Login(JObject request)
        {
            dynamic data;
            string username = request.GetValue("username").ToString();
            string password = request.GetValue("password").ToString();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    data = new JObject();
                    data.response = 0;
                    data.message = "Error de conexión en la fuente de datos.";
                    data.value = ex.ToString();

                    return BadRequest(data);
                }

                using (SqlCommand cmd = new SqlCommand("adm.crudLogin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@username", username);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {
                        adapter.Fill(setter, "tabla");

                        if (setter.Tables["tabla"] == null)
                        {
                            data = new JObject();
                            data.message = "No existen datos relacionados con la búsqueda.";
                            data.response = 2;
                            data.value = 2;
                            return BadRequest(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        data = new JObject();
                        data.message = "No se ha podido realizar la petición solicitada.";
                        data.response = 3;
                        data.value = ex.ToString();
                        return BadRequest(data);
                    }

                    if (setter.Tables["tabla"].Rows.Count <= 0)
                    {
                        data = new JObject();
                        data.message = "No se han encontrado datos relacionados con la búsqueda.";
                        data.response = 4;
                        data.value = 4;
                        return BadRequest(data);
                    }

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

                    string encryptPassword = setter.Tables["tabla"].Rows[0]["password"].ToString();
                    string id = setter.Tables["tabla"].Rows[0]["codigo"].ToString();
                    string usernameDb = setter.Tables["tabla"].Rows[0]["username"].ToString();
                    string estado = setter.Tables["tabla"].Rows[0]["estado"].ToString();
                    string roles = setter.Tables["tabla"].Rows[0]["roles"].ToString();
                    string nombreCompleto = setter.Tables["tabla"].Rows[0]["nombreCompleto"].ToString();
                    string extension = setter.Tables["tabla"].Rows[0]["extension"].ToString();
                    string i = setter.Tables["tabla"].Rows[0]["i"].ToString();
                    string foto = imageBase64;

                    if (!BCrypt.Net.BCrypt.Verify(password, encryptPassword))
                    {
                        data = new JObject();
                        data.message = "Contraseña incorrecta.";
                        data.response = 4;
                        data.value = 4;
                        return BadRequest(data);
                    }

                    dynamic resultado = null;

                    resultado = new JObject();
                    resultado.id = id;
                    resultado.username = usernameDb;
                    resultado.nombreCompleto = nombreCompleto;
                    resultado.roles = roles;
                    resultado.estado = estado;
                    resultado.token = GenerarJsonWebToken(resultado);
                    resultado.extension = extension;
                    resultado.i = i;
                    resultado.foto = foto;                    
                    return Ok(resultado);
                }
            }
        }

        private string GenerarJsonWebToken(dynamic objeto)
        {
            JObject obj = new JObject();
            obj.Add("username", objeto.username);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, obj.GetValue("username").ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                        issuer: Configuration["Jwt:Issuer"],
                        audience: Configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: credentials
                );

            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodeToken;
        }
    }
}
