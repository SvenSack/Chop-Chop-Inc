using UnityEngine;

namespace Animal
{
    public class AnimalSprite : MonoBehaviour
    {
        public AudioSource mouth;
        public float soundProximity = 10f;
        public float idleTimer = 20f;
        public float idleTime;
        public Animator animator;
        public SoundMan soundMan;

        protected Camera camera;

        void Awake()
        {
            soundMan = FindObjectOfType<SoundMan>();
            camera = Camera.main;
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
        }
    }
}
