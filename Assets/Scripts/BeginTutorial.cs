using System.Collections;
using System.Collections.Generic;
// using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

public class BeginTutorial : MonoBehaviour
{
    public float waitForVoiceOverTime = 3f;
    public float instrumentPlayingTime = 120f; //set 5 or less for testing, 120 for actual playing time
    [SerializeField]
    private List<GameObject> m_GameObjectList;

    [SerializeField]
    private List<GameObject> m_NetworkedObjectList;
    private AudioSource audiosource;
    // [SerializeField]
    // private AudioClip tutorialVoiceOver1;
    [SerializeField]
    private PlayableDirector m_Director;

    [SerializeField] private PlayableDirector EleanorVoiceOver;

    public static int playerCount = 0;
    public bool dissolveInstruments = false; //also triggers the DetectingPlayersForTransition script

    private CountdownTimer countdownTimer;
    // [SerializeField] private SkipTutorialInstruments skipTutorialInstruments;

    private Coroutine startTutorialCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        countdownTimer = GetComponent<CountdownTimer>();
        startTutorialCoroutine = StartCoroutine(WaitTutorial());
    }


    IEnumerator WaitTutorial()
    {
        yield return new WaitForSeconds(waitForVoiceOverTime); // Wait for seconds for players to play
        // audiosource.PlayOneShot(tutorialVoiceOver1);  //voice over telling them the beginning instructions
        StartTutorial();
        countdownTimer.StartCountdown(); //timer starting
        yield return new WaitForSeconds(instrumentPlayingTime);
        StartPlayableDirectorCall();

    }

    public void DissolveInstruments()
    {
        dissolveInstruments = true;
    }

    public void DestroyAndDeSpawnInstruments()
    {
        for (int i = 0; i < m_GameObjectList.Count; i++) //Destroy or DeSpawn objects

        {
            Destroy(m_GameObjectList[i]);
        }

        // for (int i = 0; i < m_NetworkedObjectList.Count; i++)
        // {
        //     if (m_NetworkedObjectList[i].IsSpawned)
        //     {
        //         m_NetworkedObjectList[i].Despawn(true);
        //     }


        // }
    }

    public void StartPlayableDirectorCall()
    {
        StartPlayableDirectorServerRpc();
    }

    // [ServerRpc(RequireOwnership = false)]
    private void StartPlayableDirectorServerRpc()
    {
        StartPlayableDirectorClientRpc();
    }

    // [ClientRpc]
    private void StartPlayableDirectorClientRpc()
    {
        StopCoroutine(startTutorialCoroutine);
        EleanorVoiceOver.Stop();
        m_Director.Play();

    }

    public void StartTutorial()
    {
        StartTutorialServerRpc();
    }

    // [ServerRpc(RequireOwnership = false)]
    private void StartTutorialServerRpc()
    {
        StartTutorialClientRpc();
    }

    // [ClientRpc]
    private void StartTutorialClientRpc()
    {
        EleanorVoiceOver.Play();

    }
}
