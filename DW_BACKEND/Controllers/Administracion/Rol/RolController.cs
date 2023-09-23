﻿using dw_backend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;

namespace dw_backend.Controllers.Administracion.Rol
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly string _connectionString;
        private readonly string _nameProcedure;

        public RolController(IConfiguration _configuration)
        {
            Configuration = _configuration;
            _connectionString = Configuration.GetConnectionString("MainConnection");
            _nameProcedure = "adm.crudRol";
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

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
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
        [Route("one")]
        //[Authorize]
        public IActionResult One(JObject request)
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

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 5);
                    cmd.Parameters.AddWithValue("@id", id);

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
        [Route("store")]
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

                string nombre = request.GetValue("nombre").ToString();
                string usuario = request.GetValue("usuario").ToString();

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 1);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
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
                    resultado.message = "Rol agregado con éxito.";
                    resultado.value = 1;

                    return Ok(resultado);
                }
            }
        }

        [HttpPost]
        [Route("update")]
        //[Authorize]
        public IActionResult Update(JObject request)
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
                string nombre = request.GetValue("nombre").ToString();
                string usuario = request.GetValue("usuario").ToString();

                using (SqlCommand cmd = new SqlCommand(_nameProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@opcion", 2);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
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
                    resultado.message = "Rol actualizado con éxito.";
                    resultado.value = 1;

                    return Ok(resultado);
                }
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
                    resultado.message = "Rol se ha dado de baja con éxito.";
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
                    resultado.message = "Rol se ha dado de alta con éxito.";
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
    }
}