using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Conexion
{
    public class Parametro
    {

        public Parametro(String nombre, object valor)
        {
            this.nombre = nombre;
            this.valor = valor;
            this.direction = ParameterDirection.Input;
        }
        public Parametro(String nombre, MySqlDbType tipoDato, int tamanyo)
        {
            this.nombre = nombre;
            this.tipoDato = tipoDato;
            this.tamanyo = tamanyo;
            this.direction = ParameterDirection.Output;
        }

        public String myNombre
        {
            set { nombre = value; }
            get { return nombre; }
        }
        public Object myValor
        {
            set { valor = value; }
            get { return valor; }
        }
        public MySqlDbType myTipoDato
        {
            set { tipoDato = value; }
            get { return tipoDato; }
        }
        public int myTamanyo
        {
            set { tamanyo = value; }
            get { return tamanyo; }
        }
        public ParameterDirection myDireccion
        {
            set { direction = value; }
            get { return direction; }
        }

        private String nombre;
        private Object valor;
        private MySqlDbType tipoDato;
        private int tamanyo;
        private ParameterDirection direction;

    }
}
