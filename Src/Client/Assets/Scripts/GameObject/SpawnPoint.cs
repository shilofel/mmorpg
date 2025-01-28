using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    // Use this for initialization
    public int ID;
    Mesh mesh = null;
    void Start()
    {
        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //仅在编辑器下生效
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 pos = this.transform.position + Vector3.up * this.transform.localScale.y * .5f;
        Gizmos.color = Color.red;
        if (this.mesh != null)
        {
            Gizmos.DrawWireMesh(this.mesh, pos,this.transform.rotation, this.transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, this.transform.position, this.transform.rotation, 1f, EventType.Repaint);
        UnityEditor.Handles.Label(pos, "SpawnPoint" + this.ID);
    }
#endif
}
