using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCanSelect : MonoBehaviour
{
    public GameObject select;
    public GameObject selectBtn;
    public bool enemy;
    void Update()
    {
        if (!MinionManager.instance.selectMinionEvent)
        {
            select.SetActive(false);
            selectBtn.SetActive(false);
            return;
        }

        if ((!enemy && !DragLineRenderer.instance.CheckMask(타겟.아군영웅)) || (enemy && !DragLineRenderer.instance.CheckMask(타겟.적영웅)))
        {
            select.SetActive(false);
            selectBtn.SetActive(false);
            return;
        }

        select.SetActive(true);
        selectBtn.SetActive(true);
    }
}
