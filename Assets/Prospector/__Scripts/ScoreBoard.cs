using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Be sure not to miss this line. TextMeshPro needs it to work!
// ScoreBoard class manages showing the score to the player
[RequireComponent(typeof(TMP_Text))]
public class ScoreBoard : MonoBehaviour
{
    private static ScoreBoard S;

    [Header("Dynamic")]
    [SerializeField]
    private int _score = 0;
    // The score property also sets the text of the TMP_Text
    public int score
    {
        get { return _score; }
        set
        {
            _score = value;
            textMP.text = _score.ToString("#,##0"); // The 0 in #,##0 is a zero
        }
    }

    private TMP_Text textMP;

    void Awake()
    {
        if (S != null) Debug.LogError("ScoreBoard.S is already set!");
        S = this;

        textMP = GetComponent<TMP_Text>();
    }

    public static int SCORE
    {
        get { return S.score; }
        set { S.score = value; }
    }

    // When calle by SendMessage, this adds the fs.score to S.score

    static public void FS_CALLBACK(FloatingScore fs)
    {
        SCORE += fs.score;
    }
}
