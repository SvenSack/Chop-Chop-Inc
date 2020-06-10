using UnityEngine;

namespace Animal
{
    public class Squirrel : AnimalSprite
    {

        public override void Update()
        {
            Vector3 cameraPosition = camera.transform.position;
            Vector3 lookAtPosition = new Vector3(cameraPosition.x, transform.position.y, cameraPosition.y);
            transform.LookAt(lookAtPosition);
            
            if (Vector3.Distance(cameraPosition, transform.position) < soundProximity && !animator.GetBool("scared"))
            {
                if(idleTimer <= idleTime)
                {
                    idleTime = 0;
                    mouth.clip = soundMan.foxSounds[Random.Range(0, 2)];
                    mouth.time = 0;
                    mouth.Play();
                }
                else
                {
                    idleTime += Time.deltaTime;
                }
            }
        }
        
        
    }
}
