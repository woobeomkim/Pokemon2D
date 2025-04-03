using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        player.Character.Animator.IsMoving = false;
        StartCoroutine(SwitchScene());
    }

    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }
    IEnumerator SwitchScene()
    {
        // ���� �������� �ı�����
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PausedGame(true);
        yield return fader.FadeIn(0.5f);

        yield return  SceneManager.LoadSceneAsync(sceneToLoad);

        var desPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(desPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);

        GameController.Instance.PausedGame(false);
        // ���� ������ �ı�
        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;

    public bool TriggerRepeatedly => false;
}


public enum DestinationIdentifier { A,B,C,D,E}