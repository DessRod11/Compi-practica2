using System;
using System.Collections.Generic;

namespace Compilador
{
    public class TablaSimbolos
    {
        private readonly Dictionary<string, string> _simbolos = new Dictionary<string, string>();

        public bool Agregar(string nombre, string tipo)
        {
            if (_simbolos.ContainsKey(nombre.ToUpper()))
                return false;

            _simbolos.Add(nombre.ToUpper(), tipo.ToUpper());
            return true;
        }

        public string ObtenerTipo(string nombre)
        {
            if (_simbolos.TryGetValue(nombre.ToUpper(), out string tipo))
                return tipo;
            return "ERROR";
        }

        public bool Existe(string nombre)
        {
            return _simbolos.ContainsKey(nombre.ToUpper());
        }

        public void Limpiar()
        {
            _simbolos.Clear();
        }
    }
}
