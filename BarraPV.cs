using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraPV : MonoBehaviour
{
    public Slider slider;

    public void definePVMax(float pv)
    {
        slider.maxValue = pv;
        slider.value = pv;
    }

    public void definePVAtual(float pv)
    {
        slider.value = pv;
        
    }
}
