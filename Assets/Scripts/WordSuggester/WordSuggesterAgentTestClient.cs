using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordSuggesterAgentTestClient : MonoBehaviour
{
    [SerializeField] private Button _wordsButton;
    [SerializeField] private Button _sentenceButton;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private InputField _inputField;
    [SerializeField] private WordSuggesterAgent _agent;
    [SerializeField] private TMP_Text _textField;

    // Start is called before the first frame update
    void Start()
    {
        _wordsButton.onClick.AddListener(OnWordsClick);
        _sentenceButton.onClick.AddListener(OnSentenceClick);
    }

    private void AppendMessage(String message)
    {
        Debug.Log($"{message}");
        _textField.text += message + "\n";
    }

    async void ProcessWordsQueryAsync()
    {
        WordSuggesterAgent.WordCategory categoryEnum = 
            (WordSuggesterAgent.WordCategory) Enum.Parse(typeof(WordSuggesterAgent.WordCategory), 
                _dropdown.value.ToString());

        var result =  await 
            _agent.FindRelatedWords(_inputField.text, 
                "English", "Spanish", 
                categoryEnum);
        var relatedWords = result;
        
        // Iterate through the list and print each column
        foreach (var tuple in relatedWords)
        {
            // Access the first column of the tuple
            string column1 = tuple.Item1;
            // Access the second column of the tuple
            string column2 = tuple.Item2;

            // Print each column
            AppendMessage($"{column1} -> {column2}");
        }

        if (relatedWords.Count == 0)
        {
            AppendMessage("empty/non-parseable response");
        }
    }
    
    async void ProcessSentenceQueryAsync()
    {
        var result = await
            _agent.ConstructSentence(_inputField.text,
                "Spanish", "English");
        var sentences = result;
        
        // Iterate through the list and print each column
        // Print each column
        AppendMessage(sentences.Item1);
        AppendMessage(sentences.Item2);
    }

    void OnWordsClick()
    {
        _textField.text = "";
        ProcessWordsQueryAsync();
    }

    void OnSentenceClick()
    {
        _textField.text = "";
        ProcessSentenceQueryAsync();
    }
}
