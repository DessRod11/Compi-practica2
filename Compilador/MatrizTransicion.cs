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
        // 2 = Espacio en blanco
        // 3 = Otro símbolo (por ejemplo: operadores, delimitadores, etc.)

        // Estados o RENGLONeS:
        // 0 = inicial
        // 1 = leyendo ID / palabra reservada
        // 2 = leyendo número entero

        // Valores ACEPTADOS:
        // 100 = aceptar ID/palabra reservada
        // 200 = aceptar número entero
        // >500 = error léxico

        private readonly int[,] _matriz =
        {    

              //    L     D      .     OP    Sim   ESP
            /*0*/ { 1,    2,    500,  3,    300,   0  }, // Inicio
            /*1*/ { 1,    1,    100,  100,  100,  100 }, // ID o Palabra Reservada
            /*2*/ { 200,  2,    4,    200,  200,  200 }, // Numero Entero
            /*3*/ { 300,  300,  300,  6,    300,  300 }, // Operador simple
            /*4*/ { 500,  5,    500,  500,  500,  500 }, // Punto decimal 
            /*5*/ { 400,  5,    400,  400,  400,  400 }, // Numero Real
            /*6*/ { 300,  300,  300,  305,  300,  300 }, // Operador compuesto
           
            

        };



        public int ObtenerColumna(char c)
        {
            if (char.IsLetter(c) || c == '_')
                return 0; // columna de letras/underscore

            if (char.IsDigit(c))
                return 1; // dígito

            if (char.IsWhiteSpace(c))
                return 2; // espacios

            if ("+-*;;:{}() ,\'".Contains(c))
                return 4;

            if (c == '.')
                return 5;

            if ("=<>/".Contains(c)) 
                return 6;


            return 3; // otros caracteres 

        }

        public int SiguienteEstado(int estado, int columna)
        {
            // Segun la cantidad de estados
            if (estado < 0 || estado > _matriz.GetUpperBound(0))
            {
                return 500;
            }

            if (estado < 0 || estado >= _matriz.GetLength(0)) return 500;

           
            if (columna < 0 || columna >= _matriz.GetLength(1)) return 500;

            return _matriz[estado, columna];
        }


    }
}
