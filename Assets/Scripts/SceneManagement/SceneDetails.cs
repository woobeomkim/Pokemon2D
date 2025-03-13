using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    public bool IsLoaded { get; private set; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log($"Entered {gameObject.name}");

            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            // Load all connected scene
            foreach(var scene in connectedScenes)
                scene.LoadScene();

            // Unload the scenes thar are no longer connected
            if(GameController.Instance.PrevScene != null)
            {
                var previoslyLoadedScenes = GameController.Instance.PrevScene.connectedScenes;
                foreach(var scene in previoslyLoadedScenes)
                {
                    if(!connectedScenes.Contains(scene) && scene != this)
                        scene.UnLoadScene();
                }
            }
        }
        
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

    public void UnLoadScene()
    {
        if (IsLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}
