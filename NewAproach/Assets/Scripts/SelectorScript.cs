using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SelectorScript : MonoBehaviour
{
    
    public Mainscript main;
    public GameObject caller;
    public GameObject canv;
    public CanvasLaser canvScript;
    public int allegiance;
    public int movattcan;
    public GameObject myEnemy;
    
    void Start()
    {
        outOfBounds();
        switch(main.phase)
        {
            case 1: collGeneral(); break;
            case 2:
                switch(movattcan)
                {
                    case 0: collGeneral(); break;
                    case 1: collGeneral(); break;
                    case 2: collAtk(); break;
                    case 3: collFull(); break;
                    case 4: collKiller(); break;
                }
            break;
        }
    }
    public void Action()
    {
        switch(main.phase)
        {
            case 1:
                FormAdd();
            break;
            case 2:
                switch(movattcan)
                {
                    case 0: ConMov(true); break;
                    case 1: ConMov(false); caller.GetComponent<Mymove>().ActionChacker(true); break;
                    case 2: Attack(); caller.GetComponent<Mymove>().ActionChacker(false); break;
                    case 4: Exile(); break;
                }
            break;
        }
    }
    void OnDestroy()
    {
        main.SelectCheck();
    }
    void outOfBounds()
    {
        float xpos=gameObject.transform.localPosition.x;
        float ypos=gameObject.transform.localPosition.y;
       if (xpos>376 || xpos<-376 || ypos>300 || ypos<-300)
       {
           Destroy(gameObject);
       }
    }

    void collGeneral()
    {
        float xpos=transform.position.x;
        float ypos=transform.position.y;
        int isColl=canvScript.collUI(xpos,ypos,gameObject);
        if (isColl!=0)
        {
            Destroy(gameObject);
        }
    }
    void collAtk()
    {
        float xpos=transform.position.x;
        float ypos=transform.position.y;
        int isColl=canvScript.collUI(xpos,ypos,gameObject);
        if (isColl==2)
        {
            gameObject.GetComponent<Image>().color=new Color(1f,0f,0f,0.5f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void collFull()
    {
        float xpos=transform.position.x;
        float ypos=transform.position.y;
        int isColl=canvScript.collUI(xpos,ypos,gameObject);
        switch (isColl)
        {
            case 0: movattcan=1; break;
            case 1: Destroy(gameObject); break;
            case 2: gameObject.GetComponent<Image>().color=new Color(1f,0f,0f,0.5f); movattcan=2; break;
        }
    }
    public bool collNext()
    {
        bool p;
        float xpos=transform.position.x;
        float ypos=transform.position.y;
        int isColl=canvScript.collUI(xpos,ypos,gameObject);
        if (isColl!=0) {p=true;} else {p=false;}
        return p;
    }
    void collKiller()
    {
        float xpos=transform.position.x;
        float ypos=transform.position.y;
        int isColl=canvScript.collUI(xpos,ypos,gameObject);
        gameObject.GetComponent<Image>().color=new Color(1f,0f,0f,0.5f);
    }

    public void move2me()
    {
        float xpos=gameObject.transform.localPosition.x;
        float ypos=gameObject.transform.localPosition.y;
        caller.transform.localPosition=new Vector3(xpos,ypos,0);
    }

    void FormAdd()
    {
        //Mueve la pieza a la posición del selector activado
        move2me();
        //Llama a la función de agregar pieza de MainScript
        main.PieceAdd(allegiance-1);
        //Obtenemos el factor del oponente
        float next = Mathf.Pow(allegiance-2,2);
        //Pregunto si mi oponente todavía tiene posibilidad de jugar
        if (main.rtp[(int)next]==0)
        {
            //caso afirmativo: cambio el turno
            main.Turner();
        }
        else
        {
            //Pregunto si ya estoy listo y si es el turno 2
            if (main.rtp[allegiance-1]>0 && main.turn==2)
            {
                //en caso afirmativo: cambio al turno 1
                main.Turner();
            }
        }
        main.DestroySelectors();
    }

    void ConMov(bool add)
    {
        move2me();
        if (add) { main.PieceAdd(allegiance-1); main.Acter();} else { CheckWinnOnMove(); }
        main.DestroySelectors();
    }
    void CheckWinnOnMove()
    {
        if (caller.GetComponent<Mymove>().dtype==5)
        {
            float xpos=gameObject.transform.localPosition.x;
            float ypos=gameObject.transform.localPosition.y;
            switch(caller.GetComponent<Mymove>().tag)
            {
                case "ally":
                    if (xpos==main.enemyKing.GetComponent<Mymove>().xorigin && ypos==main.enemyKing.GetComponent<Mymove>().yorigin)
                    {
                        main.winner=1;
                        main.EndPhase();
                    }
                    else
                    {
                        main.Acter();    
                    }
                break;
                case "enemy":
                    if (xpos==main.allyKing.GetComponent<Mymove>().xorigin && ypos==main.allyKing.GetComponent<Mymove>().yorigin)
                    {
                        main.winner=2;
                        main.EndPhase();
                    }
                    else
                    {
                        main.Acter();
                    }
                break;
            }
        }
        else
        {
            main.Acter();
        }
    }
    void Attack()
    {
        int pow = caller.GetComponent<Mymove>().power;
        int def = myEnemy.GetComponent<Mymove>().power;
        if (pow<def)
        {
            myEnemy.GetComponent<Mymove>().power-=pow;
            myEnemy.GetComponent<Mymove>().Displayer();
            main.Acter();
        }
        else
        {
            if (myEnemy.GetComponent<Mymove>().dtype==5)
            {
                myEnemy.GetComponent<Mymove>().power=0;
                myEnemy.GetComponent<Mymove>().Displayer();
                if (myEnemy.tag=="ally"){main.winner=2;}else{main.winner=1;}
                main.EndPhase();
            }
            else
            {
                myEnemy.GetComponent<Mymove>().Dead();
            }
        }
        main.DestroySelectors();
    }
    void Exile()
    {
        myEnemy.GetComponent<Mymove>().Dead();
        main.DestroySelectors();
    }
}
