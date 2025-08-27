using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine;

public class RankingNoReviveRow : MonoBehaviour
{
    [SerializeField] TMP_Text textRank;
    [SerializeField] TMP_Text textName;
    [SerializeField] TMP_Text textWave;
    [SerializeField] TMP_Text textKilled;

    public void Set(string rank,string name,string wave,string killed)
    {
        textRank.text = rank;
        textName.text = name;
        textWave.text = wave;
        textKilled.text = killed;
    }
}
