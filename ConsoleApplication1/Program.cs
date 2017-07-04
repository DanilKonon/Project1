﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum TypeofToken { Plus, Minus, Div, Mul, OpenPar, ClPar, Number, EndofString, Error };

public struct Token
{
    public TypeofToken type;
    public double val;
    public char symbol;
    public Token(double x, char y)
    {
        val = x;
        symbol = y;
        type = TypeofToken.Error;
    }
};

class Parser
{
    public Token currToken = new Token();
    int index = 0;
    string curr_str;
    private
    void Expression()
    {
        Term();
        Expression1();
    }
    void Expression1()
    {
        switch (currToken.type)
        {
            case TypeofToken.Plus:
                GetNextToken();
                Term();
                Expression1();
                break;
            case TypeofToken.Minus:
                GetNextToken();
                Term();
                Expression1();
                break;
        }
    }
    void Term()
    {
        Factor();
        Term1();
    }
    void Term1()
    {
        switch (currToken.type)
        {
            case TypeofToken.Mul:
                GetNextToken();
                Factor();
                Term1();
                break;
            case TypeofToken.Div:
                GetNextToken();
                Factor();
                Term1();
                break;
        }
    }
    void Factor()
    {
        switch (currToken.type)
        {
            case TypeofToken.OpenPar:
                GetNextToken();
                Expression();
                Match(')');
                break;
            case TypeofToken.Minus:
                GetNextToken();
                Expression();
                break;
            case TypeofToken.Number:
                GetNextToken();
                break;
            default:
                {
                    // to write!!!
                    Console.WriteLine($"Unexpected token { currToken.symbol } at position { index }.");
                    //throw ParserException(sstr.str(), m_Index);
                    break;
                }

        }
    }

    double GetNumber()
    {
        string x = "";
        while (Char.IsDigit(curr_str[index]) || curr_str[index] == ',' || curr_str[index] == '.')
        {
            if (curr_str[index] == '.')
            {
                x += ','.ToString();
            }
            else
            {
                x += curr_str[index].ToString();
            }
            if (curr_str.Length - 1 == index) { break; } else { index++; };//increment
        }
        try
        {
            return Convert.ToDouble(x);
        }
        catch
        {
            Console.WriteLine("Problem!");
            return 0.0;
        }
        
    }

    void CleanSpaces()
    {
        while (Char.IsWhiteSpace(curr_str[index])) { if (curr_str.Length - 1 == index) { break; } else { index++; }; } //increment
    }

    void GetNextToken()
    {
        CleanSpaces();
        currToken.val = 0;
        currToken.symbol = '0';
        if (index == curr_str.Length)
        {
            currToken.type = TypeofToken.EndofString;
            return;
        }
        if (Char.IsDigit(curr_str[index]))
        {
            currToken.type = TypeofToken.Number;
            currToken.val = GetNumber();
            return;
        }
        currToken.type = TypeofToken.Error;
        switch (curr_str[index])
        {
            case '+': currToken.type = TypeofToken.Plus; break;
            case '-': currToken.type = TypeofToken.Minus; break;
            case '*': currToken.type = TypeofToken.Mul; break;
            case '/': currToken.type = TypeofToken.Div; break;
            case '(': currToken.type = TypeofToken.OpenPar; break;
            case ')': currToken.type = TypeofToken.ClPar; break;
        }
        if (currToken.type != TypeofToken.Error)
        {
            currToken.symbol = curr_str[index];
            if (curr_str.Length - 1 != index)
            {
                index++;
            };
        }
        else
        {
            Console.WriteLine($"Unexpected token { curr_str[index] } .");
        }
    }

    void Match(char symb)
    {
        if (curr_str[index] == symb)
        {
            //index++;
            GetNextToken();  //difference
        }
        //else error
    }
    public
    void Parse(string str)
    {
        curr_str = str;
        index = 0;
        GetNextToken();
        Expression();
    }

}

class E_Space
{
    public
    void Test(string text)
    {
        Parser parser = new Parser();
        parser.Parse(text);
    }
}

class HeyYou
{

    static void Main()
    {
        E_Space tester = new E_Space();
        tester.Test("1+2+3+4");
        tester.Test("1*2*3*4");
        tester.Test("1-2-3-4");
        tester.Test("1/2/3/4");
        tester.Test("1*2+3*4");
        tester.Test("1+2*3+4");
        tester.Test("(1+2)*(3+4)");
        tester.Test("1+(2*3)*(4+5)");
        tester.Test("1+(2*3)/4+5");
        tester.Test("5/(4+3)/2");
        tester.Test("1 + 2.5");
        tester.Test("125");
        tester.Test("-1");
        tester.Test("-1+(-2)");
        tester.Test("-1+(-2.0)");
        tester.Test("   1*2,5");
        tester.Test("   1*2.5e2");
        Console.ReadKey();
        tester.Test("M1 + 2.5");
        tester.Test("1 + 2&5");
        tester.Test("1 * 2.5.6");
        tester.Test("1 ** 2.5");
        tester.Test("*1 / 2.5");
        Console.ReadKey();
    }
}
