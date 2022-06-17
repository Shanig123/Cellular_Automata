using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata3D : MonoBehaviour
{
    [SerializeField] private int width;//x
    [SerializeField] private int height;//y
    [SerializeField] private int length;//z

    [SerializeField] private int seed;
    [SerializeField] private bool useRandomSeed;

    [Range(0, 100)]
    [SerializeField] private int randomFillPercent;
    [SerializeField] private int smoothNum;
    [SerializeField] private int neighbourCheckCount=4;
  
    private int[,,] map;
    private const int ROAD = 0;
    private const int WALL = 1;

    //[SerializeField] private Tilemap tilemap;
    //[SerializeField] private Tile tile;
    //[SerializeField] private Texture2D m_Cellular;
    //[SerializeField] private Color[] m_Cellularcolors;
    [SerializeField] private GameObject m_CellularAutomataPixelPrefab;
    [SerializeField] private GameObject[,,] m_CellularAutomataPixel;
    [SerializeField] private Texture2D m_Noise;
    [SerializeField] private Color[] m_NoiseColors;
    [SerializeField] private Renderer m_Renderer;
    private void Awake()
    {
        m_CellularAutomataPixelPrefab.GetComponent<Renderer>().sharedMaterial.enableInstancing = true;
        m_Renderer = m_CellularAutomataPixelPrefab.GetComponent<Renderer>();
    }
    private void Start()
    {
        //m_Renderer = GetComponent<Renderer>();
        m_Noise = GameObject.Find("PerlinNoise").GetComponent<PerlinNoise>().GetTex();
        m_NoiseColors = m_Noise.GetPixels();
        m_CellularAutomataPixel = new GameObject[width,height,length];
        for (int i=0; i<width;++i)
        {
            for (int j = 0; j< height; ++j)
            {
                for (int k = 0; k < length; ++k)
                {
                   var obj = GameObject.Instantiate(
                        m_CellularAutomataPixelPrefab,
                        new Vector3(i, j, k),
                        Quaternion.identity,
                        transform);
                    obj.name = obj.name + i.ToString() + "_" + j.ToString() + "_" + k.ToString();
              

                    m_CellularAutomataPixel[i, j, k] = obj;

                }
            }
        }
        map = new int[width, height, length];
    }
    private bool clockCheck =false;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) GenerateMap();
    }

    private void GenerateMap()
    {
        //if (clockCheck)
        //{
        //    clockCheck = !clockCheck;
        //    MapRandomFill();
        //}
        //else
        //{
        //    clockCheck = !clockCheck;
        //    StartCoroutine(corutine());
        //}

        //MapRandomFill();
        //for (int i = 0; i < smoothNum; i++) //반복이 많을수록 동굴의 경계면이 매끄러워진다.
        //{
        //    SmoothMap();
        //}
        MapRandomFill();
        StartCoroutine(corutine());
    }
    
    private IEnumerator corutine()
    {
        for (int i = 0; i < smoothNum; i++) //반복이 많을수록 동굴의 경계면이 매끄러워진다.
        {
            yield return null;
            SmoothMap();
        }
        yield return null;
    }
    private void MapRandomFill() //맵을 비율에 따라 벽 혹은 빈 공간으로 랜덤하게 채우는 메소드
    {
        if (useRandomSeed) seed = System.DateTime.Now.GetHashCode(); ; //시드

        System.Random pseudoRandom = new System.Random(seed); //시드로 부터 의사 난수 생성

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    float noisePer=Mathf.PerlinNoise( m_Noise.GetPixel(x, y).r, pseudoRandom.Next(0, 100) * 0.01f);
                   
                    //if (x == 0 || x == width - 1 ||
                    //    y == 0 || y == height - 1 |
                    //    z == 0 || z == length - 1
                    //    )
                    //{
                    //    map[x, y, z] = ROAD;
                    //}//가장자리는 벽으로 채움
                    //else
                    //{
                        //float noisePer = pseudoRandom.Next(0, 100) * 0.01f;
                        float per = randomFillPercent * 0.01f;
                        map[x, y, z] = (noisePer < per) ? WALL : ROAD;
                    //}//비율에 따라 벽 혹은 빈 공간 생성
                    //OnDrawTile(x, y); //타일 생성
                    SetTileColor(x, y,z); //타일 색상 설정
                }
            
            }
        }
        //m_Cellular.Apply();
    }

    private void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y,z);
                    if (neighbourWallTiles > neighbourCheckCount)
                    {
                        map[x, y, z] = WALL;
                        print("WALL");
                    }//주변 칸 중 벽이 4칸을 초과할 경우 현재 타일을 벽으로 바꿈
                    else if (neighbourWallTiles < 4)
                    {
                        map[x, y, z] = ROAD;
                        print("ROAD");
                    }//주변 칸 중 벽이 4칸 미만일 경우 현재 타일을 빈 공간으로 바꿈
                    else if (neighbourWallTiles < neighbourCheckCount)
                    {
                        map[x, y, z] = ROAD;
                        print("ROAD");
                    }//주변 칸 중 벽이 4칸 미만일 경우 현재 타일을 빈 공간으로 바꿈
                    SetTileColor(x, y,z); //타일 색상 변경
                }
            }
        }
    }

    private int GetSurroundingWallCount(int gridX, int gridY, int gridZ)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        { //현재 좌표를 기준으로 주변 8칸 검사
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                for (int neighbourZ = gridZ - 1; neighbourZ <= gridZ + 1; neighbourZ++)
                {
                    if (neighbourX >= 0 && 
                        neighbourX < width && 
                        neighbourY >= 0 && 
                        neighbourY < height &&
                        neighbourZ >= 0 &&
                        neighbourZ < length
                        )
                    { //맵 범위를 초과하지 않게 조건문으로 검사
                        if (neighbourX != gridX ||
                            neighbourY != gridY ||
                            neighbourZ != gridZ
                            )
                        //벽은 1이고 빈 공간은 0이므로 벽일 경우 wallCount 증가                        
                        {
                            wallCount += map[neighbourX, neighbourY, neighbourZ];
                        }
                    }
                    else wallCount++; //주변 타일이 맵 범위를 벗어날 경우 wallCount 증가
                }
                 
            }
        }
        print(wallCount);
        return wallCount;
    }

    private void SetTileColor(int x, int y, int z)
    {
        //Vector3Int pos = new Vector3Int(-width / 2 + x, -height / 2 + y, 0); //화면 중앙 정렬
        //tilemap.SetTileFlags(pos, TileFlags.None); //타일 색상을 수정하기 위해 TileFlags를 None으로 설정
      // print(map[x, y, z]);
        switch (map[x, y,z])
        {
            //      noiseTex.SetPixels(pix);
            //noiseTex.Apply();
            case ROAD: m_CellularAutomataPixel[x,y,z].SetActive(false); break;
            case WALL: m_CellularAutomataPixel[x, y, z].SetActive(true); break;
        }

    }

    //private void OnDrawTile(int x, int y)
    //{
    //    Vector3Int pos = new Vector3Int(-width / 2 + x, -height / 2 + y, 0);
    //    //tilemap.SetTile(pos, tile);
    //}
}
