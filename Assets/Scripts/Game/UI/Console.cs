using System;
using System.Collections.Generic;
using Game.ItemSystem;
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

        private string ParseCommand(string input)
        {
            if (input.Length > 0 && input[0] != '/') return input;

            input = input.Replace("/", "");
            //Debug.Log(input);
            var command = "";
            
            var arguments = new List<string>();
            var argument = "";

            var giveCommand = true;
            for (var index = 0; index < input.Length; index++)
            {
                var t = input[index];
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
                    if (index == input.Length - 1)
                    {
                        Debug.Log(argument);
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
                case "timeset":
                    if (arguments.Count >= 1)
                    {
                        switch (arguments[0])
                        {
                            case "day": 
                                timeController.SetTime(14f);
                                return "Time Set Day - 14";    
                            case "night": 
                                timeController.SetTime(0f);
                                return "Time Set Night - 0";
                            case "morning": 
                                timeController.SetTime(8f);
                                return "Time Set Morning - 8";
                        }
                    }
                    else
                    {
                        return "Invalid Command";
                    }
                    break;
                case "give":
                    if (arguments.Count >= 2)
                    {
                        var itemName = arguments[0];
                        var count = int.Parse(arguments[1]);

                        var item = dataBase.GetItem(itemName);
                        
                        inventory.AddItemCount(item, count);
                        return "Gived: " + itemName;
                    }
                    else
                    {
                        return "Invalid Command";
                    }
                default:
                    return "Invalid Command";
            }
            return "";
        }
    }
}