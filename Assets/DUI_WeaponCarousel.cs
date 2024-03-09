using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DUI_WeaponCarousel : MonoBehaviour
{
    public float triggerAngle = 0.25f;
    public Image cursor;
    public Image[] points;
    public bool[] empty;
    private Vector2 limits = new Vector2(0, 0.1f);
    private Vector3[] pointsPos;

    public void SetAmmo(int i, bool b)
    {
        empty[i] = b;
    }
    
    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
    }
    
    private void Awake()
    {
        pointsPos = new Vector3 [points.Length];
        empty = new bool[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            pointsPos[i] = points[i].rectTransform.localPosition;
        }
    }

    public int UpdateView(float x, float y)
    {
        for (int j = 0; j < points.Length; j++)
        {
            points[j].color = empty[j] ? Color.gray : Color.white;
        }

        x = Mathf.LerpUnclamped(limits.x, limits.y, x);
        y = Mathf.LerpUnclamped(limits.x, limits.y, y);

        cursor.rectTransform.localPosition = new Vector3(x, y, 0);
        int bestFit = -1;
        float bestFitValue = float.MinValue;
        Vector3 cursorPos = new Vector3(x, y, 0);
        int i = 0;
        
        foreach (Vector3 p in pointsPos)
        {
            float t = Vector3.Dot(cursorPos.normalized, p.normalized);
            if (t > bestFitValue)
            {
                bestFit = i;
                bestFitValue = t;
            }
            i++;
        }
        
        if (bestFitValue > 1 - triggerAngle)
        {
            cursor.color = Color.green;
            points[bestFit].color = Color.green;
            return bestFit;
        }
        else
        {
            cursor.color = Color.white;
            return -1;
        }
    }
}
