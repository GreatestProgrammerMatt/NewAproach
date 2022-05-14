using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFeedback : MonoBehaviour
{
    public int type;
    public Mainscript mains;
    string report;

    void Update()
    {
        Reporter(type);
        GetComponent<Text>().text=report;
    }

    void Reporter(int t)
    {
        if (mains.turn==t+1)
        {
            switch(mains.phase)
            {
                case 1:
                    if(mains.pob[t]==0)
                    {
                        report="Coloca una pieza en el tablero";
                    }
                    else
                    {
                        report="Coloca o finaliza la formación";
                    }
                break;
                case 2:
                    report="Te quedan "+mains.ax.ToString()+" acciones";
                break;
                case 3:
                    Phase3();
                break;
            }
        }
        else
        {
            switch (mains.phase)
            {
                default:
                    report="Turno del rival";
                break;
                case 3:
                    Phase3();
                break;
            }
        }
        if (mains.phase==4)
        {
            if (mains.winner==t+1)
            {
                report="¡Ganaste!";
            }
            else
            {
                report="Perdiste";
            }
        }
    }

    void Phase3()
    {
        if (mains.bustedTag==type)
        {
            report="Remueve 1 pieza del tablero";
        }
        else
        {
            report="La catapulta de tu rival ha explotado";
        }
    }
}
