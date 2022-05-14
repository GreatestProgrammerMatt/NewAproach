using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Mainscript : MonoBehaviour
{
  //Sprites de las piezas
  public Sprite hex;
  public Sprite tri;
  public Sprite sqr;
  public Sprite rom;
  public Sprite cir;
  public Sprite pen;
  Sprite[] img;
  //Define el turno actual
  public int turn=0;
  //define la fase de juego actual
  public int phase=0;
  //pieces on board, cuenta cuantas piezas de un mismo tipo hay en el tablero
  public int[] pob;
  //ready to play, revisa cuando un jugador está listo para empezar
  public int[] rtp;
  //acciones restantes, cuantes acciones puede realizar el jugador
  public int ax;
  //Permite hacer clic al usuario o no
  public int selectCan=0;
  //identifica qué jugador ganó el juego
  public int winner;
  //referencias a otros elementos del juego
  public GameObject canvo;
  public GameObject pieza;
  public GameObject selector;
  //Texto informativo de la interfase
  public GameObject feedback;
  //Botón de los usuarios
  public GameObject phasebutton;
  //Array de botones de los usuarios
  GameObject[] phbtt=new GameObject[2];
  //Define al rey aliado y al enemigo
  public GameObject allyKing;
  public GameObject enemyKing;
  //Define la pieza seleccionada actual
  public GameObject selectedPiece;
  //Guarda la referencia de la catapulta cuando están en fase de explosion
  public GameObject busted;
  public int bustedTag;
  
  
  
  void Start()
  {
    //carga el array img con las sprites de las piezas
    img=new Sprite[] {tri,sqr,rom,cir,pen,hex};
    //Establece los valores iniciales de las variables de control de turno (ax,turn,phase,pob,rtp)
    CountSetters();
    //Crea las fichas aliadas, luego el rey aliado, luego las enemigas
    Creator("ally",-375f,-750,5);
    Creator("ally",0,-300,1);
    Creator("enemy",375f,750,5);
    Creator("enemy",0,300,1);
    //genera y actualiza el texto y el botón de los usuarios
    Feeder();
  }

  void Update()
  {
    //Actualiza el botón de los usuarios
    ReadyButtonCheck();
    //comprueba si algún jugador se quedó sin piezas suyas en el tablero
    CheckWinnOnEmptyBoard();
  }

  void CountSetters()
  {
    //acciones restates
    ax=2;
    //turno del jugador actual
    turn=1;
    //fase del juego
    phase=1;
    //pieceses on board, crea los array que van a contar las piezas en el tablero
    pob=new int[2];
    //ready to play, crea los array que definen si cada jugador está listo para jugar
    rtp=new int[2];
  }
  //Crea las piezas en el juego, requiere un string que defina si son piezas aliadas o enemigas, un origen en x & y, y una cantidad de piezas a crear
  void Creator(string s,float xx,int yy,int a)
  {
    //el fator positivo o negativo para el eje x (aliados: de izq a der/enemigos:viceversa)
    int xfac;
    //definido en base a si es aliado o enemigo
    if (s=="ally"){xfac=1;}else{xfac=-1;}
    //un bucle para crear las piezas que se repite tantas veces igual a "a"
    for (int i=0;i<a;i++)
    {
      //instanciamos un elemento y lo colocamos en la variable local p1
      GameObject p1=Instantiate(pieza,canvo.transform.localPosition,canvo.transform.rotation,canvo.transform);
      //establece la posición de la pieza
      p1.transform.localPosition=new Vector3(xx+((i*187.5f)*xfac),yy,0);
      //si es enemigo lo rota 180°
      if (s=="enemy") {p1.transform.Rotate(new Vector3(0,0,180));}
      //Establece el tag de la instancia como "ally" o "enemy"
      p1.tag=s;
      //Pasa referencias al script de la instancia
      p1.GetComponent<Mymove>().canvo=canvo;
      p1.GetComponent<Mymove>().mains=gameObject.GetComponent<Mainscript>();
      //Si a es mayor a 1 es una pieza normal, sino es el rey
      if (a>1)
      {
        //Las piezas tienen el tipo y sprite igual a su posición en el bucle
        p1.GetComponent<Mymove>().dtype=i;
        p1.GetComponent<Image>().sprite=img[i];
      }
      else
      {
        //los reyes tienen tipo 5 y sprite 5
        p1.GetComponent<Mymove>().dtype=5;
        p1.GetComponent<Image>().sprite=img[5];
        //Guarda la referencia de la instancia en la variable para el rey aliado y enemigo
        if (s=="ally"){allyKing=p1;}else{enemyKing=p1;}
      }
    }
  }
  //Crea los textos informativos y los botones de lso usuarios
  void Feeder()
  {
    //referencia a la posición x & y del canvas
    float cxx=canvo.transform.localPosition.x;
    float cyy=canvo.transform.localPosition.y;
    //un bucle que repetirá el código por cada jugador
    for (int i=0;i<2;i++)
    {
      //Se instancia el texto y el botón
      GameObject f1 = Instantiate(feedback,canvo.transform.localPosition,canvo.transform.rotation,canvo.transform);
      phbtt[i] = Instantiate(phasebutton,canvo.transform.localPosition,canvo.transform.rotation,canvo.transform);
      //Dependiendo de la posición del bucle los creará los del aliado o el enemigo
      switch(i)
      {
        //establece la posición del texto y el botón
        case 0:
          f1.transform.localPosition=new Vector3(0,-525,0);
          phbtt[i].transform.localPosition=new Vector3(0,-600,0);
        break;
        //establece la posición del texto y el botón y los rota 180°
        case 1:
          f1.transform.localPosition=new Vector3(0,525,0);
          f1.transform.Rotate(new Vector3(0,0,180));
          phbtt[i].transform.localPosition=new Vector3(0,600,0);
          phbtt[i].transform.Rotate(new Vector3(0,0,180));
        break;
      }
      //Pasa las referencias y define el tipo en el script de la nueva instancia
      f1.GetComponent<PlayerFeedback>().mains=gameObject.GetComponent<Mainscript>();
      f1.GetComponent<PlayerFeedback>().type=i;
      phbtt[i].GetComponent<PBscript>().main=gameObject.GetComponent<Mainscript>();
      phbtt[i].GetComponent<PBscript>().type=i;
    }
  }
  //Destruye todas las instancias de los selectores y permite hacer clic
  public void DestroySelectors()
  {
    GameObject[] selectores;
    selectores=GameObject.FindGameObjectsWithTag("slctr");
    for(var i = 0 ; i < selectores.Length ; i ++)
    {
        Destroy(selectores[i]);
    }
    if (selectCan!=0){selectCan=0;}
  }
  
  //Controla si existen setectores, en caso negativo permite hacer clic
  public void SelectCheck()
  {
    GameObject[] selectores;
    selectores=GameObject.FindGameObjectsWithTag("slctr");
    if (selectores.Length==0){selectCan=0;}
  }
  //Aumenta la cuenta de piezas en el tablero cuando se coloca una nueva pieza
  public void PieceAdd(int t)
  {
    switch (phase)
    {
      case 1:
        pob[t]+=1;
        if (pob[t]==4 && rtp[t]==0)
        {
          rtp[t]=1;
          PhaseChanger();
        }
      break;
      default:
        pob[t]+=1;
      break;
    }
  }
  //Si ambos jugadores están listos para jugar cambia a la siguiente fase
  public void PhaseChanger()
  {
    if (rtp[0]>0 && rtp[1]>0)
    {
      phase=2;
    }
  }
  //Controla cuantas acciones restan, y en caso de no quedar acciones cambia el turno
  public void Acter()
  {
    if (ax>1) {ax--;} else {Turner();}
  }
  //Cambia el turno, reseteando los parámetros de las fichas
  public void Turner()
  {
      PiecesActionSetter("ally");
      PiecesActionSetter("enemy");
      if (turn==1){turn=2;}else{turn=1;}
      ax=2;
  }
  //Busca a todas las fichas con tag "s" y llama a su función "ActionSetter" en su script "Mymove"
  void PiecesActionSetter(string s)
  {
    GameObject[] piezas;
    piezas=GameObject.FindGameObjectsWithTag(s);
    for(var i = 0 ; i < piezas.Length ; i ++){piezas[i].GetComponent<Mymove>().ActionSetter();}
  }
  //Cambia la fase al modo final
  public void EndPhase()
  {
    phase=4;
  }
  //Controla si no hay más piezas de un jugador en el tablero y llama a la fase final
  void CheckWinnOnEmptyBoard()
  {
    for (int i=0;i<2;i++)
    {
      if (phase==2)
      {
        //si el jugador i no tiene más piezas, declara al otro como ganador
        if (pob[i]==0)
        {
          //i-1 elevado a 2 : i=0 --> (0-1)*2= 1 : i=1 --> (1-1)*2 = 0
          float next = Mathf.Pow(i-1,2);
          winner=(int)next+1;
          EndPhase();
        }
      }
    }
  }
  //Activa o desactiva el botón de los usuarios dependiendo de las condiciones
  void ReadyButtonCheck()
  {
    for (int i=0;i<2;i++)
    {
      switch (phase)
      {
        case 1:
          if (turn==i+1 && rtp[i]<1 && pob[i]>0)
          {phbtt[i].SetActive(true);}
          else
          {phbtt[i].SetActive(false);}
        break;
        case 2:
          if (turn==i+1 && selectCan==0 && pob[i]>1)
          {
              phbtt[i].SetActive(true);
              phbtt[i].GetComponent<PBscript>().ExileButton(0);
          }
          else
          {
            phbtt[i].SetActive(false);
          }
        break;
        case 4:
          phbtt[winner-1].SetActive(true);
          phbtt[winner-1].GetComponent<PBscript>().ExileButton(1);
        break;
      }
    }
  }
}
