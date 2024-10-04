using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameMode{
    idle,
    playing,
    levelEnd
}

public class MissionDemolishon : MonoBehaviour
{
    static private MissionDemolishon S;

    [Header("Inscribed")]
    public TextMeshProUGUI uitLevel;
    public TextMeshProUGUI uitShots;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";
    // Start is called before the first frame update
    void Start()
    {
        S = this;

        levelMax = 0;
        shotsTaken = 0;
        levelMax = castles.Length;

        StartLevel();
    }

    void StartLevel(){
        if(castle != null){
            Destroy(castle);
        }

        Projectile.DESTROY_PROECTILES();

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI(){
        //show the data in the guitexts
        uitLevel.text = "Level: " + (level + 1);
        uitShots.text = "Shots Taken: " + shotsTaken;        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGUI();

        //checkk for level end
        if((mode == GameMode.playing) && Goal.goalMet){
            //change mode to stop checking for level end
            mode = GameMode.levelEnd;
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);

            //start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }
    }
    void NextLevel(){
        level++;
        if(level == levelMax){
            level = 0;
            shotsTaken = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        StartLevel();
    }

    //static method that allows code anywhere to increment shots taken
    static public void SHOT_FIRED(){
        S.shotsTaken++;
    }
    //static method that allows code anywhere to get a reference to s.castle
    static public GameObject GET_CASTLE(){
        return S.castle;
    }
}
