// XETTER 2020  Copyright (C) 2020  Ken'ichi Kuromusha
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
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    int startScene, maxStartScene;
    GameObject text;
    Button buttonUp, buttonDown;

    // Start is called before the first frame update
    void Start()
    {
        startScene = PlayerPrefs.GetInt(Common.SAVEDATA_START_SCENE, 1);
        maxStartScene = System.Math.Min(PlayerPrefs.GetInt(Common.SAVEDATA_MAX_SCENE, 1), Common.MAX_SCENE);
        text = GameObject.Find(Common.TEXT_START_SCENE);
        buttonUp = GameObject.Find(Common.BUTTON_UP).GetComponent<Button>();
        buttonDown = GameObject.Find(Common.BUTTON_DOWN).GetComponent<Button>();
        RewriteStartScene();
        Common.AdjustScreen(true);
    }

    // Update is called once per frame
    private void Update()
    {
        Common.AdjustScreen();
    }

    public void OnClickStart()
    {
        PlayerPrefs.SetInt(Common.SAVEDATA_START_SCENE, startScene);
        Main.leftMyNumber = Common.INITIAL_MY_NUMBER;
        Main.score = 0;
        SceneManager.LoadScene(Common.SCENE_MAIN);
    }

    public void OnClickUp()
    {
        RewriteStartScene(1);
    }

    public void OnClickDown()
    {
        RewriteStartScene(-1);
    }

    void RewriteStartScene(int diff = 0)
    {
        startScene += diff;
        if (startScene < 1)
        {
            startScene = 1;
        }
        else if (startScene > maxStartScene)
        {
            startScene = maxStartScene;
        }
        text.GetComponentInChildren<Text>().text = $"From Scene {startScene}";
        buttonUp.interactable = startScene < maxStartScene;
        buttonDown.interactable = startScene > 1;
    }
}
