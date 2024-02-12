using System.CodeDom.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    T_int, T_float, T_string, T_main, T_read, T_write, T_repeat, T_until, T_if, T_elseif, T_else, T_then,
    T_return, T_endl, Dot, Semicolon, Comma, LParanthesis, RParanthesis, RBracket, LBracket, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp, AndOp, OrOp, AssignOp,
    Idenifier, Number
}
namespace JASON_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

   
    public class Scanner
    {
        int type = 0;
        bool flag = true;
        bool flag2 = true;
        bool flag3 = true;
        bool flag4 = true;
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.T_if);
            ReservedWords.Add("main", Token_Class.T_main);
            ReservedWords.Add("endl", Token_Class.T_endl);
            ReservedWords.Add("string", Token_Class.T_string);
            ReservedWords.Add("else", Token_Class.T_else);
            ReservedWords.Add("float", Token_Class.T_float);
            ReservedWords.Add("int", Token_Class.T_int);
            ReservedWords.Add("read", Token_Class.T_read);
            ReservedWords.Add("then", Token_Class.T_then);
            ReservedWords.Add("until", Token_Class.T_until);
            ReservedWords.Add("elseif", Token_Class.T_elseif);
            ReservedWords.Add("return", Token_Class.T_return);
            ReservedWords.Add("write", Token_Class.T_write);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LBracket);
            Operators.Add(")", Token_Class.RBracket);
            Operators.Add("{", Token_Class.LParanthesis);
            Operators.Add("}", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add(":=", Token_Class.AssignOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
        }

        public void StartScanning(string SourceCode)
        {

            for (int i = 0; i < SourceCode.Length; i++)
            {
                type = 0;
                flag4 = true;
                 int j = i;//0
                char CurrentChar = SourceCode[i];//"
                string CurrentLexeme = CurrentChar.ToString();
                if(flag==false)
                {
                    if (CurrentChar != '\r' || CurrentChar == '\n')
                        continue;
                }
                if (flag2 == false)
                {
                    if (CurrentChar != '\r' || CurrentChar == '\n')
                        continue;
                }
                if (flag3 == false)
                {
                    if (CurrentChar != '\r' || CurrentChar == '\n')
                        continue;
                }

                if (CurrentChar == '\r' || CurrentChar == '\n')
                {
                    flag = true;
                    flag2 = true;
                    flag3 = true;
                }
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                {
                    continue;
                }

                //Identifiers or reserved words
                if (CurrentChar >= 'A' && CurrentChar <= 'Z' || CurrentChar >= 'a' && CurrentChar <= 'z')
                {
                    j++;
                    if (j > SourceCode.Length - 1)
                        break;
                    CurrentChar = SourceCode[j];
                    while ((CurrentChar >= 'A' && CurrentChar <= 'Z' || CurrentChar >= 'a' && CurrentChar <= 'z' || CurrentChar >= '0' && CurrentChar <= '9'))
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        if (j > SourceCode.Length - 1)
                            break;
                        CurrentChar = SourceCode[j];
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }

                //number
                //1234
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    j++;
                    if (j > SourceCode.Length - 1)
                        break;
                    CurrentChar = SourceCode[j];

                    while ((CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '.')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        if (j > SourceCode.Length - 1)
                            break;
                        CurrentChar = SourceCode[j];
                    }
                    i = j - 1;
                    if (j  <= SourceCode.Length - 1 && (SourceCode[j ] >= 'A' && SourceCode[j ] <= 'Z' || SourceCode[j ] >= 'a' && SourceCode[j ] <= 'z'))
                    {
                        flag2 = false;
                        type = 1;
                    }
                        
                    FindTokenClass(CurrentLexeme);
                }

                //string
                else if (CurrentChar == '\"')
                {
                 //"ab"cd"  
                    j++;
                    if (j > SourceCode.Length - 1)
                    {
                        type = 3;
                        break;

                    }
                    CurrentChar = SourceCode[j];//a
                    while (CurrentChar != '\"')
                    {
                        CurrentLexeme += CurrentChar.ToString();//ab
                        j++;//3
                        if (CurrentChar == '\r' || j > SourceCode.Length - 1 || CurrentChar == '\n')
                        {
                            type = 3;
                            i = j-2;
                            break;
                        }
                        CurrentChar = SourceCode[j];//"
                    }
                    if (CurrentChar == '\"')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        if (j+1  <= SourceCode.Length - 1 &&  SourceCode[j+1] != '\r')
                        {
                            flag = false;
                            type = 3;
                        }
                            
                        i = j ;//4
                    }
                   
                    FindTokenClass(CurrentLexeme);
                }

                //comment
                else if (CurrentChar == '/')
                {
                    j++;
                    if (j > SourceCode.Length - 1)
                        break;
                    CurrentChar = SourceCode[j];
                    if (CurrentChar == '*')
                    {
                        while (true)
                        {
                            CurrentLexeme += CurrentChar.ToString();
                            j++;
                            if (j > SourceCode.Length - 1)
                                break;
                            CurrentChar = SourceCode[j];
                            if (CurrentChar == '*')
                            {
                                CurrentLexeme += CurrentChar.ToString(); 
                                j++;
                                if (j > SourceCode.Length - 1)
                                    break;
                                CurrentChar = SourceCode[j];
                                if (CurrentChar == '/')
                                {
                                    CurrentLexeme += CurrentChar.ToString(); 
                                    type = 4;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }

                            }
                            else
                            {
                                continue;
                            }

                        }
                    }
                    i = j;
                    FindTokenClass(CurrentLexeme);
                }

                //braces
                else if (CurrentChar == '{') 
                {
                    i = j;
                    j++;
                    FindTokenClass(CurrentLexeme);
                    if (j > SourceCode.Length - 1)
                        break;
                    CurrentChar = SourceCode[j];
                    CurrentLexeme += CurrentChar.ToString();
                    while (CurrentChar != '}')
                    {
                        j++;
                        if (j > SourceCode.Length - 1)
                            break;
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                        continue;
                    }
                    CurrentLexeme += CurrentChar.ToString();
                    FindTokenClass(CurrentLexeme);
                }

                //bracket
                else if (CurrentChar == '(') 
                {
                    i = j;
                    j++;
                    FindTokenClass(CurrentLexeme);
                    if (j > SourceCode.Length - 1)
                        break;
                    CurrentChar = SourceCode[j];
                    CurrentLexeme += CurrentChar.ToString();
                    while (CurrentChar != ')')
                    {
                        j++;
                        if (j > SourceCode.Length - 1)
                            break;
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                        continue;
                    }
                    Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);
                }


                else
                {
                    if (i + 1 <= SourceCode.Length - 1 && SourceCode[i] == SourceCode[i + 1])
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        FindTokenClass(CurrentLexeme);
                        i++;
                    }
                    else if (CurrentChar == '.' && (SourceCode[i + 1] >= '0' && SourceCode[i + 1] <= '9'))
                    {
                        flag3 = false;
                        type = 2;
                        FindTokenClass(CurrentLexeme);
                    }
                    else if ((CurrentChar == '>'|| CurrentChar == '<') && SourceCode[i + 1] == '=' )
                    {
                        flag4 = false;
                        type = 5;
                        i++;
                        FindTokenClass(CurrentLexeme);
                    }
                    else if (CurrentChar == ':'  && SourceCode[i + 1] == '=')
                    {
                        i++;
                        CurrentLexeme += SourceCode[i];
                        FindTokenClass(CurrentLexeme);
                    }
                    else
                        FindTokenClass(CurrentLexeme);
                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }

            //Is it a Number?
            else if (isNumber(Lex)&&flag2&&flag3)
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }

            //Is it an string?
            else if (isString(Lex)&&flag)
            {
                Tok.token_type = Token_Class.T_string;
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if (Operators.ContainsKey(Lex) && flag3&&flag4)
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            /*else if (!Operators.ContainsKey(Lex))
            {
                type = 5;
            }
            else if (!ReservedWords.ContainsKey(Lex))
            {
                type = 6;
            }*/

            //Is it an undefined?
            else
            {
                if (type == 1)
                {
                    Errors.Error_List.Add("Invalid Identifier");
                }
                else if (type == 2)
                {
                    Errors.Error_List.Add("Invalid number");
                }
                else if (type == 3)
                {
                    Errors.Error_List.Add("Invalid string");
                }
                else if (type == 5)
                {
                    Errors.Error_List.Add("Invalid opoerator");

                }
                else if (type == 6)
                {
                    Errors.Error_List.Add("Invalid reserved words");

                }
                /*else if (type == 4)
                {
                    Errors.Error_List.Add("comment");

                }*/
                else if (type == 10)
                {
                    Errors.Error_List.Add("Invalid bracket");

                }
                /*else
                {
                    Errors.Error_List.Add("undefined");
                }*/

            }

        }


        
        string wrongLex = "";
        bool isIdentifier(string lex)
        {
            bool isValid = true;
            // Check if the lex is an identifier or not.
            var reId = new Regex("^[a-zA-Z]([a-zA-Z0-9])*$", RegexOptions.Compiled);
            if (reId.IsMatch(lex))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
               // type = 1;
                wrongLex = lex;
            }
            return isValid;
        }
        bool isNumber(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.
            var reId = new Regex(@"^[0-9]+(.[0-9]+)?$", RegexOptions.Compiled);
            if (reId.IsMatch(lex))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
                //type = 2;
                wrongLex = lex;
            }
            return isValid;
        }
        bool isString(string lex)
        {
            bool isValid = true;
            // Check if the lex is a string or not.
            var reId = new Regex("^\"([a-zA-Z0-9]|[^a-zA-Z0-9])*\"$", RegexOptions.Compiled);
            if (reId.IsMatch(lex))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
                //type = 3;
                wrongLex = lex;
            }
            return isValid;
        }
    }
}