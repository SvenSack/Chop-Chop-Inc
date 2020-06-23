using Animal;
using UnityEngine;

public class TreeFallParticle : MonoBehaviour
{
    public ParticleSystem[] leaves;
    public ParticleSystem dust;
    private bool inAir = true;

    public AudioSource fallSound;
    private SoundMan soundMan;
    private UIMan uiMan;
    
    // Start is called before the first frame update
    void Start()
    {
        
        soundMan = FindObjectOfType<SoundMan>();
        uiMan = FindObjectOfType<UIMan>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            
            Vector3 landingPoint = other.GetContact(0).point;
            if (inAir)
            {
                /*if(leaves != null)
                {
                    foreach (var leaf in leaves)
                    {
                        leaf.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    }
                }*/
                if(dust != null)
                {
                    dust.transform.rotation = Quaternion.Euler(other.GetContact(0).normal);
                    dust.transform.position = landingPoint;
                    dust.Play();
                }
            
                fallSound.time = 0;
                fallSound.clip = soundMan.treeFall[1];
                fallSound.Play();
                inAir = false;
            }
            Fox[] foxes = FindObjectsOfType<Fox>();
            Capybara[] capys = FindObjectsOfType<Capybara>();
            Maccaw[] maccis = FindObjectsOfType<Maccaw>();
            Armadillo[] dillos = FindObjectsOfType<Armadillo>();
            Caracal[] cars = FindObjectsOfType<Caracal>();
            Debug.DrawLine(landingPoint, landingPoint+Vector3.up*10, Color.red, 1000f);
            foreach (var fox in foxes)
            {
                if (Vector3.Distance(fox.transform.position, landingPoint) < 5f)
                {
                    fox.Scare(landingPoint);
                    uiMan.TryVoiceLine(6);
                }
            }
            foreach (var capy in capys)
            {
                if (Vector3.Distance(capy.transform.position, landingPoint) < 5f)
                {
                    capy.Scare(landingPoint);
                    uiMan.TryVoiceLine(8);
                }
            }
            foreach (var maccaw in maccis)
            {
                if (Vector3.Distance(maccaw.transform.position, landingPoint) < 15f)
                {
                    maccaw.Scare(landingPoint);
                    uiMan.TryVoiceLine(9);
                }
            }
            foreach (var car in cars)
            {
                if (Vector3.Distance(car.transform.position, landingPoint) < 15f)
                {
                    car.Scare(landingPoint);
                    uiMan.TryVoiceLine(11);
                }
            }
            foreach (var dillo in dillos)
            {
                if (Vector3.Distance(dillo.transform.position, landingPoint) < 30f)
                {
                    dillo.Scare(landingPoint);
                    uiMan.TryVoiceLine(10);
                }
            }
        }
    }
}
