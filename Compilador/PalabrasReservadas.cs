using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class PalabrasReservadas
    {

        private readonly Dictionary<string, int> _mapa = new Dictionary<string, int>
        {

            //Palabras reservadas 
            { "VAR",        102 },
            { "INTEGER",    103 },
            { "FLOAT",      104 },
            { "IF",         105 },
            { "THEN",       106 },
            { "ELSE",       107 },
            { "WHILE",      108 },

            // Operadores simples y simnbolos
            { "=",          305 },
            { ";",          110 },
            { "/",          111 },
            { "+",          112 },
            { "-",          113 },
            { "*",          114 },
            { ">",          306 },
            { "<",          308 },
            { ":",          117 },
            { "{",          118 },
            { "}",          119 },
            { ",",          120 },

            //Operadores compuestos nuevos agregados
            { "==",         305 },
            { ">=",         307 },
            { "<=",         309 },
            { "'",          310 },

        };

        //101 será el token para ID, PALABRAS, IDENTIFICADORES, VARIABLES
        public int ObtenerToken(string lexema)
        {
            if (string.IsNullOrWhiteSpace(lexema)) 
                return 101;
            var clave = lexema.ToUpperInvariant();
            return _mapa.TryGetValue(clave, out int token)
                ? token
                : 101; // identificador
        }


    }
}
