using UnityEngine;

public class BarManager : MonoBehaviour
{
    //public
    public Bar[] bars;
    
    public void InitBar(float maxValue, Bar.Type type)
    {
        foreach (var bar in bars)
        {
            if (bar.type == type)
            {
                bar.SetMaxValue(maxValue);
                break;
            }
        }
    }
    public void InitAllBar(float maxValue)
    {
        foreach (var bar in bars)
        {
            bar.SetMaxValue(maxValue);
        }
    }
    public void EnterValue(float value, Bar.Type type)
    {
        foreach (var bar in bars)
        {
            if (bar.type == type)
            {
                bar.EnterValue(value);
                break;
            }
        }
    }
    public void EnterAllValue(float value)
    {
        foreach (Bar bar in bars)
        {
            bar.EnterValue(value);           
        }
    }

    public void Enable(Bar.Type type)
    {
        foreach (var bar in bars)
        {
            if (bar.type == type)
            {
                bar.Enable();
                break;
            }
        }
    }
    public void Disable(Bar.Type nameBar)
    {
        foreach (var bar in bars)
        {
            if (bar.type == nameBar)
            {
                bar.Disable();
                break;
            }
        }
    }
}

public abstract class Bar : MonoBehaviour
{
    public enum Type
    {
        HP
    }
    protected float Value { get; set; }
    protected float MaxValue { get; set; }
    public Type type;
    public bool barEnabled;

    public virtual void Enable() 
    {
        barEnabled = true;
    }
    public virtual void Disable()
    {
        barEnabled = false;
    }
    public void EnterValue(float value)
    {
        Value = value;
        UpdateBar();
    }
    public void SetMaxValue(float maxValue)
    {
        MaxValue = maxValue;
        Value = maxValue;
        UpdateBar();
    }
    public abstract void UpdateBar();
}