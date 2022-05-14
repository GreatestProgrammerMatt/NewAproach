using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasLaser : MonoBehaviour
{
    public Camera cam;
    RectTransform rect;
    GraphicRaycaster ray;
    Vector2 point;
    EventSystem eve;
    PointerEventData pointer;

    void Start()
    {
        rect=GetComponent<RectTransform>();
        ray=GetComponent<GraphicRaycaster>();
        eve=GetComponent<EventSystem>();
    }

    public int collUI(float xx, float yy,GameObject call)
    {
        int answer=0;
        point = RectTransformUtility.WorldToScreenPoint(cam,new Vector3(xx,yy,0));
        pointer=new PointerEventData(eve);
        pointer.position=point;
        List<RaycastResult> results = new List<RaycastResult>();
        ray.Raycast(pointer,results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.name=="pieza(Clone)")
            {
                int a=call.GetComponent<SelectorScript>().allegiance;
                switch (result.gameObject.tag)
                {
                    case "ally":
                    if (a==1){answer=1;}else{answer=2;}
                    break;
                    case "enemy":
                    if (a==2){answer=1;}else{answer=2;}
                    break;
                }
                call.GetComponent<SelectorScript>().myEnemy=result.gameObject;
            }
        }
        return answer;
    }
}
