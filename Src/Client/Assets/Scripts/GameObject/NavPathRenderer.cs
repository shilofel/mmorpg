using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavPathRenderer:MonoSingleton<NavPathRenderer>
{
    LineRenderer pathRender;
    NavMeshPath path;

    private void Start()
    {
        pathRender = this.GetComponent<LineRenderer>();
        pathRender.enabled = false;
    }

    public void SetPath(NavMeshPath path, Vector3 target)
    {
        this.path = path;
        if(this.path == null)
        {
            pathRender.enabled = false;
            pathRender.positionCount = 0;
        }
        else
        {
            pathRender.enabled = true;
            pathRender.positionCount = path.corners.Length + 1;
            pathRender.SetPositions(path.corners);
            pathRender.SetPosition(pathRender.positionCount - 1, target);

            for(int i=0;i<pathRender.positionCount;i++)
            {
                pathRender.SetPosition(i, pathRender.GetPosition(i) + Vector3.up * 0.2f);
            }
        }
    }
}
