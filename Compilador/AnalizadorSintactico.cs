using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class AnalizadorSintactico
    {
        private List<Token> _tokens = new List<Token>();
        private int _posicionActual;
        private Token _tokenActual = null!;
        public List<string> Errores = new List<string>();
        private TablaSimbolos _tablaSimbolos = new TablaSimbolos();

        public TablaSimbolos TablaSimbolos => _tablaSimbolos;

        // Constantes para tokens
        private const int TKN_ID = 101;
        private const int TKN_PROGRAM = 102;
        private const int TKN_VAR = 103;
        private const int TKN_INT = 104;
        private const int TKN_FLOAT = 105;
        private const int TKN_BEGIN = 106;
        private const int TKN_END = 107;
        private const int TKN_IF = 108;
        private const int TKN_THEN = 109;
        private const int TKN_ELSE = 110;
        private const int TKN_WHILE = 111;
        private const int TKN_DO = 112;
        private const int TKN_WRITE = 113;
        private const int TKN_WRITELN = 114;
        private const int TKN_PRINT = 115;
        private const int TKN_PROCEDURE = 116;

        private const int TKN_ASIGNACION = 302;  
        private const int TKN_PUNTOYCOMA = 303;
        private const int TKN_PUNTO = 317;

        private const int TKN_ENTERO = 200;
        private const int TKN_REAL = 202;
        private const int TKN_COMENTARIO = 400;

        public void Parse(List<Token> tokensTotales)
        {
            _tokens = tokensTotales.Where(t => t.Tipo != TKN_COMENTARIO).ToList();
            Errores.Clear();
            _tablaSimbolos.Limpiar();
            _posicionActual = 0;

            if (_tokens.Count == 0)
            {
                Errores.Add("El código fuente está vacío o no generó tokens válidos.");
                return;
            }

            _tokenActual = _tokens[_posicionActual];
            try
            {
                ParserPrograma();

                if (_posicionActual < _tokens.Count && _tokenActual.Tipo != -1)
                {
                    Errores.Add($"Error sintáctico en línea {_tokenActual.Linea}: Tokens inesperados después del fin del programa ('{_tokenActual.Lexema}').");
                }
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
            }
        }

        private void ParserPrograma()
        {
            MatchTipo(TKN_PROGRAM, "Se esperaba 'PROGRAM'");
            MatchTipo(TKN_ID, "Se esperaba identificador del programa");
            MatchLexema(";", "Falta ';' después del identificador del programa");

            ParserBloque();

            MatchLexema(".", "Falta '.' al finalizar el programa");
        }

        private void ParserBloque()
        {
            if (_tokenActual.Tipo == TKN_VAR)
            {
                ParserDeclaracionesVariables();
            }

            while (_tokenActual.Tipo == TKN_PROCEDURE)
            {
                ParserDeclaracionProcedimiento();
            }

            MatchTipo(TKN_BEGIN, "Se esperaba 'BEGIN'");
            ParserInstrucciones();
            MatchTipo(TKN_END, "Se esperaba 'END'");
        }

        private void ParserDeclaracionesVariables()
        {
            MatchTipo(TKN_VAR, "Se esperaba 'VAR'");

            while (_tokenActual.Tipo == TKN_ID)
            {
                List<string> nombres = new List<string>();
                nombres.Add(_tokenActual.Lexema);
                MatchTipo(TKN_ID, "Se esperaba identificador de variable");

                while (CheckLexema(","))
                {
                    Avanzar(); 
                    nombres.Add(_tokenActual.Lexema);
                    MatchTipo(TKN_ID, "Se esperaba identificador después de la ','");
                }

                MatchLexema(":", "Falta ':' en la declaración de variable");
                string tipo = _tokenActual.Lexema; 
                int lineaDeclaracion = _tokenActual.Linea;
                ParserTipo();
                
                foreach (var n in nombres)
                {
                    if (!_tablaSimbolos.Agregar(n, tipo, lineaDeclaracion))
                    {
                        Errores.Add($"Error Semántico en línea {lineaDeclaracion}: La variable '{n}' ya ha sido declarada.");
                    }
                }

                MatchLexema(";", "Falta ';' al final de la declaración de variable");
            }
        }

        private void ParserTipo()
        {
            if (_tokenActual.Tipo == TKN_INT || _tokenActual.Tipo == TKN_FLOAT)
            {
                Avanzar(); 
            }
            else
            {
                throw new Exception($"Error sintáctico en línea {_tokenActual.Linea}: Se esperaba un tipo de dato válido (INT o FLOAT).");
            }
        }

        private void ParserDeclaracionProcedimiento()
        {
            MatchTipo(TKN_PROCEDURE, "Se esperaba 'PROCEDURE'");
            MatchTipo(TKN_ID, "Se esperaba nombre del procedimiento");

            if (CheckLexema("("))
            {
                Avanzar(); 
                if (_tokenActual.Tipo == TKN_ID)
                {
                    string nombreParam = _tokenActual.Lexema;
                    int lineaParam = _tokenActual.Linea;
                    MatchTipo(TKN_ID, "Identificador de parámetro");
                    MatchLexema(":", "Falta ':'");
                    string tipoParam = _tokenActual.Lexema;
                    ParserTipo();
                    _tablaSimbolos.Agregar(nombreParam, tipoParam, lineaParam);
                }
                MatchLexema(")", "Falta ')'");
            }

            MatchLexema(";", "Falta ';' después de cabecera de procedimiento");
            ParserBloque();
            MatchLexema(";", "Falta ';' después de cuerpo de procedimiento");
        }

        private void ParserInstrucciones()
        {
            ParserInstruccion();
            while (CheckLexema(";"))
            {
                Avanzar(); 
                if (_tokenActual.Tipo != TKN_END) 
                {
                    ParserInstruccion();
                }
            }
        }

        private void ParserInstruccion()
        {
            if (_tokenActual.Tipo == TKN_ID)
            {
                string nombreVar = _tokenActual.Lexema;
                int linea = _tokenActual.Linea;
                Avanzar(); 

                if (!_tablaSimbolos.Existe(nombreVar))
                {
                    Errores.Add($"Error Semántico en línea {linea}: La variable '{nombreVar}' no ha sido declarada.");
                }

                MatchLexema(":=", "Se esperaba operador de asignación ':='");
                string tipoExp = ParserExpresion();
                string tipoVar = _tablaSimbolos.ObtenerTipo(nombreVar);

                if (tipoVar != "ERROR" && tipoExp != "ERROR" && tipoVar != tipoExp)
                {
                    Errores.Add($"Error Semántico en línea {linea}: Tipos no coinciden. No se puede asignar {tipoExp} a variable {tipoVar} ('{nombreVar}').");
                }
            }
            else if (_tokenActual.Tipo == TKN_IF)
            {
                Avanzar(); 
                ParserExpresion();
                MatchTipo(TKN_THEN, "Se esperaba 'THEN'");
                ParserInstruccion();

                if (_tokenActual.Tipo == TKN_ELSE)
                {
                    Avanzar(); 
                    ParserInstruccion();
                }
            }
            else if (_tokenActual.Tipo == TKN_WHILE)
            {
                Avanzar(); 
                ParserExpresion();
                MatchTipo(TKN_DO, "Se esperaba 'DO'");
                ParserInstruccion();
            }
            else if (_tokenActual.Tipo == TKN_BEGIN)
            {
                Avanzar(); 
                ParserInstrucciones();
                MatchTipo(TKN_END, "Se esperaba 'END' en bloque anidado");
            }
            else if (_tokenActual.Tipo == TKN_WRITE || _tokenActual.Tipo == TKN_WRITELN || _tokenActual.Tipo == TKN_PRINT)
            {
                Avanzar(); 
                MatchLexema("(", "Falta '(' para función de impresión");
                ParserExpresion();
                while (CheckLexema(","))
                {
                    Avanzar();
                    ParserExpresion();
                }
                MatchLexema(")", "Falta ')' en la función de impresión");
            }
        }

        private string ParserExpresion()
        {
            string tipo1 = ParserTermino();
            while (CheckLexema("+") || CheckLexema("-") || CheckLexema("==") || CheckLexema(">") ||
                   CheckLexema("<") || CheckLexema(">=") || CheckLexema("<=") || CheckLexema("<>"))
            {
                Avanzar(); 
                string tipo2 = ParserTermino();
                if (tipo1 != tipo2) tipo1 = "FLOAT"; // Promoción simple
            }
            return tipo1;
        }

        private string ParserTermino()
        {
            string tipo1 = ParserFactor();
            while (CheckLexema("*") || CheckLexema("/"))
            {
                Avanzar(); 
                string tipo2 = ParserFactor();
                if (tipo1 != tipo2) tipo1 = "FLOAT";
            }
            return tipo1;
        }

        private string ParserFactor()
        {
            if (_tokenActual.Tipo == TKN_ID)
            {
                string nombre = _tokenActual.Lexema;
                int linea = _tokenActual.Linea;
                if (!_tablaSimbolos.Existe(nombre))
                {
                    Errores.Add($"Error Semántico en línea {linea}: La variable '{nombre}' no ha sido declarada.");
                    Avanzar();
                    return "ERROR";
                }
                string tipo = _tablaSimbolos.ObtenerTipo(nombre);
                Avanzar();
                return tipo;
            }
            else if (_tokenActual.Tipo == TKN_ENTERO)
            {
                Avanzar();
                return "INT";
            }
            else if (_tokenActual.Tipo == TKN_REAL)
            {
                Avanzar();
                return "FLOAT";
            }
            else if (CheckLexema("("))
            {
                Avanzar();
                string tipo = ParserExpresion();
                MatchLexema(")", "Falta ')'");
                return tipo;
            }
            else
            {
                throw new Exception($"Error sintáctico en línea {_tokenActual.Linea}: Se esperaba un identificador, número o expresión entre paréntesis.");
            }
        }

        private void Avanzar()
        {
            _posicionActual++;
            if (_posicionActual < _tokens.Count)
            {
                _tokenActual = _tokens[_posicionActual];
            }
            else
            {
                _tokenActual = new Token(-1, "EOF", _tokenActual?.Linea ?? 0);
            }
        }

        private void MatchTipo(int tipoEsperado, string mensajeError)
        {
            if (_tokenActual.Tipo == tipoEsperado)
            {
                Avanzar();
            }
            else
            {
                throw new Exception($"Error sintáctico en línea {_tokenActual.Linea}: {mensajeError}. Encontrado '{_tokenActual.Lexema}'.");
            }
        }

        private void MatchLexema(string lexemaEsperado, string mensajeError)
        {
            if (_tokenActual.Lexema == lexemaEsperado)
            {
                Avanzar();
            }
            else
            {
                throw new Exception($"Error sintáctico en línea {_tokenActual.Linea}: {mensajeError}. Encontrado '{_tokenActual.Lexema}'.");
            }
        }

        private bool CheckLexema(string lexemaEsperado)
        {
            return _tokenActual != null && _tokenActual.Lexema == lexemaEsperado;
        }
    }
}
