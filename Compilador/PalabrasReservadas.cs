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
            { "PROGRAM",   102 },
            { "VAR",       103 },
            { "INT",       104 },   // O usa INTEGER
            { "FLOAT",     105 },
            { "BEGIN",     106 },
            { "END",       107 },
            { "IF",        108 },
            { "THEN",      109 },
            { "ELSE",      110 },
            { "WHILE",     111 },
            { "DO",        112 },
            { "WRITE",     113 },
            { "WRITELN",   114 },
            { "PRINT",     115 },
            { "PROCEDURE", 116 },

            // Operadores y símbolos
            { "=",         301 },
            { ":=",        302 },
            { ";",         303 },
            { "+",         304 },
            { "-",         305 },
            { "*",         306 },
            { "/",         307 },
            { ">",         308 },
            { "<",         309 },
            { ">=",        310 },
            { "<=",        311 },
            { "<>",        312 },
            { "(",         313 },
            { ")",         314 },
            { ":",         315 },
            { ",",         316 },
            { ".",         317 },

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
