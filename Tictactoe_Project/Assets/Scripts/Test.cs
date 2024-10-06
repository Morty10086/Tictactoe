using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject chessBoard;
    //public GameObject chess1;
    public Button btn1;

    void Start()
    {
        //chess1.transform.position=chessBoard.transform.Find("Pos3").transform.position;
        btn1.onClick.AddListener(()=>{print("111");});
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
