using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animal
{
    public class AnimalSprite : MonoBehaviour
    {
        public AudioSource mouth;
        public float soundProximity = 10f;
        public float idleTimer = 20f;
        [HideInInspector] public float idleTime;
        public Animator animator;
        [HideInInspector] public SoundMan soundMan;
        [HideInInspector] public Vector3 cameraPosition;
        public bool directionIsLeft = true;
        public float walkSpeed = .1f;
        [HideInInspector] public Transform spriteTrans;
        [HideInInspector] public bool isTurning;
        [SerializeField] public float turnTime = 2f;
        public bool waiting = true;

        protected Camera camera;

        public virtual void Start()
        {
            soundMan = FindObjectOfType<SoundMan>();
            camera = Camera.main;
            spriteTrans = transform.GetComponentInChildren<SpriteRenderer>().transform;
            cameraPosition = camera.transform.position;
        }

        // Update is called once per frame
        public virtual void Update()
        {
            
        }


        public void SnapToCamera()
        {
            Vector3 cameraPosition = camera.transform.position;
            Vector3 lookAtPosition = new Vector3(cameraPosition.x, transform.position.y, cameraPosition.y);

            transform.LookAt(lookAtPosition);
            waiting = false;
        }
        
        public void PerformMotion()
        {
            Vector3 targetVec = transform.right;
            if (!directionIsLeft)
                targetVec = targetVec * -1;
            var position = transform.position;
            position = Vector3.MoveTowards(position, position + targetVec, walkSpeed * Time.deltaTime);
            transform.position = position;
        }

        public void CheckForFlip()
        {
            
            Vector3 viewPoint = camera.WorldToScreenPoint(transform.position);
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

        public IEnumerator TurnCompleter(float timer)
        {
            yield return new WaitForSeconds(timer);
            isTurning = false;
        }
    }
}
