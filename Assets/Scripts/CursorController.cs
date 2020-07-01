using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/** 
 * Esta clase se encarga de :
 * - actualizar la informacion sobre pocision y velocidad actual del mouse. 
 * - actualizar la pocision del Cursor game object 
 * - recibir el input para agarrar y soltar la bola.
 * **/
public class CursorController : MonoBehaviour
{

    public Vector3 offset = new Vector3(0,0,20); //este offset hace que la pos del curor no quede con un z negativo y no se vea en la camara.
    public Vector3 escalaAlPresionar = Vector3.one;
    public Vector3 offsetObjeto;
    public Vector3 velocidadDelCursor;


    Vector3 ultimaPosicion; 
    Vector3 escalaRegular = Vector3.one; 
    Camera camara;
    GameObject ObjetoClickeado;
    OrlaFisicasInterface orlaFisicasInterface; 

    void Start()
    {
        //Cursor.visible = false;
        //guardamos la camera principal. 
        camara = Camera.main;
        //guardamos la escala inicial, como escala regular. 
        escalaRegular = transform.localScale;

        //iniciamos la ultima pocision con la posicion actual del mouse. 
        ultimaPosicion = camara.ScreenToWorldPoint(Input.mousePosition); 
    }

    void Update()
    {
        //actualizamos la pocision de este cursor.
        transform.position = camara.ScreenToWorldPoint(Input.mousePosition)+ offset;

        //si tenemos un objetoClickeado activo actualizamos su pocision tambien. 
        if (ObjetoClickeado != null)
        {
            var nuevaPosicion = camara.ScreenToWorldPoint(Input.mousePosition) + offsetObjeto;
            nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, OrlaFisicas.Instancia.Paredes.x+orlaFisicasInterface.MargenObjeto(), OrlaFisicas.Instancia.Paredes.y- +orlaFisicasInterface.MargenObjeto()); //el objeto no puede salirse de los limites 
            nuevaPosicion.y = Mathf.Clamp(nuevaPosicion.y, OrlaFisicas.Instancia.NivelDelPiso+orlaFisicasInterface.MargenObjeto(), 1000); 
            ObjetoClickeado.transform.position = nuevaPosicion;
        }
        //calculamos la velocidaddel mouse.
        velocidadDelCursor = (camara.ScreenToWorldPoint(Input.mousePosition) - ultimaPosicion); 

        //Manejamos los inputs. 
        if (Input.GetMouseButtonDown(0))
        {
            //si presionan el boton izq del mouse, actualizamos la escala y revisamos si tocamos un objeto.
            transform.localScale = escalaAlPresionar;
            RevisarSiTocamosAlgo(); 
        }
        else if(Input.GetMouseButtonUp(0))
        {
            //al soltar el boton, reiniciamos la escala del objeto, y volvemos nulo (soltamos) el objeto clickeado.
            transform.localScale = escalaRegular;
            ObjetoClickeado = null;
            orlaFisicasInterface.Soltar(velocidadDelCursor); 
        }

        //actualizamos la ultima pocision para calcular la velocidad en el proximo frame. 
        ultimaPosicion = camara.ScreenToWorldPoint(Input.mousePosition); 
    }

    private void RevisarSiTocamosAlgo()
    {
        GameObject hit = OrlaFisicas.Instancia.CastearPunto(camara.ScreenToWorldPoint(Input.mousePosition));
        if (hit != null)
        {
            ObjetoClickeado = hit;
            ObjetoClickeado.transform.localScale = Vector3.one; 
            orlaFisicasInterface = ObjetoClickeado.GetComponent<OrlaFisicasInterface>();
            if (orlaFisicasInterface != null) orlaFisicasInterface.Agarrar(); 
        }
    }
}
