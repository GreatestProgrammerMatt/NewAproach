using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PBscript : MonoBehaviour
{
    public int type;
    public Mainscript main;
    
    public void PhaseCheck()
    {
        switch (main.phase)
        {
            case 1:
                main.rtp[type]=1;
                main.PhaseChanger();
                if (main.turn==2)
                {
                    main.Turner();
                }
                else
                {
                    if (main.rtp[1]==0)
                    {
                        main.Turner();
                    }
                }
                gameObject.SetActive(false);
            break;
            case 2:
                if (main.selectCan==0)
                {
                    main.selectCan=1;
                    KillerSelectores(type);
                }
            break;
            case 4:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            break;
        }
    }
    public void ExileButton(int t)
    {
        string s="";
        switch(t)
        {
            case 0: s="Desterrar"; break;
            case 1: s="Revancha"; break;
        }
        gameObject.GetComponentInChildren<Text>().text=s;
    }
    void KillerSelectores(int t)
    {
        string s;
        if (t==0){s="ally";}else{s="enemy";}
        GameObject[] piezas;
        piezas=GameObject.FindGameObjectsWithTag(s);
        for(var i = 0 ; i < piezas.Length ; i ++){piezas[i].GetComponent<Mymove>().KillerSelectores(t);}
    }
}
