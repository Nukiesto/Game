using UnityEngine;
using System;
using System.Collections;

public class TimerToDelete : MonoBehaviour
{
    public enum Mode
    {
        Destroy,
        Disable
    }
    [SerializeField] private float time;
    [SerializeField] private Mode mode;
    [SerializeField] private bool repeatOnEnable;

    private bool _canOnEnable;

    private void Start()
    {
        _canOnEnable = true;
        StartCoroutine(StartDelete());
    }

    private void OnEnable()
    {
        if (repeatOnEnable && _canOnEnable)
        {
            StartCoroutine(StartDelete());
        }
    }
    
    private IEnumerator StartDelete()
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            if (mode == Mode.Disable)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
            yield break;
        }
    }
}
