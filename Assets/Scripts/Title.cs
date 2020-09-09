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
#pragma warning disable 649
    [SerializeField] Button buttonDown;
    [SerializeField] Button buttonUp;
    [SerializeField] Text textStartScene;
#pragma warning restore 649

    int startScene, maxStartScene;

    // Start is called before the first frame update
    void Start()
    {
        startScene = PlayerPrefs.GetInt(Common.SAVEDATA_START_SCENE, 1);
        maxStartScene = System.Math.Min(PlayerPrefs.GetInt(Common.SAVEDATA_MAX_SCENE, 1), Common.MAX_SCENE);
        RewriteStartScene();
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
        textStartScene.text = $"From Scene {startScene}";
        buttonUp.interactable = startScene < maxStartScene;
        buttonDown.interactable = startScene > 1;
    }
}
