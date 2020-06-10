using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animal
{
    public class Fox : AnimalSprite
    {

        public bool directionIsLeft = true;
        public float walkSpeed = .1f;
        private Transform spriteTrans;
        private bool isTurning;
        private bool isShocked;
        private Vector3 cameraPosition;
        [SerializeField] private float turnTime = 2f;

        public void Start()
        {
            spriteTrans = transform.GetComponentInChildren<SpriteRenderer>().transform;
            cameraPosition = Camera.main.transform.position;
        }

        public override void Update()
        {
            
            if(Input.GetKeyDown("r"))
                SnapToCamera();
            
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

            
            if(!isTurning && !isShocked)
                PerformMotion();
            
            
            if(!animator.GetBool("scared") && !isTurning)
                CheckForFlip();
            
            if(Input.GetKeyDown("l"))
                Scare(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }


        private void PerformMotion()
        {
            Vector3 targetVec = transform.right;
            if (!directionIsLeft)
                targetVec = targetVec * -1;
            var position = transform.position;
            position = Vector3.MoveTowards(position, position + targetVec, walkSpeed * Time.deltaTime);
            transform.position = position;
        }

        private void CheckForFlip()
        {
            Camera mainCam = Camera.main;
            Vector3 viewPoint = mainCam.WorldToScreenPoint(transform.position);
            if ( !(viewPoint.z > 0 && viewPoint.x > 1 && viewPoint.x < Screen.width) )
            {
                Vector3 targetRot = spriteTrans.rotation.eulerAngles;
                targetRot.y += 180;
                directionIsLeft = !directionIsLeft;
                isTurning = true;
                StartCoroutine(TurnCompleter(turnTime));
                spriteTrans.LeanRotate(targetRot, turnTime);
            }
        }

        public void PatchworkFlip(float time)
        {
            Vector3 targetRot = spriteTrans.rotation.eulerAngles;
            targetRot.y += 180;
            directionIsLeft = !directionIsLeft;
            spriteTrans.LeanRotate(targetRot, time);
        }

        IEnumerator TurnCompleter(float timer)
        {
            yield return new WaitForSeconds(timer);
            isTurning = false;
        }
        
        IEnumerator ScareCompleter(float timer)
        {
            yield return new WaitForSeconds(timer-1.7f);
            mouth.clip = soundMan.foxSounds[Random.Range(5, 8)];
            mouth.time = 0;
            mouth.Play();
            yield return new WaitForSeconds(timer-1.4f);
            PatchworkFlip(.1f);
            yield return new WaitForSeconds(0.3f);
            walkSpeed = walkSpeed * 3;
            isShocked = false;
        }

        public void Scare(Vector3 positionOfScaryness)
        {
            Camera mainCam = Camera.main;
            if(mainCam.WorldToScreenPoint(transform.position).x < mainCam.WorldToScreenPoint(positionOfScaryness).x && directionIsLeft)
            {
                Debug.Log("Fox: " + mainCam.WorldToScreenPoint(transform.position).x);
                Debug.Log("Danger: " + mainCam.WorldToScreenPoint(positionOfScaryness).x);
                Debug.Log("Fox is walking left is " + directionIsLeft);
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
