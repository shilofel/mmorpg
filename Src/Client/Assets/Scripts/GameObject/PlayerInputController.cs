using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities;
using SkillBridge.Message;
using Services;
using System;

public class PlayerInputController : MonoBehaviour {

    public Rigidbody rb;

    SkillBridge.Message.CharacterState state;

    public Character character;

    public EntityController entityController;

    public float rotateSpeed = 2.0f;
    public float turnAngle = 10;
    public int speed;
    public bool onAir = false;

    public NavMeshAgent agent;
    private bool autoNav = false;

    public bool enableRigidbody
    {
        get { return !this.rb.isKinematic; }
        set
        {
            this.rb.isKinematic = !value;
            this.rb.detectCollisions = value;
        }
    }
    // 角色状态设置为idle
    void Start () {
        state = SkillBridge.Message.CharacterState.Idle;
        //角色为空，重新读取
        //if(this.character == null)
        //{
        //    DataManager.Instance.Load();
        //    NCharacterInfo cinfo = new NCharacterInfo();
        //    cinfo.Id = 1;
        //    cinfo.Name = "Test";
        //    cinfo.configId = 1;
        //    cinfo.Entity = new NEntity();
        //    cinfo.Entity.Position = new NVector3();
        //    cinfo.Entity.Direction = new NVector3();
        //    cinfo.Entity.Direction.X = 0;
        //    cinfo.Entity.Direction.Y = 100;
        //    cinfo.Entity.Direction.Z = 0;
        //    this.character = new Character(cinfo);

        //    if (entityController != null) entityController.entity = this.character;
        //}

        if(agent ==null)
        {
            agent = this.gameObject.AddComponent<NavMeshAgent>();
            //防止角色重合
            agent.stoppingDistance = 0.3f;
            agent.updatePosition = false;
        }
	}

    public void StartNav(Vector3 target)
    {
        StartCoroutine(BeginNav(target));
    }

    private IEnumerator BeginNav(Vector3 target)
    {
        agent.updatePosition = true;
        agent.SetDestination(target);
        yield return null;
        autoNav = true;
        if(state!=SkillBridge.Message.CharacterState.Move)
        {
            state = SkillBridge.Message.CharacterState.Move;
            this.character.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            agent.speed = this.character.speed / 100f;
        }
    }

    public void StopNav()
    {
        autoNav = false;
        agent.ResetPath();
        if (state != SkillBridge.Message.CharacterState.Idle)
        {
            state = SkillBridge.Message.CharacterState.Idle;
            this.rb.velocity = Vector3.zero;
            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }
        agent.updatePosition = false;
        NavPathRenderer.Instance.SetPath(null,Vector3.zero);
    }

    public void NavMove()
    {
        if (agent.pathPending) return;
        if(agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            StopNav();
            return;
        }
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) return;
        if(Mathf.Abs(Input.GetAxis("Vertical"))>0.1||Mathf.Abs(Input.GetAxis("Horizontal"))>0.1)
        {
            StopNav();
            return;
        }

        NavPathRenderer.Instance.SetPath(agent.path, agent.destination);
        if (agent.isStopped||agent.remainingDistance< 1)
        {
            StopNav();
            return;
        }
    }

    internal void OnLevelLevel()
    {
        this.enableRigidbody = false;
        this.rb.velocity = Vector3.zero;
    }

    internal void OnEnterLevel()
    {
        this.rb.velocity = Vector3.zero;
        //恢复之前进行一次状态更新，确保位置正确，再启用刚体
        this.entityController.UpdateTransform();
        this.lastPos = this.rb.transform.position;
        this.enableRigidbody = true;
    }

    private void FixedUpdate()
    {
        if (character == null||!character.ready)
            return;
        if(autoNav)
        {
            NavMove();
            return;
        }
        if (InputManager.Instance != null &&InputManager.Instance.IsInputMode) return;

        float v = Input.GetAxis("Vertical");
        if (v > 0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.8f) / 100f;
        }
        else if (v < -0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.8f) / 100f;
        }
        else
        {
            if (state != SkillBridge.Message.CharacterState.Idle)
            {
                state = SkillBridge.Message.CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                character.Stop();
                character.EntityData.Speed = 0;
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        //在转向
        float h = Input.GetAxis("Horizontal");
        if (h > 0.1 || h < -0.1)
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            //转向有最小角度限制
            if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }
        }
        //Debug.LogFormat("velocity {0}", this.rb.velocity.magnitude);

    }

    Vector3 lastPos;

    float lastSync = 0;

    //位置同步
    private void LateUpdate()
    {
        if (this.character == null||!character.ready) return;
        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude*100f/Time.deltaTime);

        this.lastPos = this.rb.transform.position;

        Vector3Int goLogicPos = (GameObjectTool.WorldToLogic(this.rb.transform.position));
        float logicOffset = (goLogicPos - this.character.position).magnitude;

        if (logicOffset > 50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;

        Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir, this.transform.forward);

        if(rot.eulerAngles.y>this.turnAngle&&rot.eulerAngles.y<(360-this.turnAngle))
        {
            character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
            this.SendEntityEvent(EntityEvent.None);
        }
    }

    public void SendEntityEvent(EntityEvent entityEvent,int param =0)
    {
        if (entityController != null)
            entityController.OnEntityEvent(entityEvent, param);
        //事件、实体数据
        MapService.Instance.SendMapEntitySync(entityEvent, character.EntityData, param);
    }
}
