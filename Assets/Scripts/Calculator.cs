using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.UI;

public class Calculator : MonoBehaviour
{
    [SerializeField]
    TMP_Text m_InputField;

    [SerializeField] 
    private UISquareDrawer m_SquareDrawer;

    [SerializeField]
    private GameObject m_SquareWindow;


    private static readonly HashSet<char> SymbolsToHandle = new HashSet<char> { '+', '-', '(', ')', '.', '*', '/' };

    private void Start()
    {
        Clear();

        m_SquareWindow.gameObject.SetActive(false);
    }

    public void UpdateSquare()
    {
        string expression = m_InputField.text;
        string[] parts = Regex.Split(expression, @"(\+|\-|\*|\/)").Select(p => p.Trim()).ToArray();

        if (parts.Length >= 3 && double.TryParse(parts[2], out double operand))
        {
            float size = (float)operand * 10f;

            m_SquareDrawer.SquareSize = (float)operand * 10f;
            m_SquareWindow.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            m_SquareWindow.gameObject.SetActive(false);
        }
    }

    public void FindResult()
    {
        string expression = m_InputField.text;
        if (expression.Length < 1)
            return;

        char lastCharacter = expression[^1];
        if (IsExpressionSymbol(lastCharacter))
            expression = expression[..^1];

        if (!AreParenthesesBalanced(expression))
        {
            m_InputField.text = "Error: Unbalanced Parentheses";
            return;
        }

        try
        {
            double result = EvaluateExpression(expression);
            m_InputField.text = FormatResult(result);
        }
        catch (Exception ex)
        {
            HandleEvaluationError(ex);
        }
    }

    public void ConvertSecondArgumentToBinary()
    {
        string expression = m_InputField.text;

        // Split the expression while keeping operators
        string[] parts = Regex.Split(expression, @"(\+|\-|\*|\/)").Select(p => p.Trim()).ToArray();

        if (parts.Length >= 3 && double.TryParse(parts[2], out double decimalNumber))
        {
            // Convert the second operand to binary (only the integer part)
            long integerPart = (long)decimalNumber;
            string binaryNumber = Convert.ToString(integerPart, 2);

            // Rebuild the expression with only the 2nd operand converted
            parts[2] = binaryNumber;
            m_InputField.text = string.Join("", parts);
        }
    }

    private bool AreParenthesesBalanced(string expression)
    {
        int balance = 0;
        foreach (char c in expression)
        {
            if (c == '(') balance++;
            else if (c == ')') balance--;
            if (balance < 0) return false;
        }
        return balance == 0;
    }

    private double EvaluateExpression(string expression)
    {
        CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            DataTable table = new DataTable();
            return Convert.ToDouble(table.Compute(expression, ""));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    private string FormatResult(double result)
    {
        return result % 1 == 0 ? $"{result:F0}" : $"{result}";
    }

    private void HandleEvaluationError(Exception ex)
    {
        Debug.LogError($"Invalid Expression: {ex.Message}");
        m_InputField.text = ex.Message.Contains("divide by zero")
            ? "Error: Division by Zero"
            : "Error: Invalid Expression";
    }

    public void Clear()
    {
        m_InputField.text = string.Empty;
    }

    public void RemoveLast()
    {
        if (m_InputField.text.Length > 0)
            m_InputField.text = m_InputField.text[..^1];
    }

    public void EmplaceSymbol(string symbol)
    {
        if (string.IsNullOrEmpty(symbol)) return;

        char c = symbol[0];
        string currentText = m_InputField.text;

        if (currentText.Length > 0 && SymbolsToHandle.Contains(c))
            if (HandleCharacter(c)) return;

        if (c == '.' && CurrentNumberHasDecimal())
            return;

        m_InputField.text += c;
    }

    private bool HandleCharacter(char c)
    {
        char lastChar = m_InputField.text[^1];

        if (c == '-' && (IsExpressionSymbol(lastChar) || lastChar == '('))
        {
            m_InputField.text += c;
            return true;
        }

        if (IsExpressionSymbol(lastChar) && c != '(' && c != ')')
        {
            m_InputField.text = $"{m_InputField.text[..^1]}{c}";
            return true;
        }

        return lastChar == c;
    }

    private bool CurrentNumberHasDecimal()
    {
        string text = m_InputField.text;
        int start = text.Length - 1;

        for (int i = start; i >= 0; i--)
        {
            if (IsExpressionSymbol(text[i]) || text[i] == '(' || text[i] == ')')
            {
                start = i + 1;
                break;
            }
            if (i == 0) start = 0;
        }

        return text.Substring(start).Contains('.');
    }

    private bool IsExpressionSymbol(char c)
    {
        return c is '+' or '-' or '*' or '/' or '.';
    }
}