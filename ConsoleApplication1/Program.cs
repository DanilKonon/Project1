using System;public enum TypeofToken { Plus, Minus, Div, Mul, OpenPar, ClPar, Number, EndofString, Error };public struct Token{    public TypeofToken type;    public double val;    public char symbol;    public Token(double x, char y)    {        val = x;        symbol = y;        type = TypeofToken.Error;    }};public enum AstNodeType { OpPlus, OpMinus, OpMul, UnMinus, OpDiv, Number, Undefined, OpExp, OpTerm } //experiment with opexp

public class AstNode{    public AstNodeType Type;    public double Value;    public AstNode Left;    public AstNode Right;

    public AstNode(AstNodeType type, double val, AstNode lnode, AstNode rnode)
    {
        Type = type;
        Value = val;
        Left = lnode;
        Right = rnode;
    }}class Parser{    public Token currToken = new Token();    int index;    string curr_str;    private    AstNode Expression()    {

        AstNode lnode = Term();        AstNode rnode = Expression1();

        return CreateNode(AstNodeType.OpPlus, lnode, rnode);
    }    AstNode Expression1()    {        AstNode lnode, rnode;        switch (currToken.type)        {
            case TypeofToken.Plus:                GetNextToken();                lnode = Term();                rnode = Expression1();                return CreateNode(AstNodeType.OpPlus, lnode, rnode);            case TypeofToken.Minus:                GetNextToken();
                lnode = Term();
                rnode = Expression1();                return CreateNode(AstNodeType.OpMinus, lnode, rnode);        }        return CreateNumberNode(0); //to check 
    }    AstNode Term()    {        AstNode lnode = Factor();        AstNode rnode = Term1();        return CreateNode(AstNodeType.OpMul, lnode, rnode);    }    AstNode Term1()    {        AstNode lnode, rnode;        switch (currToken.type)        {            case TypeofToken.Mul:                GetNextToken();                lnode = Factor();                rnode = Term1();                return CreateNode(AstNodeType.OpMul, lnode, rnode);            case TypeofToken.Div:                GetNextToken();                lnode = Factor();                rnode = Term1();                return CreateNode(AstNodeType.OpDiv, lnode, rnode);        }        return CreateNumberNode(1); //to check 
    }    AstNode Factor()    {        AstNode node;        switch (currToken.type)        {            case TypeofToken.OpenPar:                GetNextToken();                node = Expression();                Match(')');                return node; //why????
            case TypeofToken.Minus:                GetNextToken();                node = Expression();                return CreateUnaryMinus(node);            case TypeofToken.Number:                double val = currToken.val;                GetNextToken();                return CreateNumberNode(val);            default:                {
                    // to write!!!
                    Console.WriteLine($"Unexpected token { currToken.symbol } at position { index }");
                    //throw ParserException(sstr.str(), m_Index);
                    return null;                }        }    }    double GetNumber()    {        string x = "";        while (Char.IsDigit(curr_str[index]) || curr_str[index] == ',' || curr_str[index] == '.')        {            if (curr_str[index] == '.')            {                x += ','.ToString();            }            else            {                x += curr_str[index].ToString();            }            if (curr_str.Length - 1 == index) { break; } else { index++; }
        }        try        {            return Convert.ToDouble(x);        }        catch        {            Console.WriteLine("Wrong format of float!");            return 0.0;        }

    }    void CleanSpaces()    {        while (Char.IsWhiteSpace(curr_str[index]))
        {
            if (curr_str.Length - 1 == index)
            {
                break;
            }
            index++;
        } //increment
    }    void GetNextToken()    {        CleanSpaces();        currToken.val = 0;        currToken.symbol = '0';        if (index == curr_str.Length)        {            currToken.type = TypeofToken.EndofString;            return;        }        if (Char.IsDigit(curr_str[index]))        {            currToken.type = TypeofToken.Number;            currToken.val = GetNumber();            return;        }        currToken.type = TypeofToken.Error;        switch (curr_str[index])        {            case '+': currToken.type = TypeofToken.Plus; break;            case '-': currToken.type = TypeofToken.Minus; break;            case '*': currToken.type = TypeofToken.Mul; break;            case '/': currToken.type = TypeofToken.Div; break;            case '(': currToken.type = TypeofToken.OpenPar; break;            case ')': currToken.type = TypeofToken.ClPar; break;        }        if (currToken.type != TypeofToken.Error)        {            currToken.symbol = curr_str[index];            if (curr_str.Length - 1 != index)            {                index++;            }        }        else        {            Console.WriteLine($"Unexpected token { curr_str[index] }");            return;
            //to write Exception
        }    }    void Match(char symb)    {        if (curr_str[index] == symb)        {
            //index++;
            GetNextToken();  //difference
        }
        //else error
    }

    AstNode CreateNode(AstNodeType type, AstNode left, AstNode right)
    {
        AstNode node = new AstNode(type, 0, left, right);
        return node;
    }    AstNode CreateNumberNode(double val)    {        AstNode node = new AstNode(AstNodeType.Number, val, null, null);        return node;    }

    AstNode CreateUnaryMinus(AstNode lnode)
    {
        AstNode node = new AstNode(AstNodeType.UnMinus, 0, lnode, null); //to check
        return node;
    }    public    AstNode Parse(string str)    {        AstNode node;        curr_str = str;        index = 0;        GetNextToken();        node = Expression();        return node;    }}class E_Space{    public    void Test(string text)    {        Parser parser = new Parser();        AstNode node = parser.Parse(text);        Evaluator eval = new Evaluator();        try
        {
            Console.WriteLine(eval.EvalSubTree(node));        }
        catch        {            Console.WriteLine("Error!");        }    }

}class Evaluator{    public    double EvalSubTree(AstNode head)    {        if (head == null)        {
            throw new EvaluatorException("Incorrect syntax tree!");        }        if (head.Type == AstNodeType.Number)
        {            return head.Value;        }
        if (head.Type == AstNodeType.UnMinus)
        {
            return -EvalSubTree(head.Left);
        }        double v1 = EvalSubTree(head.Left);        double v2 = EvalSubTree(head.Right);        switch (head.Type)
        {            case AstNodeType.OpDiv:
                return v1 / v2;            case AstNodeType.OpMinus:                return v1 - v2;
            case AstNodeType.OpPlus:
                return v1 + v2;
            case AstNodeType.OpMul:
                return v1 * v2;        }        throw new EvaluatorException("Incorrect syntax tree!");    }}class EvaluatorException : Exception{
    public
        EvaluatorException(string message)
    {        Console.WriteLine(message);    }}class HeyYou{    static void Main()    {        E_Space tester = new E_Space();
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
        tester.Test("M1 + 2.5");
        tester.Test("1 + 2&5");
        tester.Test("1 * 2.5.6");
        tester.Test("1 ** 2.5");
        tester.Test("*1 / 2.5");        Console.ReadKey();    }}