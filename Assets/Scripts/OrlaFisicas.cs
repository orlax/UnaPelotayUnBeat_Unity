using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Esta Clase define las estructuras, variables y funciones necesarias para un motor de fisicas basico. 
 * Orla fisicas tiene soporte para dos tipos de objetos: 
 * - Circulos. 
 * - Lineas
 * 
 * Los circulos son afectados por la gravedad y se les pueden aplicar fuerzas externas. 
 * 
 * Las lineas son elementos estaticos que sirven como Muros, pueden colisionar con los circulos,
 * y ejercer una fuerza de rebote con en direccion de su vector normal.
 * * **/
public class OrlaFisicas : MonoBehaviour
{
    //evento que se dispara cuando hay un rebote. 
    public delegate void DelegadoRebote();
    public DelegadoRebote Rebote; 

    #region SINGLETON PATTERN
    static OrlaFisicas instancia; 
    public static OrlaFisicas Instancia
    {
        get
        {
            if (instancia == null)
            {
                instancia = FindObjectOfType<OrlaFisicas>();
                return instancia; 
            }
            return instancia; 
        }
    }
    #endregion


    public struct Circulo
    {
        public Vector2 posicion;
        public float radio;
        public float deformacionMinima;
        public float deformacion; 
    }

    public struct Linea
    {
        public Vector2 inicio;
        public Vector2 final;
        public bool colision;
        public Vector2 normal; 
    }

    public float Gravedad; //variable global de gravedad. 
    public float NivelDelPiso; //limite global horizontal, nada puede estar por debajo de este nivel. 
    public Vector2 Paredes; //limites laterales, nada puede estar a la izquierda de estos dos valores.

   

    [Range(0,1)]
    public float MultiplicadorDeReboteDePiso;
    public float VelocidadMinimaDeRebote;
    public float velocidadMinimaDeDeformacion;  

    public List<OrlaPelota> Pelotas; //una lista con todos los circulos existentes en el sistema. 
    public List<OrlaLinea> Lineas; //una lista con todas las lineas existentes en el sistema. 

    //Al iniciar, buscamos todos los objetos existentes y guardamos referencias hacia ellos. 
    private void Start()
    {
        Lineas = new List<OrlaLinea>();
        Pelotas = new List<OrlaPelota>(); 

        var todasLasLienas = FindObjectsOfType<OrlaLinea>(); 
        foreach(OrlaLinea linea in todasLasLienas)
        {
            Lineas.Add(linea); ; 
        }
        //circulos: 
        var todasLosCirculos = FindObjectsOfType<OrlaPelota>();
        foreach (OrlaPelota pelota in todasLosCirculos)
        {
            Pelotas.Add(pelota); ;
        }

    }

    private void Update()
    {
       // var lado = PuntoVsLinea(Pelotas[0].circulo.posicion, Lineas[0].linea); 
       // Debug.Log("esta del lado: " + lado);
    }

    internal void AplicarDeformacion(ref Circulo circulo, Vector3 velocidad_)
    {
        //revisamos si la velocidad que llevamos es suficiente para deformarnos.
        if (velocidad_.y * -1 > velocidadMinimaDeDeformacion)
        {
            //cuando llegamos al piso empezamos a deformar la pelota. 
            circulo.deformacion -= Time.deltaTime * 8;
            if (circulo.deformacion <= circulo.deformacionMinima)
            {
                circulo.deformacion = 1;
            }
        }
        //si no lo es no hay deformacion. 
        else
        {
            circulo.deformacion = 1;
        }
    }

    //recibe un circulo y devuelve la velocidad modificada de acuerdo al limite del piso. 
    internal Vector3 LimiteCirculoVsPiso(ref Circulo circulo, Vector3 velocidad_)
    {
        if ((circulo.posicion.y + velocidad_.y) - (circulo.radio * circulo.deformacion) < NivelDelPiso)
        {

            AplicarDeformacion(ref circulo, velocidad_); 

            //solo rebotamos si la deformacion actual es igual a 1 (no hay deformacion)
            if (circulo.deformacion == 1)
            {
                velocidad_.y = velocidad_.y * MultiplicadorDeReboteDePiso * -1;

                if (velocidad_.y < VelocidadMinimaDeRebote)
                {
                    velocidad_.y = 0; //si no superamos la velocidad minima, la velocidad es 0. 
                }
                else
                {
                    //si la velocidad es suficiente para mover la pelota, significa que hemos rebotado. 
                    Rebote();
                }
            }
          
        }
        return velocidad_; 
    }

    //recibe un circulo y devuelve la velocidad modificada de acuerdo a los limites laterales. 
    internal Vector3 LimiteCirculoVsLateral(Circulo circulo, Vector3 velocidad_)
    {
        var nuevaPosicion = (circulo.posicion.x + velocidad_.x);
        if (nuevaPosicion - circulo.radio < Paredes.x || nuevaPosicion + circulo.radio > Paredes.y)
        {
            velocidad_.x = velocidad_.x * MultiplicadorDeReboteDePiso * -1;
            //al rebotar de los muros, tambien rebotamos. 
            Rebote(); 
        }
        return velocidad_;
    }

    //recibe un circulo y devuelve la velocidad modificada de acuerdo a todas las lineas presentes, 
    //el rebote es en la direccion del vector normal de la linea con la que ocurre la colision.
    internal Vector3 CirculoVsLineas(ref Circulo circulo, Vector3 velocidad_)
    {
        //comprobamos los limites del circulo contra todas las lineas presentes. 
        foreach(OrlaLinea linea in Lineas)
        {
           var colision =  CirculoVsLinea(circulo, linea.linea);
            if (colision)
            {
                velocidad_ = velocidad_.magnitude * MultiplicadorDeReboteDePiso * linea.linea.normal;
                if (velocidad_.y < VelocidadMinimaDeRebote)
                {
                    velocidad_.y = 0; //si no superamos la velocidad minima, la velocidad es 0. 
                }
                else
                {
                    //si la velocidad es suficiente para mover la pelota, significa que hemos rebotado. 
                    Rebote();
                }

            }
        }
        return velocidad_;
    }

    public Vector2 puntoMasCercano;

    //recibe un circulo y una linea y devuelve true si estan colisionando
    //estas matematicas se ponen un poquitin complejas, 
    //referencia de donde saque estos calculos aca: http://www.jeffreythompson.org/collision-detection/line-circle.php
    internal bool CirculoVsLinea(Circulo circulo, Linea linea)
    {
        var colision = false;
        if (PuntoVsCirculo(circulo, linea.inicio)) colision = true;
        if (PuntoVsCirculo(circulo, linea.final)) colision = true;

        //primero obtenemos el punto mas cercano en la linea al circulo. 
        var largoDeLaLinea = Vector2.Distance(linea.inicio, linea.final); 
        var dot =  ( ((circulo.posicion.x-linea.inicio.x)*(linea.final.x-linea.inicio.x) ) + ( (circulo.posicion.y-linea.inicio.y)*(linea.final.y-linea.inicio.y)) )  / Math.Pow(largoDeLaLinea, 2);
        float puntoXmasCercano =linea.inicio.x + (float)(dot * (linea.final.x - linea.inicio.x)); 
        float puntoYmasCercano = linea.inicio.y + (float)(dot * (linea.final.y - linea.inicio.y));
        puntoMasCercano = new Vector2(puntoXmasCercano, puntoYmasCercano);

        //si el punto mas cercano en la linea esta dentro de la linea es posible que halla una colision.
        if (PuntoVsLinea(puntoMasCercano, linea) == 0)
        {
            colision = PuntoVsCirculo(circulo, puntoMasCercano);
            //si la colision se da, debemos verificar que se de solo de un lado de la linea. 
            if (colision)
            {
                var lado = PuntoVsLinea(circulo.posicion+new Vector2(0,circulo.radio), linea);
                //si el lado no es -1 el centro de la pelota esta del lado incorrecto, o atravesando la linea.
                //TODO esto se puede mejorar, un monton. Pero por ahora funciona lo suficientemente bien.
                if (lado != -1)
                {
                    colision = false;
                }
            }
        }
        linea.colision = colision; 
        return linea.colision;
    }

    //castea un punto vs todas las pelotas, y devuelve el game object de la primera pelota con la que tengamos colision.
    internal GameObject CastearPunto(Vector2 punto)
    {
       foreach(OrlaPelota pelota in Pelotas)
        {
            var hit = PuntoVsCirculo(pelota.circulo, punto);
            if (hit)
            {
                return pelota.gameObject; 
            }
        }
        return null;
    }

    //recibe un punto y un circulo y devuelve si estan colisionando. 
    internal bool PuntoVsCirculo(Circulo circulo, Vector3 punto)
    {
        return Vector3.Distance(circulo.posicion, punto)<circulo.radio*circulo.deformacion; 
    }

    //recibe un punto y una linea retorna un entero. 
    // si el punto colisiona con la linea, retorna 0. 
    // retorna 1 o -1 si el punto no colisiona con la linea, dependiendo del lado en el que se encuentra.
    internal int PuntoVsLinea(Vector2 punto, Linea linea) {
        var distanciaTotal = Vector2.Distance(linea.inicio, punto) + Vector2.Distance(punto, linea.final);
        var largoDeLaLinea = Vector2.Distance(linea.inicio, linea.final);
        if(distanciaTotal >= largoDeLaLinea-0.1f && distanciaTotal <= largoDeLaLinea + 0.1f) return 0;
        //si la linea no esta colisionando con el punto, determinamos de que lado se encuentra el punto 
        //estas matematicas tienen una razon de ser: https://math.stackexchange.com/questions/274712/calculate-on-which-side-of-a-straight-line-is-a-given-point-located
        //d = (x−x1)(y2−y1)−(y−y1)(x2−x1)
        var d = (punto.x - linea.inicio.x) * (linea.final.y - linea.inicio.y) - (punto.y - linea.inicio.y)*(linea.final.x - linea.inicio.x);
        return Math.Sign(d); 
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawLine(new Vector3(-1000, NivelDelPiso), new Vector3(1000, NivelDelPiso)); 
        Gizmos.DrawLine(new Vector3(Paredes.x, 1000), new Vector3(Paredes.x, -1000)); 
        Gizmos.DrawLine(new Vector3(Paredes.y, 1000), new Vector3(Paredes.y, -1000));

        Gizmos.DrawSphere(puntoMasCercano, 0.3f);
    }

}
