using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCenter
{
    public static Dictionary<string, Color> CardColors = new Dictionary<string, Color>()
    {
        {"Bullet1", new Color(1f, 1f, 1f)},
        {"Bullet2", new Color(1.0f, 1f, 0.2f)},
        {"Bullet3", new Color(0.8f, 0.8f, 1f)},
        {"Bullet4", new Color(0.7f, 0.6f, 1f)},
        {"Bullet5", new Color(0.6f, 0.4f, 1f)},
        {"Bullet6", new Color(0.5f, 0.3f, 1f)},
        {"Bullet7", new Color(0.6f, 0.2f, 0.9f)},
        {"Bullet8", new Color(0.8f, 0.3f, 0.6f)},
        {"Bullet9", new Color(1f, 0.4f, 0.3f)},
        
        {"Enemy1", new Color(1f, 1f, 1f)},
        {"Enemy2", new Color(1f, 1f, 0.2f)},
        {"Enemy3", new Color(0.8f, 0.8f, 1f)},
        {"Enemy4", new Color(0.7f, 0.6f, 1f)},
        {"Enemy5", new Color(0.6f, 0.4f, 1f)},
        {"Enemy6", new Color(0.5f, 0.3f, 1f)},
        {"Enemy7", new Color(0.6f, 0.2f, 0.9f)},
        {"Enemy8", new Color(0.8f, 0.3f, 0.6f)},
        {"Enemy9", new Color(1f, 0.4f, 0.3f)},

        {"Boss1", new Color(1f, 1f, 1f)},
        {"Boss2", new Color(1f, 1f, 0f)},
        {"Boss3", new Color(1f, 1f, 1f)},
        {"Boss4", new Color(1f, 1f, 0.4f)},
        {"Boss5", new Color(1f, 1f, 1f)},
        {"Boss6", new Color(1f, 1f, 0f)},
        {"Boss7", new Color(1f, 1f, 1f)},
        {"Boss8", new Color(1f, 1f, 0f)},
        {"Boss9", new Color(120/255f,194/255f,87/255f)}
    };

    public static Dictionary<string, Color> CardTypeColors = new Dictionary<string, Color>()
    {
        {"Functional", new Color(22/255f,196/255f,255/255f) },
        {"Health",new Color(1f,0f,0f) },
        {"Bullet", new Color(1f, 1f, 1f)},
        {"BuiltIn", new Color(255/255f,192/255f,84/255f) }
    };

    
    public static Dictionary<string, Color> SelectorPanelColors = new Dictionary<string, Color>()
    {
        {"Green",new Color(159/255f,255/255f,104/255f) },
        {"Red",new Color(209/255f,43/255f,28/255f) }
    };

    public static Dictionary<string, Color> RankingPanelColors = new Dictionary<string, Color>()
    {

        {"HeadButtonInactive",new Color(48/255f,45/255f,33/255f,255f/255f) },
        {"HeadButtonActive",new Color(119/255f,74/255f,61/255f,255f/255f) }
    };

    public static Color platformHiddenColor = new Color(1, 1, 1, 0);
    public static Color platformFadeColor = new Color(1, 1, 1, 47 / 255f);
}
