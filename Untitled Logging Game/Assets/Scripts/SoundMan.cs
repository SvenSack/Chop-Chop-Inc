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
        chainsawSoundSource = GenerateAudio(chainsaw[0]);
        chainsawSoundObject = chainsawSoundSource.gameObject;
        chainsawSoundSource.Play();
        chainsawSoundSource.spatialBlend = .5f;
        cueCutLoop = StartCoroutine(CueCutLoop(chainsawSoundSource.clip.length));
    }

    public void ToggleWood()
    {
        SwapChainsawSound(chainsawSoundSource.clip != chainsaw[3] ? chainsaw[3] : chainsaw[1], true, true);
    }

    public void StopCut()
    {
        if (chainsawSoundSource.clip != chainsaw[0])
            SwapChainsawSound(chainsaw[2], false, true);
        else
        {
                StopCoroutine(cueCutLoop);
                SwapChainsawSound(chainsaw[2], false, true);
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
