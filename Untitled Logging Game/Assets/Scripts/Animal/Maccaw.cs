using System.Collections;
using UnityEngine;

namespace Animal
{
    public class Maccaw : AnimalSprite
    {
        private bool flying;

        public override void Update()
        {
            bool isFlying = animator.GetBool("fly");
            
            if (Vector3.Distance(cameraPosition, transform.position) < soundProximity && !isFlying)
            {
                if(idleTimer <= idleTime)
                {
                    idleTime = 0;
                    mouth.clip = soundMan.maccawSounds[Random.Range(0, 7)];
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

            if(isFlying && flying && !waiting)
                PerformMotion();
        }

        
        IEnumerator WakeCompleter(float timer)
        {
            if(timer > 0)
                yield return new WaitForSeconds(timer-.2f);
            mouth.clip = soundMan.maccawSounds[Random.Range(7, 13)];
            mouth.time = 0;
            mouth.Play();
            if(timer > 0)
                yield return new WaitForSeconds(0.2f);
            flying = true;
        }

        public void Scare(Vector3 positionOfScaryness)
        {
          
            if(camera.WorldToScreenPoint(transform.position).x < camera.WorldToScreenPoint(positionOfScaryness).x && directionIsLeft)
            {
                PatchworkFlip(.3f);
            }
            if(!animator.GetBool("fly"))
            {
                StartCoroutine(WakeCompleter(1f));
                animator.SetBool("fly", true);
            }
        }
        
    }
}
