using System.Collections;
using UnityEngine;

namespace Animal
{
    public class Capybara : AnimalSprite
    {
        private bool running;
        [SerializeField] private float tireTimer = 5f;
        private float tireTime;

        public override void Update()
        {
            bool laying = animator.GetBool("laying");
            bool isRunning = animator.GetBool("running");
            
            if (Vector3.Distance(cameraPosition, transform.position) < soundProximity && !isRunning)
            {
                if(idleTimer <= idleTime)
                {
                    idleTime = 0;
                    mouth.clip = soundMan.capybaraSounds[Random.Range(0, 9)];
                    mouth.time = 0;
                    mouth.Play();
                }
                else
                {
                    idleTime += Random.Range(1f,3f)*Time.deltaTime;
                }
            }

            if (tireTime >= tireTimer && !laying && !isRunning)
            {
                animator.SetBool("laying", true);
            }
            else
            {
                if(!waiting)
                tireTime += Time.deltaTime;
            }
            
            if(waiting)
                SnapToCamera(false);

            
            if(!isTurning && !laying && (isRunning && running || !isRunning) && !waiting)
                PerformMotion();
            
            
            if((!laying && !running) && !isTurning)
                CheckForFlip();
        }

        
        IEnumerator WakeCompleter(float timer)
        {
            if(timer > 0)
                yield return new WaitForSeconds(timer-.2f);
            mouth.clip = soundMan.capybaraSounds[Random.Range(9, 16)];
            mouth.time = 0;
            mouth.Play();
            if(timer > 0)
                yield return new WaitForSeconds(0.2f);
            running = true;
            walkSpeed = walkSpeed * 3;
        }

        public void Scare(Vector3 positionOfScaryness)
        {
          
            if(camera.WorldToScreenPoint(transform.position).x < camera.WorldToScreenPoint(positionOfScaryness).x && directionIsLeft)
            {
                PatchworkFlip(.3f);
            }
            if(!animator.GetBool("running"))
            {
                if(animator.GetBool("laying"))
                    StartCoroutine(WakeCompleter(0.5f));
                else
                {
                    StartCoroutine(WakeCompleter(0f));
                }
                animator.SetBool("laying", false);
                animator.SetBool("running", true);
            }
        }
        
    }
}
