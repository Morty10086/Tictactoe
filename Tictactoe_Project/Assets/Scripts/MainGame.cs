using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
    //难度
    public int diffDegree;
    //控制难度曲线的参数
    public int myWin3;
    public int myDecue3;
    public int myWin2;
    public int myDecue2;
    public int myWin1;
    public int myDecue1;
    //棋盘
    public GameObject chessBoard;
    //获取所有按钮(格子)
    private Button[] btns;
    //存储格子位置
    private RectTransform[,] rectTrans=new RectTransform[3,3]; 
    //记录当前棋盘落子情况，0：空  1：玩家棋子  -1：AI棋子
    private int[,] whoChess=new int[3,3];

    //记录可能赢的点
    List<Vector2> mayWin=new();
    //没有赢面时的空位
    List<Vector2> cantWin=new();
    //记录对方后续可能赢时自己当前落子位置
    List<Vector2> mayOtherWin=new();
    //记录不会输也不会赢的落子位置
    List<Vector2> deucePos=new();

    //先手
    private int firstP;
    //当前下棋的玩家，1：玩家  -1：AI
    private int currentP;
    //玩家和AI代号
    const int PLAYER=1;
    const int AI=-1;
    //AI思考时间模拟
    private float aiThinkTime=0;
    public float minAIThinkTime;
    public float maxAIThinkTime;

    //已下棋子数量
    private int nowChessCount=0;

    //玩家当前是否可以输入
    private bool ifPlayerCanDo=true;
    //游戏是否已结束
    private bool isGameEnd;

    void Start()
    {
        diffDegree=DiffDegreeData.Instance.diffDegree;
        btns=chessBoard.GetComponentsInChildren<Button>();
        for(int i=0;i<btns.Length;i++)
        {
            int x=i/3;
            int y=i%3;

            rectTrans[x,y]=btns[i].GetComponent<RectTransform>();
            Button btn=btns[i];
            RectTransform rect=rectTrans[x,y];
            btn.onClick.AddListener(()=>PlayerDo(x,y,rect));
            //btns[i].onClick.AddListener(()=>PlayerDo(rectTrans[i]));
        }
        firstP=PLAYER;
        currentP=PLAYER;
        aiThinkTime=Random.Range(minAIThinkTime,maxAIThinkTime);
    }

    // Update is called once per frame
    void Update()
    {

        if(isGameEnd)
            return;
        if(currentP==AI)
        {
            UIManager.Instance.ChangeRoundTip(AI);
            if(aiThinkTime<=0)
            {
                AIDo();
                aiThinkTime=Random.Range(minAIThinkTime,maxAIThinkTime);
            }
            else
            {
                aiThinkTime-=Time.deltaTime;
            }
                
            
        }

        /*if(ifPlayerCanDo&&currentP==PLAYER)
        {
            UIManager.Instance.ChangeRoundTip(PLAYER);
        }*/

    }

    //玩家下棋
    public void PlayerDo(int x,int y,RectTransform rectTrans)
    {
        if(currentP==PLAYER&&ifPlayerCanDo&&whoChess[x,y]==0)
        {
            //print("aaa");
            UIManager.Instance.ShowChess(true,rectTrans);
            whoChess[x,y]=PLAYER;
            nowChessCount++;
            TurnP(AI);
            if(CheckWin(x,y)==PLAYER)
                EndGame(PLAYER);
            else if(nowChessCount==9)
                EndGame(0);
        }
    }


    //AI下棋
    public void AIDo()
    {       
        ifPlayerCanDo=false;
        //原通过深度搜索深度控制难度，已弃用
        /*if(DiffDegreeData.Instance.diffDegree==1)
        {
            int temp=Random.Range(1,31);
            if(temp>20)
                diffDegree=3;
            else if(temp<21&&temp>10)
                diffDegree=2;
            else
                diffDegree=1;
        }            
        else if(DiffDegreeData.Instance.diffDegree==2)
        {
            int temp=Random.Range(1,31);
            if(temp>10)
                diffDegree=3;
            else
                diffDegree=1;
        }
        else
        {
            int temp=Random.Range(1,51);
            if(temp>10)
                diffDegree=3;
            else
                diffDegree=1;
        }*/
        print(GuessBestPos(nowChessCount));
        int r=Random.Range(1,51);
        int win=0;
        int decue=0;
        if(DiffDegreeData.Instance.diffDegree==3)
        {
            win=myWin3;  
            decue=myDecue3; 
        }        
        else if(DiffDegreeData.Instance.diffDegree==2)
        {
            win=myWin2;  
            decue=myDecue2; 
        }
        else
        {
             win=myWin1;  
            decue=myDecue1; 
        }           
         print(r);
        if(mayWin.Count!=0&&r>win)
        {
            print("1111");
            int bestIndex=Random.Range(0,mayWin.Count);
            bestX=(int)mayWin[bestIndex].x;
            bestY=(int)mayWin[bestIndex].y;
        }  
        else if(deucePos.Count!=0&&r>decue)
        {
                print("2222");
            int bestIndex=Random.Range(0,deucePos.Count);
            bestX=(int)deucePos[bestIndex].x;
            bestY=(int)deucePos[bestIndex].y;
        }
        else
            CantWinPos();

        whoChess[bestX,bestY]=AI;        
        nowChessCount++;
        //print("AI");
        UIManager.Instance.ShowChess(false,rectTrans[bestX,bestY]);
        
        mayWin.Clear();
        mayOtherWin.Clear();
        deucePos.Clear();
        ifPlayerCanDo=true;
        cantWin.Clear();
        TurnP(PLAYER);
       
        if(CheckWin(bestX,bestY)==AI)
        {
            EndGame(AI);
        }
        else if(nowChessCount==9)
        {
            EndGame(0);
        }
        
    }
    //AI猜测合适落子位置
    int bestX;
    int bestY;
    
    int falseDepth=0;

    public int GuessBestPos(int chessCount)
    {
        if(nowChessCount==0)
        {
            bestX=Random.Range(0,3);
            bestY=Random.Range(0,3);
            return 0;
        }
        
        if(chessCount>=9)
            return 0;
        int bestValue;
        int value;
        
        if(currentP==AI)
            bestValue=int.MinValue;
        else
            bestValue=int.MaxValue;
        
        for(int i=0;i<3;i++)
        {
            for(int j=0;j<3;j++)
            {
                if(whoChess[i,j]!=0)
                    continue;         
                if(currentP==AI)
                {
                    SimulatePlay(i,j);
                    if(CheckWin(i,j)==AI)
                        value=int.MaxValue;
                    else
                        value=GuessBestPos(chessCount+1);                    
                    CancelPlay(i,j);
                    if(value>=bestValue)
                    {
                        bestValue=value;
                        if(chessCount==nowChessCount&&value>=0)
                        {
                             bestX=i;
                             bestY=j;
                             mayWin.Add(new Vector2(bestX,bestY));
                        }                          
                    }
                    if(chessCount==nowChessCount&&value==0)
                        deucePos.Add(new Vector2(i,j));             
                    if(chessCount==nowChessCount&&value<0)
                        mayOtherWin.Add(new Vector2(i,j));
                }
                else
                {
                    SimulatePlay(i,j);                                  
                    if(CheckWin(i,j)==PLAYER)
                    {
                        value=int.MinValue;                        
                    }                        
                    else
                        value=GuessBestPos(chessCount+1);                        
                    CancelPlay(i,j);
                    if(value<=bestValue)
                    {
                        bestValue=value;  
                    }
                }
          
            }          
        }
        
        return bestValue;
    }

    private void SimulatePlay(int x,int y)
    {
        whoChess[x,y]=currentP;
        currentP=currentP==PLAYER?AI:PLAYER;
    }

    private void CancelPlay(int x,int y)
    {
        whoChess[x,y]=0;
        currentP=currentP==PLAYER?AI:PLAYER;
    }
    //无论怎样都会输时，随便找一个空位落子
    private void CantWinPos()
    {
        print("4444");
        int r=0;
        for(int i=0;i<3;i++)
        {
            for(int j=0;j<3;j++)
            {
                if(whoChess[i,j]==0)
                {
                    int x=i;
                    int y=j;
                    cantWin.Add(new Vector2(x,y));
                }
                    
            }
        }
        if(cantWin.Count>0)
            r=Random.Range(0,cantWin.Count);
        bestX=(int)cantWin[r].x;
        bestY=(int)cantWin[r].y;

    }
    
    //每次落完一子都检查是否有玩家赢了
    private int CheckWin(int x,int y)
    {
        int nowPosP=whoChess[x,y];
        bool isWin=true;
        //横向
        for(int i=1;i<=2;i++)
        {
            if(whoChess[x,(y+i)%3]!=nowPosP)
            {
                isWin=false;
                break;
            }
        }
        if(isWin)
            return nowPosP;
        //纵向
        isWin=true;
        for(int j=1;j<=2;j++)
        {
            if(whoChess[(x+j)%3,y]!=nowPosP)
            {
                isWin=false;
                break;
            }
        }
         if(isWin)
            return nowPosP;
        //斜向
        int delta=Mathf.Abs(x-y);
        if(delta!=1)
        {
            if(delta==0)
            {
                isWin=true;
                for(int i=1;i<=2;i++)
                {
                    if(whoChess[(x+i)%3,(y+i)%3]!=nowPosP)
                    {
                        isWin=false;
                        break;
                    }
                }
                if(isWin)
                    return nowPosP;
            }

            if(delta==2||(x==1&&y==1))
            {
                isWin=true;
                for(int i=1;i<=2;i++)
                {
                    if(whoChess[(x+i)%3,(y+3-i)%3]!=nowPosP)
                    {
                        isWin=false;
                        break;
                    }
                }
                if(isWin)
                    return nowPosP;
            }
        }
        
        return 0;
    }
    //切换回合
    public void TurnP(int p)
    {
        currentP=p;
        if(p==PLAYER)
        {
            if(ifPlayerCanDo)
                UIManager.Instance.ChangeRoundTip(p);
        }
        else
        {
            UIManager.Instance.ChangeRoundTip(p);
        }      
    }

    //一方获胜或平局结束游戏
    private void EndGame(int winner)
    {
        isGameEnd=true;
        UIManager.Instance.ChangeScoreTip(winner);
        UIManager.Instance.ShowGameEnd(winner);

    }

    //重新开始
    public void RestartGame()
    {
        isGameEnd=false;
        nowChessCount=0;
        for(int i=0;i<3;i++)
        {
            for(int j=0;j<3;j++)
            {
                whoChess[i,j]=0;
            }
        }
        if(firstP==PLAYER)
            firstP=AI;
        else
            firstP=PLAYER;
        TurnP(firstP);
        UIManager.Instance.GameAgine(firstP);

    }
    //返回主页
    public void BackHome()
    {
        SceneManager.LoadScene("BeginScene");
    }
    //退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }
}
