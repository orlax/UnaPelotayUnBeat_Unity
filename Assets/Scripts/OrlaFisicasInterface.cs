using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface OrlaFisicasInterface
{
    void Agarrar();
    void Soltar(Vector3 velocidad);

    float MargenObjeto(); 
}
