using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    [SerializeField]
    private Text m_dayLeftText;
    [SerializeField]
    private float m_timeLeft = 180;

    void Start()
    {
        m_dayLeftText.text = "Daylight Left: "+180;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Sync better with song length
        m_timeLeft -= Time.deltaTime;
        m_dayLeftText.text = "Daylight Left: " + (int)m_timeLeft;
    }
}
