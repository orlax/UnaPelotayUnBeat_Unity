using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggablePoint : PropertyAttribute { }

[ExecuteAlways]
public class OrlaLinea : MonoBehaviour
{

    [DraggablePoint]  public Vector3 inicio;
    [DraggablePoint] public Vector3 fin; 

    public OrlaFisicas.Linea linea;
    public Vector2 normal;
   
    // Start is called before the first frame update
    void Start()
    {
        linea = new OrlaFisicas.Linea(); 
        linea.inicio = inicio;
        linea.final = fin;

        var diff = fin - inicio;
        normal = new Vector2(-diff.y, diff.x);
        normal = normal.normalized;
        linea.normal = normal; 
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(inicio, 0.05f); 
        Gizmos.DrawSphere(fin, 0.05f);
        Gizmos.DrawLine(inicio, fin); 
        
        Gizmos.color = Color.green;
        Vector2 midPoint = (inicio + fin) / 2;
        Gizmos.DrawLine(midPoint, midPoint + normal * 1);
    }
}
