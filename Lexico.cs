





using System.Runtime.InteropServices;

namespace Lexico2
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        int linea;

        const int F = -1;
        const int E = -2

        public Lexico()
        {
            linea = 1;
            log     = new StreamWriter("prueba.log");
            asm     = new StreamWriter("prueba.asm");
            log.AutoFlush = true;
            asm.AutoFlush = true;
            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
        }

        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }

        private int automata(char c, int estado)
        {
            int nuevoEstado = estado;

            switch (estado)
            {
            case 0: if (char.IsWhiteSpace(c))
                {
                    nuevoEstado = 0;
                }
                else if (char.IsLetter(c))
                {
                    nuevoEstado = 1;
                
                }
                else if (char.IsDigit(c))
                {
                    nuevoEstado = 2;
                }
                else
                {
                    nuevoEstado = 8;
                }
                break;

            case 1:
                setClasificacion(Tipos.Identificador);
                if (char.IsLetterOrDigit(c))
                {
                    nuevoEstado = 1;
                }
                else
                {
                    nuevoEstado = F;
                }

                break;

            case 2:
                setClasificacion(Tipos.Numero);
                if (char.IsDigit(c))
                {
                    nuevoEstado = 2;
                }
                else if (char == '.')
                {
                    nuevoEstado = 3;
                }
                else if (char,ToLower(c)) == 'e'
                {
                    nuevoEstado = 5;

                }
                else
                {
                    nuevoEstado = F;
                }
                break;
                case 3:
                if (char.IsDigit(c))
                {
                    nuevoEstado = 4;
                }
                else
                {
                    nuevoEstado = E;
                }
                break;
                case 4:
                if (char.IsDigit(c))
                {
                    nuevoEstado = 2;
                }
                else if (char,ToLower(c)) == 'e'
                {
                    nuevoEstado = 5;

                }
                else
                {
                    nuevoEstado = F;
                }
                break;

            case 5:
                if (c == '+' || c == '-')
                {
                    nuevoEstado = 6;
                }
                else if (char.IsDigit(c))
                {
                    nuevoEstado = 7;
                }
                else 
                {
                    nuevoEstado = E;
                }
                break;

            case 6:
                else if (char.IsDigit(c))
                {
                    nuevoEstado = 7;
                }
                else
                {
                    nuevoEstado = E;
                }
                break;

            case 7:
                if (char.IsDigit(c))
                {
                    nuevoEstado = 7;
                }
                else
                {
                    nuevoEstado = F;
                }
                break;

            case 8:
                setClasificacion(Tipos.Caracter);
                if (char.IsDigit(c))
                {
                    nuevoEstado = 8;
                }
                else
                {
                    nuevoEstado = F;
                }
                break;

            case 9:
                setClasificacion(Tipos.InicioBloque);
                if (char.IsDigit(c))
                {
                    nuevoEstado = 9;
                }
                else
                {
                    nuevoEstado = F;
                }
                break;

            case 10:
                setClasificacion(Tipos.FinBloque);
                if (char.IsDigit(c))
                {
                    nuevoEstado = 10;
                }
                else
                {
                    nuevoEstado = F;
                }
                break;

            case 11:
                setClasificacion(Tipos.OperadorTernario);
                if (char.IsDigit(c))
                {
                    nuevoEstado = 11;
                }
                else
                {
                    nuevoEstado = F;
                }
                break;
            
            case 12:
                setClasificacion(Tipos.OperadorTermino);
                if (char.IsDigit(c))
                {
                    nuevoEstado = 12;
                }
                else if (char.IsDigit(c))

            }
            return nuevoEstado;
        }
        

        public void nextToken()
        {
            char transicion;
            string buffer = "";
            int estado = 0;

            while(estado > 0)
            {
                transicion = (char)archivo.Peek();
                estado = automata(transicion,estado);
                if (estado == E)
                {
                    if (getClasificacion()== Tipos.Numero)
                    {
                    throw new Error ("Lexico, se espera un digito", log, linea);
                    }
                }
                if (estado >= 0)
                {
                    archivo.Read();
                    {
                    if (transicion == '\n') 
                        linea++;
                    }
                    if (estado >= 0)
                    {
                        buffer += transicion;
                    }
                }
                
            }


            if (!finArchivo())
            {
                setContenido(buffer);
                log.WriteLine(getContenido() + " = " + getClasificacion());
            }
        }

        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}
/*

Expresion regular: Metodo formal que atraves de una secuencia decaracteres que define un PATRON
de busqueda.


a) Reglas BNF
b) Reglas BNF extendidas
c) Operaciones aplicadas al lenguaje

OAL

--1: Concatenacion simple (·)
2: Concatenacion exponencial (Exponente)
3: Cerradura de Kleene (*)
4: Cerradura positiva (+)
5: Cerradura Epsilom (?)
6: Operador OR (|)
7: Parentesis para agrupaciones ()

L = {A, B, C, D, E, ... Z, a, b, c, d,...,z}
D = {0,1,2,3,4,5,6,7,8,9}

1. L · D ( o se omite el punto)

2. L^3 = LLL
    =2 = ==
    L^3D^2 = LLLDD

3. L* = Cero o mas letras
    D* = Cero o mas digitos

4. L+ = Una o mas letras
    D+ = Una o mas digitos

5. L? = Cero o una letra (La letra es octativa-Opcional)

6. L | D = Letra o Digito
    + | - = + o menos

7. (L D) L? = (Letra seguido de un digito y al final Letra opcional)

Produccion gramatical

Clasificacion del token -> Expresion regular

1 Identificador -> L (L | D)*

aqui va la imagen

2 Numero -> D+ (·D+)? (E(+|-)? D+)?

Caracter ->

FinSentencia-> ;

08/10/24

InicioBloque -> {

FinBloque -> }

OperadorTernario -> ?


Puntero -> ->


OperadorTermino -> + | -

OperadorFactor ->

IncrementoTermino -> ++| +=| --| -=

Termino+ -> + (+ | =)?
Termino- -> - (- | =|>)?

IncrementoFactor -> *=| /= |%=

OperadorFactor -> - | / | %

Factor -> * | / | % (=)?

OperadorLogico -> && | || |!

NotOpRel -> ! (=)?

OperadorRelacional -> > (=)?| <(> | =)? | <= | == |!=

Asignacion -> =

AsgOpRel -> =(=)?

Cadena -> "c*"

Caracter -> 'c' | #D* | Lamda



Automata: Modelo matematico que representa una expresion regular a traves de un GRAFO,
para una maquina de stado finito consiste en unconjunto de estados
bien definidos:
un estado inicial,
un alfabeto de entrada y
una funcion de transicion.

*/
