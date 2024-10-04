using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Goal : MonoBehaviour
{
    //a static field accessible by code everywhere
    static public bool goalMet = false;

    void OnTriggerEnter(Collider other){
        //when the trigger is hit by something
        //check to see if it's a projectile
        Projectile proj = other.GetComponent<Projectile>();;
        if(proj != null){
            //if so, set goalmet to true
            Goal.goalMet = true;
            //also set the alphs of the color to a higher opacity
            Material mat = GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = 0.75f;
            mat.color = c;
        }
    }
}
