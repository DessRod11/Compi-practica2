using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class AnalizadorLexico
    {
        private readonly MatrizTransicion _matriz;
        private readonly PalabrasReservadas _palabrasReservadas;
        private readonly HashSet<int> _lineasConError = new HashSet<int>();

        public AnalizadorLexico()
        {
            _matriz = new MatrizTransicion();
            _palabrasReservadas = new PalabrasReservadas();
        }

        public ResultadoLexico Analizar(CodigoFuente fuente)
        {
            var resultado = new ResultadoLexico();
            _lineasConError.Clear();
            resultado.AgregarAviso("Ha iniciado el léxico");

            try
            {
                for (int numLinea = 1; numLinea <= fuente.NumeroLineas; numLinea++)
                {
                    ProcesarLinea(fuente.ObtenerLinea(numLinea), numLinea, resultado);
                }
                resultado.AgregarAviso("Análisis léxico realizado con éxito");
            }
            catch (Exception ex)
            {
                resultado.AgregarAviso("ERROR GRAVE EN LÉXICO: " + ex.Message);
            }

            return resultado;
        }

        private void ProcesarLinea(string lineaOriginal, int numLinea, ResultadoLexico resultado)
        {
            int estado = 0;
            var lexema = new StringBuilder();
            string linea = (lineaOriginal ?? string.Empty) + " "; 

            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];

                // Manejo especial de comentarios de una sola línea
                if (estado == 0 && c == '/' && i + 1 < linea.Length && linea[i + 1] == '/')
                {
                    resultado.AgregarAviso($"Comentario en línea {numLinea}: {linea.Substring(i).Trim()}");
                    break;
                }

                int columna = _matriz.ObtenerColumna(c);
                int siguiente = _matriz.SiguienteEstado(estado, columna);

                if (siguiente == 500)
                {
                    if (!_lineasConError.Contains(numLinea))
                    {
                        resultado.AgregarAviso($"ERROR LÉXICO: Símbolo '{c}' no reconocido en línea {numLinea}");
                        _lineasConError.Add(numLinea);
                    }
                    estado = 0;
                    lexema.Clear();
                    continue;
                }

                if (siguiente >= 100)
                {
                    // Estado de aceptación alcanzado
                    if (siguiente != 301) // Si no es aceptación inmediata, no incluir el carácter actual
                    {
                        // El lexema ya está completo, i-- para procesar 'c' en el siguiente token
                        i--;
                    }
                    else
                    {
                        // Aceptación inmediata, incluir el carácter actual
                        lexema.Append(c);
                    }

                    string lex = lexema.ToString().Trim();
                    if (!string.IsNullOrEmpty(lex))
                    {
                        int tokenID = DeterminarTokenID(siguiente, lex);
                        resultado.Tokens.Add(new Token(tokenID, lex, numLinea));
                        Console.WriteLine($"Token: {tokenID} | Lexema: {lex} | Línea: {numLinea}");
                    }
                    
                    lexema.Clear();
                    estado = 0;
                }
                else
                {
                    // Estado de transición
                    if (siguiente != 0 || !char.IsWhiteSpace(c))
                    {
                        lexema.Append(c);
                    }
                    estado = siguiente;
                }
            }
        }

        private int DeterminarTokenID(int estadoAceptacion, string lexema)
        {
            switch (estadoAceptacion)
            {
                case 100: // ID o Palabra Reservada
                    return _palabrasReservadas.ObtenerToken(lexema);
                case 200: // Entero
                    return 200;
                case 400: // Real
                    return 202; // TKN_REAL en AnalizadorSintactico
                case 300: // Símbolo / Operador
                case 301:
                    return _palabrasReservadas.ObtenerToken(lexema);
                default:
                    return 500; // Error
            }
        }
    }
}