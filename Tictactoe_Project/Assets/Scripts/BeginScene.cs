using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginScene : MonoBehaviour
{
    // Start is called before the first frame update
    public Button startBtn;
    public Button quitBtn;
    public Button easyBtn;
    public Button midBtn;
    public Button hardBtn;
    public GameObject diffDegreePanel;
    void Start()
    {
        startBtn.onClick.AddListener(()=>{
            diffDegreePanel.SetActive(true);
        });
        quitBtn.onClick.AddListener(()=>{
           Application.Quit();
        });
        easyBtn.onClick.AddListener(()=>{
            DiffDegreeData.Instance.diffDegree=1;
            SceneManager.LoadScene("GameScene");
        });
        midBtn.onClick.AddListener(()=>{
            DiffDegreeData.Instance.diffDegree=2;
            SceneManager.LoadScene("GameScene");
        });
        hardBtn.onClick.AddListener(()=>{
            DiffDegreeData.Instance.diffDegree=3;
            SceneManager.LoadScene("GameScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
