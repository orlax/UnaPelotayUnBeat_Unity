using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrlaSound : MonoBehaviour
{
    public AudioClip[] SonidosRebote;
    public AudioSource AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        OrlaFisicas.Instancia.Rebote += Rebote; 
    }

    private void Rebote()
    {
        AudioSource.clip = SonidosRebote[UnityEngine.Random.Range(0, SonidosRebote.Length)];
        AudioSource.Play(); //TODO agregar algo aqui para que el volumen del sonido dependa de la velocidad de la pelota. 
    }

    private void OnDestroy()
    {
        OrlaFisicas.Instancia.Rebote -= Rebote;
    }

}
