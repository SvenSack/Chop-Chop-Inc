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
    
    public AudioClip[] foxSounds = new AudioClip[15];
    public AudioClip[] squirrelSounds = new AudioClip[11];
    public AudioClip[] capybaraSounds = new AudioClip[16];
    public AudioClip[] maccawSounds = new AudioClip[14];
    public AudioClip[] armadilloSounds = new AudioClip[11];
    public AudioClip[] caracalSounds = new AudioClip[10];

    public AudioClip[] treeFall = new AudioClip[2];

    public AudioClip[] treeRustle =  new AudioClip[2];

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
        if (chainsawSoundSource == null)
        {
            chainsawSoundSource = GenerateAudio(chainsaw[0]);
            chainsawSoundObject = chainsawSoundSource.gameObject;
            chainsawSoundSource.Play();
            chainsawSoundSource.spatialBlend = .5f;
            cueCutLoop = StartCoroutine(CueCutLoop(chainsawSoundSource.clip.length-.01f));
        }
        else
        {
            SwapChainsawSound(chainsaw[1], false, true, true);
        }
        chainsawSoundSource.GetComponent<AudioCleanup>().enabled = false;
    }

    public void ToggleWood()
    {
        //if(chainsaw[3] != null && chainsaw[1] != null && chainsawSoundSource.clip != null)
        //{
            SwapChainsawSound(chainsawSoundSource.clip != chainsaw[3] ? chainsaw[3] : chainsaw[1], true, true, false);
        //}
        //else
        //{
        //    Debug.LogError("chainsaw[3]/ chainsaw[1]/chainsawSoundSource.clip is null ");
        //}
            
    }

    public AudioSource TreeShake(Transform treeToShake)
    {
        var ret = GenerateAudio(treeRustle[0]);
        ret.transform.position = treeToShake.position;
        ret.Play();
        return ret;
    }

    public void StopCut()
    {
        Debug.Log("stopped cut sound");
        if (chainsawSoundSource.clip != chainsaw[0])
        {
            SwapChainsawSound(chainsaw[2], false, true, true);
            chainsawSoundSource.GetComponent<AudioCleanup>().enabled = true;
        }
        else
        {
                StopCoroutine(cueCutLoop);
                SwapChainsawSound(chainsaw[2], false, true, true);
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

    private void SwapChainsawSound(AudioClip newSound, bool looping, bool spatial, bool reset)
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
        if (reset)
            chainsawSoundSource.time = 0;
    }

    IEnumerator CueCutLoop(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (chainsawSoundSource.clip == chainsaw[0])
        {
            SwapChainsawSound(chainsaw[1], true, true, true);
        }
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
