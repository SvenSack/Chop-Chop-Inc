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
        public LayerMask groundMask;

        protected Camera camera;

        public virtual void Start()
        {
            soundMan = FindObjectOfType<SoundMan>();
            camera = Camera.main;
            spriteTrans = transform.GetComponentInChildren<SpriteRenderer>().transform;
            cameraPosition = camera.transform.position;
            groundMask = LayerMask.GetMask("Ground");
        }

        // Update is called once per frame
        public virtual void Update()
        {
            
        }


        public void SnapToCamera(bool action)
        {
            Vector3 cameraPosition = camera.transform.position;
            Vector3 lookAtPosition = new Vector3(cameraPosition.x, transform.position.y, cameraPosition.z);

            transform.LookAt(lookAtPosition);
            if(action)
                waiting = false;
        }
        
        public void PerformMotion(bool doTerrain)
        {
            Vector3 position;
            if (doTerrain)
            {
                Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, groundMask);
                Debug.DrawRay(hit.point, hit.normal, Color.cyan, 2f);
                Debug.DrawRay(hit.point, transform.up, Color.blue, 2f);
                Vector3 currentRot = transform.rotation.eulerAngles;
                float walkAngle = Vector3.Angle(transform.position, hit.normal) - 90f;
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.Euler(new Vector3(currentRot.x, currentRot.y, walkAngle)), 1*(walkAngle/5f));
                Vector3 targetVec = transform.right;
                if (!directionIsLeft)
                    targetVec = targetVec * -1;
                position = transform.position;
                float margin1 = .05f * transform.localScale.x;
                float margin2 = .1f * transform.localScale.x;
                if (hit.distance > margin1 && hit.distance < margin2)
                    position = Vector3.MoveTowards(position, position + targetVec, walkSpeed * Time.deltaTime);
                else
                {
                    if (hit.distance > margin1)
                    {
                        position = Vector3.MoveTowards(position, position + targetVec + (transform.up * -1),
                            walkSpeed * Time.deltaTime);
                    }
                    else
                    {
                        position = Vector3.MoveTowards(position, position + targetVec + transform.up,
                            walkSpeed * Time.deltaTime);
                    }
                }
            }
            else
            {
                Vector3 targetVec = transform.right;
                if (!directionIsLeft)
                    targetVec = targetVec * -1;
                position = transform.position;
                position = Vector3.MoveTowards(position, position + targetVec, walkSpeed * Time.deltaTime);
            }

            transform.position = position;
            Vector3 viewPoint = camera.WorldToScreenPoint(transform.position);
            if (!(viewPoint.z > 0 && viewPoint.x > 5 && viewPoint.x < Screen.width - 5))
            {
                Destroy(gameObject);
            }
        }

        public void CheckForFlip()
        {
            
            Vector3 viewPoint = camera.WorldToScreenPoint(transform.position);
            if ( !(viewPoint.z > 0 && viewPoint.x > 10 && viewPoint.x < Screen.width-10) )
            {
                Flip();
            }
        }

        public void PatchworkFlip(float time)
        {
            Vector3 targetRot = spriteTrans.rotation.eulerAngles;
            targetRot.y += 180;
            directionIsLeft = !directionIsLeft;
            spriteTrans.LeanRotate(targetRot, time);
        }

        private void Flip()
        {
            Vector3 targetRot = spriteTrans.rotation.eulerAngles;
            targetRot.y += 180;
            directionIsLeft = !directionIsLeft;
            isTurning = true;
            StartCoroutine(TurnCompleter(turnTime));
            spriteTrans.LeanRotate(targetRot, turnTime);
        }

        public IEnumerator TurnCompleter(float timer)
        {
            yield return new WaitForSeconds(timer);
            isTurning = false;
        }
    }
}
