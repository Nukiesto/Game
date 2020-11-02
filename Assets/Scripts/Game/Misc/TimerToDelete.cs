using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using static UsefulScripts.OtherScripts;
public class TimerToDelete : MonoBehaviour
{
    public enum Mode
    {
        Destroy,
        Disable
    }
    [Header("Main")]
    [SerializeField] private float time;
    [SerializeField] private Mode mode;
    [SerializeField] private bool repeatOnEnable;
    
    
    [Header("Blend")]
    [SerializeField] private bool toBlend;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Text text = null;
    
    private bool _canOnEnable;
    private bool _blending;
    
    private void Start()
    {
        _canOnEnable = true;
        StartCoroutine(StartDelete());
        if (toBlend)
            StartCoroutine(StartBlend());
    }

    private void OnEnable()
    {
        if (repeatOnEnable && _canOnEnable)
        {
            StartCoroutine(StartDelete());
            if (toBlend)
                StartCoroutine(StartBlend());
        }
    }

    private void OnDisable()
    {
        StopCoroutine(StartBlend());
        if (spriteRenderer != null)
        {
            var color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
        }
        else
        {
            if (text != null)
            {
                var color = text.color;
                color.a = 1;
                text.color = color;
            }
        }
    }

    private IEnumerator StartBlend()
    {
        _blending = true;
        var color = Color.white;
        var value = 1f;
        var valuePlus0 = 1f / time;
        
        //Debug.Log("time: " + time);
        if (spriteRenderer != null)
        {
            //Debug.Log(spriteRenderer);
            color = spriteRenderer.color;
        }
        else
        {
            if (text != null)
            {
                //Debug.Log(color);
                color = text.color;
            }
        }
        while (true)
        {
            yield return null;
            var valuePlus = valuePlus0 / (1 / Time.deltaTime);
            value -= valuePlus;
            color.a = value;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
            else
            {
                if (text != null)
                {
                    //Debug.Log(color);
                    text.color = color;
                }
            }
            if (value < 0)
                yield break;
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

            _blending = false;
            yield break;
        }
    }
}
