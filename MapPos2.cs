using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using NUnit.Framework.Api;

public class MapPos2 : MonoBehaviour {
   
    public GameObject box;
    public GameObject star;
    public GameObject target;
    public GameObject pathBox;
    public GameObject startGame;

    public int FloorMask;
    public int[,] map;//用来记录地图的信息
    public Pos[,] sPos;//用来储存每个格子的步数
    List<Pos> SearchPos;//用来记录同样步数的坐标
    List<Pos> PathPos;
    List<Pos> PositionRecorder;
    private Vector3 mousemap;
    private bool hasEnd = false;
    private bool hasPath=false;
    //public int FloorMask;
    public int step = 1;
    
    void Start ()
    {
        FloorMask = LayerMask.GetMask("floor");
        map = new int[30, 30];
        sPos = new Pos[30, 30];//用来记录步数
        SearchPos = new List<Pos>();//实例化一个列表
        PathPos = new List<Pos>();//实例化一个列表
        PositionRecorder=new List<Pos>();
        ReadMapFile();
        Draw();
    }
 
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Setmap();
            hasEnd = true;
        }
        if (hasEnd)
        {
            DealList();
            DrawPath();
        }
        Debug.Log(PositionRecorder.Count);
        if (hasPath)
        {
            MoveToTarget();
        }
       
        
    }
    public void Setmap()
    {
        int x;
        int y;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, FloorMask))
        {
            mousemap = hit.point;//鼠标点击的位置
            x = Mathf.RoundToInt(mousemap.x);//吧鼠标点击的位置坐标转化为int
            y = -Mathf.RoundToInt(mousemap.z);
            mousemap.y = 0.5f;
            map[x, y] = 9;//在地图上标记结束的点
            sPos[x, y] = new Pos(x, y);
            PathPos.Add(sPos[x, y]);
            //Instantiate(star, , Quaternion.identity);
        }
    }
    public void ReadMapFile()//读取TXT文件
    {
        string path = Application.dataPath + "//" + "map2.txt";
        if (!File.Exists(path))
        {
            return;
        }

        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        StreamReader read = new StreamReader(fs, Encoding.Default);
        string strReadline;
        int y = 0;

        while ((strReadline = read.ReadLine()) != null)
        {
            for (int x = 0; x < strReadline.Length; ++x)
            {
                if (strReadline[x] == '1')
                {
                    map[x, y] = 1;//墙的标记
                }
                if (strReadline[x] == '8')
                {
                    map[x, y] = 8;//开始的位置
                    sPos[x, y] = new Pos(x, y); //把坐标写到Pos类型的变量中;
                    sPos[x, y].step = 1;//记录开始的步数是1
                    SearchPos.Add(sPos[x, y]);
                }
                /*if (strReadline[x] == '9')
                {
                    map[x, y] = 9;//结束的位置
                    sPos[x, y] = new Pos(x, y);
                    PathPos.Add(sPos[x, y]);
                }*/
            }
            y += 1;
        }
    }
    void Draw()
    {
       
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                if (map[i, j] == 1)//画出墙
                {
                    Instantiate(box, new Vector3(i, 0.5f, -j), Quaternion.identity);
                  
                }
                if (map[i, j] == 8)//画出起点
                {
                     startGame=Instantiate(star, new Vector3(i, 0.5f, -j), Quaternion.identity);
                   
                }
            }
        }
    }
    //处理列表里面的步数
    void DealList()
    {
        
        int x;//用来记录横坐标
        int y;//用来记录纵坐标

        if (SearchPos.Count >= 1)
        {
           
            x=SearchPos[0].X ;
            y=SearchPos[0].Y ;
            /*if (map[x + 1, y] == 9 || map[x - 1, y] == 9 || map[x, y - 1] == 9 || map[x, y + 1] == 9)
            {
                Debug.Log("END");
                return;
            }*/
            if (map[x + 1, y] == 9)
            {
                sPos[x + 1, y] = new Pos(x + 1, y);
                sPos[x + 1, y].step = sPos[x, y].step + 1;
                //DrawPath();
                return;
            }
            if (map[x -1, y] == 9)
            {
                sPos[x - 1, y] = new Pos(x - 1, y);
                sPos[x - 1, y].step = sPos[x, y].step + 1;
                //DrawPath();
                return;
            }
            if (map[x , y-1] == 9)
            {
                sPos[x , y-1] = new Pos(x , y-1);
                sPos[x , y-1].step = sPos[x, y].step + 1;
                //DrawPath();
                return;
            }
            if (map[x , y+1] == 9)
            {
                sPos[x , y+1] = new Pos(x , y+1);
                sPos[x , y+1].step = sPos[x, y].step + 1;
                //DrawPath();
                return;
            }

            if (map[x + 1, y] !=1 && sPos[x + 1, y]==null)
            {
                sPos[x + 1, y] = new Pos(x + 1, y);
                sPos[x + 1, y].step = sPos[x, y].step + 1;
                SearchPos.Add(sPos[x + 1, y]);
                Instantiate(target, new Vector3(x+1, 0f, -y), Quaternion.identity);
            }
            if (map[x - 1, y] != 1 && sPos[x - 1, y]==null)
            {
                sPos[x - 1, y] = new Pos(x - 1, y);//实例化
                sPos[x - 1, y].step = sPos[x, y].step + 1;
                SearchPos.Add(sPos[x - 1, y]);//增加到待处理列表
                Instantiate(target, new Vector3(x -1, 0f, -y), Quaternion.identity);
            }
            if (map[x , y-1] != 1 && sPos[x , y-1]==null)
            {
                sPos[x, y - 1] = new Pos(x, y - 1);
                sPos[x , y-1].step = sPos[x, y].step + 1;
                SearchPos.Add(sPos[x , y-1]);
                Instantiate(target, new Vector3(x , 0f, 1-y), Quaternion.identity);
            }
            if (map[x , y+1] != 1 && sPos[x , y+1]==null)
            {
                sPos[x, y + 1] = new Pos(x, y + 1);
                sPos[x , y+1].step = sPos[x, y].step + 1;
                SearchPos.Add(sPos[x , y+1]);
                Instantiate(target, new Vector3(x, 0f, -y-1), Quaternion.identity);
            }
            SearchPos.RemoveAt(0);  
        } 
    }
    void DrawPath()
    {
        int x;
        int y;
        if (PathPos[0].step==1)
        {
            hasPath=true;
        }
        if (PathPos.Count > 0)
        {
            x = PathPos[0].X;
            y = PathPos[0].Y;
            
            if (sPos[x - 1, y]!=null&&sPos[x - 1, y].step == (sPos[x, y].step - 1))
            {
                PathPos.Add(sPos[x-1,y]);//记录一个路线
                PositionRecorder.Add(sPos[x - 1, y]);//记录路线位置
                Instantiate(pathBox, new Vector3(x-1, 0.2f, -y), Quaternion.identity);
                PathPos.RemoveAt(0);
            }
            else if (sPos[x + 1, y] != null && sPos[x + 1, y].step == (sPos[x, y].step - 1))
            {
                PathPos.Add(sPos[x + 1, y]);
                PositionRecorder.Add(sPos[x + 1, y]);
                Instantiate(pathBox, new Vector3(x+1, 0.2f, -y), Quaternion.identity);
                PathPos.RemoveAt(0);
            }
            else if (sPos[x , y-1] != null && sPos[x , y-1].step == (sPos[x, y].step - 1))
            {
                PathPos.Add(sPos[x , y-1]);
                PositionRecorder.Add(sPos[x , y-1]);
                Instantiate(pathBox, new Vector3(x, 0.2f, 1-y), Quaternion.identity);
                PathPos.RemoveAt(0);
            }
            else if (sPos[x , y+1] != null && sPos[x , y+1].step == (sPos[x, y].step - 1))
            {
                PathPos.Add(sPos[x , y+1]);
                PositionRecorder.Add(sPos[x , y+1]);
                Instantiate(pathBox, new Vector3(x, 0.2f, -y-1), Quaternion.identity);
                PathPos.RemoveAt(0);
            }
        }
        //PathPos.RemoveAt(0);
    }

    void MoveToTarget()
    {
        int n = PositionRecorder.Count-1;
        int x;
        int y;
        if (PositionRecorder.Count == 0)
        {
            startGame.transform.position = mousemap;
        }
        if (PositionRecorder.Count > 0  )
        {
            x = PositionRecorder[n].X; //向目标移动第一个点的位置信息，类型为Pos
            y = PositionRecorder[n].Y;
            Debug.Log(startGame.transform.position);
            startGame.transform.position=new Vector3(x,0.5f,-y);
            PositionRecorder.RemoveAt(n);
        }
    }
}
