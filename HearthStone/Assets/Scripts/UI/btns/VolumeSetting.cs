using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    public Slider BGM;
    public Slider SE;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        SoundManager soundManager = SoundManager.instance;
        BGM.value = soundManager.GetBGM_value();
        SE.value = soundManager.GetSE_value();
    }

    public void ChangeBGM()
    {
        //슬라이드바가 움직이면
        //사운드 매니저를 참조해서 배경음 크기를 변경
        SoundManager soundManager = SoundManager.instance;
        soundManager.SetBGM_value(BGM.value);
    }

    public void ChangeSE()
    {
        //슬라이드바가 움직이면
        //사운드 매니저를 참조해서 효과음 크기를 변경
        SoundManager soundManager = SoundManager.instance;
        soundManager.SetSE_value(SE.value);
    }
}
