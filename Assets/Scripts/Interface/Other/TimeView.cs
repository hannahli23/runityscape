﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * This manages the Time display on the bottom left corner
 * of the screen
 */
public class TimeView : MonoBehaviour {

    [SerializeField]
    private Text day;
    [SerializeField]
    private Text time;

    public int Day {
        set {
            day.text = string.Format("Day {0}", value);
        }
    }

    public string Time { set { time.text = string.Format("{0}", value); } }
    public bool IsTimeEnabled {
        set {
            time.enabled = value;
        }
    }

    public bool IsDayEnabled {
        set {
            day.enabled = value;
        }
    }

    public bool IsEnabled {
        set {
            IsDayEnabled = value;
            IsTimeEnabled = value;
        }
    }
}
