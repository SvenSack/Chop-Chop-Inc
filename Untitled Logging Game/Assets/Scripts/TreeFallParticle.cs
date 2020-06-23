using Animal;
using UnityEngine;

public class TreeFallParticle : MonoBehaviour
{
    public ParticleSystem[] leaves;
    public ParticleSystem dust;
    private bool inAir = true;
    public bool isPalm;

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
                if(leaves != null && isPalm)
                {
                    foreach (var leaf in leaves)
                    {
                        GameObject dadLeaf = leaf.transform.parent.parent.gameObject;
                        dadLeaf.transform.SetParent(null);
                        Rigidbody rb = dadLeaf.AddComponent<Rigidbody>();
                        MeshCollider mc = dadLeaf.AddComponent<MeshCollider>();
                        mc.convex = true;
                        rb.mass = .1f;
                    }
                }
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
                if(!fox.waiting)
                    if (Vector3.Distance(fox.transform.position, landingPoint) < fox.scareDistance)
                    {
                        fox.Scare(landingPoint);
                        uiMan.TryVoiceLine(6);
                    }
            }
            foreach (var capy in capys)
            {
                if(!capy.waiting)
                    if (Vector3.Distance(capy.transform.position, landingPoint) < capy.scareDistance)
                    {
                        capy.Scare(landingPoint);
                        uiMan.TryVoiceLine(8);
                    }
            }
            foreach (var maccaw in maccis)
            {
                if(!maccaw.waiting)
                    if (Vector3.Distance(maccaw.transform.position, landingPoint) < maccaw.scareDistance)
                    {
                        maccaw.Scare(landingPoint);
                        uiMan.TryVoiceLine(9);
                    }
            }
            foreach (var car in cars)
            {
                if(!car.waiting)
                    if (Vector3.Distance(car.transform.position, landingPoint) < car.scareDistance)
                    {
                        car.Scare(landingPoint);
                        uiMan.TryVoiceLine(11);
                    }
            }
            foreach (var dillo in dillos)
            {
                if(!dillo.waiting)
                    if (Vector3.Distance(dillo.transform.position, landingPoint) < dillo.scareDistance)
                    {
                        dillo.Scare(landingPoint);
                        uiMan.TryVoiceLine(10);
                    }
            }
        }
    }
}
