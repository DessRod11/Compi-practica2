using System;
using System.Collections.Generic;

namespace Compilador
{
    public class TablaSimbolos
    {
        private readonly Dictionary<string, (string Tipo, int Linea)> _simbolos = new Dictionary<string, (string Tipo, int Linea)>();

        public bool Agregar(string nombre, string tipo, int linea)
        {
            if (_simbolos.ContainsKey(nombre.ToUpper()))
                return false;

            _simbolos.Add(nombre.ToUpper(), (tipo.ToUpper(), linea));
            return true;
        }

        public string ObtenerTipo(string nombre)
        {
            if (_simbolos.TryGetValue(nombre.ToUpper(), out var info))
                return info.Tipo;
            return "ERROR";
        }

        public bool Existe(string nombre)
        {
            return _simbolos.ContainsKey(nombre.ToUpper());
        }

        public Dictionary<string, (string Tipo, int Linea)> ObtenerSimbolos()
        {
            return new Dictionary<string, (string Tipo, int Linea)>(_simbolos);
        }

        public void Limpiar()
        {
            _simbolos.Clear();
        }
    }
   
}
