using UnityEngine;

public class TreeFallParticle : MonoBehaviour
{
    private ParticleSystem leaves;
    private ParticleSystem dust;

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
        if (other.gameObject.CompareTag("Ground"))
        {
            if(leaves != null)
            {
                leaves.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
            
            dust.Play();
            fallSound.time = 0;
            fallSound.clip = soundMan.treeFall[1];
            fallSound.Play();
        }
    }
}
