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

        void Awake()
        {
            soundMan = FindObjectOfType<SoundMan>();
        }

        // Update is called once per frame
        public virtual void Update()
        {
            
        }


        public void SnapToCamera()
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 lookAtPosition = new Vector3(cameraPosition.x, transform.position.y, cameraPosition.y);

            transform.LookAt(lookAtPosition);
        }
    }
}
