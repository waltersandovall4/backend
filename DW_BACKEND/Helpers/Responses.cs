using Newtonsoft.Json.Linq;

namespace dw_backend.Helpers
{
    public class Responses
    {
        private int val;
        private string valException;

#nullable enable
        public Responses(int value, string? ex)
        {
            val = value;
            valException = string.IsNullOrEmpty(ex) ? value.ToString() : ex;
        }

        public JObject Payback()
        {
            dynamic data = new JObject();

            switch (val)
            {
                //success code
                case 1:
                    data.value = valException;
                    data.message = "Proceso realizado con éxito.";
                    data.response = 1;
                    return data;

                // database codes 1000 - 1999
                case 1001:
                    data.value = valException;
                    data.message = "No se ha podido conectar con la fuente de datos.";
                    data.response = 1001;
                    return data;

                case 1002:
                    data.value = valException;
                    data.message = "Problema interno con la fuente de datos.";
                    data.response = 1002;
                    return data;

                case 1003:
                    data.value = valException;
                    data.message = "No se ha podido almacenar el registro en la fuente de datos.";
                    data.response = 1003;
                    return data;

                // empty or null codes 2000 -2999
                case 2001:
                    data.value = 2001;
                    data.message = "El valor del identificador no puede ir vacío.";
                    data.response = 2001;
                    return data;

                case 2002:
                    data.value = 2002;
                    data.message = "El valor inicial no puede ir vacío.";
                    data.response = 2002;
                    return data;

                case 2003:
                    data.value = 2003;
                    data.message = "El valor final no puede ir vacío.";
                    data.response = 2003;
                    return data;

                case 2004:
                    data.value = 2004;
                    data.message = "Debe definir el campo del dato inicial.";
                    data.response = 2004;
                    return data;

                case 2005:
                    data.value = 2005;
                    data.message = "Debe definir el campo del dato final.";
                    data.response = 2005;
                    return data;

                case 2006:
                    data.value = 2006;
                    data.message = "Debe definir el campo del valor de estación.";
                    data.response = 2006;
                    return data;

                case 2007:
                    data.value = 2007;
                    data.message = "Debe definir el campo del valor del tamaño de imagen.";
                    data.response = 2007;
                    return data;

                case 2008:
                    data.value = 2008;
                    data.message = "El valor del tamaño de la imagen no puede ir vacío.";
                    data.response = 2008;
                    return data;

                case 2009:
                    data.value = 2009;
                    data.message = "No se han encontrado datos relacionados con la búsqueda.";
                    data.response = 2009;
                    return data;

                case 2010:
                    data.value = 2010;
                    data.message = "Uno o más parámetros requeridos no contienen valor.";
                    data.response = 2010;
                    return data;

                case 2011:
                    data.value = 2011;
                    data.message = "Parámetro principal es requerido.";
                    data.response = 2011;
                    return data;

                //conversion codes 3000 - 3999
                case 3001:
                    data.value = valException;
                    data.message = "No se ha podido realizar la conversión al tipo de dato entero.";
                    data.response = 3001;
                    return data;

                case 3002:
                    data.value = 3002;
                    data.message = "No se ha podido realizar la conversión al tipo de dato entero.";
                    data.response = 3002;
                    return data;

                case 3003:
                    data.value = valException;
                    data.message = "No se ha podido realizar la conversión al tipo de dato entero.";
                    data.response = 3003;
                    return data;

                case 3004:
                    data.value = 3004;
                    data.message = "No se ha podido realizar la conversión al tipo de dato string.";
                    data.response = 3004;
                    return data;

                //request codes 4000 - 4999
                case 4001:
                    data.value = valException;
                    data.message = "No se ha podido realizar la petición al recurso de origen.";
                    data.response = 4001;
                    return data;

                //length codes 5000 - 5999
                case 5001:
                    data.value = 5001;
                    data.message = "No existe dimension de datos relacionados con la búsqueda.";
                    data.response = 5001;
                    return data;

                case 5002:
                    data.value = valException;
                    data.message = "No se ha encontrado dimension datos en la respuesta.";
                    data.response = 5002;
                    return data;

                //files codes 6000 - 6999
                case 6001:
                    data.value = valException;
                    data.message = "No se ha podido descargar el recurso solicitado.";
                    data.response = 6001;
                    return data;

                case 6002:
                    data.value = valException;
                    data.message = "No se ha encontrado el archivo solicitado.";
                    data.response = 6002;
                    return data;
                //null codes 7000 - 7999
                case 7001:
                    data.value = 7001;
                    data.message = "No existen datos relacionados con la búsqueda.";
                    data.response = 7001;
                    return data;

                case 7002:
                    data.value = 7002;
                    data.message = "Uno o mas campos requeridos no fueron enviados.";
                    data.response = 7002;
                    return data;

                //existing - codes 8000 - 8999
                case 8001:
                    data.value = 8001;
                    data.message = "El registro a ingresar ya se encuentra dentro del sistema.";
                    data.response = 8001;
                    return data;
                //Codes mail server config
                case 10002:
                    data.message = "Credenciales inválidas para servidor de correo.";
                    data.response = 10002;
                    data.value = 10002;
                    return data;

                case 10003:
                    data.message = "Error al construir y enviar correo.";
                    data.response = 10003;
                    data.value = 10003;
                    return data;

                case 10004:
                    data.message = "No se ha podido realizar la petición al recurso de origen.";
                    data.response = 10004;
                    data.value = 10004;
                    return data;

                default:
                    data.value = 0;
                    data.message = "Proceso no realizado.";
                    data.response = 0;
                    return data;
                    //break;
            }
        }
    }
}
