using System.Collections;
using UnityEngine;

namespace Animal
{
    public class Squirrel : AnimalSprite
    {
        public float floorHeight;
        [SerializeField] private float speed = 2f;
        private bool running;

        public override void Start()
        {
            base.Start();
            SnapToCamera(true);
        }

        public override void Update()
        {
            bool landed = animator.GetBool("land");
            var position = transform.position;
            if(!landed)
            {
                if (position.y > floorHeight)
                {
                    transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
                    if (position.y < floorHeight)
                        position.y = floorHeight;
                }
                else
                {
                    animator.SetBool("land", true);
                    StartCoroutine(LandCompleter(1.8f));
                }
            }
            
            if(running)
                PerformMotion();
        }
        
        IEnumerator LandCompleter(float timer)
        {
            yield return new WaitForSeconds(timer-1f);
            mouth.clip = soundMan.foxSounds[Random.Range(5, 8)];
            mouth.time = 0;
            mouth.Play();
            yield return new WaitForSeconds(1f);
            running = true;
        }
    }
}
