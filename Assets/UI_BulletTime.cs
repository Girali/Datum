using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BulletTime : MonoBehaviour
{
    public Text count;
    public Image fill;

    public void UpdateView(float t)
    {
        count.text = "" + ((int)(100 * t));
        fill.fillAmount = t;
    }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
    }
}
