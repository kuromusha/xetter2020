// XETTER 2020  Copyright (C) 2020-2021  Ken'ichi Kuromusha
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    // share with other classes
    public static int leftMyNumber, score;

#pragma warning disable 649
    [SerializeField] AudioSource audioBad;
    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioClear;
    [SerializeField] AudioSource audioGameOver;
    [SerializeField] AudioSource audioHit;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject controllerMoveButton;
    [SerializeField] GameObject textCongratulations;
    [SerializeField] GameObject textGameOver;
    [SerializeField] Image joystick;
    [SerializeField] Sprite spriteBlock;
    [SerializeField] Sprite spriteDot;
    [SerializeField] Sprite spriteMyb;
    [SerializeField] Sprite spriteMyc;
    [SerializeField] Sprite spriteMyd;
    [SerializeField] Sprite spriteMyl1;
    [SerializeField] Sprite spriteMyl2;
    [SerializeField] Sprite spriteMyr1;
    [SerializeField] Sprite spriteMyr2;
    [SerializeField] Sprite spriteMyu;
    [SerializeField] Sprite spriteT1;
    [SerializeField] Sprite spriteT1b;
    [SerializeField] Sprite spriteT2;
    [SerializeField] Sprite spriteT2b;
    [SerializeField] Sprite spriteWall;
    [SerializeField] Text textControllerMove;
    [SerializeField] Text textHiScore;
    [SerializeField] Text textSence;
    [SerializeField] Text textScore;
    [SerializeField] Text textTime;
#pragma warning restore 649

    bool action;
    char[,,] sceneMap;
    float timeElapsed;
    int highScore, scene, time, dotNum, myPosX, myPosY, playingHit, afterHit, lastScreenWidth, lastScreenHeight;
    Common.STATUS status;
    Image[,] objScene;
    IDictionary<char, Sprite> objDictionary;
    IDictionary<char, int> numDictionary;
    IDictionary<char, int[,]> posDictionary;
    IList<Image> objHit;

    // Start is called before the first frame update
    void Start()
    {
        // init
        dotNum = myPosX = myPosY = playingHit = afterHit = 0;
        status = Common.STATUS.NORMAL;
        timeElapsed = 0;
        numDictionary = new Dictionary<char, int>
        {
            [Common.MAP_T1] = 0,
            [Common.MAP_T2] = 0,
            [Common.MAP_BLOCK] = 0
        };
        posDictionary = new Dictionary<char, int[,]>
        {
            [Common.MAP_T1] = new int[Common.MAX_T1_NUM, 2],
            [Common.MAP_T2] = new int[Common.MAX_T2_NUM, 2],
            [Common.MAP_BLOCK] = new int[Common.MAX_BLOCK_NUM, 2]
        };
        foreach (char key in posDictionary.Keys)
        {
            for (int i = 0; i < posDictionary[key].GetLength(0); i++)
            {
                posDictionary[key][i, 0] = -1;
                posDictionary[key][i, 1] = -1;
            }
        }
        action = false;
        sceneMap = new char[Common.WIDTH, Common.HEIGHT, 2];
        objScene = new Image[Common.WIDTH, Common.HEIGHT];
        objHit = new List<Image>();

        // hi score & score
        highScore = PlayerPrefs.GetInt(Common.SAVEDATA_HISH_SCORE, 0);
        DisplayScore();

        // scene
        scene = PlayerPrefs.GetInt(Common.SAVEDATA_START_SCENE, 1);
        textSence.text = $"{scene}";

        // time
        time = Common.INITIAL_TIME;
        textTime.text = $"{time}";

        // sptites
        objDictionary = new Dictionary<char, Sprite>();
        objDictionary[Common.MAP_MY] = spriteMyd;
        objDictionary[Common.MAP_WALL] = spriteWall;
        objDictionary[Common.MAP_BLOCK] = spriteBlock;
        objDictionary[Common.MAP_DOT] = spriteDot;
        objDictionary[Common.MAP_T1] = spriteT1;
        objDictionary[Common.MAP_T2] = spriteT2;

        // left my number
        for (int i = 0; i < leftMyNumber - 1; i++)
        {
            Display(Common.MAP_MY, Common.X_BAR_2ND + 1 + i * 2, 1);
        }

        // outer wall
        for (int i = 0; i < Common.WIDTH; i++)
        {
            Display(Common.MAP_WALL, i, 0);
            Display(Common.MAP_WALL, i, 3);
            Display(Common.MAP_WALL, i, Common.HEIGHT - 1);
        }
        for (int i = 1; i <= 2; i++)
        {
            Display(Common.MAP_WALL, 0, i);
            Display(Common.MAP_WALL, Common.X_BAR_1ST, i);
            Display(Common.MAP_WALL, Common.X_BAR_2ND, i);
            Display(Common.MAP_WALL, Common.WIDTH - 1, i);
        }
        for (int i = 4; i < Common.HEIGHT; i++)
        {
            Display(Common.MAP_WALL, 0, i);
            Display(Common.MAP_WALL, Common.WIDTH - 1, i);
        }

        // inner map
        for (int i = 0; i < Common.INNER_HIGHT; i += 2)
        {
            for (int j = 0; j < Common.INNER_WIDTH; j += 2)
            {
                Display(Common.sceneData[scene - 1, i / 2][j / 2], j, i, true);
            }
        }

        // hide unnecessary texts and adjust screen
        textGameOver.SetActive(false);
        textCongratulations.SetActive(false);
        AdjustScreen(true);
    }

    void DisplayScore()
    {
        if (highScore < score)
        {
            highScore = score;
            PlayerPrefs.SetInt(Common.SAVEDATA_HISH_SCORE, highScore);
        }
        textHiScore.text = $"{highScore}";
        textScore.text = $"{score}";
    }

    void Display(char character, int x, int y, bool inner = false)
    {
        // check max num
        if (numDictionary.ContainsKey(character) &&
            numDictionary[character] >= posDictionary[character].Length)
        {
            return;
        }
        // update sceneMap
        if (inner)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    sceneMap[x + i, y + j, 0] = character;
                    if (numDictionary.ContainsKey(character))
                    {
                        sceneMap[x + i, y + j, 1] = (char)numDictionary[character];
                    }
                }
            }
            if (numDictionary.ContainsKey(character))
            {
                posDictionary[character][numDictionary[character], 0] = x;
                posDictionary[character][numDictionary[character]++, 1] = y;
            }
            else if (character == Common.MAP_DOT)
            {
                dotNum += 4;
            }
        }
        // display character
        if (character != Common.MAP_SPACE)
        {
            int loopNum = character == Common.MAP_WALL || character == Common.MAP_DOT ? 2 : 1;
            int size = Common.SPRITE_UNIT_SIZE * (3 - loopNum);
            int diff = size / 2;
            for (int i = 0; i < loopNum; i++)
            {
                for (int j = 0; j < loopNum ; j++)
                {
                    GameObject tmpObj = new GameObject();
                    Image image = tmpObj.AddComponent<Image>();
                    image.sprite = objDictionary[character];
                    image.transform.SetParent(canvas.transform, false);
                    image.transform.localPosition = new Vector3(
                        Common.DISPLAY_OFFSET_X + (x + (inner ? Common.MAP_OFFSET_X : 0) + i) * Common.SPRITE_UNIT_SIZE + diff,
                        Common.DISPLAY_OFFSET_Y - (y + (inner ? Common.MAP_OFFSET_Y : 0) + j) * Common.SPRITE_UNIT_SIZE - diff, 0);
                    image.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
                    if (!inner)
                    {
                        return;
                    }
                    objScene[x + i, y + j] = image;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        AdjustScreen();
        timeElapsed += Time.deltaTime;
        switch (status)
        {
            case Common.STATUS.NORMAL:
                // check playingHit
                if (playingHit > 0)
                {
                    if (audioHit.isPlaying)
                    {
                        return;
                    }
                    if (--playingHit > 0)
                    {
                        // play agein
                        audioHit.Play();
                        return;
                    }
                    // check bad
                    if (afterHit < 0)
                    {
                        Bad(false);
                        return;
                    }
                    // clear hit enemies
                    foreach (Image obj in objHit)
                    {
                        obj.enabled = false;
                    }
                    objHit.Clear();
                }
                if (timeElapsed >= Common.DURATION || afterHit != 0)
                {
                    if (afterHit == 0)
                    {
                        // inpuit & move
                        float inX = Input.GetAxisRaw("Horizontal");
                        float inY = Input.GetAxisRaw("Vertical");
                        if (inX == 0 && inY == 0)
                        {
                            for (int i = 0; i < Input.touchCount; i++)
                            {
                                Touch touch = Input.GetTouch(i);
                                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                                {
                                    if(UpdateInXYByInputPosition(touch.position, ref inX, ref inY))
                                    {
                                        break;
                                    }
                                }
                            }
                            if (inX == 0 && inY == 0 && Input.GetMouseButton(0))
                            {
                                UpdateInXYByInputPosition(Input.mousePosition, ref inX, ref inY);
                            }
                        }
                        int dMyPosX = 0, dMyPosY = 0;
                        Sprite sprite = null;

                        if (inX < 0)
                        {
                            if (myPosX > 0)
                            {
                                dMyPosX--;
                            }
                            sprite = action ? spriteMyl1 : spriteMyl2;
                        }
                        else if (inX > 0)
                        {
                            if (myPosX < Common.INNER_WIDTH - 2)
                            {
                                dMyPosX++;
                            }
                            sprite = action ? spriteMyr1 : spriteMyr2;
                        }
                        else if (inY < 0)
                        {
                            if (myPosY < Common.INNER_HIGHT - 2)
                            {
                                dMyPosY++;
                            }
                            sprite = spriteMyd;
                        }
                        else if (inY > 0)
                        {
                            if (myPosY > 0)
                            {
                                dMyPosY--;
                            }
                            sprite = spriteMyu;
                        }
                        if (sprite != null)
                        {
                            objScene[myPosX, myPosY].sprite = sprite;
                        }
                        if (dMyPosX != 0 || dMyPosY != 0)
                        {
                            int nextX = dMyPosX == 0 ? 1 : 0;
                            int nextY = dMyPosY == 0 ? 1 : 0;
                            int addexX = myPosX + (dMyPosX > 0 ? 2 : dMyPosX);
                            int addedY = myPosY + (dMyPosY > 0 ? 2 : dMyPosY);

                            if (dMyPosX != 0 && CheckBlockMove(myPosX, myPosY, dMyPosX < 0) ||
                                MatchScene(addexX, addedY, nextX, nextY, false, true, Common.MAP_BLOCK, Common.MAP_WALL))
                            {
                                bool badFlag = CheckEnemyAndDot(addexX, addedY, nextX, nextY);
                                Move(myPosX, myPosY, dMyPosX, dMyPosY);
                                myPosX += dMyPosX;
                                myPosY += dMyPosY;

                                // score
                                DisplayScore();

                                // check bad
                                if (badFlag)
                                {
                                    Bad();
                                    return;
                                }
                            }
                        }

                        // check clear
                        if (dotNum <= 0)
                        {
                            // update score
                            score += 1000;
                            DisplayScore();
                            // wait until audioClear ends
                            restartButton.interactable = false;
                            audioBgm.Stop();
                            audioClear.Play();
                            status = Common.STATUS.CLEAR;
                            return;
                        }
                    }

                    // move blocks
                    for (int i = afterHit > 0 ? afterHit : 0; i < posDictionary[Common.MAP_BLOCK].GetLength(0); i++)
                    {
                        int x = posDictionary[Common.MAP_BLOCK][i, 0];
                        int y = posDictionary[Common.MAP_BLOCK][i, 1];
                        bool badFlag = false;
                        if (x >= 0 && y < Common.INNER_HIGHT - 2 &&
                            MatchScene(x, y + 2, 1, 0, false, true, Common.MAP_BLOCK, Common.MAP_DOT, Common.MAP_WALL))
                        {
                            // check bad
                            if (sceneMap[x, y + 2, 0] == Common.MAP_MY ||
                                sceneMap[x + 1, y + 2, 0] == Common.MAP_MY)
                            {
                                objScene[myPosX, myPosY].sprite = spriteMyb;
                                Move(myPosX, myPosY, 0, 1, true);
                                badFlag = true;
                            }
                            // check enemies
                            for (int j = 0; j <= 1; j++)
                            {
                                int currentX = x + j;
                                if (MatchScene(currentX, y + 2, 0, 0, true, true, Common.MAP_T1, Common.MAP_T2))
                                {
                                    char character = sceneMap[currentX, y + 2, 0];
                                    char id = sceneMap[currentX, y + 2, 1];
                                    int tx = posDictionary[character][id, 0];
                                    int ty = posDictionary[character][id, 1];
                                    objHit.Add(objScene[tx, ty]);
                                    objScene[tx, ty].sprite = character == Common.MAP_T1 ? spriteT1b : spriteT2b;
                                    Move(tx, ty, 0, 1, true);
                                    posDictionary[character][id, 0] = -1;
                                    posDictionary[character][id, 1] = -1;
                                    playingHit++;
                                }
                            }
                            Move(x, y, 0, 1);
                        }
                        // hit sound
                        if (playingHit > 0)
                        {
                            // update score
                            score += 200 * playingHit;
                            DisplayScore();
                            // set afterHit
                            afterHit = badFlag ? -1 : i + 1;
                            // wait until audioHit ends
                            audioHit.Play();
                            return;
                        }
                        // check bad
                        if (badFlag)
                        {
                            Bad(false);
                            return;
                        }
                    }

                    // move enemies
                    foreach (char character in new char[] { Common.MAP_T1, Common.MAP_T2 })
                    {
                        for (int i = 0; i < posDictionary[character].GetLength(0); i++)
                        {
                            int x = posDictionary[character][i, 0];
                            int y = posDictionary[character][i, 1];
                            if (x >= 0)
                            {
                                // high priority direction
                                int high
                                    = (character == Common.MAP_T1 || myPosY == y) && myPosX < x ? 0
                                    : (character == Common.MAP_T1 || myPosY == y) && myPosX > x ? 1
                                    : myPosX == x && myPosY < y ? 2
                                    : myPosX == x && myPosY > y ? 3
                                    : -1;
                                // shuffle array except high
                                int[] direction = new int[4];
                                if (high >= 0)
                                {
                                    direction[0] = high;
                                }
                                for (int j = 0, k = high >= 0 ? 1 : 0; j < 4; j++)
                                {
                                    if (j != high)
                                    {
                                        direction[k++] = j;
                                    }
                                }
                                for (int j = high >= 0 ? 1 : 0; j < direction.Length - 1; j++)
                                {
                                    int k = Random.Range(j, direction.Length);
                                    int tmp = direction[k];
                                    direction[k] = direction[j];
                                    direction[j] = tmp;
                                }
                                // move
                                foreach (int j in direction)
                                {
                                    int dx
                                        = j == 0 && x > 0 ? -1
                                        : j == 1 && x < Common.INNER_WIDTH - 2 ? 1
                                        : 0;
                                    int dy
                                        = j == 2 && y > 0 ? -1
                                        : j == 3 && y < Common.INNER_HIGHT - 2 ? 1
                                        : 0;
                                    if (dx != 0 || dy != 0)
                                    {
                                        int ret = EnemyMove(x, y, dx, dy);
                                        // check bad
                                        if (ret < 0)
                                        {
                                            Bad();
                                            return;
                                        }
                                        // check moved
                                        if (ret != 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // time
                    textTime.text = $"{--time}";
                    if (time <= 0)
                    {
                        Bad();
                    }

                    // reset timeElapsed & afterHit
                    timeElapsed = 0;
                    afterHit = 0;
                }
                break;
            case Common.STATUS.CLEAR:
                if (!audioClear.isPlaying)
                {
                    // update SAVEDATA_MAX_SCENE
                    if (++scene > PlayerPrefs.GetInt(Common.SAVEDATA_MAX_SCENE, 1))
                    {
                        PlayerPrefs.SetInt(Common.SAVEDATA_MAX_SCENE, scene);
                    }
                    // check all clear
                    if (scene <= Common.MAX_SCENE)
                    {
                        // next scene
                        PlayerPrefs.SetInt(Common.SAVEDATA_START_SCENE, scene);
                        SceneManager.LoadScene(Common.SCENE_MAIN);
                    }
                    else
                    {
                        ClearInner();
                        textCongratulations.SetActive(true);
                        status = Common.STATUS.WAIT4EVER;
                    }
                }
                break;
            case Common.STATUS.BAD:
                if (!audioBad.isPlaying)
                {
                    DecreaseLeftMyNumber();
                }
                break;
            case Common.STATUS.GAMEOVER:
                if (!audioGameOver.isPlaying)
                {
                    ClearInner();
                    textGameOver.SetActive(true);
                    status = Common.STATUS.WAIT2QUIT;
                    timeElapsed = 0;
                }
                break;
            case Common.STATUS.WAIT2QUIT:
                if (timeElapsed >= Common.WAIT_GAMEOVER)
                {
                    OnClickQuit();
                }
                break;
        }
    }

    void Bad(bool redraw = true)
    {
        restartButton.interactable = false;
        if (redraw)
        {
            objScene[myPosX, myPosY].sprite = spriteMyc;
            objScene[myPosX, myPosY].transform.SetAsLastSibling();
        }
        audioBgm.Stop();
        audioBad.Play();
        status = Common.STATUS.BAD;
    }

    void DecreaseLeftMyNumber(bool fromRestartButton = false)
    {
        if (--leftMyNumber > 0)
        {
            // restart
            SceneManager.LoadScene(Common.SCENE_MAIN);
        }
        else
        {
            if (fromRestartButton)
            {
                restartButton.interactable = false;
                audioBgm.Stop();
            }
            audioGameOver.Play();
            status = Common.STATUS.GAMEOVER;
        }
    }

    public void OnClickRestart()
    {
        DecreaseLeftMyNumber(true);
    }

    public void OnClickQuit()
    {
        SceneManager.LoadScene(Common.SCENE_TITLE);
    }

    void ClearInner()
    {
        for (int i = 0; i < Common.INNER_WIDTH; i++)
        {
            for (int j = 0; j < Common.INNER_HIGHT; j++)
            {
                if (objScene[i, j] != null)
                {
                    objScene[i, j].enabled = false;
                }
            }
        }
    }

    bool CheckEnemyAndDot(int x, int y, int dx, int dy)
    {
        bool badFlag = false;

        for (int i = 0; i <= dx; i++)
        {
            for (int j = 0; j <= dy; j++)
            {
                switch (sceneMap[x + i, y + j, 0])
                {
                    case Common.MAP_T1:
                    case Common.MAP_T2:
                        badFlag = true;
                        break;
                    case Common.MAP_DOT:
                        dotNum--;
                        score += 10;
                        objScene[x + i, y + j].enabled = false;
                        break;
                }
            }
        }

        return badFlag;
    }

    bool CheckBlockMove(int x, int y, bool toLeft)
    {
        int[] diff = toLeft ? new int[2] { -1, -1 } : new int[2] { 2, 1 };
        int nextX = x + diff[0];
        int nextNextX = nextX + diff[1] * 2;

        if ((toLeft && x >= 3 || !toLeft && x <= Common.INNER_WIDTH - 5) &&
            MatchScene(nextX, y, 0, 1, true, true, Common.MAP_BLOCK) &&
            sceneMap[nextX, y, 1] == sceneMap[nextX, y + 1, 1] &&
            MatchScene(nextNextX, y, 0, 1, true, true, Common.MAP_SPACE))
        {
            Move(x + diff[1] * 2, y, diff[1], 0);
            return true;
        }

        return false;
    }

    void Move(int x, int y, int dx, int dy, bool crushed = false)
    {
        int newX = x + dx;
        int newY = y + dy;
        int nextX = dx == 0 ? 1 : 0;
        int nextY = dy == 0 ? 1 : 0;
        int addedX = x + (dx > 0 ? 2 : dx);
        int addedY = y + (dy > 0 ? 2 : dy);
        int deletedX = x + (dx < 0 ? 1 : 0);
        int deletedY = y + (dy < 0 ? 1 : 0);
        char character = sceneMap[x, y, 0];
        char id = sceneMap[x, y, 1];
        Vector3 objSize = objScene[x, y].sprite.bounds.size * Common.SPRITE_SCALE;

        objScene[x, y].transform.position += new Vector3(dx * objSize.x / 2, -dy * objSize.y / 2, 0);
        objScene[x, y].GetComponent<RectTransform>().sizeDelta = new Vector2(objSize.x, objSize.y);
        objScene[newX, newY] = objScene[x, y];
        objScene[x, y] = null;
        if (crushed)
        {
            sceneMap[newX, newY, 0] = Common.MAP_SPACE;
            sceneMap[newX + nextX, newY + nextY, 0] = Common.MAP_SPACE;
        }
        else
        {
            sceneMap[addedX, addedY, 0] = character;
            sceneMap[addedX + nextX, addedY + nextY, 0] = character;
            if (posDictionary.ContainsKey(character))
            {
                sceneMap[addedX, addedY, 1] = id;
                sceneMap[addedX + nextX, addedY + nextY, 1] = id;
            }
        }
        sceneMap[deletedX, deletedY, 0] = Common.MAP_SPACE;
        sceneMap[deletedX + nextX, deletedY + nextY, 0] = Common.MAP_SPACE;
        if (posDictionary.ContainsKey(character))
        {
            posDictionary[character][id, 0] = newX;
            posDictionary[character][id, 1] = newY;
        }
        else if (character == Common.MAP_MY && dx != 0)
        {
            action = !action;
        }
    }

    int EnemyMove(int x, int y, int dx, int dy)
    {
        int nextX = dx == 0 ? 1 : 0;
        int nextY = dy == 0 ? 1 : 0;
        int addedX = x + (dx > 0 ? 2 : dx);
        int addedY = y + (dy > 0 ? 2 : dy);

        if (MatchScene(addedX, addedY, nextX, nextY, true, true, Common.MAP_MY, Common.MAP_SPACE))
        {
            int ret = MatchScene(addedX, addedY, nextX, nextY, true, false, Common.MAP_MY) ? -1 : 1;
            Move(x, y, dx, dy);
            return ret;
        }

        return 0;
    }

    bool MatchScene(int x, int y, int dx, int dy, bool match, bool and, params char [] pattern)
    {
        bool result = and;

        foreach (int[] pos in new int[][] { new int[] { x, y }, new int[] { x + dx, y + dy } })
        {
            bool tmpResult = !match;
            char character = sceneMap[pos[0], pos[1], 0];
            foreach (char p in pattern)
            {
                bool chatacterMatch = character == p;
                tmpResult = match ? (tmpResult || chatacterMatch) : (tmpResult && !chatacterMatch);
            }
            result = and ? (result && tmpResult) : (result || tmpResult);
            if (dx == 0 && dy == 0)
            {
                break;
            }
        }

        return result;
    }

    void AdjustScreen(bool force = false)
    {
        if (force || lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
        {
            float ratio = (float)Screen.height / Screen.width;
            if (ratio <= Common.SCREEN_ADJUST_THRESHOLD)
            {
                bool controllerLeft = PlayerPrefs.GetInt(Common.SAVEDATA_CONTROLLER_POSITION, 0) == 0;
                Vector3 xDirection = new Vector3(controllerLeft ? 1 : -1, 1, 1);

                quitButton.transform.localPosition = Vector3.Scale(Common.POS_LANDSCAPE_QUIT, xDirection);
                restartButton.transform.localPosition = Vector3.Scale(Common.POS_LANDSCAPE_RESTART, xDirection);
                controllerMoveButton.transform.localPosition = Vector3.Scale(Common.POS_LANDSCAPE_CONTROLLER_MOVE, xDirection);
                joystick.transform.localPosition = Vector3.Scale(Common.POS_LANDSCAPE_JOYSTICK, xDirection);
                joystick.transform.localScale = new Vector3(1, 1, 1);
                textControllerMove.text = $"TO {(controllerLeft ? "LEFT" : "RIGHT")}";
                controllerMoveButton.SetActive(true);
                canvas.transform.position = new Vector3(
                    (Common.BUTTON_WIDTH + Common.GAP_LENGTH) / 2 * (controllerLeft ? -1 : 1),
                    (Common.DISPLAY_OFFSET_Y - Common.GAME_SCREEN_HEIGHT) / 4, 0);
                Camera.main.orthographicSize = Common.GAME_SCREEN_HEIGHT / 2
                    * (ratio <= Common.SCREEN_LANDSCAPE_THRESHOLD ? 1 : ratio / Common.SCREEN_LANDSCAPE_THRESHOLD);
            }
            else
            {
                quitButton.transform.localPosition = Common.POS_PORTRATE_QUIT;
                restartButton.transform.localPosition = Common.POS_PORTRATE_RESTART;
                joystick.transform.localPosition = Common.POS_PORTRATE_JOYSTICK;
                joystick.transform.localScale = new Vector3(1, 1, 1) / Common.GAME_SCREEN_RATIO;
                controllerMoveButton.SetActive(false);
                canvas.transform.position = new Vector3(0, Common.PORTRATE_JOYSTICK_DELTA - Common.PORTRATE_HEIGHT_DIFF_UP_AND_DOWN / 2, 0);
                Camera.main.orthographicSize = (Common.PORTRATE_ORTHOGRAPHIC_SIZE + Common.PORTRATE_JOYSTICK_DELTA)
                    * (ratio <= Common.SCREEN_PORTRATE_THRESHOLD ? 1 : ratio / Common.SCREEN_PORTRATE_THRESHOLD);
            }
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    public void OnControllerMove()
    {
        PlayerPrefs.SetInt(Common.SAVEDATA_CONTROLLER_POSITION,
            (PlayerPrefs.GetInt(Common.SAVEDATA_CONTROLLER_POSITION, 0) + 1) % 2);
        AdjustScreen(true);
    }

    bool UpdateInXYByInputPosition(Vector2 inputPosition, ref float inX, ref float inY)
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(inputPosition) - joystick.transform.position;

        if (direction.magnitude <= joystick.rectTransform.rect.width / 2 * joystick.transform.localScale.x)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                inX = direction.x > 0 ? 1 : -1;
            }
            else
            {
                inY = direction.y > 0 ? 1 : -1;
            }
            return true;
        }

        return false;
    }
}
