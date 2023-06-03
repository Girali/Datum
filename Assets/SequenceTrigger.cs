using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequenceTrigger : MonoBehaviour
{
    [SerializeField]
    private Sequence[] sequences;
    private Coroutine coroutine;

    public void StartSequence()
    {
        coroutine = StartCoroutine(CRT_Sequence());
    }

    public void StopSequence()
    {
        if(coroutine != null)
            StopCoroutine(coroutine);
    }

    private IEnumerator CRT_Sequence()
    {
        for (int i = 0; i < sequences.Length; i++)
        {
            yield return sequences[i].CRT_Sequence();
        }
    }

    [System.Serializable]
    public class Sequence
    {
        public UnityEvent onStepStart;
        public UnityEvent onStepEnd;
        public float delay;

        public IEnumerator CRT_Sequence()
        {
            onStepStart.Invoke();

            yield return new WaitForSeconds(delay);

            onStepEnd.Invoke();
        }
    }
}
