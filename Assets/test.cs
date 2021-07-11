using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class test : MonoBehaviour
{

  bool right;
   bool top ;
    bool down;
   bool left ;
    // this function replicate the object surrounding the targetvoid OnCollisionEnter2D(Collision2D  collision) 

        void OnCollisionEnter2D(Collision2D  collision) 
     {
         top=false;
         right=false;
         down=false;
         left=false;
         Collider2D collider = collision.collider;
  
             Vector3 contactPoint = collision.contacts[1].point;
             Vector3 center = collider.bounds.center;
 
              right = (center.x+ collider.bounds.extents.x >= contactPoint.x);
              top = (center.y+ collider.bounds.extents.y >= contactPoint.y);
                left = (center.x+ collider.bounds.extents.x <= contactPoint.x);
              down = (center.y+ collider.bounds.extents.y <= contactPoint.y);


            if(right){
                this.transform.position=new Vector2(transform.position.x-1,transform.position.y);
            }
               if(left){
                this.transform.position=new Vector2(transform.position.x+1,transform.position.y);
            }

            if(top){
                this.transform.position=new Vector2(transform.position.x,transform.position.y-1);
            }
               if(down){
                this.transform.position=new Vector2(transform.position.x+1,transform.position.y+1);
            }
         
     }

     }
