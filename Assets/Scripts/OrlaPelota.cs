using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrlaPelota : MonoBehaviour, OrlaFisicasInterface
{
    public OrlaFisicas.Circulo circulo; // estructura basica de este objeto. 
    public float tamano = .5f;
    public float deformacion = 0.5f; 

    public Vector3 velocidad;
    private Vector3 ultimaVelocidad; 
 
    [Range(0,1)]
    public float peso; //Peso es una variable para disminuir la cantidad de velocidad recibida cuando corre la funcion de Soltar
    

    public float friccionTerrestre; //La Friccion determina que tan rapido se detiene la pelota al rodar por el piso. 
    [Range(0, 1)]
    public float friccionAerea; //La Friccion determina que tan rapido se detiene la pelota al rodar por el piso. 

    bool fisicasActivadas = true;
    public bool EnElPiso = true;
    public float margenes;

    void Start()
    {
        circulo = new OrlaFisicas.Circulo();
        circulo.posicion = transform.position;
        circulo.radio = tamano / 2; 
        circulo.deformacionMinima = deformacion;
        circulo.deformacion = 1; 

        velocidad = Vector3.zero; 
    }

    // Update is called once per frame
    void Update()
    {
        //actualizamos la estructura de Circulo para el calculo de las fisicas.
        circulo.posicion = transform.position;

        if (!fisicasActivadas) return;

        //guardamos la ultima velocidad conocida, antes de que la modifiquemos. 
        ultimaVelocidad = velocidad;

        //GRAVEDAD solo si no nos estamos deformando.
        velocidad += new Vector3(0, OrlaFisicas.Instancia.Gravedad * circulo.deformacion * Time.deltaTime, 0);

        //Limites
        velocidad = OrlaFisicas.Instancia.LimiteCirculoVsPiso(ref circulo, velocidad);
        velocidad = OrlaFisicas.Instancia.LimiteCirculoVsLateral(circulo, velocidad); 
        velocidad = OrlaFisicas.Instancia.CirculoVsLineas(ref circulo, velocidad); 

        Friccion();
        Deformacion(); 

        
        if(circulo.deformacion == 1)
        {
            transform.position += velocidad; //aplicamos la velocidad en cada frame si no nos estamos deformando
        }
       

        //estamos en el piso? si la velocidad en y es diferente de cero, asumimos que No estamos en el piso. 
        if (velocidad.y != 0) { EnElPiso = false; }
        else { EnElPiso = true; }
    }

    /*
     * Esta funcion cambia la rotacion y escala de la pelota basado en su velocidad actual.
     * la deformacion es diferente cuando esta en el piso/aire
     * Hay un caso especial cuando rebota/hay un cambio repentino en la velocidad.
     */
    private void Deformacion()
    {
        if (!EnElPiso)
        {
            //rotar de acuerdo a la velocidad 
            var angulo = Mathf.Atan2(velocidad.x, velocidad.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
            if (circulo.deformacion == 1)
            {
                //escalar en x de acuerdo a la velocidad en y
                var estiramiento = velocidad.magnitude * 2f;
                estiramiento = Mathf.Clamp(estiramiento, 0, 0.5f); 
                transform.localScale = new Vector3( 1- estiramiento, 1+ estiramiento, 1);
            }
            else
            {
                var estiramiento = 1-circulo.deformacion; 
                transform.localScale = new Vector3(1 + estiramiento, 1 - estiramiento, 1); 
            }
        }
        else
        {
            transform.localScale = Vector3.one;
            transform.Rotate(0, 0, 1024 * velocidad.x * -1);
        }
    }

    //aplicamos disminuciones a la velocidad dependiendo del estado de la pelota.
    private void Friccion()
    {
        //si estamos en el piso y nos estamos moviendo lateralmente, disminuimos la velocidad
        if (velocidad.x != 0 && EnElPiso)
        {
            velocidad.x -= friccionTerrestre*Time.deltaTime*Mathf.Sign(velocidad.x);        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(180,180,0,10);
        Gizmos.DrawSphere(transform.position, tamano); 
    }

    /** Cuando un circulo es agarrado, sus fisicas se desactivan. */
    public void Agarrar()
    {
        velocidad = Vector3.zero;
        circulo.deformacion = 1; 
        fisicasActivadas = false; 
    }

    //cuando un circulo lo sueltan, puede recibir la velocidad a la que fue soltado y se activan sus fisicas. 
    public void Soltar(Vector3 velocidad_)
    {
        fisicasActivadas = true;
        velocidad = velocidad_*(1-peso); 
    }

    public float MargenObjeto()
    {
        return margenes; 
    }
}
