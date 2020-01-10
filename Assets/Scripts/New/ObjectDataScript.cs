using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectData
{
    public enum OBJECT_COLOR
    {
        RED,
        BLUE,
        GREEN,
        YELLOW,
        NONE
    };

    public enum PLAYER_NUMBER
    {
        ONE,
        TWO,
        THREE,
        FOUR
    };

    public enum GAME_STATE
    {
        INITIALIZE_DATA,
        SELECT_PLAYER,
        IN_GAME,
        END_GAME
    }

    public enum SKILL_TYPE
    {
        SKILL1,
        SKILL2,
        SKILL3,
        SKILL4
    }
}
