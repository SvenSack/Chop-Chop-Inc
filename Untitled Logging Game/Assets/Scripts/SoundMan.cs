using System.Collections;
using UnityEngine;

public class SoundMan : MonoBehaviour
{
    private static SoundMan instance;
    public AudioClip[] chainsaw = new AudioClip[4];
    public GameObject audioSource;
    public GameObject chainsawSoundObject;
    private AudioSource chainsawSoundSource;
    private Coroutine cueCutLoop;

    public AudioClip[] treeFall = new AudioClip[2];

    void Awake() {
        if(!instance )
            instance = this;
        else {
            Destroy(gameObject) ;
            return;
        }

        DontDestroyOnLoad(gameObject) ;
    }

    public void StartCut()
    {
        if(chainsawSoundSource == null)
        {
            chainsawSoundSource = GenerateAudio(chainsaw[0]);
            chainsawSoundObject = chainsawSoundSource.gameObject;
            chainsawSoundSource.Play();
            chainsawSoundSource.spatialBlend = .5f;
            chainsawSoundSource.GetComponent<AudioCleanup>().enabled = false;
            cueCutLoop = StartCoroutine(CueCutLoop(chainsawSoundSource.clip.length-.01f));
        }
    }

    public void ToggleWood()
    {
        //if(chainsaw[3] != null && chainsaw[1] != null && chainsawSoundSource.clip != null)
        //{
            SwapChainsawSound(chainsawSoundSource.clip != chainsaw[3] ? chainsaw[3] : chainsaw[1], true, true);
        //}
        //else
        //{
        //    Debug.LogError("chainsaw[3]/ chainsaw[1]/chainsawSoundSource.clip is null ");
        //}
            
    }

    public void StopCut()
    {
        Debug.Log("stopped cut sound");
        if (chainsawSoundSource.clip != chainsaw[0])
        {
            SwapChainsawSound(chainsaw[2], false, true);
            chainsawSoundSource.GetComponent<AudioCleanup>().enabled = true;
        }
        else
        {
                StopCoroutine(cueCutLoop);
                SwapChainsawSound(chainsaw[2], false, true);
                chainsawSoundSource.GetComponent<AudioCleanup>().enabled = true;
        }
    }

    private AudioSource GenerateAudio(AudioClip audioC)
    {
        GameObject inst = Instantiate(audioSource);
        AudioSource ret = inst.GetComponent<AudioSource>();
        ret.clip = audioC;
        return ret;
    }

    private void SwapChainsawSound(AudioClip newSound, bool looping, bool spatial)
    {
        chainsawSoundSource = GenerateAudio(newSound);
        Vector3 temp = chainsawSoundObject.transform.position;
        Destroy(chainsawSoundObject);
        chainsawSoundObject = chainsawSoundSource.gameObject;
        chainsawSoundObject.transform.position = temp;
        if (looping)
            chainsawSoundSource.loop = true;
        if (spatial)
            chainsawSoundSource.spatialBlend = .5f;
        chainsawSoundSource.Play();
    }

    IEnumerator CueCutLoop(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if(chainsawSoundSource.clip == chainsaw[0])
            SwapChainsawSound(chainsaw[1], true, true);
    }

    public AudioSource TreeFall(GameObject tree)
    {
        AudioSource temp = GenerateAudio(treeFall[0]);
        temp.Play();
        temp.spatialBlend = 0.5f;
        temp.transform.SetParent(tree.transform, false);
        return temp;
    }
}
