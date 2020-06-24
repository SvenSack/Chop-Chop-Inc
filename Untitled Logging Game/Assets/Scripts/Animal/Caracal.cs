using System.Collections;
using UnityEngine;

namespace Animal
{
    public class Caracal : AnimalSprite
    {
        private bool running;

        public override void Update()
        {
            bool isRunning = animator.GetBool("running");
            
            if (Vector3.Distance(cameraPosition, transform.position) < soundProximity && !isRunning)
            {
                if(idleTimer <= idleTime)
                {
                    idleTime = 0;
                    mouth.clip = soundMan.caracalSounds[Random.Range(5, 10)];
                    mouth.time = 0;
                    mouth.Play();
                }
                else
                {
                    idleTime += Random.Range(1f,3f)*Time.deltaTime;
                }
            }
            
            if(waiting)
                SnapToCamera(false);

            
            if(!isTurning && isRunning && running && !waiting)
                PerformMotion(true);
        }

        
        IEnumerator WakeCompleter(float timer)
        {
            if(timer > 0)
                yield return new WaitForSeconds(timer-.4f);
            mouth.clip = soundMan.caracalSounds[Random.Range(0, 5)];
            mouth.time = 0;
            mouth.Play();
            if(timer > 0)
                yield return new WaitForSeconds(0.4f);
            running = true;
        }

        public void Scare(Vector3 positionOfScaryness)
        {
          Debug.Log("cat want run");
            if(camera.WorldToScreenPoint(transform.position).x < camera.WorldToScreenPoint(positionOfScaryness).x && directionIsLeft)
            {
                PatchworkFlip(.3f);
            }
            if(!animator.GetBool("running"))
            {
                StartCoroutine(WakeCompleter(1f));
                animator.SetBool("running", true);
            }
        }
        
    }
}
