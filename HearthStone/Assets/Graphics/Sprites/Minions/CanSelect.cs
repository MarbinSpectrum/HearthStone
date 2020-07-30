using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSelect : MonoBehaviour
{

    public MinionObject minionObject;
    public SpriteRenderer select_normal;
    public SpriteRenderer select_taunt;
    public SpriteRenderer select_legend;
    public Material select_obj_Mat;
    public Material act_obj_Mat;
    public GameObject selectBtn;

    void Update()
    {
        if(!DragLineRenderer.instance.CheckMask(타겟.실행주체) && DragLineRenderer.instance.CheckActObj(minionObject.gameObject))
        {
            select_normal.material = act_obj_Mat;
            select_taunt.material = act_obj_Mat;
            select_legend.material = act_obj_Mat;
        }
        else
        {
            select_normal.material = select_obj_Mat;
            select_taunt.material = select_obj_Mat;
            select_legend.material = select_obj_Mat;
        }

        if (!MinionManager.instance.selectMinionEvent)
        {
            select_taunt.gameObject.SetActive(false);
            select_normal.gameObject.SetActive(false);
            select_legend.gameObject.SetActive(false);
            selectBtn.gameObject.SetActive(false);
            return;
        }

        if(MinionManager.instance.eventMininon)
            if(!MinionManager.instance.CheckConditionMinion(minionObject,MinionManager.instance.eventMininon,MinionManager.instance.eventNum))
            {
                select_taunt.gameObject.SetActive(false);
                select_normal.gameObject.SetActive(false);
                select_legend.gameObject.SetActive(false);
                selectBtn.gameObject.SetActive(false);
                return;
            }

        if ((!minionObject.enemy && !DragLineRenderer.instance.CheckMask(타겟.아군하수인)) || (minionObject.enemy && !DragLineRenderer.instance.CheckMask(타겟.적하수인)))
        {
            select_taunt.gameObject.SetActive(false);
            select_normal.gameObject.SetActive(false);
            select_legend.gameObject.SetActive(false);
            selectBtn.gameObject.SetActive(false);
            return;
        }

        if (minionObject.taunt)
        {
            select_taunt.gameObject.SetActive(true);
            select_normal.gameObject.SetActive(false);
        }
        else
        {
            select_taunt.gameObject.SetActive(false);
            select_normal.gameObject.SetActive(true);
        }
        selectBtn.gameObject.SetActive(true);
        select_legend.gameObject.SetActive(minionObject.legend);
    }
}
