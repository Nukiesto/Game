using System;
using System.Collections.Generic;
using System.Globalization;
using Game.ChunkSystem;
using Game.Game;
using Game.ItemSystem;
using Game.Player;
using Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class Console : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private InputField inputField;
        [SerializeField] private GameObject field;
        [SerializeField] private GameObject text;
        [SerializeField] private Transform texts;

        [Header("Other")] 
        [SerializeField] private TimeController timeController;
        [SerializeField] private Inventory inventory;
        [SerializeField] private DataBase dataBase;
        [SerializeField] private Transform blockSelector;
        private bool _isOpen;

        public delegate void OnToggleConsole(bool value);

        public static event OnToggleConsole OnToggleConsoleEvent;
        private void Start()
        {
            field.SetActive(_isOpen);
        }

        private void Update()
        {
            if (!_isOpen && Input.GetKeyDown(KeyCode.T))
            {
                inputField.text = "";
                _isOpen = !_isOpen;
                field.SetActive(_isOpen);
                OnToggleConsoleEvent?.Invoke(!_isOpen);
                
                inputField.ActivateInputField();
                return;
            }
            
            if (_isOpen && Input.GetKeyDown(KeyCode.Return))
            {
                var inputFieldText = inputField.text;
                var newText = Instantiate(text, texts);
                
                newText.GetComponent<Text>().text = ParseCommand(inputFieldText);;
                
                _isOpen = false;
                inputField.text = "";
                field.SetActive(false);
                OnToggleConsoleEvent?.Invoke(!_isOpen);
            }
        }

        public void WriteString(string inputText)
        {
            var newText = Instantiate(text, texts);
            newText.GetComponent<Text>().text = inputText;;
        }
        private string ParseCommand(string input)
        {
            if (input.Length > 0 && input[0] != '/') return input;

            input = input.Replace("/", "");
            //Debug.Log(input);
            var command = "";
            
            var arguments = new List<string>();
            var argument = "";

            var giveCommand = true;
            for (var i = 0; i < input.Length; i++)
            {
                var t = input[i];
                if (giveCommand && t != ' ')
                {
                    command += t;
                    continue;
                }
                if (giveCommand && t == ' ')
                {
                    giveCommand = false;
                    continue;
                }
                
                if (t != ' ')
                {
                    argument += t;
                    if (i == input.Length - 1)
                    {
                        //Debug.Log(argument);
                        arguments.Add(argument);
                    }
                    continue;
                }
                
                if (t == ' ')
                {
                    arguments.Add(argument);
                    argument = "";
                }
            }

            //Debug.Log("Command: " + command);
            //foreach (var item in arguments)
            //{
            //    Debug.Log(item);
            //}

            switch (command)
            {
                case "gamemode":
                    var gameManager = Toolbox.Instance.mGameManager;
                    if (arguments.Count == 1)
                    {
                        if (int.TryParse(arguments[0], out var n))
                        {
                            GameMode gamemode;
                            switch (n)
                            {
                                case 0:
                                    gamemode = GameMode.Survival;
                                    break;
                                case 1:
                                    gamemode = GameMode.Sandbox;
                                    break;
                                default:
                                    gamemode = gameManager.GetGameMode();
                                    break;
                            }
                        
                            gameManager.SetGameMode(gamemode);
                            return "GameMode: " + gamemode;
                        }
                    }
                    return "GameMode: " + gameManager.GetGameMode();
                case "getpos":
                    Vector3 posGet;
                    if (arguments.Count >= 1)
                    {
                        switch (arguments[0])
                        {
                            case "sel":
                                posGet = blockSelector.position;
                                return "Selector Pos: " + posGet.x + ", " + posGet.y;
                            case "player":
                                posGet = PlayerController.Instance.transform.position;
                                return "Player Pos: " + posGet.x + ", " + posGet.y;
                        }
                    }
            
                    posGet = PlayerController.Instance.transform.position;
                    return "Player Pos: " + posGet.x + ", " + posGet.y;
                case "spawn":
                    PlayerController.Instance.MoveToSpawn();
                    return "Tp to spawn";
                case "spawnset":
                    if (arguments.Count >= 2)
                    {
                        if (int.TryParse(arguments[1], out var x))
                        {
                            if (int.TryParse(arguments[2], out var y))
                            {
                                var worldManager = Toolbox.Instance.mWorldManager;
                                worldManager.SetSpawnPoint(new Vector3(x, y, 0));
                                return "SpawnPointSeted: " + x + ", " + y;
                            }
                        }
                        return "Invalid Arguments";
                    }
                    if (arguments.Count == 0)
                    {
                        var worldManager = Toolbox.Instance.mWorldManager;
                        var pos = PlayerController.Instance.transform.position;
                        worldManager.SetSpawnPoint(pos);
                        return "SpawnPoint Seted: " + pos.x + ", " + pos.y;
                    }
                    return "Invalid Command";
                case "setblock":
                    if (arguments.Count >= 3)
                    {
                        if (int.TryParse(arguments[1], out var x))
                        {
                            if (int.TryParse(arguments[2], out var y))
                            {
                                var layer = BlockLayer.Front;
                                if (arguments.Count >= 4 && Enum.TryParse<BlockLayer>(arguments[3], out var _layer))
                                {
                                    layer = _layer;
                                }
                                var blockname = arguments[0];
                                var blockdata = dataBase.GetBlock(blockname);
                                var pos = new Vector3(x, y, 0);
                                var chunkUnit = ChunkManager.Instance.GetChunk(pos);

                                chunkUnit.SetBlock(pos, blockdata, false, layer, true);
                                return "Block Seted: " + x + ", " + y;
                            }
                        }

                        //return "Invalid Arguments";
                    }
                    return "Invalid Command";
                case "tp":
                    if (arguments.Count >= 1)
                    {
                        switch (arguments[0])
                        {
                            case "spawn":
                                PlayerController.Instance.MoveToSpawn();
                                return "Tp to spawn";
                            default:
                                if (int.TryParse(arguments[0], out var x))
                                {
                                    if (int.TryParse(arguments[1], out var y))
                                    {
                                        var pos = new Vector3(x, y, 0);
                                        if (ChunkManager.Instance.InBounds(pos))
                                        {
                                            PlayerController.Instance.MoveToPos(pos);
                                            return "Tp to: " + x + ", " + y;
                                        }
                                    }
                                }

                                return "Invalid Pos";
                        }
                    }
                    return "Invalid Command";
                case "timeget":
                    if (arguments.Count == 0)
                        return timeController.GetTime();
                    return "Invalid Command";
                case "timeset":
                    if (arguments.Count == 1)
                    {
                        switch (arguments[0])
                        {
                            case "day": 
                                timeController.SetTime(TimeController.Time.Day);
                                return "Time Set Day";    
                            case "night": 
                                timeController.SetTime(TimeController.Time.Night);
                                return "Time Set Night";
                            case "morning": 
                                timeController.SetTime(TimeController.Time.Morning);
                                return "Time Set Morning";
                            case "evening": 
                                timeController.SetTime(TimeController.Time.Evening);
                                return "Time Set Evening";
                            default:
                                if (float.TryParse(arguments[0], out var time))
                                {
                                    timeController.SetTime(time);
                                    return "Time Set: " + time;
                                }
                                break;
                        }
                    }
                    return "Invalid Command";
                case "give":
                    if (arguments.Count >= 2)
                    {
                        var itemName = arguments[0];
                        var count = int.Parse(arguments[1]);

                        var item = dataBase.GetItem(itemName);
                        
                        inventory.AddItemCount(item, count);
                        return "Gived: " + itemName;
                    }
                    return "Invalid Command";
                default:
                    return "Invalid Command";
            }
        }
    }
}