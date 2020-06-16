using Animal;
using UnityEngine;

public class TreeFallParticle : MonoBehaviour
{
    private ParticleSystem leaves;
    private ParticleSystem dust;
    private bool inAir = true;

    public AudioSource fallSound;
    private SoundMan soundMan;
    
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem[] temp = transform.GetComponentsInChildren<ParticleSystem>();

        if(temp.Length >= 2)
        {
            leaves = temp[0];
            dust = temp[1];
        }
        
        soundMan = FindObjectOfType<SoundMan>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") && inAir)
        {
            if(leaves != null)
            {
                leaves.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
            if(dust != null)
            {
                dust.Play();
            }
            
            fallSound.time = 0;
            fallSound.clip = soundMan.treeFall[1];
            fallSound.Play();
            inAir = false;
            Fox[] foxes = FindObjectsOfType<Fox>();
            Capybara[] capys = FindObjectsOfType<Capybara>();
            Vector3 landingPoint = other.GetContact(0).point;
            Debug.DrawLine(landingPoint, landingPoint+Vector3.up*10, Color.red, 1000f);
            foreach (var fox in foxes)
            {
                if (Vector3.Distance(fox.transform.position, landingPoint) < 5f)
                {
                    fox.Scare(landingPoint);
                }
            }
            foreach (var capy in capys)
            {
                if (Vector3.Distance(capy.transform.position, landingPoint) < 5f)
                {
                    capy.Scare(landingPoint);
                }
            }
        }
    }
}
