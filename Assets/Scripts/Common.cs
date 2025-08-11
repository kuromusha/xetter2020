// XETTER 2020  Copyright (C) 2020-2025  Ken'ichi Kuromusha
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using UnityEngine;

public class Common : MonoBehaviour
{
    public const string SAVEDATA_START_SCENE = "scene";
    public const string SAVEDATA_HISH_SCORE = "high";
    public const string SAVEDATA_CONTROLLER_POSITION = "controller";
    public const string SCENE_MAIN = "Main";
    public const string SCENE_TITLE = "Title";

    public const char MAP_T1 = '1';
    public const char MAP_T2 = '2';
    public const char MAP_BLOCK = '3';
    public const char MAP_DOT = '4';
    public const char MAP_WALL = '5';
    public const char MAP_MY = '7';
    public const char MAP_SPACE = ' ';

    public const int MAX_SCENE = 30;
    public const int MAX_T1_NUM = 4;
    public const int MAX_T2_NUM = 4;
    public const int MAX_BLOCK_NUM = 15;
    public const int INITIAL_MY_NUMBER = 5;
    public const int WIDTH = 40;
    public const int HEIGHT = 25;
    public const int X_BAR_1ST = 17;
    public const int X_BAR_2ND = 27;
    public const int SPRITE_SCALE = 4;
    public const int DISPLAY_OFFSET_X = -160 * SPRITE_SCALE;
    public const int DISPLAY_OFFSET_Y = 120 * SPRITE_SCALE;
    public const int INNER_HIGHT = 20;
    public const int INNER_WIDTH = 38;
    public const int INITIAL_TIME = 1500;
    public const int SPRITE_UNIT_SIZE = 8 * SPRITE_SCALE;
    public const int MAP_OFFSET_X = 1;
    public const int MAP_OFFSET_Y = 4;
    public const int SORTING_ORDER_MY = 1;
    public const int SORTING_ORDER_OTHERS = 0;
    public const int GAME_SCREEN_WIDTH = 320 * SPRITE_SCALE;
    public const int GAME_SCREEN_HEIGHT = 200 * SPRITE_SCALE;
    public const int GAP_LENGTH = 10 * SPRITE_SCALE;
    public const int BUTTON_WIDTH = 70 * SPRITE_SCALE;
    public const int BUTTON_HEIGHT = 20 * SPRITE_SCALE;
    public const int BUTTON_AREA_HEIGHT = BUTTON_HEIGHT + GAP_LENGTH;
    public const int JOYSTICK_LENGTH = 75 * SPRITE_SCALE;
    public const int CONTROLLER_AREA_HEIGHT = JOYSTICK_LENGTH + GAP_LENGTH;
    public const int LANDSCAPE_SCREEN_WIDTH = GAME_SCREEN_WIDTH + GAP_LENGTH + BUTTON_WIDTH;
    public const int LANDSCAPE_BUTTON_X = (GAME_SCREEN_WIDTH + BUTTON_WIDTH) / 2 + GAP_LENGTH;
    public const int PORTRATE_BUTTON_X = (GAME_SCREEN_WIDTH - BUTTON_WIDTH) / 2;
    public const int PORTRATE_BUTTON_Y = DISPLAY_OFFSET_Y + GAP_LENGTH + BUTTON_HEIGHT / 2;
    public const int PORTRATE_ORTHOGRAPHIC_SIZE = (BUTTON_AREA_HEIGHT + GAME_SCREEN_HEIGHT + CONTROLLER_AREA_HEIGHT) / 2;
    public const int PORTRATE_GAME_SCREEN_BOTTOM = DISPLAY_OFFSET_Y - GAME_SCREEN_HEIGHT;
    public const int PORTRATE_HEIGHT_DIFF_UP_AND_DOWN = BUTTON_AREA_HEIGHT + DISPLAY_OFFSET_Y + PORTRATE_GAME_SCREEN_BOTTOM - CONTROLLER_AREA_HEIGHT;

    public const float DURATION = 0.15f;
    public const float WAIT_GAMEOVER = 3f;
    public const float GAME_SCREEN_RATIO = (float)GAME_SCREEN_HEIGHT / GAME_SCREEN_WIDTH;
    public const float PORTRATE_JOYSTICK_DELTA = GAME_SCREEN_RATIO * JOYSTICK_LENGTH / 2;
    public const float PORTRATE_SCREEN_HEIGHT = BUTTON_AREA_HEIGHT + GAME_SCREEN_HEIGHT + CONTROLLER_AREA_HEIGHT + PORTRATE_JOYSTICK_DELTA * 2;
    public const float SCREEN_ADJUST_THRESHOLD = PORTRATE_SCREEN_HEIGHT / LANDSCAPE_SCREEN_WIDTH;
    public const float SCREEN_LANDSCAPE_THRESHOLD = (float)(GAME_SCREEN_HEIGHT) / LANDSCAPE_SCREEN_WIDTH;
    public const float SCREEN_PORTRATE_THRESHOLD = PORTRATE_SCREEN_HEIGHT / GAME_SCREEN_WIDTH;
    public const float APAD_THRESHOLD = 0.95f;
    public const float DPAD_THRESHOLD = 0.5f;

    public static readonly Vector3 POS_LANDSCAPE_QUIT = new Vector3(LANDSCAPE_BUTTON_X, DISPLAY_OFFSET_Y - BUTTON_HEIGHT * 3 / 2 - GAP_LENGTH, 0);
    public static readonly Vector3 POS_LANDSCAPE_RESTART = new Vector3(LANDSCAPE_BUTTON_X, DISPLAY_OFFSET_Y - BUTTON_HEIGHT / 2, 0);
    public static readonly Vector3 POS_LANDSCAPE_CONTROLLER_MOVE = new Vector3(LANDSCAPE_BUTTON_X, DISPLAY_OFFSET_Y - BUTTON_HEIGHT * 5 / 2 - GAP_LENGTH * 2, 0);
    public static readonly Vector3 POS_LANDSCAPE_JOYSTICK = new Vector3(LANDSCAPE_BUTTON_X - (JOYSTICK_LENGTH - BUTTON_WIDTH) / 2, PORTRATE_GAME_SCREEN_BOTTOM + JOYSTICK_LENGTH / 2 + SPRITE_UNIT_SIZE * 2, 0);
    public static readonly Vector3 POS_PORTRATE_QUIT = new Vector3(-PORTRATE_BUTTON_X, PORTRATE_BUTTON_Y, 0);
    public static readonly Vector3 POS_PORTRATE_RESTART = new Vector3(PORTRATE_BUTTON_X, PORTRATE_BUTTON_Y, 0);
    public static readonly Vector3 POS_PORTRATE_JOYSTICK = new Vector3(0, PORTRATE_GAME_SCREEN_BOTTOM - GAP_LENGTH - JOYSTICK_LENGTH / 2 - PORTRATE_JOYSTICK_DELTA, 0);

    public enum STATUS
    {
        NORMAL,
        BAD,
        CLEAR,
        GAMEOVER,
        WAIT2QUIT,
        WAIT4EVER
    }

    public static readonly string[,] sceneData = new string[,]
    {
        { // 1
            "7                  ",
            " 5555 555 555 5555 ",
            "  3     3 3     3  ",
            " 55 555 555 555 55 ",
            " 3   3   3   3   3 ",
            " 5 5 5 5 5 5 5 5 5 ",
            " 5 5 5 5 5 5 5 5 5 ",
            " 545454545454545451",
            "   4           4  2",
            "4545454545454545454"
        },
        { // 2
            "735 3    3    3 52 ",
            " 45 44444444444 54 ",
            " 55 44444444444 55 ",
            "                   ",
            " 44444444444444444 ",
            " 5 5 5 5 5 5 5 5 5 ",
            " 5 5 5 5 5 5 5 5 5 ",
            " 5 5 5 5 5 5 5 5 5 ",
            "        1 1        ",
            "4545454545454545454"
        },
        { // 3
            "7       3          ",
            "        3          ",
            "        3          ",
            "        3          ",
            "        3          ",
            "        3          ",
            "        3          ",
            "        3          ",
            "      5 5 5      12",
            "5454545454545454545"
        },
        { // 4
            "7                  ",
            " 55555443 34455555 ",
            " 45555544 44555553 ",
            " 34555554 45555544 ",
            " 44455555 55555444 ",
            " 44455555 55555444 ",
            " 44555554 45555544 ",
            " 45555543 34555554 ",
            " 55555444 44455555 ",
            "         2         "
        },
        { // 5
            "7       3 3        ",
            " 4554 454 4554 454 ",
            " 5445 5 5 5445 5 5 ",
            " 4445 5 5 4445 5 5 ",
            " 4454 5 5 4454 5 5 ",
            " 4544 5 5 4544 5 5 ",
            " 5444 5 5 5444 5 5 ",
            " 5444 5 5 5444 5 5 ",
            " 5555 454 5555 454 ",
            "         2         "
        },
        { // 6
            "73   3   3   3   3 ",
            " 55445544544554455 ",
            "         5        2",
            " 55445544544554455 ",
            "     55     55     ",
            " 55445544544554455 ",
            " 55             552",
            " 55445544544554455 ",
            "         5         ",
            "4554455445445544554"
        },
        { // 7
            "7                  ",
            "                   ",
            "    44444444444    ",
            "    44343434344    ",
            "    44444444444    ",
            "    43434343434    ",
            "    44444444444    ",
            "    44444444444    ",
            "                   ",
            "1                 1"
        },
        { // 8
            "7                  ",
            "  3   3     3   3  ",
            "  444444444444444  ",
            "  4442       2444  ",
            "  55455 555 55455  ",
            "  54555 555 55545  ",
            "  4442       2444  ",
            "  444444444444444  ",
            "                   ",
            "                   "
        },
        { // 9
            "7       3 3        ",
            " 55 55 55 55 55 55 ",
            "     3       3     ",
            " 55 55 55 55 55 55 ",
            " 3               3 ",
            " 5 5 5 5 5 5 5 5 5 ",
            " 3 3 3 3 3 3 3 3 3 ",
            " 5 5 5 5 5 5 5 5 5 ",
            "1122           2211",
            "4545454545454545454"
        },
        { // 10
            "75               5 ",
            "   3    3 3    3   ",
            "5554 55 555 55 4555",
            "                   ",
            " 55555555 55555555 ",
            "     3       3     ",
            "   5 555555555 5   ",
            "  5             5  ",
            "45               54",
            "54      121      45"
        },
        { // 11
            "7   3         3    ",
            "5 555 5555555 555 5",
            "4 3             3 4",
            "5 55555 555 55555 5",
            "4   3         3   4",
            "5 5 5555 5 5555 5 5",
            "4                 4",
            "5 555 555 555 555 5",
            "4             11224",
            "5454545454545454545"
        },
        { // 12
            "75   5   5   5   5 ",
            " 5 5 5 5 5 5 5 5 5 ",
            " 3 5 3 5 3 5 3 5 3 ",
            " 5 5 5 5 5 5 5 5 5 ",
            " 5   5   5   5   5 ",
            " 5 3 5 3 5 3 5 3 5 ",
            " 5 5 5 5 5 5 5 5 5 ",
            "   5   5   5   5   ",
            " 5 5 5 5 5 5 5 5 5 ",
            " 5  452 452 452 452"
        },
        { // 13
            "7                  ",
            "                   ",
            "                   ",
            "   3 3 3   3 3 3   ",
            "   55555   55555   ",
            "44               44",
            " 4               4 ",
            " 4               4 ",
            " 4   444444444   4 ",
            "24   4   2   4   42"
        },
        { // 14
            "7555555555555555555",
            " 5 4 5 4 5 4 4543 5",
            "454545454545455 515",
            " 5 5 5 5 5 5 5 4525",
            "4545454545454545515",
            " 5 5 5 5 5 5 5 4525",
            "454545454545455 515",
            " 5 5 5 5 5 5 5 4525",
            "4 454 454 454 45514",
            "5555555555555555525"
        },
        { // 15
            "7               3  ",
            "              3 45 ",
            "            3 45   ",
            "          3 45     ",
            "        3 45       ",
            "      3 45         ",
            "    3 45           ",
            "  3 45            1",
            "  45             12",
            "45              124"
        },
        { // 16
            "7                  ",
            "                   ",
            " 3               3 ",
            " 55555555555555555 ",
            " 3               5 ",
            " 3           4   5 ",
            " 44444444444444445 ",
            " 55555555555555555 ",
            "                   ",
            "        111        "
        },
        { // 17
            "73                 ",
            " 43                ",
            " 443               ",
            " 4443              ",
            " 44443             ",
            " 444443            ",
            " 4444443           ",
            " 44444443          ",
            " 444444443        2",
            " 44444444434     22"
        },
        { // 18
            "7          5     54",
            " 555555555 5 5 5   ",
            "        5    5 5 55",
            " 555545 5 5555 5  5",
            "    554 5   54 55 4",
            "555   55455  545  5",
            "45  5  5   5  54 54",
            " 54545   5  5  5   ",
            "  55   5555  1 555 ",
            "5    5    4545  454"
        },
        { // 19
            "73 55555 4         ",
            " 3    25 5 555 555 ",
            " 3   225 5   3 3   ",
            " 3 55515 5 5 555 5 ",
            " 3 55515 5         ",
            " 5 34415 5  55555  ",
            " 5444415 5         ",
            " 5555555 5 5 555 5 ",
            " 5   5   5    2    ",
            "   5   5 5454545454"
        },
        { // 20
            "75   5   5   5   51",
            " 5 5 5 5 5 5 5 5 5 ",
            " 5 5 5 5 5 5 5 5 5 ",
            "   5   5 5 5   5   ",
            "55555555 5 55555555",
            "      3    54444444",
            "      3 55544545454",
            "  44444 5          ",
            " 55555554 55555555 ",
            "                   "
        },
        { // 21
            "7  3     3     3   ",
            " 454 454 5 454 454 ",
            " 545 545 5 545 545 ",
            "     3       3     ",
            "  5  5  5 5  5  5  ",
            "         2         ",
            " 5  5  5 5 5  5  5 ",
            "                   ",
            "4444444444444444444",
            "5551555552555551555"
        },
        { // 22
            "7        3         ",
            "   454  454  454   ",
            " 3 515  525  515 3 ",
            " 3 444  444  444 3 ",
            " 55555555555555555 ",
            "    3         3    ",
            " 55 55 5 5 5 55 55 ",
            "                   ",
            "155 555 555 555 551",
            "54       2       45"
        },
        { // 23
            "7                  ",
            "         3         ",
            "        444        ",
            "        414        ",
            "        444        ",
            "        424        ",
            "        444        ",
            "        414        ",
            "4444444444444444444",
            "5555555552555555555"
        },
        { // 24
            "7                  ",
            "                   ",
            "     434444434     ",
            "     444414444     ",
            "     444343444     ",
            "     424444424     ",
            "     444414444     ",
            "     444444444     ",
            "         1         ",
            "         1         "
        },
        { // 25
            "7                  ",
            "                   ",
            "                   ",
            " 3   3   3   3   3 ",
            " 5 5 5 5 5 5 5 5 5 ",
            "                   ",
            "                   ",
            "1                  ",
            "4444444444444444444",
            "5555125521255215555"
        },
        { // 26
            "7                  ",
            "                   ",
            "       34443       ",
            "       45154       ",
            "       44444       ",
            "       45254       ",
            "       44444       ",
            "       45154       ",
            "       44444       ",
            "5555555552555555555"
        },
        { // 27
            "73                 ",
            "545                ",
            "                   ",
            "                  1",
            "                  1",
            "                  1",
            "                  1",
            "        3      3   ",
            "  5 5   44444444   ",
            "  54555552255225555"
        },
        { // 28
            "7                  ",
            " 535               ",
            " 535               ",
            " 535               ",
            " 535               ",
            " 535               ",
            "  3                ",
            " 545               ",
            "4444444444444444444",
            "5555552515251525155"
        },
        { // 29
            "75               3 ",
            " 5  4            3 ",
            " 54444444444444444 ",
            " 54145555555555555 ",
            " 54445   5   55445 ",
            " 5     5   5   445 ",
            " 55555555555555555 ",
            " 3                 ",
            " 3            4 444",
            " 444444444444444414"
        },
        { // 30
            "7                  ",
            " 3                 ",
            " 3                 ",
            " 3                 ",
            " 3                 ",
            " 3                 ",
            " 3                 ",
            " 3                 ",
            " 344444444444444444",
            " 441424142424142414"
        }
    };
}
