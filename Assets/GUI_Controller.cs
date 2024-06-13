using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;

public class GUI_Controller : MonoBehaviour
{
    private static GUI_Controller instance;
    public static GUI_Controller Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GUI_Controller>();
            }
            return instance;
        }
    }

    private List<string> debugs = new List<string>();

    public void AddTrackedValue(string s)
    {
        debugs.Add(s);
    }

    public static string GetVariableName<T>(Expression<Func<T>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }
        else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operandMemberExpression)
        {
            return operandMemberExpression.Member.Name;
        }
        else if (expression.Body is ParameterExpression parameterExpression)
        {
            return parameterExpression.Name;
        }

        throw new ArgumentException("Invalid expression");
    }

    private void Update()
    {
        string debug = "";

        foreach (var d in debugs)
        {
            debug += d + "\n";
        }

        debug1.text = debug;
        debugs.Clear();
    }


    [SerializeField]
    private Text debug1;

    public Jun_TweenRuntime fadeIn;
    public Jun_TweenRuntime fadeOut;
}
