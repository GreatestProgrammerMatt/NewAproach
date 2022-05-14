using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mymove : MonoBehaviour
{
  //Tipo de pieza
  public int dtype=0;
  //Puntos de poder de la pieza
  public int power;
  //Puntos máximos posibles
  int powf;
  //Controla si puede realizar movimiento
  public bool canmove;
  //Controla si puede realizar ataque
  public bool canatt;
  //Controla que combinación de movimiento/ataque puede relalizar
  int myact;
  //Coordenadas de origen
  public float xorigin;
  public float yorigin;
  //Referencias a otros Scripts y elementos
  public Mainscript mains;
  public GameObject canvo;
  public GameObject slc;

    void Start()
    {
        //Define los puntos máximos y establece los puntos actuales de manera aleatoria
        if (dtype==3) {powf=10;}else{powf=5+(2*dtype);}
        RandMe(1,powf);
        //cambia el color
        ColorMe();
        //Define las coordenadas de origen
        xorigin=transform.localPosition.x;
        yorigin=transform.localPosition.y;
    }
    //Define aleatoriamente los nuevos puntos y los muestra
    public void RandMe(int min,int max)
    {
        if (dtype<5)
        {
            int newpow = Random.Range(min,max);
            power=newpow;
        }
        else
        {
            power=20;
        }
        Displayer();
    }
    //Muestra los puntos actuales
    public void Displayer()
    {
        int pmax=4+(2*dtype);
        gameObject.GetComponentInChildren<Text>().text=power.ToString();
    }
    void ColorMe()
    {
        if (gameObject.tag=="ally")
        {
            gameObject.GetComponent<Image>().color=new Color32(100,165,255,255);
        }
        else
        {
            gameObject.GetComponent<Image>().color=new Color32(255,165,100,255);
        }
    }
    //Define las acciones en base a la dase, el turno, las acciones restantes, y el tipo de pieza
    public void Action()
    {
        int myturn;
        if (gameObject.tag=="ally"){myturn=1;}else{myturn=2;};
        float xpos=gameObject.transform.localPosition.x;
        float ypos=gameObject.transform.localPosition.y;
        if(mains.turn==myturn)
        {
            if (mains.selectCan==0)
            {
                mains.selectCan=1;
                mains.selectedPiece=gameObject;
                switch(mains.phase)
                {
                    case 1:
                        if (xpos>376 || xpos<-376 || ypos>300 || ypos<-300)
                        {
                                FormSeletors(myturn);
                        }
                    break;

                    case 2:
                        if (xpos>376 || xpos<-376 || ypos>300 || ypos<-300)
                        {
                            if (mains.pob[myturn-1]<4)
                            {
                                ConvSelectors(myturn);
                            }
                        }
                        else
                        {
                            switch(dtype)
                            {
                                case 0:
                                    ActCheck();
                                    PawnSelectors(myturn,myact);
                                break;
                                case 1:
                                    if (canatt) {NormalSelectors(myturn,2);}
                                    if (canmove) {KnightSelectors(myturn,1);}
                                break;
                                case 3:
                                    CatapultSelectors(myturn);
                                break;
                                default:
                                    ActCheck();
                                    NormalSelectors(myturn,myact);
                                break;
                            }
                        }
                    break;
                }
            }
            else
            {
                mains.DestroySelectors();
                mains.selectCan=0;
            }
        }
        if (mains.phase==3)
        {
            Phase3(myturn,xpos,ypos);
        }
    }
    void Phase3(int m,float xx,float yy)
    {
        if (m==(mains.bustedTag+1) && dtype!=3 && dtype!=5 && xx<376 && xx>-376 && yy<301 && yy>-301)
        {
            Dead();
            mains.busted.GetComponent<Mymove>().Dead();
        }
    }
    public void ActionSetter()
    {
        if (dtype==4) {canmove=false;} else {canmove=true;}
        canatt=true;
    }
    public void ActionChacker(bool moved)
    {
        //La Torre(4) y el Rey(5) nunca cambian su configuración
        if (dtype<4)
        {
            if (moved){canmove=false;}else{canatt=false;}
        }
    }
    void FormSeletors(int t)
    {
        int yfac;
        if(t==1){yfac=-1;}else{yfac=1;}
        float xx=0f;
        float yy=0f;
        for (int i = 6; i > 0; i--)
        {
            switch (i)
            {
                case 1:xx = -300f;  yy = (yfac)*300f;break;
                case 2:xx = -150f;  yy = (yfac)*300f;break;
                case 3:xx = -75f;   yy = (yfac)*150f;break;
                case 4:xx = 75f;    yy = (yfac)*150f;break;
                case 5:xx = 150f;   yy = (yfac)*300f;break;
                case 6:xx = 300f;   yy = (yfac)*300f;break;
            }
            Creator(t,xx,yy,canvo,0);
        }
    }

    void ConvSelectors(int t)
    {
        GameObject myKing;
        if (t==1){myKing=mains.allyKing;}else{myKing=mains.enemyKing;}
        for (int i = 6; i > 0; i--)
        {
            float xx = 0f;
            float yy = 0f;
            switch (i)
            {
                case 1:xx = -75f;   yy = -150f;break;
                case 2:xx = -150f;  yy = 0f;break;
                case 3:xx = -75f;   yy = 150f;break;
                case 4:xx = 75f;    yy = -150f;break;
                case 5:xx = 150f;   yy = 0f;break;
                case 6:xx = 75f;    yy = 150f;break;
            }
            float gxx = myKing.transform.localPosition.x+xx;
            float gyy = myKing.transform.localPosition.y+yy;
            Creator(t,gxx,gyy,myKing,0);
        }
    }

    void Creator(int t, float xx,float yy,GameObject r,int mac)
    {
        GameObject s =Instantiate(slc,r.transform.localPosition,r.transform.rotation,canvo.transform);
        s.transform.localPosition= new Vector3(xx,yy,0);
        s.GetComponent<SelectorScript>().main=mains;
        s.GetComponent<SelectorScript>().caller=gameObject;
        s.GetComponent<SelectorScript>().canv=canvo;
        s.GetComponent<SelectorScript>().canvScript=canvo.GetComponent<CanvasLaser>();
        s.GetComponent<SelectorScript>().movattcan=mac;
        s.GetComponent<SelectorScript>().allegiance=t;
    }

    GameObject Creator2(int t, float xx,float yy,GameObject r,int mac)
    {
        GameObject s =Instantiate(slc,r.transform.localPosition,r.transform.rotation,canvo.transform);
        s.transform.localPosition= new Vector3(xx,yy,0);
        s.GetComponent<SelectorScript>().main=mains;
        s.GetComponent<SelectorScript>().caller=gameObject;
        s.GetComponent<SelectorScript>().canv=canvo;
        s.GetComponent<SelectorScript>().canvScript=canvo.GetComponent<CanvasLaser>();
        s.GetComponent<SelectorScript>().movattcan=mac;
        s.GetComponent<SelectorScript>().allegiance=t;
        return s;
    }

    void ActCheck()
    {
        myact=0;
        if (canmove){myact+=1;}
        if(canatt){myact+=2;}
    }

    void NormalSelectors(int t,int m)
    {
        for (int i = 6; i > 0; i--)
        {
            float xx = 0f;
            float yy = 0f;
            switch (i)
            {
                case 1:xx = -75f;   yy = -150f;break;
                case 2:xx = -150f;  yy = 0f;break;
                case 3:xx = -75f;   yy = 150f;break;
                case 4:xx = 75f;    yy = -150f;break;
                case 5:xx = 150f;   yy = 0f;break;
                case 6:xx = 75f;    yy = 150f;break;
            }
            float gxx = transform.localPosition.x+xx;
            float gyy = transform.localPosition.y+yy;
            Creator(t,gxx,gyy,canvo,m);
        }
    }
    void KnightSelectors(int t,int m)
    {
        for (int i = 6; i > 0; i--)
        {
            float xx = 0f;
            float yy = 0f;
            switch (i)
            {
                case 1:xx = -225;   yy = -150f;break;
                case 2:xx = -225;   yy = 150f;break;
                case 3:xx = 0f;     yy = -300f;break;
                case 4:xx = 0f;     yy = 300f;break;
                case 5:xx = 225;    yy = -150f;break;
                case 6:xx = 225;    yy = 150f;break;
            }
            float gxx = transform.localPosition.x+xx;
            float gyy = transform.localPosition.y+yy;
            Creator(t,gxx,gyy,canvo,m);
        }
    }
    void PawnSelectors(int t,int m)
    {
        for (int i = 4; i > 0; i--)
        {
            bool tope=false;
            Branch(i,t,m,tope);
        }
        if (canatt)
        {
            for (int i=0;i<2;i++)
            {
                float xfactor;
                if (i==0){xfactor=1;}else{xfactor=-1;}
                float gxx = transform.localPosition.x+(150*xfactor);
                float gyy = transform.localPosition.y;
                Creator(t,gxx,gyy,canvo,2);

            }
        }
    }
    void Branch(int i,int t,int m,bool p)
    {
        for (int ñ = 0; ñ < 4 ; ñ++)
        {
            float xfactor;
            float yfactor;
            if ((i%2)==0){xfactor=1;}else{xfactor=-1;}
            if (i>2) {yfactor=1;}else{yfactor=-1;}
            float xx = ((75*(ñ+1))*xfactor);
            float yy = ((150*(ñ+1))*yfactor);
            float gxx = transform.localPosition.x+xx;
            float gyy = transform.localPosition.y+yy;
            if (ñ==0)
            {
                GameObject s = Creator2(t,gxx,gyy,canvo,m);
                bool tope = s.GetComponent<SelectorScript>().collNext();
                if (tope) {return;}
            }
            else
            {
                if (canmove)
                {
                    GameObject s = Creator2(t,gxx,gyy,canvo,1);
                    bool tope = s.GetComponent<SelectorScript>().collNext();
                    if (tope) {return;}
                }
            }
        }
    }
    void CatapultSelectors(int t)
    {
        if (canmove)
        {
            for (int i=2 ; i>0 ; i--)
            {
                float xfactor;
                if (i>1) {xfactor=1;}else{xfactor=-1;}
                Arm(t,xfactor);
            }
        }
        if (canatt)
        {
            for (int i = 6; i > 0; i--)
            {
                float xx = 0f;
                float yy = 0f;
                switch (i)
                {
                    case 1:xx = -150f;  yy = -300f;break;
                    case 2:xx = 0f;     yy = -300f;break;
                    case 3:xx = 150f;   yy = -300f;break;
                    case 4:xx = -150f;  yy = 300f;break;
                    case 5:xx = 0f;     yy = 300f;break;
                    case 6:xx = 150f;   yy = 300f;break;
                }
                float gxx = transform.localPosition.x+xx;
                float gyy = transform.localPosition.y+yy;
                Creator(t,gxx,gyy,canvo,2);
            }
        }
    }
    void Arm(int t,float xf)
    {
        for (int i=0 ; i<5 ; i++)
        {
            float xx = (150*(i+1))*xf;
            float gxx = transform.localPosition.x+xx;
            float gyy = transform.localPosition.y;
            GameObject s = Creator2(t,gxx,gyy,canvo,1);
            bool tope = s.GetComponent<SelectorScript>().collNext();
            if (tope) {return;}
        }
    }
    public void Dead()
    {
        int myturn;
        float yf;
        if (gameObject.tag=="ally"){myturn=0; yf=150f;}else{myturn=1; yf=-150f;};
        mains.pob[myturn]-=1;
        if (dtype==3)
        {
            if (mains.pob[myturn]>0)
            {
                RandMe(0,10);
                if (power==0)
                {
                    transform.localPosition=new Vector3(xorigin,yorigin+yf,0);
                    mains.busted=gameObject;
                    mains.bustedTag=myturn;
                    mains.phase=3;
                }
                else
                {
                    transform.localPosition=new Vector3(xorigin,yorigin,0);
                    if (mains.phase!=2){mains.phase=2;}
                    mains.Acter();
                }
            }
            else
            {
                transform.localPosition=new Vector3(xorigin,yorigin,0);
                if (mains.phase!=2){mains.phase=2;}
                mains.Acter();
            }
        }
        else
        {
            RandMe(1,powf);
            transform.localPosition=new Vector3(xorigin,yorigin,0);
            if (mains.phase==2){mains.Acter();}
        }
    }

    public void KillerSelectores(int t)
    {
        float gxx = transform.localPosition.x;
        float gyy = transform.localPosition.y;
        if (dtype<5 && gxx<376 && gxx>-376 && gyy<301 && gyy>-301)
        {
            Creator(t+1,gxx,gyy,canvo,4);
        }
    }
}
