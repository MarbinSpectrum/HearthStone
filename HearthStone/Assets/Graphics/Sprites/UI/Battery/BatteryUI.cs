using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    public Image batteryStateImg;      // 배터리 상태 표시 이미지
    public Image batteryFrameImg;      // 배터리 모양 프레임 이미지
    public Image batteryCharging;      // 배터리 충전 모양중 이미지
    public float t;
    public void Update()
    {
        UpdateBatteryUI();
    }

    public void UpdateBatteryUI()
    {
        float batteryLevel = SystemInfo.batteryLevel;
        switch (SystemInfo.batteryStatus)
        {
            case BatteryStatus.Full:
            case BatteryStatus.Charging:

                batteryStateImg.color = batteryFrameImg.color = Color.green;
                batteryCharging.gameObject.SetActive(true);
                batteryStateImg.fillAmount = 1f;
                break;
            case BatteryStatus.Discharging:
                batteryCharging.gameObject.SetActive(false);
                if (batteryLevel < 0.1f) // 배터리가 부족하면 이미지를 빨갛게
                    batteryStateImg.color = batteryFrameImg.color = Color.red;
                else
                    batteryStateImg.color = batteryFrameImg.color = Color.green;
                batteryStateImg.fillAmount = (int)(batteryLevel*4)/4f;
                break;
        }
    }
}