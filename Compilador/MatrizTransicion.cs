using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class MatrizTransicion
    {
        // Columnas:
        // 0 = Letra o '_'
        // 1 = Dígito
        // 2 = Punto '.'
        // 3 = Dos puntos ':'
        // 4 = Menor que '<'
        // 5 = Mayor que '>'
        // 6 = Igual '='
        // 7 = Otros símbolos (+ - * / ( ) ; ,)
        // 8 = Espacio en blanco / Otros
        
        // Estados:
        // 0  = Inicial
        // 1  = Leyendo ID
        // 2  = Leyendo Entero
        // 3  = Punto decimal (después de dígito)
        // 4  = Leyendo Real
        // 5  = Leyendo ':'
        // 6  = Leyendo ':=' (Aceptación inmediata)
        // 7  = Leyendo '<'
        // 8  = Leyendo '<=' (Aceptación inmediata)
        // 9  = Leyendo '<>' (Aceptación inmediata)
        // 10 = Leyendo '>'
        // 11 = Leyendo '>=' (Aceptación inmediata)
        // 12 = Símbolo simple (Aceptación inmediata)

        // Estados de Aceptación:
        // 100 = ID / Palabra Reservada (Requiere i--)
        // 200 = Entero (Requiere i--)
        // 300 = Símbolo / Operador (Requiere i--)
        // 301 = Símbolo / Operador (Aceptación sin i--)
        // 400 = Real (Requiere i--)
        // 500 = Error

        private readonly int[,] _matriz =
        {
        //        L    D    .    :    <    >    =    S    WS
        /*0*/ {   1,   2,  301,   5,   7,  10,  301, 301,   0 },
        /*1*/ {   1,   1,  100, 100, 100, 100, 100, 100, 100 },
        /*2*/ { 200,   2,    3, 200, 200, 200, 200, 200, 200 },
        /*3*/ { 500,   4,  500, 500, 500, 500, 500, 500, 500 },
        /*4*/ { 400,   4,  400, 400, 400, 400, 400, 400, 400 },
        /*5*/ { 300, 300,  300, 300, 300, 300,  301, 300, 300 },
        /*6*/ { 301, 301,  301, 301, 301, 301, 301, 301, 301 }, // No se usa pero se deja por estructura
        /*7*/ { 300, 300,  300, 300, 300,  301, 301, 300, 300 },
        /*8*/ { 301, 301,  301, 301, 301, 301, 301, 301, 301 },
        /*9*/ { 301, 301,  301, 301, 301, 301, 301, 301, 301 },
        /*10*/{ 300, 300,  300, 300, 300, 300,  301, 300, 300 }
        };

        public int ObtenerColumna(char c)
        {
            if (char.IsLetter(c) || c == '_') return 0;
            if (char.IsDigit(c)) return 1;
            if (c == '.') return 2;
            if (c == ':') return 3;
            if (c == '<') return 4;
            if (c == '>') return 5;
            if (c == '=') return 6;
            if ("+-*/();,".Contains(c)) return 7;
            if (char.IsWhiteSpace(c)) return 8;
            return 8; // Otros se tratan como espacio para cerrar tokens
        }

        public int SiguienteEstado(int estado, int columna)
        {
            if (estado < 0 || estado >= _matriz.GetLength(0)) return 500;
            if (columna < 0 || columna >= _matriz.GetLength(1)) return 500;
            return _matriz[estado, columna];
        }
    }
}
