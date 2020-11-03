using EasyButtons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Bars.Bars
{
    public class UiBar : Bar
    {
        public Image image;

        public override void UpdateBar()
        {
            //Debug.Log("Updated: " + Value + ";" + MaxValue);
            if (Value <= MaxValue)
            {
                image.fillAmount = 1 - (MaxValue - Value) / MaxValue;
            }
        }
        [Button]
        public void BarTest()
        {
            image.fillAmount = 0.5f;
        }
    }
}
