﻿using System;
using System.Collections.Generic;

namespace Language.Lexer
{
    /// <summary>
    /// From source code create series of tokens.
    /// </summary>
    internal class Lexer
    {
        private readonly List<Token> _tokens = new();
        private static readonly Dictionary<string, TokenType> Keywords = new();

        private readonly string _source;
        private int _start;
        private int _current;


        /// <summary>
        /// Setup all keywords.
        /// </summary>
        static Lexer()
        {
            Keywords.Add("var", TokenType.Var);
            Keywords.Add("while", TokenType.While);
            Keywords.Add("if", TokenType.If);
            Keywords.Add("else", TokenType.Else);
            Keywords.Add("System", TokenType.System);
            Keywords.Add("Turtle", TokenType.Turtle);
        }

        public Lexer(string source)
        {
            _source = source;
        }

        /// <summary>
        /// Return tokens created from source code.
        /// </summary>
        public List<Token> GetTokens()
        {
            while (!End())
            {
                _start = _current;
                GetToken();
            }

            _tokens.Add(new Token(TokenType.Eof, null));
            return _tokens;
        }

        private void GetToken()
        {
            var character = GetCharAndAdvance();
            switch (character)
            {
                case '(':
                    AddToken(TokenType.LeftParen);
                    break;
                case ')':
                    AddToken(TokenType.RightParen);
                    break;
                case '{':
                    AddToken(TokenType.LeftBracket);
                    break;
                case '}':
                    AddToken(TokenType.RightBracket);
                    break;
                case ';':
                    AddToken(TokenType.Semicolon);
                    break;
                case '+':
                    AddToken(TokenType.Plus);
                    break;
                case '-':
                    AddToken(TokenType.Minus);
                    break;
                case '/':
                    AddToken(TokenType.Slash);
                    break;
                case '%':
                    AddToken(TokenType.Modulo);
                    break;
                case '*':
                    AddToken(TokenType.Star);
                    break;
                case ',':
                    AddToken(TokenType.Comma);
                    break;
                case '.':
                    AddToken(TokenType.Period);
                    break;
                case '!':
                    AddToken(IsNext('=')
                        ? TokenType.NotEqual
                        : throw new ArgumentException("Bad syntax: After ! must by ="));
                    break;
                case '=':
                    AddToken(IsNext('=') ? TokenType.Equal : TokenType.Assignment);
                    break;
                case '<':
                    AddToken(IsNext('=') ? TokenType.LessEqual : TokenType.Less);
                    break;
                case '>':
                    AddToken(IsNext('=') ? TokenType.GreaterEqual : TokenType.Greater);
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    break;
                case '"':
                    ReadString();
                    break;
                default:
                    if (char.IsNumber(character))
                    {
                        ReadNumber();
                    }
                    else if (char.IsLetter(character))
                    {
                        Identifier();
                    }
                    else
                    {
                        throw new ArgumentException("Unexpected character.");
                    }

                    break;
            }
        }

        private void ReadString()
        {
            while (LookAhead() != '"' && !End())
            {
                GetCharAndAdvance();
            }

            if (End())
            {
                throw new ArgumentException("Unterminated string.");
            }

            GetCharAndAdvance();

            var value = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenType.String, value);
        }

        private void Identifier()
        {
            while (char.IsLetterOrDigit(char.ToLower(LookAhead())))
            {
                GetCharAndAdvance();
            }

            var text = _source.Substring(_start, _current - _start);
            if (Keywords.TryGetValue(text, out var type))
            {
                AddToken(type);
            }
            else
            {
                AddToken(TokenType.Identifier, text);
            }
        }

        private void ReadNumber()
        {
            while (char.IsDigit(LookAhead()))
            {
                GetCharAndAdvance();
            }

            if (LookAhead() == '.' && char.IsDigit(LookAheadNext()))
            {
                GetCharAndAdvance();
                while (char.IsDigit(LookAhead())) GetCharAndAdvance();
            }

            AddToken(TokenType.Number,
                double.Parse(_source.Substring(_start, _current - _start),
                    System.Globalization.CultureInfo.InvariantCulture));
        }

        private char LookAhead()
        {
            if (End())
            {
                return '\0';
            }

            return _source[_current];
        }

        private char LookAheadNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];
        }

        private bool IsNext(char expected)
        {
            if (End()) return false;
            if (_source[_current] != expected) return false;

            _current++;
            return true;
        }

        private void AddToken(TokenType type, object literal = null)
        {
            _tokens.Add(new Token(type, literal));
        }

        private char GetCharAndAdvance()
        {
            return _source[_current++];
        }

        private bool End()
        {
            return _current >= _source.Length;
        }
    }
}