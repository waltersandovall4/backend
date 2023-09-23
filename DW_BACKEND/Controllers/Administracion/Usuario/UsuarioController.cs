using dw_backend.Helpers;
using dw_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using MailKit.Net.Smtp;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;




namespace dw_backend.Controllers.Administracion.Usuario
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly string _connectionString;
        private readonly string _nameProcedure;
        private readonly string _updatePasswordTemplate;
        private readonly string _resetPasswordTemplate;
        private readonly string _newUserTemplate;

        public UsuarioController(IConfiguration _configuration)
        {
            Configuration = _configuration;
            _connectionString = Configuration.GetConnectionString("MainConnection");
            _updatePasswordTemplate = _configuration.GetValue<string>("MailTemplates:updatePassword");
            _resetPasswordTemplate = _configuration.GetValue<string>("MailTemplates:resetPassword");
            _newUserTemplate = _configuration.GetValue<string>("MailTemplates:newUser");
            _nameProcedure = "adm.crudUsuario";
        }


        [HttpGet]
        [Route("all")]
        //[Authorize]
        public IActionResult All()
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

                using (SqlCommand cmd = new SqlCommand("adm.crudUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 4);

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

                    return Ok(setter.Tables["tabla"]);
                }
            }
        }

        [HttpPost]
        [Route("asignar-rol")]
        //[Authorize]
        public IActionResult Store(UsuarioModel request)
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
                    cmd.Parameters.AddWithValue("@opcion", 1);
                    cmd.Parameters.AddWithValue("@id", request.id);
                    cmd.Parameters.AddWithValue("@persona", request.persona);
                    cmd.Parameters.AddWithValue("@roles", request.roles);
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
                    resultado.message = "Rol agregado con éxito.";
                    resultado.value = 1;

                    return Ok(resultado);
                }
            }
        }

        [HttpPost]
        [Route("roles/update")]
        //[Authorize]
        public IActionResult StoreUpdateRoles(JObject request)
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

                using (SqlCommand cmd = new SqlCommand("adm.crudUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 1);
                    cmd.Parameters.AddWithValue("@roles", request.GetValue("roles").ToString());
                    cmd.Parameters.AddWithValue("@usuario", request.GetValue("usuario").ToString());
                    cmd.Parameters.AddWithValue("@persona", Int32.Parse(request.GetValue("persona").ToString()));

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
                    resultado.message = "Rol agregado con éxito.";
                    resultado.value = 1;

                    return Ok(resultado);
                }
            }
        }

        [HttpPost]
        [Route("get-roles")]
        //[Authorize]
        public IActionResult AllRoles(JObject request)
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
                    cmd.Parameters.AddWithValue("@id", request.GetValue("id").ToString());
                    cmd.Parameters.AddWithValue("@opcion", 2);

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

                    return Ok(setter.Tables["tabla"]);
                }
            }
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("change-pass")]
        public IActionResult ChangePass(JObject request)
        {

            Responses result;
            dynamic data;

            string password0 = request.GetValue("password0").ToString();
            string password = request.GetValue("password").ToString();
            string id = request.GetValue("id").ToString();

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password0) || string.IsNullOrEmpty(id))
            {
                result = new Responses(7002, null);
                return BadRequest(result.Payback());
            }

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
                    cmd.Parameters.AddWithValue("@id", Int32.Parse(request.GetValue("id").ToString()));
                    cmd.Parameters.AddWithValue("@opcion", 11);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {
                        adapter.Fill(setter, "usuario");
                        if (setter.Tables["usuario"] == null)
                        {
                            result = new Responses(2009, null);
                            return BadRequest(result.Payback());
                        }
                    }
                    catch (Exception ex)
                    {
                        result = new Responses(7001, ex.ToString());
                        return BadRequest(result.Payback());
                    }

                    if (setter.Tables["usuario"].Rows.Count <= 0)
                    {
                        result = new Responses(2009, null);
                        return BadRequest(result.Payback());
                    }

                    //get user data                    
                    int estadoUsuario = Int32.Parse(setter.Tables["usuario"].Rows[0]["estadoUsuario"].ToString());
                    int idPersona = Int32.Parse(setter.Tables["usuario"].Rows[0]["idPersona"].ToString());
                    string passHash = setter.Tables["usuario"].Rows[0]["password"].ToString();

                    if (BCrypt.Net.BCrypt.Verify(password0, passHash))
                    {

                        cmd.Parameters.Clear();
                        adapter.Dispose();
                        setter.Dispose();
                        setter.Tables.Clear();

                        //transaction about password reset
                        SqlTransaction transaction = conn.BeginTransaction("ChangePasswordTransaction");
                        cmd.Transaction = transaction;

                        //Hash new Password
                        string hashNewPassword = BCrypt.Net.BCrypt.HashPassword(password);

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = _nameProcedure;
                        cmd.Parameters.AddWithValue("@id", request.GetValue("id").ToString());
                        cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(password));
                        cmd.Parameters.AddWithValue("@opcion", 12);

                        try
                        {
                            int resultado = cmd.ExecuteNonQuery();
                            if (resultado >= 1)
                            {
                                try
                                {
                                    // commit transaction
                                    data = new JObject();
                                    data.message = "Su contraseña se ha actualizado exitosamente";
                                    data.value = 1;
                                    data.response = 1;
                                    transaction.Commit();
                                    return Ok(data);
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    result = new Responses(1003, ex.ToString());
                                    return BadRequest(result.Payback());
                                }
                            }
                            else
                            {
                                data = new JObject();
                                data.message = "Ha ocurrido un problema con la solicitud para restablecer la contraseña";
                                data.value = 1;
                                data.response = 1;
                                transaction.Rollback();
                                return Ok(data);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            result = new Responses(1002, ex.ToString());
                            return BadRequest(result.Payback());
                        }
                    }
                    else
                    {
                        data = new JObject();
                        data.message = "Proceso no realizado, puede que la contraseña proporcionada no coincida con la contraseña actual.";
                        data.value = 0;
                        data.response = 0;
                        return BadRequest(data);
                    }
                }
            }
        }


        [HttpPost]
        [Produces("application/json")]
        [Route("cambiarpass")]
        public IActionResult CambiarPass(JObject request)
        {

            Responses payback;
            dynamic data;

            string password0 = request.GetValue("password0").ToString();
            string password = request.GetValue("password").ToString();
            string id = request.GetValue("id").ToString();

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password0) || string.IsNullOrEmpty(id))
            {
                payback = new Responses(2001, null);
                return BadRequest(payback.Payback());
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    payback = new Responses(1001, ex.ToString());
                    return BadRequest(payback.Payback());
                }

                using (SqlCommand cmd = new SqlCommand("adm.crudUsuario", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", Int32.Parse(request.GetValue("id").ToString()));
                    cmd.Parameters.AddWithValue("@opcion", 8);


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {

                        adapter.Fill(setter, "usuario");
                        if (setter.Tables["usuario"] == null)
                        {
                            data = new JObject();
                            data.value = 6;
                            data.message = "No existen datos relacionados con la busqueda.";
                            data.response = 6;

                            return BadRequest(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        data = new JObject();
                        data.value = ex.ToString();
                        data.message = "No se ha podido realizar la buscqueda de datos.";
                        data.response = 7;

                        return BadRequest(data);
                    }

                    if (setter.Tables["usuario"].Rows.Count <= 0)
                    {
                        payback = new Responses(2009, null);
                        return BadRequest(payback.Payback());
                    }

                    //get user data
                    string email = setter.Tables["usuario"].Rows[0]["email"].ToString();
                    int estadoUsuario = Int32.Parse(setter.Tables["usuario"].Rows[0]["estadoUsuario"].ToString());
                    int idPersona = Int32.Parse(setter.Tables["usuario"].Rows[0]["idPersona"].ToString());
                    string passHash = setter.Tables["usuario"].Rows[0]["password"].ToString();

                    if (BCrypt.Net.BCrypt.Verify(password0, passHash))
                    {

                        cmd.Parameters.Clear();
                        adapter.Dispose();
                        setter.Dispose();
                        setter.Tables.Clear();

                        //transaction to send notification about password reset

                        cmd.CommandText = "crud_configuracion_correo";
                        cmd.Parameters.AddWithValue("@opcion", 4);

                        SqlTransaction transaction = conn.BeginTransaction("resetPasswordTransaction");
                        cmd.Transaction = transaction;

                        try
                        {
                            adapter.SelectCommand = cmd;
                            adapter.Fill(setter, "emailServer");

                            if (setter.Tables["emailServer"] == null)
                            {
                                transaction.Rollback();
                                payback = new Responses(5, null);
                                return BadRequest(payback.Payback());
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            payback = new Responses(7001, ex.ToString());
                            return BadRequest(payback.Payback());
                        }

                        if (setter.Tables["emailServer"].Rows.Count <= 0)
                        {
                            transaction.Rollback();
                            payback = new Responses(7001, null);
                            return BadRequest(payback.Payback());
                        }

                        //get email server data
                        int smtpPort = Int32.Parse(setter.Tables["emailServer"].Rows[0]["smtpPort"].ToString());
                        string smtpServer = setter.Tables["emailServer"].Rows[0]["smtpServer"].ToString();
                        string passEmailAccount = setter.Tables["emailServer"].Rows[0]["passwordEmailAccount"].ToString();
                        string emailServerAccount = setter.Tables["emailServer"].Rows[0]["emailServerAccount"].ToString();

                        var mailMessage = new MimeMessage();
                        mailMessage.From.Add(new MailboxAddress("no-reply", emailServerAccount));
                        mailMessage.To.Add(new MailboxAddress(email, email));
                        mailMessage.Subject = "Su contraseña ha sido actualizada";
                        var bodyBuilder = new BodyBuilder();
                        string messageBody;

                        using (StreamReader reader = System.IO.File.OpenText(_updatePasswordTemplate))
                        {
                            messageBody = reader.ReadToEnd();
                            bodyBuilder.HtmlBody = string.Format(Regex.Replace(messageBody, @"[\r\n\t ]+", " "),
                                                    email,
                                                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            reader.Close();
                            reader.Dispose();
                        }

                        using (var smtpClient = new SmtpClient())
                        {

                            try
                            {
                                smtpClient.Connect(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                payback = new Responses(10001, ex.ToString());
                                return BadRequest(payback.Payback());
                            }

                            try
                            {
                                smtpClient.Authenticate(emailServerAccount, passEmailAccount);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                payback = new Responses(10002, ex.ToString());
                                return BadRequest(payback.Payback());
                            }

                            cmd.Parameters.Clear();
                            setter.Tables.Clear();
                            setter.Dispose();
                            adapter.Dispose();

                            //Hash new Password
                            string hashNewPassword = BCrypt.Net.BCrypt.HashPassword(password);

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "adm.crudUsuario";
                            cmd.Parameters.AddWithValue("@id", request.GetValue("id").ToString());
                            cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(password));
                            cmd.Parameters.AddWithValue("@usuario", request.GetValue("usuario").ToString());
                            cmd.Parameters.AddWithValue("@estado", 1);
                            cmd.Parameters.AddWithValue("@opcion", 7);

                            try
                            {
                                int result = cmd.ExecuteNonQuery();
                                if (result >= 1)
                                {
                                    try
                                    {
                                        //send message and commit
                                        mailMessage.Body = bodyBuilder.ToMessageBody();
                                        smtpClient.Send(mailMessage);
                                        smtpClient.Disconnect(true);
                                        smtpClient.Dispose();

                                        data = new JObject();
                                        data.message = "Su contraseña se a actualizado exitosamente";
                                        data.value = 1;
                                        data.response = 1;
                                        transaction.Commit();
                                        return Ok(data);
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        payback = new Responses(10003, ex.ToString());
                                        return BadRequest(payback.Payback());
                                    }
                                }
                                else
                                {
                                    data = new JObject();
                                    data.message = "Ha ocurrido un problema con la solicitud para restablecer la contraseña";
                                    data.value = 1;
                                    data.response = 1;
                                    transaction.Rollback();
                                    return Ok(data);
                                }
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                payback = new Responses(10004, ex.ToString());
                                return BadRequest(payback.Payback());
                            }
                        }

                    }
                    else
                    {
                        payback = new Responses(0, null);
                        return BadRequest(payback.Payback());
                    }

                }
            }
        }


        [HttpPost]
        [Route("perfil")]
        //[Authorize]
        public IActionResult Perfil(JObject request)
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

                using (SqlCommand cmd = new SqlCommand("adm.crudUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 9);
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

                    return Ok(setter.Tables["tabla"]);
                }
            }
        }
        [HttpPost]
        [Route("list-roles")]
        //[Authorize]
        public IActionResult ListRoles(JObject request)
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

                using (SqlCommand cmd = new SqlCommand("adm.crudUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 11);
                    cmd.Parameters.AddWithValue("@id", request.GetValue("i").ToString());

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

                    return Ok(setter.Tables["tabla"]);
                }
            }
        }


        [HttpPost]
        [Produces("application/json")]
        [Route("reset-password")]
        public IActionResult ResetPasword(JObject request)
        {

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("adm.crudUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", request.GetValue("id").ToString());
                    cmd.Parameters.AddWithValue("@opcion", 10);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    return Ok(new { request });
                }

            }
        }


        [HttpPost]
        [Produces("application/json")]
        [Route("reset/default/password")]
        public IActionResult DefaultPass(JObject request)
        {
            Responses payback;
            dynamic data;
            string parameter, parameterValue;

            //if receive parameter email

            if (request.ContainsKey("email"))
            {
                if (string.IsNullOrEmpty(request.GetValue("email").ToString()))
                {
                    payback = new Responses(25, null);
                    return BadRequest(payback.Payback());
                }
                parameterValue = request.GetValue("email").ToString();
                parameter = "@email";
            }
            else
            {
                //if receive parameter id
                if (request.ContainsKey("id"))
                {
                    if (string.IsNullOrEmpty(request.GetValue("id").ToString()))
                    {
                        payback = new Responses(2010, null);
                        return BadRequest(payback.Payback());
                    }
                    parameterValue = request.GetValue("id").ToString();
                    parameter = "@id";
                }
                else
                {
                    payback = new Responses(2010, null);
                    return BadRequest(payback.Payback());
                }
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    payback = new Responses(1001, ex.ToString());
                    return BadRequest(payback.Payback());
                }

                using (SqlCommand cmd = new SqlCommand("adm.crudUsuario", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(parameter, parameterValue);
                    cmd.Parameters.AddWithValue("@opcion", 8);


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet setter = new DataSet();

                    try
                    {

                        adapter.Fill(setter, "usuario");
                        if (setter.Tables["usuario"] == null)
                        {
                            data = new JObject();
                            data.value = 6;
                            data.message = "No existen datos relacionados con la busqueda.";
                            data.response = 6;

                            return BadRequest(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        data = new JObject();
                        data.value = ex.ToString();
                        data.message = "No se ha podido realizar la buscqueda de datos.";
                        data.response = 7;

                        return BadRequest(data);
                    }

                    if (setter.Tables["usuario"].Rows.Count <= 0)
                    {
                        payback = new Responses(1001, null);
                        return BadRequest(payback.Payback());
                    }

                    //get user data
                    string email = setter.Tables["usuario"].Rows[0]["email"].ToString();
                    int estadoPersona = Int32.Parse(setter.Tables["usuario"].Rows[0]["estadoPersona"].ToString());
                    int estadoUsuario = Int32.Parse(setter.Tables["usuario"].Rows[0]["estadoUsuario"].ToString());
                    int idUsuario = Int32.Parse(setter.Tables["usuario"].Rows[0]["idUsuario"].ToString());
                    int idPersona = Int32.Parse(setter.Tables["usuario"].Rows[0]["idPersona"].ToString());

                    if (estadoUsuario == 2) //Already reset password
                    {
                        data = new JObject();
                        data.message = "Ya se le ha enviado un PIN para restablecer su contraseña, revise su buzón de correo";
                        data.response = 4;
                        data.value = 4;
                        return Ok(data);
                    }
                    else if (estadoUsuario != 1) //Not found email
                    {
                        data = new JObject();
                        data.message = "No se ha encontrado la direccion de correo ingresada";
                        data.response = 5;
                        data.value = 5;
                        return Ok(data);
                    }

                    cmd.Parameters.Clear();
                    adapter.Dispose();
                    setter.Dispose();
                    setter.Tables.Clear();

                    //transaction to send notification about password reset

                    cmd.CommandText = "crud_configuracion_correo";
                    cmd.Parameters.AddWithValue("@opcion", 4);

                    SqlTransaction transaction = conn.BeginTransaction("sendPinTransaction");
                    cmd.Transaction = transaction;

                    try
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(setter, "emailServer");

                        if (setter.Tables["emailServer"] == null)
                        {
                            transaction.Rollback();
                            payback = new Responses(1001, null);
                            return BadRequest(payback.Payback());
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        payback = new Responses(1001, ex.ToString());
                        return BadRequest(payback.Payback());
                    }

                    if (setter.Tables["emailServer"].Rows.Count <= 0)
                    {
                        transaction.Rollback();
                        payback = new Responses(2009, null);
                        return BadRequest(payback.Payback());
                    }

                    string pinEmail = GenerateRandomPinEmail();
                    string hashPinEmail = BCrypt.Net.BCrypt.HashPassword(pinEmail);

                    //get email server data
                    int smtpPort = Int32.Parse(setter.Tables["emailServer"].Rows[0]["smtpPort"].ToString());
                    string smtpServer = setter.Tables["emailServer"].Rows[0]["smtpServer"].ToString();
                    string passEmailAccount = setter.Tables["emailServer"].Rows[0]["passwordEmailAccount"].ToString();
                    string emailServerAccount = setter.Tables["emailServer"].Rows[0]["emailServerAccount"].ToString();

                    var mailMessage = new MimeMessage();
                    mailMessage.From.Add(new MailboxAddress("no-reply", emailServerAccount));
                    mailMessage.To.Add(new MailboxAddress(email, email));
                    mailMessage.Subject = "Notificación de reseteo de contraseña";
                    var bodyBuilder = new BodyBuilder();
                    string messageBody;

                    using (StreamReader reader = System.IO.File.OpenText(_resetPasswordTemplate))
                    {
                        messageBody = reader.ReadToEnd();
                        bodyBuilder.HtmlBody = string.Format(Regex.Replace(messageBody, @"[\r\n\t ]+", " "),
                                                email,
                                                pinEmail,
                                                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        reader.Close();
                        reader.Dispose();
                    }

                    using (var smtpClient = new SmtpClient())
                    {

                        try
                        {
                            smtpClient.Connect(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            payback = new Responses(10001, ex.ToString());
                            return BadRequest(payback.Payback());
                        }

                        try
                        {
                            smtpClient.Authenticate(emailServerAccount, passEmailAccount);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            payback = new Responses(10002, ex.ToString());
                            return BadRequest(payback.Payback());
                        }

                        cmd.Parameters.Clear();
                        setter.Tables.Clear();
                        setter.Dispose();
                        adapter.Dispose();


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "adm.crudUsuario";
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        cmd.Parameters.AddWithValue("@password", hashPinEmail);
                        cmd.Parameters.AddWithValue("@usuario", idPersona);
                        cmd.Parameters.AddWithValue("@estado", 2);
                        cmd.Parameters.AddWithValue("@opcion", 7);

                        try
                        {
                            int result = cmd.ExecuteNonQuery();
                            if (result >= 1)
                            {
                                try
                                {
                                    //send message and commit
                                    mailMessage.Body = bodyBuilder.ToMessageBody();
                                    smtpClient.Send(mailMessage);
                                    smtpClient.Disconnect(true);
                                    smtpClient.Dispose();

                                    data = new JObject();
                                    data.message = "Se le ha enviado un PIN a su correo para poder restablecer su contraseña";
                                    data.value = 1;
                                    data.response = 1;
                                    transaction.Commit();
                                    return Ok(data);
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    payback = new Responses(10003, ex.ToString());
                                    return BadRequest(payback.Payback());
                                }
                            }
                            else
                            {
                                data = new JObject();
                                data.message = "Ha ocurrido un problema con la solicitud para restablecer la contraseña";
                                data.value = 1;
                                data.response = 1;
                                transaction.Rollback();
                                return Ok(data);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            payback = new Responses(10004, ex.ToString());
                            return BadRequest(payback.Payback());
                        }
                    }

                }
            }
        }
        private string GenerateRandomPinEmail()
        {
            int randomPin = new Random().Next(100002, 999998);
            string pinToken = randomPin.ToString();

            return pinToken;
        }


    }
}
