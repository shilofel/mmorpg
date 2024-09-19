using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using System;
using Managers;

public class UIMiniMap : MonoBehaviour {
    public Collider miniMapBoundingBox;
    public Image miniMap;
    public Image arrow;
    public Text mapName;

    private Transform playerTransform;
	// Use this for initialization
	void Start () {
        MinimapManager.Instance.minimap = this;
        this.UpdateMap();
	}
    //更换地图 调用
    public void UpdateMap()
    {
        this.mapName.text = User.Instance.CurrentMapData.Name;

        this.miniMap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();

        this.miniMap.SetNativeSize();
        this.miniMap.transform.localPosition = Vector3.zero;
        this.miniMapBoundingBox = MinimapManager.Instance.MiniMapBoundingBox;
        this.playerTransform = null;
    }

    // Update is called once per frame
    void Update () {

        //if(this.playerTransform ==null&& User.Instance.CurrentCharacterObject!=null)
        if (this.playerTransform == null)
            playerTransform = MinimapManager.Instance.PlayerTransform;
            //this.playerTransform = User.Instance.CurrentCharacterObject.transform;
        if (miniMapBoundingBox == null || playerTransform == null) return;
        //坐标转换
        float realWidth = miniMapBoundingBox.bounds.size.x;
        float realHeight = miniMapBoundingBox.bounds.size.z;

        float relaX = playerTransform.position.x - miniMapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - miniMapBoundingBox.bounds.min.z;

        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;

        this.miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.miniMap.rectTransform.localPosition = Vector2.zero;
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);

    }
}
