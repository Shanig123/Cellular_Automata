using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TREE_NODE : MonoBehaviour
{

    public TREE_NODE(Vector3 _vPos,
         float _width,
         float _height,
         float _splitPer)
        :base()
    {
        vPos = _vPos;
        this.width = _width;
        this.height = _height;
        this.splitPer = _splitPer;
    }

    public Vector3 vPos;
    public float width;
    public float size;
    public float height;
    public float splitPer;
}
