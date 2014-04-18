/*
 * Copyright (c) 2014, Nick Gravelyn.
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 *    1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 *
 *    2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 *
 *    3. This notice may not be removed or altered from any source
 *    distribution.
 */

using UnityEngine;
using System;
using System.Collections;

namespace UnityToolbag
{
    // A basic demonstration of using the game save system.
    public class GameSaveSystemDemo : MonoBehaviour
    {
        private DemoGameSave _gameSave;
        private bool _isLoading, _isSaving;
        private int _dotCount = 0;

        void Start()
        {
            // Games have to Initialize the system before using it.
            GameSaveSystem.Initialize(new GameSaveSystemSettings
            {
                companyName = "Test Company",
                gameName = "Test Game",
                useRollingBackups = true,
                backupCount = 2
            });

            // Log the output folder where the saves will be
            Debug.Log("Save location: " + GameSaveSystem.saveLocation, this);

            // Use a coroutine to update the animated dots for our text
            StartCoroutine(UpdateDots());
        }

        IEnumerator UpdateDots()
        {
            while (true) {
                yield return new WaitForSeconds(0.25f);
                _dotCount = (_dotCount + 1) % 4;
            }
        }

        void DemoLoad(bool forceFromDisk)
        {
            // Don't do more than one load/save at a time
            if (!_isLoading && !_isSaving) {
                _isLoading = true;

                // Load the save and handle the results.
                GameSaveSystem.Load<DemoGameSave>("demosave", forceFromDisk)
                    .OnSuccess(f =>
                    {
                        if (f.value.usedBackupFile) {
                            Debug.LogWarning("Load successful, but a backup file was used.", this);
                        }
                        else {
                            Debug.Log("Load successful. Was cached? " + f.value.wasCached, this);
                        }

                        _gameSave = f.value.save;
                    })
                    .OnError(f =>
                    {
                        Debug.LogWarning("Load failed: " + f.error.Message + ". Creating new game save for demo.", this);
                        _gameSave = new DemoGameSave();
                    })
                    .OnComplete(f => _isLoading = false);
            }
        }

        void DemoSave()
        {
            // Don't do more than one load/save at a time
            if (!_isLoading && !_isSaving && _gameSave != null) {
                _isSaving = true;

                // Save our game save and handle the result
                GameSaveSystem.Save<DemoGameSave>("demosave", _gameSave)
                    .OnSuccess(f => Debug.Log("Save successful.", this))
                    .OnError(f => Debug.LogWarning("Save failed: " + f.error.Message, this))
                    .OnComplete(f => _isSaving = false);
            }
        }

        void OnGUI()
        {
            // Show animated status if loading or saving.
            if (_isLoading) {
                GUILayout.Label("Loading" + new string('.', _dotCount));
            }
            else if (_isSaving) {
                GUILayout.Label("Saving" + new string('.', _dotCount));
            }
            else {
                // If we have a save, show the text from it and add an option to change the text
                if (_gameSave != null) {
                    GUILayout.Label("Game save text: " + _gameSave.text);

                    if (GUILayout.Button("Change demo save text")) {
                        _gameSave.text = Guid.NewGuid().ToString();
                    }
                }

                // Show some buttons for playing with the save
                if (GUILayout.Button("New Save")) {
                    _gameSave = new DemoGameSave();
                }
                if (GUILayout.Button("Load")) {
                    DemoLoad(false);
                }
                if (GUILayout.Button("Force Load")) {
                    DemoLoad(true);
                }
                if (GUILayout.Button("Load and Fail")) {
                    DemoGameSave.FailNextLoad = true;
                    DemoLoad(true);
                }
                if (GUILayout.Button("Save")) {
                    DemoSave();
                }
                if (GUILayout.Button("Save and Fail")) {
                    DemoGameSave.FailNextSave = true;
                    DemoSave();
                }
            }
        }
    }
}
