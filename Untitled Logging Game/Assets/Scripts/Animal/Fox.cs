using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animal
{
    public class Fox : AnimalSprite
    {
        
        
        private bool isShocked;
        

        public override void Update()
        {
            if (Vector3.Distance(cameraPosition, transform.position) < soundProximity && !animator.GetBool("scared"))
            {
                if(idleTimer <= idleTime)
                {
                    idleTime = 0;
                    mouth.clip = soundMan.foxSounds[Random.Range(0, 4)];
                    mouth.time = 0;
                    mouth.Play();
                }
                else
                {
                    idleTime += Random.Range(1f,3f)*Time.deltaTime;
                }
            }

            
            if(!isTurning && !isShocked && !waiting)
                PerformMotion(true);
            
            
            if(!animator.GetBool("scared") && !isTurning)
                CheckForFlip();
            
        }


        
        
        IEnumerator ScareCompleter(float timer)
        {
            yield return new WaitForSeconds(timer-1.4f);
            mouth.clip = soundMan.foxSounds[Random.Range(5, 15)];
            mouth.time = 0;
            mouth.Play();
            yield return new WaitForSeconds(timer-1.0f);
            PatchworkFlip(.1f);
            yield return new WaitForSeconds(0.3f);
            walkSpeed = walkSpeed * 3;
            isShocked = false;
        }

        public void Scare(Vector3 positionOfScaryness)
        {
          
            if(camera.WorldToScreenPoint(transform.position).x < camera.WorldToScreenPoint(positionOfScaryness).x && directionIsLeft)
            {
                PatchworkFlip(.3f);
            }
            if(!animator.GetBool("scared"))
            {
                animator.SetBool("scared", true);
                isShocked = true;
                StartCoroutine(ScareCompleter(2.8f));
            }
        }
    }
}
