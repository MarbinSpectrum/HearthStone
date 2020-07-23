using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSelect : MonoBehaviour
{
    public MinionObject minionObject;
    public GameObject select_normal;
    public GameObject select_taunt;
    public GameObject select_legend;

    // Update is called once per frame
    void Update()
    {
        if(!BattleUI.instance.grayFilter.activeSelf)
        {
            select_taunt.SetActive(false);
            select_normal.SetActive(false);
            select_legend.SetActive(false);
            return;
        }

        if (!DragLineRenderer.instance.CheckMask(타겟.실행주체) && DragLineRenderer.instance.CheckActObj(minionObject.gameObject))
        {
            select_taunt.SetActive(false);
            select_normal.SetActive(false);
            select_legend.SetActive(false);
            return;
        }
        if ((!minionObject.enemy && !DragLineRenderer.instance.CheckMask(타겟.아군하수인)) || (minionObject.enemy && !DragLineRenderer.instance.CheckMask(타겟.적하수인)))
        {
            select_taunt.SetActive(false);
            select_normal.SetActive(false);
            select_legend.SetActive(false);
            return;
        }

        if (minionObject.taunt)
        {
            select_taunt.SetActive(true);
            select_normal.SetActive(false);
        }
        else
        {
            select_taunt.SetActive(false);
            select_normal.SetActive(true);
        }
        select_legend.SetActive(minionObject.legend);
    }
}
