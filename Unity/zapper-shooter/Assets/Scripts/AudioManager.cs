using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    private GameObject 	m_audioSourcePrefab;
    
    private List<AudioSource> m_currentlyLooping = new List<AudioSource>();

    private static List<string> m_FxClips = new List<string>();
    private static int m_FxIndex = 0;

    private static List<string> m_SpawnClips = new List<string>();
    private static int m_SpawnIndex = 0;

    static AudioManager() {
        m_SpawnClips.Add("Audio/FX/118336__dag451__monster-growl");
        m_SpawnClips.Add("Audio/FX/61570__intimidated__what");
        m_SpawnClips.Add("Audio/FX/125919__klankbeeld__horror-what-are-you-doing-here-cathedral");
        m_SpawnClips.Add("Audio/FX/221688__nigelnix__growling-zombie");
        //m_SpawnClips.Add("Audio/FX/333832__nick121087__demonic-woman-scream");
        m_SpawnClips.Add("Audio/FX/135419__mlsprovideos__moan4-echo");
        m_SpawnClips.Add("Audio/FX/126113__klankbeeld__laugh");
        m_SpawnClips.Add("Audio/FX/370707__boodabomb__of-like-laugh");

        m_FxClips.Add("Audio/FX/31255__erh__door-1");
        m_FxClips.Add("Audio/FX/327634__rickaldo09__horror");
        m_FxClips.Add("Audio/FX/232145__raspberrytickle__demon-grumble-whisper");
        m_FxClips.Add("Audio/FX/260446__klankbeeld__horror-talk-01");
    }

    public enum AppAudioClip
	{
		RandomFx,
        ZombieSpawned,
        DieZombie,
        PistolShot,
        PistolReload
	}

    private bool _muted = false;
    public bool Muted {
        set {
            _muted = value;
        }
        get {
            return _muted;
        }
    }

    private string ClipPath(AppAudioClip clip) {

        string path = null;

		switch (clip) {
            case AppAudioClip.RandomFx:         return nextFxClip();
            case AppAudioClip.ZombieSpawned:    return nextSpawnClip();
            case AppAudioClip.DieZombie:        return "Audio/FX/333832__nick121087__demonic-woman-scream";
            case AppAudioClip.PistolReload:     return "Audio/FX/276962__gfl7__mp7-reload-sound";
            case AppAudioClip.PistolShot:       return "Audio/FX/37236__shades__gun-pistol-one-shot";
        }
		return path;
    }

    private string nextFxClip() {
        m_FxIndex = (m_FxIndex + 1) % m_FxClips.Count;
        return m_FxClips[m_FxIndex];
    }

    private string nextSpawnClip() {
        m_SpawnIndex = (m_SpawnIndex + 1) % m_SpawnClips.Count;
        return m_SpawnClips[m_SpawnIndex];
    }

    private GameObject getAudioSourcePrefab(AppAudioClip clip) {
        return m_audioSourcePrefab;
    }

    private bool shouldLoop(AppAudioClip clip) {
        return false;
    }

    public void playClip(AppAudioClip clip) {
        if (_muted) {
            Debug.Log("AudioManager Muted, not playing anything");
            return;
        }
		//Debug.Log("AudioManager play clip " + clip);

        string path = ClipPath(clip);
		Debug.Assert(path != null);

		if (path != null) {
			//AudioClip playMe = (AudioClip)Resources.Load(path, typeof(AudioClip));//Resources.Load(path) as AudioClip;
			AudioClip playMe = Resources.Load<AudioClip>(path);
			Debug.Assert(playMe != null);
			if (playMe != null) {
				//Debug.Log("AudioManager clip loaded " + playMe);

				//we have to use separate audiosources per clip, for polyphony
                GameObject audioSource = Instantiate(getAudioSourcePrefab(clip)) as GameObject;
				audioSource.transform.SetParent(this.transform);				
				
				AudioSource source = audioSource.GetComponent<AudioSource>();
				Debug.Assert(source != null);

				source.clip = playMe;
                bool looping = shouldLoop(clip); 
                source.loop = looping;

				source.Play();
				
                //cleanup after finished
                //loopable clips need to be cleaned explicitly. Or then just mute them..? then we need an audio source for each
                if (looping) {
                    m_currentlyLooping.Add(source);
                }
                else {
                    
                    float timeInSecs = playMe.length;
                    Debug.Assert (timeInSecs > 0.0f);
                    if (timeInSecs <= 0.0f) {
                        //some safety programming. important is that the clip get's cleaned up at some point
                        Debug.LogWarning("[AudioManager]: clip length reported " + timeInSecs);
                        timeInSecs = 10.0f;
                    }
                    StartCoroutine(cleanUpFinished(audioSource, timeInSecs));
                }
				
			}
        }

    }

    public void clearLoopingClips() {     
        foreach (AudioSource s in m_currentlyLooping) {
            Destroy(s.gameObject);
        }
        m_currentlyLooping.Clear();
    }

    private IEnumerator cleanUpFinished(GameObject source, float secs) {
        yield return new WaitForSeconds(secs); 
        Destroy(source);
    }
    
}
