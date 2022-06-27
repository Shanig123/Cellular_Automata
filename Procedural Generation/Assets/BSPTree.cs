using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * https://1217pgy.tistory.com/5
 * 참고 링크
 */


public class TREE_NODE_INFO
{
    public TREE_NODE_INFO(Vector3 _vPos, 
        float _width,
        float _height, 
        float _splitPer)
    {
        vPos = _vPos;
        this.width = _width;
        this.height = _height;
        this.splitPer = _splitPer;
    }
    public TREE_NODE_INFO LeftNode = null;
    public TREE_NODE_INFO RightNode = null;
    public TREE_NODE_INFO ParentNode = null;

    public Vector3 vPos;
    public float width;
    public float height;
    public float splitPer;

    public bool bRoomTruefalse = false;

    public string nodeName;

}

public class BSPTree : MonoBehaviour
{

    public Vector3 m_vLeftTopPos;
    public Vector3 m_vRightBottomPos;

    public GameObject m_cube;

    [Range(0, 1)]
    public float m_fMinPer;
    [Range(0, 1)]
    public float m_fMaxPer;
    public float m_fMinSize;

    public Vector3 m_vTileSizeOffset;
    public Vector3 m_vCreatePos;

    public List<TREE_NODE_INFO> m_BSPTree;
    public Dictionary<string,TREE_NODE_INFO> m_treeBSPTree;
    public TREE_NODE_INFO m_treenode;
    private float m_sizeY;

    // Start is called before the first frame update
    void Start()
    {
        m_treeBSPTree = new Dictionary<string, TREE_NODE_INFO>();
        m_sizeY = (m_vLeftTopPos.y + m_vRightBottomPos.y) * 0.5f;

        float width = Mathf.Abs(m_vLeftTopPos.x - m_vRightBottomPos.x);
        float height = Mathf.Abs(m_vLeftTopPos.z - m_vRightBottomPos.z);
        float size = width * height;

        System.Random pseudoRandom = new System.Random(Time.time.GetHashCode()); //시드로 부터 의사 난수 생성
        float split = pseudoRandom.Next((int)(m_fMinPer * 100), (int)(m_fMaxPer * 100)) * 0.01f;

        m_treenode = new TREE_NODE_INFO(m_vCreatePos,width,height,0.5f);
        m_treenode.nodeName = "Center_0";
        m_treeBSPTree.Add(m_treenode.nodeName, m_treenode);
        Setting_BspTreeInfo(m_treeBSPTree[m_treenode.nodeName], 1, true);
        Create_Map(m_treeBSPTree[m_treenode.nodeName], 0,true);
        //CreateNode(m_treeBSPTree[m_treenode.nodeName]);
        print("MapSize" + m_treeBSPTree.Count.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Setting_BspTreeInfo(TREE_NODE_INFO _treenode, int _count,bool _LR)
    {
        float width = Mathf.Abs(_treenode.width);
        float height = Mathf.Abs(_treenode.height);
        float size = width * height;

        if (m_fMinSize< size)
        {

            System.Random pseudoRandom = new System.Random(System.DateTime.Now.GetHashCode()+ _count); //시드로 부터 의사 난수 생성
            float split = pseudoRandom.Next((int)(m_fMinPer * 100), (int)(m_fMaxPer * 100)) * 0.01f;

            if (_treenode.width > _treenode.height)    //가로 길때
            {
                int temp = _count + 1;
                float leftWidth = _treenode.width * split;

                Vector3 leftPos = new Vector3(_treenode.vPos.x - (_treenode.width*0.5f) +  (leftWidth*0.5f), _treenode.vPos.y, _treenode.vPos.z);
                _treenode.LeftNode = new TREE_NODE_INFO(leftPos, leftWidth, _treenode.height, split);
                _treenode.LeftNode.ParentNode = _treenode;
                _treenode.LeftNode.nodeName = _treenode.nodeName+ "L" + _count.ToString();
                print(_treenode.LeftNode.nodeName);
                m_treeBSPTree.Add(_treenode.LeftNode.nodeName, _treenode.LeftNode);
                Setting_BspTreeInfo(m_treeBSPTree[_treenode.LeftNode.nodeName], _count + 1, false);
  
                float rightWidth = _treenode.width * (1f - split);
 
                Vector3 rightPos = new Vector3(_treenode.vPos.x +(_treenode.width*0.5f) -  (rightWidth*0.5f), _treenode.vPos.y, _treenode.vPos.z);
                _treenode.RightNode = new TREE_NODE_INFO(rightPos, rightWidth, _treenode.height, split);
                _treenode.RightNode.ParentNode = _treenode;
                _treenode.RightNode.nodeName = _treenode.nodeName + "R" + _count.ToString();
                print(_treenode.RightNode.nodeName);
                    m_treeBSPTree.Add(_treenode.RightNode.nodeName, _treenode.RightNode);
                Setting_BspTreeInfo(m_treeBSPTree[_treenode.RightNode.nodeName], _count + 1, true);

                
            }
            else // 세로  //세로가 길때
            {
                int temp = _count + 1;
                float topHeight = _treenode.height * split;

                Vector3 topPos = new Vector3(_treenode.vPos.x, _treenode.vPos.y, _treenode.vPos.z +(_treenode.height*0.5f) -  (topHeight*0.5f));
                    _treenode.LeftNode = new TREE_NODE_INFO(topPos, _treenode.width, topHeight, split);
                    _treenode.LeftNode.ParentNode = _treenode;
                    _treenode.LeftNode.nodeName = _treenode.nodeName + "T" + _count.ToString();
                    print(_treenode.LeftNode.nodeName);
                    m_treeBSPTree.Add(_treenode.LeftNode.nodeName, _treenode.LeftNode);
                    Setting_BspTreeInfo(m_treeBSPTree[_treenode.LeftNode.nodeName], _count + 1, false);



                float bottomHeight = _treenode.height * (1f - split);
         
                    Vector3 bottomPos = new Vector3(_treenode.vPos.x, _treenode.vPos.y, _treenode.vPos.z - (_treenode.height*0.5f) + (bottomHeight*0.5f));
                    _treenode.RightNode = new TREE_NODE_INFO(bottomPos, _treenode.width, bottomHeight, split);
                    _treenode.RightNode.ParentNode = _treenode;
                    _treenode.RightNode.nodeName = _treenode.nodeName + "B" + _count.ToString();
                    print(_treenode.RightNode.nodeName);
                    m_treeBSPTree.Add(_treenode.RightNode.nodeName, _treenode.RightNode);
                    Setting_BspTreeInfo(m_treeBSPTree[_treenode.RightNode.nodeName], _count + 1, true);
    
            }

        }
        // else
        // {
        //     // print("SizeOut");
        //     _treenode.bRoomTruefalse = true;
        //     _treenode.LeftNode = null;
        //     _treenode.RightNode = null;
        // }

    }
    public int instancecount = 0;
    void Create_Map(TREE_NODE_INFO _treenode, int _count,bool _LR)
    {
        print("CREATEMAP/////////"+_treenode.nodeName);

        //if(_treenode.LeftNode != null)
        //{
        //    if(m_treeBSPTree[_treenode.LeftNode.nodeName].bRoomTruefalse)
        //    {
        //        CreateNode(m_treeBSPTree[_treenode.LeftNode.nodeName]);
        //    }
        //    else
        //    {
        //        Create_Map(m_treeBSPTree[_treenode.LeftNode.nodeName], _count + 1, false);
        //    }
           

        //}
        //if (_treenode.RightNode != null)
        //{
        //    if (m_treeBSPTree[_treenode.RightNode.nodeName].bRoomTruefalse)
        //    {
        //        CreateNode(m_treeBSPTree[_treenode.RightNode.nodeName]);
        //    }
        //    else
        //    {
        //        Create_Map(m_treeBSPTree[_treenode.RightNode.nodeName], _count + 1, true);
        //    }
              
        //}
        foreach(var iter in m_treeBSPTree)
        {
            //if(iter.Value.bRoomTruefalse)
            //{
            //    CreateNode(iter.Value);
            //}
            if (iter.Value.LeftNode == null &&
               iter.Value.RightNode == null)
            {
                CreateNode(iter.Value);
            }
        }


       
        
    }
    private void CreateNode(TREE_NODE_INFO _treenode)
    {
        print(++instancecount);

        Vector3 vSize = new Vector3(_treenode.width, m_sizeY, _treenode.height) - m_vTileSizeOffset;
        var temp = GameObject.Instantiate(m_cube, _treenode.vPos, Quaternion.identity);
        temp.transform.localScale = vSize;
        var node = temp.AddComponent<TREE_NODE>();
        temp.name += _treenode.nodeName;

        node.width = _treenode.width;
        node.vPos = _treenode.vPos;
        node.height = _treenode.height;
        node.size = node.width * node.height;
 
    }
}
