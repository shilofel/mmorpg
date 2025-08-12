using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera camera;
    public Transform viewPoint;

    public GameObject player;

    public float followSpeed = 5f;
    public float rotateSpeed = 5f;

    Quaternion yaw = Quaternion.identity;
    //相机位置始终跟随玩家位置
    private void LateUpdate()
    {
        if (player == null&&User.Instance.CurrentCharacterObject != null)
        {
            player = User.Instance.CurrentCharacterObject.gameObject;
        }
        if (player == null)
            return;
        //插值
        this.transform.position = Vector3.Lerp(this.transform.position, player.transform.position, Time.deltaTime * followSpeed);
        if(Input.GetMouseButton(1))//鼠标右键控制旋转
        {
            Vector3 angleBase = this.transform.localRotation.eulerAngles;
            this.transform.localRotation = Quaternion.Euler(angleBase.x - Input.GetAxis("Mouse Y") * rotateSpeed,
                angleBase.y + Input.GetAxis("Mouse X") * rotateSpeed, 0);
            Vector3 angle = this.transform.eulerAngles - player.transform.rotation.eulerAngles;
            angle.z = 0;
            yaw = Quaternion.Euler(angle);
        }
        else
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, player.transform.rotation * yaw, Time.deltaTime * followSpeed);
        }
        if(Input.GetAxis("Vertical")>0.01)
        {
            yaw = Quaternion.Lerp(yaw, Quaternion.identity, Time.deltaTime * followSpeed);
        }
        //this.transform.rotation = player.transform.rotation;
    }
}
