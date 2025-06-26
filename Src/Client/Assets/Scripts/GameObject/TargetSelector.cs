using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector:MonoSingleton<TargetSelector>
{
    Projector projector;

    bool actived = false;

    Vector3 center; //中心点
    private float range; //技能释放范围
    private float size;  //技能范围
    Vector3 offset = new Vector3(0f, 2f, 0f); 

    protected Action<Vector3> selectPoint;//事件通知，广播技能选择

    protected override void OnStart()
    {
        projector = this.gameObject.GetComponentInChildren<Projector>();
        projector.gameObject.SetActive(actived);
    }

    public void Active(bool active)
    {
        this.actived = active;
        if (projector == null) return;

        projector.gameObject.SetActive(this.actived);
        projector.orthographicSize = this.size * 0.5f;
    }

    private void Update()
    {
        if (!actived) return;
        if (this.projector == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//需要给摄像机设置MainCamera的tag
        RaycastHit hitInfo;
        //只与地面做碰撞
        if(Physics.Raycast(ray,out hitInfo,100f,LayerMask.GetMask("Terrain")))
        {
            Vector3 hitPoint = hitInfo.point;
            Vector3 dist = hitPoint - this.center;
            //限制在施法范围内
            if(dist.magnitude > this.range)
            {
                hitPoint = this.center + dist.normalized * this.range;
            }

            this.projector.gameObject.transform.position = hitPoint + offset;
            if(Input.GetMouseButtonDown(0))
            {
                this.selectPoint(hitPoint);
                this.Active(false);
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            this.Active(false);
        }
    }
    //开放给外部使用
    public static void ShowSelector(Vector3Int center,int range,int size,Action<Vector3> onPositionSelected)
    {
        if (TargetSelector.Instance == null) return;
        TargetSelector.Instance.center = GameObjectTool.LogicToWorld(center);
        TargetSelector.Instance.range = GameObjectTool.LogicToWorld(range);
        TargetSelector.Instance.size = GameObjectTool.LogicToWorld(size);
        TargetSelector.Instance.selectPoint = onPositionSelected;
        TargetSelector.Instance.Active(true);
    }
}
