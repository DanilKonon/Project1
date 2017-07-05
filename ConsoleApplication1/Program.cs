using System;
using System.IO;
using System.Text;

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

public enum AstNodeType { OpPlus, OpMinus, OpMul, UnMinus, OpDiv, Number, Undefined, OpExp, OpTerm } //experiment with opexp

public class AstNode
{
    public AstNodeType Type;
    public double Value;
    public AstNode Left;
    public AstNode Right;

    public AstNode(AstNodeType type, double val, AstNode lnode, AstNode rnode)
    {
        Type = type;
        Value = val;
        Left = lnode;
        Right = rnode;
    }
}

class Parser
{
    public Token currToken = new Token();
    int index;
    string curr_str;

    private
    AstNode Expression()
    {

        AstNode lnode = Term();
        AstNode rnode = Expression1();

        return CreateNode(AstNodeType.OpPlus, lnode, rnode);
    }
    AstNode Expression1()
    {
        AstNode lnode, rnode;

        switch (currToken.type)
        {
            case TypeofToken.Plus:
                GetNextToken();
                lnode = Term();
                rnode = Expression1();
                return CreateNode(AstNodeType.OpPlus, rnode, lnode);
            case TypeofToken.Minus:
                GetNextToken();
                lnode = Term();
                rnode = Expression1();
                return CreateNode(AstNodeType.OpMinus, rnode, lnode);
        }

        return CreateNumberNode(0); //to check 
    }
    AstNode Term()
    {
        AstNode lnode = Factor();
        AstNode rnode = Term1();
        return CreateNode(AstNodeType.OpMul, lnode, rnode);
    }
    AstNode Term1()
    {
        AstNode lnode, rnode;
        switch (currToken.type)
        {
            case TypeofToken.Mul:
                GetNextToken();
                lnode = Factor();
                rnode = Term1();
                return CreateNode(AstNodeType.OpMul, rnode, lnode);
            case TypeofToken.Div:
                GetNextToken();
                lnode = Factor();
                rnode = Term1();
                return CreateNode(AstNodeType.OpDiv, rnode, lnode);
        }
        return CreateNumberNode(1); //to check 
    }
    AstNode Factor()
    {
        AstNode node;
        switch (currToken.type)
        {
            case TypeofToken.OpenPar:
                GetNextToken();
                node = Expression();
                Match(')');
                return node; //why????
            case TypeofToken.Minus:
                GetNextToken();
                node = Expression();
                return CreateUnaryMinus(node);
            case TypeofToken.Number:
                double val = currToken.val;
                GetNextToken();
                return CreateNumberNode(val);
            default:
                {
                    // to write!!!
                    Console.WriteLine($"Unexpected token { currToken.symbol } at position { index }");
                    //throw ParserException(sstr.str(), m_Index);
                    return null;
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
            if (curr_str.Length - 1 == index) { break; } else { index++; }
        }
        try
        {
            return Convert.ToDouble(x);
        }
        catch
        {
            Console.WriteLine("Wrong format of float!");
            return 0.0;
        }

    }

    void CleanSpaces()
    {
        while (Char.IsWhiteSpace(curr_str[index]))
        {
            if (curr_str.Length - 1 == index)
            {
                break;
            }
            index++;
        } //increment
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
            }
        }
        else
        {
            Console.WriteLine($"Unexpected token { curr_str[index] }");
            return;
            //to write Exception
        }
    }

    void Match(char symb)
    {
        if (curr_str[index-1] == symb)
        {
            GetNextToken();  //difference
        }
        //else error
    }

    AstNode CreateNode(AstNodeType type, AstNode left, AstNode right)
    {
        AstNode node = new AstNode(type, 0, left, right);
        return node;
    }

    AstNode CreateNumberNode(double val)
    {
        AstNode node = new AstNode(AstNodeType.Number, val, null, null);
        return node;
    }

    AstNode CreateUnaryMinus(AstNode lnode)
    {
        AstNode node = new AstNode(AstNodeType.UnMinus, 0, lnode, null); //to check
        return node;
    }

    public
    AstNode Parse(string str)
    {
        AstNode node;
        curr_str = str;
        index = 0;
        GetNextToken();
        node = Expression();
        return node;
    }

}

class E_Space
{
    public
    void Test(string text)
    {
        Parser parser = new Parser();
        AstNode node = parser.Parse(text);
        PrintTheTree printer = new PrintTheTree();
        printer.PrintTree(node, 0);
        Evaluator eval = new Evaluator();
        try
        {
            Console.WriteLine(eval.EvalSubTree(node));
        }
        catch
        {
            Console.WriteLine("Error!");
        }
    }

}

class Evaluator
{
    public
    double EvalSubTree(AstNode head)
    {
        if (head == null)
        {
            throw new EvaluatorException("Incorrect syntax tree!");
        }
        if (head.Type == AstNodeType.Number)
        {
            return head.Value;
        }
        if (head.Type == AstNodeType.UnMinus)
        {
            return -EvalSubTree(head.Left);
        }
        double v1 = EvalSubTree(head.Left);
        double v2 = EvalSubTree(head.Right);
        switch (head.Type)
        {
            case AstNodeType.OpDiv:
                return v1 / v2;
            case AstNodeType.OpMinus:
                return v1 - v2;
            case AstNodeType.OpPlus:
                return v1 + v2;
            case AstNodeType.OpMul:
                return v1 * v2;
        }
        throw new EvaluatorException("Incorrect syntax tree!");
    }

}

class EvaluatorException : Exception
{
    public
    EvaluatorException(string message)
    {
        Console.WriteLine(message);
    }
}

class PrintTheTree
{
    public
    int PrintTree(AstNode node, int index)
    {
        string path = @"C:\Users\Danil\Documents\Visual Studio 2015\Projects\ConsoleApplication1\Graph.txt";
        if (index == 0)
        {
            using (FileStream fs = File.Create(path))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes($"digraph {{ {Environment.NewLine}");
                fs.Write(info, 0, info.Length);
            }
        }
        int i = index; //error possible?
        string res = string.Empty;
        if (node.Type == AstNodeType.Number)
        {
            res = node.Value.ToString();
        }
        switch (node.Type)
        {
            case AstNodeType.OpPlus: res = "+"; break;
            case AstNodeType.OpMinus: res = "-"; break;
            case AstNodeType.OpMul: res = "*"; break;
            case AstNodeType.OpDiv: res = "/"; break; //unary minus
        }
        using (StreamWriter fs = File.AppendText(path))
        {
            if (node.Left != null) {fs.WriteLine($"{index} -> {index + 1};");}
            fs.WriteLine($"{index}[label= \"{res}\"];");
        }
        if (node.Left != null) { i = PrintTree(node.Left, index + 1); }
        using (StreamWriter fs = File.AppendText(path))
        {
            if (node.Right != null) { fs.WriteLine($"{index}->{i + 1};"); }
        }
        if (node.Right != null) { i = PrintTree(node.Right, i + 1); }
        index = i;
        return index;
    }
}

class HeyYou
{

    static void Main()
    {
        E_Space tester = new E_Space();
        tester.Test("(1*3)+2+(6*7)+3+4");
        Console.ReadKey();
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
        tester.Test("*1 / 2.5");
        Console.ReadKey();
    }
}
