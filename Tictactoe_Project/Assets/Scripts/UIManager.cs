using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    private static UIManager instance;

    public static UIManager Instance=>instance;

    //存储显示过的棋子，便于重置时销毁
    List<GameObject> chessObjs=new();

    //棋子父物体
    public GameObject chesses;
    //设置面板
    public GameObject settingPanel;
    //回合提示
    public Text roundText;
    //计分
    private int aiScore=0;
    private int playerScore=0;
    public Text aiScoreText;
    public Text playerScoreText;
    //结算面板
    public GameObject gameEndPanel;
    //结果
    public Text reslut;
    void Awake()
    {
        instance=this;
    }

    //显示棋子
    public void ShowChess(bool isPlayer,RectTransform rectTrans)
    {
        if(isPlayer)
        {
            GameObject chess=Resources.Load<GameObject>("Prefab/PlayerChess");   
            GameObject chessObj=Instantiate(chess);
            chessObj.transform.SetParent(chesses.transform,false);
            chessObj.GetComponent<RectTransform>().position=rectTrans.position;
            chessObjs.Add(chessObj);

            AudioManager.Instance.PlayAudio(1);
            Image image=chessObj.GetComponent<Image>();
            image.fillAmount=0;
            StartCoroutine(ShowPlayerChess(image));
            
        }
        else
        {
            GameObject chess=Resources.Load<GameObject>("Prefab/AIChess");  
            GameObject chessObj=Instantiate(chess);
            chessObj.transform.SetParent(chesses.transform,false);
            chessObj.GetComponent<RectTransform>().position=rectTrans.position;        
            chessObjs.Add(chessObj);
            
            GameObject aiChess2=chessObj.transform.Find("AIChess2").gameObject;  
            AudioManager.Instance.PlayAudio(0);    
            StartCoroutine(ShowAIChess(aiChess2));     
        }


    }

    IEnumerator ShowPlayerChess(Image image)
    {
        while(image.fillAmount<1)
        {
            image.fillAmount+=0.35f;
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    IEnumerator ShowAIChess(GameObject aiChess2)
    {
        yield return new WaitForSeconds(0.3f);
        aiChess2.SetActive(true);
        AudioManager.Instance.PlayAudio(-1);
    }


    //打开设置面板
    public void ShowSettingPanel()
    {
        settingPanel.SetActive(true);
        Time.timeScale=0;
    }
    //关闭设置面板
    public void HideSettingPanel()
    {
        settingPanel.SetActive(false);
        Time.timeScale=1;
    }

    //显示当前回合
    public void ChangeRoundTip(int currentP)
    {
        if(currentP==1)
            roundText.text="你的回合";
        else
            roundText.text="电脑回合";
    }

    //改变计分
    public void ChangeScoreTip(int winP)
    {
        if(winP==1)
        {
            playerScore++;
            playerScoreText.text="你的得分："+playerScore;
        }
        else if(winP==-1)
        {
            aiScore++;
            aiScoreText.text="对方得分："+aiScore;
        }
    }

    //显示结算面板
    public void ShowGameEnd(int winner)
    {
        roundText.gameObject.SetActive(false);
        gameEndPanel.SetActive(true);
        if(winner==1)
            reslut.text="你赢啦！";
        else if(winner==-1)
            reslut.text="你输了╯.╰";
        else 
            reslut.text="势均力敌！";
    }
    //隐藏结算面板
    public void HideGameEnd()
    {
        gameEndPanel.SetActive(false);
    }
    //“再来一局”时的UI设置
    public void GameAgine(int firsrP)
    {
        roundText.gameObject.SetActive(true);
        ChangeRoundTip(firsrP);
        for(int i=0;i<chessObjs.Count;i++)
        {
            if(chessObjs!=null)
                Destroy(chessObjs[i]);
        }
        chessObjs.Clear();
        HideGameEnd();
    }


}
