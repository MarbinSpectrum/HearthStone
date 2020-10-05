using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    public Slider BGM;
    public Slider SE;

    void Update()
    {
        BGM.value = SoundManager.instance.maxBGM;
        SE.value = SoundManager.instance.maxSE;
    }

    public void ChangeBGM()
    {
        SoundManager.instance.maxBGM = BGM.value;
        SoundManager.instance.BGM.volume = SoundManager.instance.maxBGM;
    }

    public void ChangeSE()
    {
        SoundManager.instance.maxSE = SE.value;
        SoundManager.instance.SE.volume = SoundManager.instance.maxSE;
    }
}
