using System;
namespace GuanajuatoAdminUsuarios.Models
{
    public class SeleccionGruaModel
    {
        public string numeroEconomico { get; set; }
        public DateTime fechaArribo { get; set; }
        

         public TimeSpan horaArribo{ get; set; }
        public TimeSpan horaInicio { get; set; }
        public TimeSpan horaTermino { get; set; }

        public DateTime fechaInicio { get; set; }
        public DateTime fechaFinal { get; set; }

        public int EstadoSalvamento { get; set; }
        public int EstadoArrastre { get; set; }
        public int EstadoAbanderamiento { get; set; }
        public int idAsignacion { get; set; }
        
        public bool SalvamentoBool => EstadoSalvamento == 1;
        public bool ArrastreBool => EstadoArrastre == 1;
        public bool AbanderamientoBool => EstadoAbanderamiento == 1;

        public string Placas { get; set; }
        public string TipoGrua { get; set; }
        public int tiempoManiobras { get; set; }
        
        public string operadorGrua { get; set; }
        public int idGrua { get; set; }
        public string fullGrua
        {
            get
            {
                return "No. Económico: " + numeroEconomico + Environment.NewLine +
                       "Tipo Grua: " + TipoGrua + Environment.NewLine +
                       "Placas: " + Placas + Environment.NewLine +
                       "Operador: " + operadorGrua;
            }
        }
        public string servicios
        {
            get
            {
                string result = "";

                if (EstadoAbanderamiento == 1)
                {
                    result += "Abanderamiento" + Environment.NewLine;
                }

                if (EstadoArrastre == 1)
                {
                    result += "Arrastre" + Environment.NewLine;
                }

                if (EstadoSalvamento == 1)
                {
                    result += "Salvamento" + Environment.NewLine;
                }

                return result;
            }
        }

        public string Abanderamiento
        {
            get
            {
                if (EstadoAbanderamiento == 1)
                    return "▶";
                else
                    return "_";
            }
        }

        public string Salvamento
        {
            get
            {
                if (EstadoSalvamento == 1)
                    return "▶";
                else
                    return "_";
            }
        }


        public string Arrastre
        {
            get
            {
                if (EstadoArrastre == 1)
                    return "▶";
                else
                    return "_";
            }
        }

        public string tiempoServicio
        {
            get
            {
                TimeSpan tiempoTranscurrido = fechaFinal - fechaInicio;

                return "Arribo: " + fechaArribo + Environment.NewLine +
                       "Inicio: " + fechaInicio + Environment.NewLine +
                       "Término: " + fechaFinal + Environment.NewLine +
                       "Tiempo: " + tiempoTranscurrido.TotalMinutes + " minutos";
            }
        }


        public string arribo
        {
            get
            {
                return fechaArribo.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        public string inicio
        {
            get
            {
                return fechaInicio.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        public string fin
        {
            get
            {
                return fechaFinal.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        public string tiempoTranscurrido
        {
            get
            {
                TimeSpan _tiempoTranscurrido = fechaFinal - fechaInicio;

                return _tiempoTranscurrido.TotalMinutes.ToString();
            }
        }


    }
}
