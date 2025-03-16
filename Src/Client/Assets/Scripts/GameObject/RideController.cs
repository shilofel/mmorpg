using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;

public class RideController : MonoBehaviour
{
    public Transform mountPoint;
    public EntityController rider;
    public Animator anim;

    public UnityEngine.Vector3 offset;

    // Use this for initialization
    void Start()
    {
        this.anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (this.mountPoint == null || this.rider == null) return;
        this.rider.SetRidePosition(this.mountPoint.position + this.mountPoint.TransformDirection(this.offset));
    }

    void OnDestroy()
    {
    }

    public void SetRider(EntityController rider)
    {
        this.rider = rider;
    }

    //设置动画
    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }
    }
}
