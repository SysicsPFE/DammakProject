using UnityEngine;
public class DrawLine : MonoBehaviour
{
    private RaycastHit _hit;

    void Update()
    {
        if(Physics.Raycast (transform.position, transform.up, out _hit))
            Debug.DrawLine (transform.position, _hit.point,Color.red);   
    } 
}
